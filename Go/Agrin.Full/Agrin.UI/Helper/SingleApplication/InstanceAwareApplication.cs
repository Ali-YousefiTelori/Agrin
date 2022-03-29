using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.Helper.SingleApplication
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    #endregion

    #region Internal interface used by the service
    /// <summary>
    ///   Interface used to signal a prior instance of the application about the startup another instance.
    /// </summary>
    [ServiceContract]
    internal interface IPriorApplicationInstance
    {
        /// <summary>
        ///   Signals the startup of the next application instance.
        /// </summary>
        /// <param name = "args">The parameters used to run the next instance of the application.</param>
        [OperationContract]
        void SignalStartupNextInstance(string[] args);
    }
    #endregion

    /// <summary>
    ///   Class used to define a WPF application which is aware of subsequent application instances running, either locally (per session) or globally (per host).
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class InstanceAwareApplication : Application, IPriorApplicationInstance, IDisposable
    {
        #region Static Members
        /// <summary>
        ///   The mutex prefix used if the application instance must be single per machine.
        /// </summary>
        private const string GLOBAL_PREFIX = @"Global\";

        /// <summary>
        ///   The mutex prefix used if the application instance must be single per user session.
        /// </summary>
        private const string LOCAL_PREFIX = @"Local\";

        /// <summary>
        ///   The milliseconds to wait to determine if the current instance is the first one.
        /// </summary>
        private const double FIRST_INSTANCE_TIMEOUT_MILLISECONDS = 500;

        /// <summary>
        ///   The milliseconds to wait for the service to be ready.
        /// </summary>
        private const double SERVICE_READY_TIMEOUT_MILLISECONDS = 1000;

        /// <summary>
        ///   The milliseconds to wait for the prior instance to signal that the startup information have been received.
        /// </summary>
        private const double PRIOR_INSTANCE_SIGNALED_TIMEOUT_MILLISECONDS = 2500;

        /// <summary>
        ///   The SID value to be used to retrieve the <c>Users</c> group identity.
        /// </summary>
        private const string USERS_SID_VALUE = "S-1-5-32-545";

        /// <summary>
        ///   Extracts some parameters from the specified <see cref = "ApplicationInstanceAwareness" /> value.
        /// </summary>
        /// <param name = "awareness">The <see cref = "ApplicationInstanceAwareness" /> value to extract parameters from.</param>
        /// <param name = "prefix">The synchronization object prefix.</param>
        /// <param name = "identity">The identity used to handle the synchronization object.</param>
        /// <exception cref = "UnexpectedInstanceAwareApplicationException">A proper identity could not be determined.</exception>
        private static void ExtractParameters(ApplicationInstanceAwareness awareness, out string prefix, out IdentityReference identity)
        {
            new SecurityPermission(SecurityPermissionFlag.ControlPrincipal).Assert();
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();

            if (currentIdentity != null)
            {
                if (awareness == ApplicationInstanceAwareness.Host && currentIdentity.Groups != null)
                {
                    prefix = GLOBAL_PREFIX;
                    identity = currentIdentity.Groups.FirstOrDefault(reference => reference.Translate(typeof(SecurityIdentifier)).Value.Equals(USERS_SID_VALUE));
                }
                else
                {
                    prefix = LOCAL_PREFIX;
                    identity = currentIdentity.User;
                }
                CodeAccessPermission.RevertAssert();

                if (identity == null)
                    throw new UnexpectedInstanceAwareApplicationException("Could not determine a proper identity to create synchronization objects access rules");
            }
            else
                throw new UnexpectedInstanceAwareApplicationException("Unable to retrieve current identity");
        }

        /// <summary>
        ///   Gets the <see cref = "Uri" /> of the pipe used for inter-process communication.
        /// </summary>
        /// <param name = "applicationPath">The application unique path, used to define the <see cref = "Uri" /> pipe.</param>
        /// <returns>The <see cref = "Uri" /> of the pipe used for inter-process communication.</returns>
        private static Uri GetPipeUri(string applicationPath)
        {
            return new Uri(string.Format("net.pipe://localhost/{0}/", applicationPath));
        }
        #endregion

        #region Events
        /// <summary>
        ///   Occurs when the <see cref = "System.Windows.Application.Run()" /> or <see cref = "System.Windows.Application.Run(Window)" /> method of the next <see cref = "InstanceAwareApplication" /> having the same <see cref = "Guid" /> is called.
        /// </summary>
        public event StartupNextInstanceEventHandler StartupNextInstance;
        #endregion

        #region Fields
        /// <summary>
        ///   The application instance awareness.
        /// </summary>
        private readonly ApplicationInstanceAwareness m_Awareness;

        /// <summary>
        ///   Flag used to determine if the synchronization objects (and the inter-process communication service) have been disposed.
        /// </summary>
        private bool m_Disposed;

        /// <summary>
        ///   The synchronization object owned by the first instance.
        /// </summary>
        private Mutex m_FirstInstanceMutex;

        /// <summary>
        ///   The host used to communicate between multiple application instances.
        /// </summary>
        private ServiceHost m_ServiceHost;

        /// <summary>
        ///   The synchronization object used to synchronize the service creation or destruction.
        /// </summary>
        private Mutex m_ServiceInitializationMutex;

        /// <summary>
        ///   The synchronization object used to signal that the service is ready.
        /// </summary>
        private EventWaitHandle m_ServiceReadySemaphore;

        /// <summary>
        ///   The synchronization object used to signal a subsequent application instance that the first one received the notification.
        /// </summary>
        private EventWaitHandle m_SignaledToFirstInstanceSemaphore;
        #endregion

        #region Properties
        /// <summary>
        ///   Gets the instance awareness of the application.
        /// </summary>
        /// <value>The instance awareness of the application.</value>
        public ApplicationInstanceAwareness Awareness
        {
            get { return m_Awareness; }
        }

        /// <summary>
        ///   Gets a value indicating whether the current application instance is the first one.
        /// </summary>
        /// <value>
        ///   <c>True</c> if the current application instance is the first one, otherwise <c>false</c>.
        /// </value>
        /// <remarks>
        ///   The first application instance gets notified about subsequent application instances startup.
        /// </remarks>
        public bool IsFirstInstance { get; private set; }
        #endregion

        /// <summary>
        ///   Initializes a new instance of the <see cref = "InstanceAwareApplication" /> class.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">More than one instance of the <see cref = "System.Windows.Application" /> class is created per <see cref = "System.AppDomain" />.</exception>
        public InstanceAwareApplication()
            : this(ApplicationInstanceAwareness.Host)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "InstanceAwareApplication" /> class.
        /// </summary>
        /// <param name = "awareness">The instance awareness of the application.</param>
        /// <exception cref = "System.InvalidOperationException">More than one instance of the <see cref = "System.Windows.Application" /> class is created per <see cref = "System.AppDomain" />.</exception>
        public InstanceAwareApplication(ApplicationInstanceAwareness awareness)
        {
            m_Awareness = awareness;
        }

        #region IDisposable Members
        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region IPriorApplicationInstance Members
        /// <summary>
        ///   Signals the startup of the next application instance.
        /// </summary>
        /// <param name = "args">The parameters used to run the next instance of the application.</param>
        void IPriorApplicationInstance.SignalStartupNextInstance(string[] args)
        {
            m_SignaledToFirstInstanceSemaphore.Set();

            ParameterizedThreadStart onStartupNextApplication = obj => OnStartupNextApplicationInstance((string[])obj);

            //Since the method is called asynchronously, invoke the function using the dispatcher!
            Dispatcher.BeginInvoke(onStartupNextApplication, DispatcherPriority.Background, (object)args);
        }
        #endregion

        /// <summary>
        ///   Gets the application unique identifier.
        /// </summary>
        /// <returns>The application unique identifier.</returns>
        private string GetApplicationId()
        {
            //By default, the application is marked using the entry assembly Guid!
            Assembly assembly = Assembly.GetEntryAssembly();
            var guidAttribute = assembly.GetCustomAttributes(typeof(GuidAttribute), false).FirstOrDefault(obj => (obj is GuidAttribute)) as GuidAttribute;
            return guidAttribute != null ? guidAttribute.Value : GenerateApplicationGuid().ToString();
        }

        /// <summary>
        ///   Called when the the application <see cref = "Guid" /> has to be generated.
        /// </summary>
        /// <returns>The <see cref = "Guid" /> used to identify the application univocally.</returns>
        /// <remarks>
        ///   <para>If the entry assembly is decorated with a <see cref = "System.Runtime.InteropServices.GuidAttribute" />, this function is ignored.</para>
        ///   <para>Special care must be taken when overriding this method.
        ///     <para>First of all, <c>do not call the base implementation</c>, since it just throws an <see cref = "UndefinedApplicationGuidException" /> to inform the developer that something is missing.</para>
        ///     <para>Moreover, the method must return a <see cref = "Guid" /> value which is <c>constant</c>, since it is used to mark univocally the application.</para>
        ///     <para>The encouraged approach to mark an application univocally, is marking the entry assembly with a proper <see cref = "System.Runtime.InteropServices.GuidAttribute" />; this method should be used only if such method is impractical or not possible.</para>
        ///   </para>
        /// </remarks>
        /// <exception cref = "UndefinedApplicationGuidException">If the function has not been properly overridden or the base implementation has been invoked in a <see cref = "InstanceAwareApplication" /> derived class.</exception>
        protected virtual Guid GenerateApplicationGuid()
        {
            throw new UndefinedApplicationGuidException("No application Guid has been defined, either specify a Guid attribute on the entry assembly (executable) or override the GenerateApplicationGuid method in the " + GetType() + " class");
        }

        /// <summary>
        ///   Raises the <see cref = "System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name = "e">A <see cref = "System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override sealed void OnStartup(StartupEventArgs e)
        {
            IsFirstInstance = InitializeInstance(e);
            OnStartup(e, IsFirstInstance);
        }

        /// <summary>
        ///   Initializes the application instance.
        /// </summary>
        /// <param name = "e">The <see cref = "System.Windows.StartupEventArgs" /> instance containing the event data.</param>
        /// <returns><c>True</c> if the current instance is the first application instance, otherwise <c>false</c>.</returns>
        private bool InitializeInstance(StartupEventArgs e)
        {
            string id = GetApplicationId();

            string prefix;
            IdentityReference identity;

            //Extract synchronization objects parameters...
            ExtractParameters(m_Awareness, out prefix, out identity);

            //Initialize synchornization objects...
            InitializeSynchronizationObjects(prefix + id, identity);

            //The mutex is acquired by the first instance, and never released until first application shutdown!
            bool isFirstInstance;

            try
            {
                isFirstInstance = m_FirstInstanceMutex.WaitOne(TimeSpan.FromMilliseconds(FIRST_INSTANCE_TIMEOUT_MILLISECONDS));
            }
            catch (AbandonedMutexException)
            {
                Debug.WriteLine("The previous application was probably killed, we can just handle the exception since in this case is not fatal!");
                isFirstInstance = true;
            }

            Uri uri = GetPipeUri(id + "/" + identity);
            if (isFirstInstance)
                InitializeFirstInstance(uri);
            else
            {
                if (InitializeNextInstance(uri, e.Args))
                    OnStartupSignaledToPriorApplicationSucceeded();
                else
                    OnStartupSignaledToPriorApplicationFailed();
            }

            return isFirstInstance;
        }

        /// <summary>
        ///   Initializes the synchronization objects needed to deal with multiple instances of the same application.
        /// </summary>
        /// <param name = "baseName">The base name of the synchronization objects.</param>
        /// <param name = "identity">The identity to be associated to the synchronization objects.</param>
        private void InitializeSynchronizationObjects(string baseName, IdentityReference identity)
        {
            string firstInstanceMutexName = baseName + "_FirstInstance";
            string serviceInitializationMutexName = baseName + "_ServiceInitialization";
            string serviceReadySemaphoreName = baseName + "_ServiceReady";
            string signaledToFirstInstanceSemaphoreName = baseName + "_SignaledToFirstInstance";

            bool isNew;
            var eventRule = new EventWaitHandleAccessRule(identity, EventWaitHandleRights.FullControl, AccessControlType.Allow);
            var eventSecurity = new EventWaitHandleSecurity();
            eventSecurity.AddAccessRule(eventRule);

            var mutexRule = new MutexAccessRule(identity, MutexRights.FullControl, AccessControlType.Allow);
            var mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(mutexRule);

            m_FirstInstanceMutex = new Mutex(false, firstInstanceMutexName, out isNew, mutexSecurity);
            m_ServiceInitializationMutex = new Mutex(false, serviceInitializationMutexName, out isNew, mutexSecurity);
            m_ServiceReadySemaphore = new EventWaitHandle(false, EventResetMode.ManualReset, serviceReadySemaphoreName, out isNew, eventSecurity);
            m_SignaledToFirstInstanceSemaphore = new EventWaitHandle(false, EventResetMode.AutoReset, signaledToFirstInstanceSemaphoreName, out isNew, eventSecurity);
        }

        /// <summary>
        ///   Initializes the first application instance.
        /// </summary>
        /// <param name = "uri">The <see cref = "Uri" /> used by the service that allows for inter-process communication.</param>
        private void InitializeFirstInstance(Uri uri)
        {
            try
            {
                //Acquire the mutex used to synchornize service initialization...
                m_ServiceInitializationMutex.WaitOne();

                //Create and start the service...
                m_ServiceHost = new ServiceHost(this, uri);
                m_ServiceHost.AddServiceEndpoint(typeof(IPriorApplicationInstance), new NetNamedPipeBinding(), uri);
                m_ServiceHost.Open();

                //Release the mutex used to synchornize service initialization...
                m_ServiceInitializationMutex.ReleaseMutex();

                //Signal that the service is ready, so that subsequent instances can go on...
                m_ServiceReadySemaphore.Set();
            }
            catch (Exception exc)
            {
                throw new UnexpectedInstanceAwareApplicationException("First instance failed to create service to communicate with other application instances", exc);
            }
        }

        /// <summary>
        ///   Initializes the next application instance.
        /// </summary>
        /// <param name = "uri">The <see cref = "Uri" /> used by the service that allows for inter-process communication.</param>
        /// <param name = "args">The arguments passed to the current instance.</param>
        /// <returns><c>True</c> if the prior instance was notified about curernt instance startup, otherwise <c>false</c>.</returns>
        private bool InitializeNextInstance(Uri uri, string[] args)
        {
            //Check if the service is up... wait a bit in case two applications are started simultaneously...
            if (!m_ServiceReadySemaphore.WaitOne(TimeSpan.FromMilliseconds(SERVICE_READY_TIMEOUT_MILLISECONDS), false))
                return false;

            try
            {
                IPriorApplicationInstance instance = ChannelFactory<IPriorApplicationInstance>.CreateChannel(new NetNamedPipeBinding(), new EndpointAddress(uri));
                instance.SignalStartupNextInstance(args);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Exception while signaling first application instance (signal while first application shutdown?)" + Environment.NewLine + exc, GetType().ToString());
                return false;
            }

            //If the first application does not notify back that the signal has been received, just return false...
            return m_SignaledToFirstInstanceSemaphore.WaitOne(TimeSpan.FromMilliseconds(PRIOR_INSTANCE_SIGNALED_TIMEOUT_MILLISECONDS), false);
        }

        /// <summary>
        ///   Raises the <see cref = "System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "System.Windows.StartupEventArgs" /> instance containing the event data.</param>
        /// <param name = "isFirstInstance">If set to <c>true</c> the current instance is the first application instance.</param>
        protected virtual void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            base.OnStartup(e);
        }

        /// <summary>
        ///   Called on next application instance startup.
        /// </summary>
        /// <param name = "args">The parameters used to run the next instance of the application.</param>
        private void OnStartupNextApplicationInstance(string[] args)
        {
            var e = new StartupNextInstanceEventArgs(args, true);
            OnStartupNextInstance(e);

            if (e.BringToForeground && (MainWindow != null))
            {
                (new UIPermission(UIPermissionWindow.AllWindows)).Assert();
                if (MainWindow.WindowState == WindowState.Minimized)
                    MainWindow.WindowState = WindowState.Normal;
                MainWindow.Activate();
                CodeAccessPermission.RevertAssert();
            }
        }

        /// <summary>
        ///   Raises the <see cref = "Custom.Windows.InstanceAwareApplication.StartupNextInstance" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "Custom.Windows.StartupNextInstanceEventArgs" /> instance containing the event data.</param>
        protected virtual void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            StartupNextInstanceEventHandler startupNextInstanceEvent = StartupNextInstance;
            if (startupNextInstanceEvent != null)
                startupNextInstanceEvent(this, e);
        }

        /// <summary>
        ///   Called when the startup of the current application was successfully signaled to the prior application instance.
        /// </summary>
        protected virtual void OnStartupSignaledToPriorApplicationSucceeded()
        {
        }

        /// <summary>
        ///   Called when the startup of the current application was unsuccessfully signaled to the prior application instance.
        /// </summary>
        protected virtual void OnStartupSignaledToPriorApplicationFailed()
        {
        }

        /// <summary>
        ///   Raises the <see cref = "System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name = "e">An <see cref = "System.Windows.ExitEventArgs" /> that contains the event data.</param>
        protected override sealed void OnExit(ExitEventArgs e)
        {
            //On exit, try to dispose everything related to the synchronization context and the inter-process communication service...
            TryDisposeSynchronizationObjects();
            OnExit(e, IsFirstInstance);
        }

        /// <summary>
        ///   Tries the dispose synchronization objects (if needed).
        /// </summary>
        private void TryDisposeSynchronizationObjects()
        {
            if (!m_Disposed)
            {
                m_Disposed = true;

                if (IsFirstInstance)
                {
                    //Signal other applications that the service is not ready anymore (it is, but since the application is going to shutdown, it is the same...)
                    m_ServiceReadySemaphore.Reset();

                    //Stop the service...
                    m_ServiceInitializationMutex.WaitOne();

                    try
                    {
                        if (m_ServiceHost.State == CommunicationState.Opened)
                            m_ServiceHost.Close(TimeSpan.Zero); //Shut down the service without waiting!
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Exception raised while closing service" + Environment.NewLine + exc, GetType().ToString());
                    }
                    finally
                    {
                        m_ServiceHost = null;
                    }

                    try
                    {
                        m_ServiceInitializationMutex.ReleaseMutex();
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Unable to release service initialization mutex" + Environment.NewLine + exc, GetType().Name);
                    }

                    try
                    {
                        //Release the first application mutex!
                        m_FirstInstanceMutex.ReleaseMutex();
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Unable to release first instance mutex" + Environment.NewLine + exc, GetType().Name);
                    }
                }

                m_FirstInstanceMutex.Close();
                m_FirstInstanceMutex = null;

                m_ServiceInitializationMutex.Close();
                m_ServiceInitializationMutex = null;

                m_ServiceReadySemaphore.Close();
                m_ServiceReadySemaphore = null;

                m_SignaledToFirstInstanceSemaphore.Close();
                m_SignaledToFirstInstanceSemaphore = null;
            }
        }

        /// <summary>
        ///   Raises the <see cref = "System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name = "e">An <see cref = "System.Windows.ExitEventArgs" /> that contains the event data.</param>
        /// <param name = "isFirstInstance">If set to <c>true</c>, the current application instance is the first one.</param>
        protected virtual void OnExit(ExitEventArgs e, bool isFirstInstance)
        {
            base.OnExit(e);
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name = "disposing"><c>True</c> to release both managed and unmanaged resources, <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                //Try to dispose the synchronization objects, just in case the application did not exit...
                if (Dispatcher.Thread != Thread.CurrentThread)
                    Dispatcher.Invoke((Action)TryDisposeSynchronizationObjects);
                else
                    TryDisposeSynchronizationObjects();
            }
            catch { }
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the <see cref = "InstanceAwareApplication" /> is reclaimed by garbage collection.
        /// </summary>
        ~InstanceAwareApplication()
        {
            Dispose(false);
        }
    }
}
