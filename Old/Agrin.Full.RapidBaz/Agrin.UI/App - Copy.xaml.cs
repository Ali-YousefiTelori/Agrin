using Agrin.Log;
using Agrin.UI.Helper.SingleApplication;
using Agrin.UI.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Agrin.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : InstanceAwareApplication
    {
        static bool _AutoRun;
        public static bool AutoRun
        {
            get { return App._AutoRun; }
            set { App._AutoRun = value; }
        }

        static bool _CanStart = true;
        public static bool CanStart
        {
            get { return _CanStart; }
            set { _CanStart = value; }
        }

        public App()
            : base(ApplicationInstanceAwareness.Host)
        {
        }
#if !USE_ASSEMBLY_GUID
        /// <summary>
        /// Called when the the application <see cref="Guid"/> has to be generated.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/> used to identify the application univocally.
        /// </returns>
        /// <remarks>
        /// 	<para>If the entry assembly is decorated with a <see cref="System.Runtime.InteropServices.GuidAttribute"/>, this function is ignored.</para>
        /// 	<para>Special care must be taken when overriding this method.
        /// <para>First of all, <c>do not call the base implementation</c>, since it just throws an <see cref="UndefinedApplicationGuidException"/> to inform the developer that something is missing.</para>
        /// 		<para>Moreover, the method must return a <see cref="Guid"/> value which is <c>constant</c>, since it is used to mark univocally the application.</para>
        /// 		<para>The encouraged approach to mark an application univocally, is marking the entry assembly with a proper <see cref="System.Runtime.InteropServices.GuidAttribute"/>; this method should be used only if such method is impractical or not possible.</para>
        /// 	</para>
        /// </remarks>
        /// <exception cref="UndefinedApplicationGuidException">If the function has not been properly overridden or the base implementation has been invoked in a <see cref="InstanceAwareApplication"/> derived class.</exception>
        protected override Guid GenerateApplicationGuid()
        {
            return new Guid("A2048947-1DBF-492b-AADF-3DCFC8B24801");
        }
#endif
        //public static void SaveErrorLog(string data)
        //{
        //    string dt = "";
        //    if (System.IO.File.Exists(Agrin.IO.Helper.MPath.CurrentAppDirectory + "\\errors.log"))
        //        dt = System.IO.File.ReadAllText(Agrin.IO.Helper.MPath.CurrentAppDirectory + "\\errors.log");
        //    dt += Environment.NewLine + "NEW LINE Error : " + Environment.NewLine + data;
        //    string errorDataFile = Agrin.IO.Helper.MPath.CurrentAppDirectory + "\\errors.data";
        //    System.IO.File.WriteAllText(Agrin.IO.Helper.MPath.CurrentAppDirectory + "\\errors.log", dt);
        //    Agrin.IO.Helper.SerializeStream.SaveSerializeStream(errorDataFile, Log.AutoLogger.GetLogsForSave());

        //    try
        //    {
        //        System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(Agrin.UI.AgrinRsourceFiles.Agrin_About);
        //        var msg = Activator.CreateInstance(assembly.GetType("Agrin.About.SendMessage"));
        //        System.Reflection.MethodInfo method = msg.GetType().GetMethod("SendMessages", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        //        //var paramsssss= method.GetParameters();
        //        bool isSend = (bool)method.Invoke(msg, new object[] { "AVS", "ali.visual.studio@gmail.com", "Error Report", dt, new string[] { errorDataFile } });
        //    }
        //    catch
        //    {

        //    }
        //}
        /// <summary>
        ///   Raises the <see cref = "Application.Startup" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "System.Windows.StartupEventArgs" /> instance containing the event data.</param>
        /// <param name = "isFirstInstance">If set to <c>true</c> the current instance is the first application instance.</param>
        protected override void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            base.OnStartup(e, isFirstInstance);
            AutoRun = e.Args.Contains("autoRun");
            if (!isFirstInstance)
            {
                CanStart = false;
                Shutdown(1);
            }
            else
            {
                //MainWindow.args = e.Args;
                this.DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
                LoadingWindow loadingWindow = null;
                object lockOBJ = new object();
                bool loaded = false;
                Thread newWindowThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        lock (lockOBJ)
                        {
                            if (!loaded)
                            {
                                loadingWindow = new LoadingWindow();
                                loadingWindow.Show();
                                loadingWindow.Activate();
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "loadingWindow", true);
                    }
                    // Start the Dispatcher Processing
                    System.Windows.Threading.Dispatcher.Run();
                }));

                newWindowThread.SetApartmentState(ApartmentState.STA);
                // Make the thread a background thread
                newWindowThread.IsBackground = true;
                // Start the thread
                newWindowThread.Start();
                try
                {
                    MainWindow windows = new MainWindow();
                    windows.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
                    windows.Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;
                    windows.Show();
                    lock (lockOBJ)
                    {
                        loaded = true;
                        if (loadingWindow != null)
                        {
                            loadingWindow.Dispatcher.Invoke(new Action(() =>
                                {
                                    loadingWindow.Close();
                                }));

                        }
                    }

                    System.Windows.Threading.Dispatcher.Run();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                catch (Exception ex)
                {
                    AutoLogger.LogError(ex, "Unhandled Exception 7", true);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Custom.Windows.InstanceAwareApplication.StartupNextInstance"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Custom.Windows.StartupNextInstanceEventArgs"/> instance containing the event data.</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);
        }

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            AutoLogger.LogError(e.Exception, "Unhandled Exception 4", true);
            e.Handled = true;
        }

        void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            AutoLogger.LogError(e.Exception, "Unhandled Exception 6", true);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show("App UnhandledException");
            AutoLogger.LogError(e.Exception, "Unhandled Exception 1", true);
            e.Handled = true;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
                AutoLogger.LogError((Exception)e.ExceptionObject, "Unhandled Exception 5", true);
            else if (e.ExceptionObject != null)
                AutoLogger.LogError(null, "Unhandled Exception 5: " + e.ExceptionObject.ToString(), true);
            else
                AutoLogger.LogError(null, "Null Unhandled Exception 5 !", true);
        }

        void Dispatcher_UnhandledExceptionFilter(object sender, System.Windows.Threading.DispatcherUnhandledExceptionFilterEventArgs e)
        {
            e.RequestCatch = true;
            AutoLogger.LogError(e.Exception, "Unhandled Exception 3", true);
        }

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            AutoLogger.LogError(e.Exception, "Unhandled Exception 2", true);
        }
    }
}
