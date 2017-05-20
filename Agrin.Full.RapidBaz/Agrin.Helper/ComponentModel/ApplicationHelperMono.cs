using System;
using System.Reflection;
using System.Windows;

namespace Agrin.Helper.ComponentModel
{
    public static class ApplicationHelperMono
    {
        public static Assembly wpfCoreAssembly;
        public static Assembly OSAssembly;
        static ApplicationHelperMono()
        {
            string path = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "RapidbazPlus.ViewModels.dll");
            if (System.IO.File.Exists(path))
            {
                try
                {
                    wpfCoreAssembly = Assembly.LoadFile(path);
                    if (wpfCoreAssembly != null)
                        DispatcherThread = wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetProperty("DispatcherThread").GetValue(null, null);
                }
                catch
                {

                }
            }
            path = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "RapidbazPlus.OS.dll");
            if (System.IO.File.Exists(path))
            {
                try
                {
                    OSAssembly = Assembly.LoadFile(path);
                }
                catch
                {

                }
            }
        }

        static Action _closeApplication;

        public static Action CloseApplication
        {
            get { return ApplicationHelperMono._closeApplication; }
            set { ApplicationHelperMono._closeApplication = value; }
        }
        public static void EnterDispatcherThreadActionBegin(Action action)
        {
            if (wpfCoreAssembly == null)
                action();
            else
                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadActionBegin").Invoke(null, new object[] { action });
        }

        public static void EnterDispatcherThreadAction(Action action)
        {
            if (wpfCoreAssembly == null)
                action();
            else
                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadAction").Invoke(null, new object[] { action });
        }

        public static void EnterDispatcherThreadByDispatcherAction(Action action, object dispatcher)
        {
            if (wpfCoreAssembly == null)
                action();
            else
                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadByDispatcherAction").Invoke(null, new object[] { action, dispatcher });
        }

        public static void EnterDispatcherThreadActionForCollections(Action action)
        {
            if (wpfCoreAssembly == null)
                action();
            else
                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadActionForCollections").Invoke(null, new object[] { action });
        }

        public static void EnterDispatcherThreadActionForCollectionsByDispatcher(Action action, object dispatcher)
        {
            if (wpfCoreAssembly == null)
                action();
            else
                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadActionForCollectionsByDispatcher").Invoke(null, new object[] { action, dispatcher });
        }

        public static string GetAppResource(object key, bool nullable = false)
        {
            if (wpfCoreAssembly == null)
            {
                if (nullable)
                    return "";
                else
                    return "Not Found";
            }
            else
            {
                var type = wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper");
                var method = type.GetMethod("GetAppResource", new Type[] { key.GetType(), nullable.GetType() });
                return method.Invoke(null, new object[] { key, nullable }).ToString();
            }
        }

        public static object GetAppResourceObject(object key)
        {
            if (wpfCoreAssembly == null)
                return null;
            else
                return wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("GetAppResourceObject").Invoke(null, new object[] { key });
        }

        public static void SetShutDown(int state)
        {
            if (state == 0)
                state = 1;
            else if (state == 1)
                state = 6;
            else
                state = 2;
            if (OSAssembly != null)
                OSAssembly.GetType("Agrin.OS.Management.WindowsController").GetMethod("SetPowerState").Invoke(null, new object[] { state });
        }
        public static void CheckInternetAndResetModem()
        {
            if (OSAssembly != null)
                OSAssembly.GetType("Agrin.OS.Management.Modem").GetMethod("CheckInternetAndResetModem").Invoke(null, null);
        }
        public static void CollectMemory()
        {
#if (MobileApp)
            GC.Collect();
#else

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
#endif
        }

        public static object DispatcherThread { get; set; }

    }
}
