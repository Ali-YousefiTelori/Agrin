using Agrin.BaseViewModels.Link;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using Agrin.IO.Streams;
using Agrin.Log;
using Agrin.ViewModels.Windows;
using Agrin.Windows.UI.ViewModels.Lists;
using Agrin.Windows.UI.ViewModels.Toolbox;
using Agrin.Windows.UI.Views.WindowLayouts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;

namespace Agrin.Windows.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application//, ISingleInstanceApp
    {
        private const string Unique = "Agrin Download Manager";

        [STAThread]
        public static void Main()
        {
            //if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            //{
                var application = new App();
                application.Startup += OnStartup;
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                //SingleInstance<App>.Cleanup();
            //}
        }

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        //[System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;

            this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream stream = a.GetManifestResourceStream("Agrin.Windows.UI.MergedResources.xaml"))
            {
                XmlReader XmlRead = XmlReader.Create(stream);
                Application.Current.Resources = (ResourceDictionary)XamlReader.Load(XmlRead);
                //foreach (var item in Application.Current.Resources)
                //{

                //}
                //foreach (var item in ((ResourceDictionary)XamlReader.Load(XmlRead)))
                //{
                //    string no = item.ToString();
                //    //Application.Current.Resources.Add(item)

                //}
                XmlRead.Close();
            }
        }
        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            try
            {
                if (args != null)
                {
                    var links = GetLinksFromArgs(args.ToArray());
                    if (links.Count != 0)
                    {
                        LinksViewModel.AddLinkListItem(links, links.First(), false);
                    }
                }
                if (Agrin.Windows.UI.MainWindow.This.IsNotify)
                {
                    Agrin.Windows.UI.MainWindow.This.IsNotify = false;
                }
            }
            catch (Exception er)
            {
                AutoLogger.LogError(er, "FlashGot Get Data Error");
            }

            return true;
        }

        #endregion

        /// <summary>
        ///   Raises the <see cref = "Application.Startup" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "System.Windows.StartupEventArgs" /> instance containing the event data.</param>
        /// <param name = "isFirstInstance">If set to <c>true</c> the current instance is the first application instance.</param>
        static void OnStartup(object sender, StartupEventArgs e)
        {
            App application = sender as App;
            try
            {
                var links = application.GetLinksFromArgs(e.Args);
                if (links.Count != 0)
                {
                    Agrin.Windows.UI.MainWindow.LinksMustAdd = links;
                }
            }
            catch (Exception er)
            {
                AutoLogger.LogError(er, "FlashGot Get Data Error");
            }
            //MainWindow.args = e.Args;
            application.DispatcherUnhandledException += application.App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += application.CurrentDomain_UnhandledException;
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += application.TaskScheduler_UnobservedTaskException;
            Application.Current.DispatcherUnhandledException += application.Current_DispatcherUnhandledException;
            IOHelperBase.Current = new IOHelperBase();
            StreamCross.OpenFile = new Func<string, FileMode, FileAccess, IStreamWriter>((path, mode, access) =>
            {
                return new IO.Streams.StreamWriter(IOHelperBase.Current.Open(path, mode, access));
            });
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
                windows.Dispatcher.UnhandledException += application.Dispatcher_UnhandledException;
                windows.Dispatcher.UnhandledExceptionFilter += application.Dispatcher_UnhandledExceptionFilter;
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
                windows.ShowDialog();


                System.Windows.Threading.Dispatcher.Run();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "Unhandled Exception 7", true);
            }
        }

        public List<string> GetLinksFromArgs(string[] args)
        {
            List<string> links = new List<string>();
            foreach (var item in args)
            {
                foreach (var link in Agrin.IO.Strings.HtmlPage.ExtractLinksFromHtmlTwo(item))
                {
                    links.Add(link);
                }
            }
            return links;
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
