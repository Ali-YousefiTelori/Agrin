using Agrin.IO.Helper;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Agrin.Helper.ComponentModel
{
    public static class ApplicationHelperBase
    {
        public static IApplicationHelper Current { get; set; }

        public static Action CloseApplication
        {
            get
            {
                return Current.CloseApplication;
            }
            set
            {
                Current.CloseApplication = value;
            }
        }

        public static Dictionary<Thread, object> Dispatchers
        {
            get
            {
                return Current.Dispatchers;
            }

            set
            {
                Current.Dispatchers = value;
            }
        }

        public static object DispatcherThread
        {
            get
            {
                return Current.DispatcherThread;
            }

            set
            {
                Current.DispatcherThread = value;
            }
        }

        public static Action RefreshCommandAction
        {
            get
            {
                return Current.RefreshCommandAction;
            }

            set
            {
                Current.RefreshCommandAction = value;
            }
        }

        public static Action RunOnUIThread
        {
            get
            {
                return Current.RunOnUIThread;
            }

            set
            {
                Current.RunOnUIThread = value;
            }
        }

        public static void AddThreadDispather(object dispatcher)
        {
            Current.AddThreadDispather(dispatcher);
        }

        public static void CollectMemory()
        {
            Current.CollectMemory();
        }

        public static void EnterDispatcherThreadAction(Action action, object dispatcher = null)
        {
            Current.EnterDispatcherThreadAction(action, dispatcher);
        }

        public static void EnterDispatcherThreadActionBegin(Action action)
        {
            Current.EnterDispatcherThreadActionBegin(action);
        }

        public static void EnterDispatcherThreadActionBegin(Action action, object dispatcher = null)
        {
            Current.EnterDispatcherThreadActionBegin(action, dispatcher);
        }

        public static void EnterDispatcherThreadActionForCollections(Action action, object dispatcher = null)
        {
            Current.EnterDispatcherThreadActionForCollections(action, dispatcher);
        }

        public static string GetAppResource(object key, bool nullable = false)
        {
            return Current.GetAppResource(key, nullable);
        }

        public static T GetAppResource<T>(object key) where T : class
        {
            return Current.GetAppResource<T>(key);
        }

        public static object GetAppResourceObject(object key)
        {
            return Current.GetAppResourceObject(key);

        }

        public static T GetAppResourceStyle<T>(object key) where T : class
        {
            return Current.GetAppResourceStyle<T>(key);
        }

        public static object GetDispatcherByThread(Thread thread)
        {
            return Current.GetDispatcherByThread(thread);
        }

        public static void SetShutDown(int state)
        {
             Current.SetShutDown(state);
        }

        //public static object GetDispatcherByThread(Thread thread)
        //{
        //    throw new NotImplementedException();
        //}
        //        public static Action RefreshCommandAction { get; set; }
        //        public static Action RunOnUIThread { get; set; }
        //        public static Dictionary<Thread, object> Dispatchers { get; set; }

        //        public static void AddThreadDispather(object dispatcher)
        //        {
        //            if (Dispatchers == null)
        //                Dispatchers = new Dictionary<Thread, object>();
        //            Dispatchers.Add(Thread.CurrentThread, dispatcher);
        //        }

        //        public static object GetDispatcherByThread(Thread thread)
        //        {
        //            try
        //            {
        //                if (Dispatchers == null)
        //                    return null;
        //                if (Dispatchers.ContainsKey(thread))
        //                    return Dispatchers[thread];
        //            }
        //            catch (Exception ex)
        //            {
        //                AutoLogger.LogError(ex, "GetDispatcherByThread");
        //            }
        //            return null;
        //        }
        //        public static Assembly wpfCoreAssembly;
        //        public static Assembly OSAssembly;
        //        static ApplicationHelperMono()
        //        {
        //            MPath.InitedAction = () =>
        //            {
        //                string path = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "Agrin.ViewModels.dll");
        //                if (System.IO.File.Exists(path))
        //                {
        //                    try
        //                    {
        //                        wpfCoreAssembly = Assembly.LoadFile(path);
        //                        if (wpfCoreAssembly != null)
        //                            DispatcherThread = wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetProperty("DispatcherThread").GetValue(null, null);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        AutoLogger.LogError(ex, "ApplicationHelperMono");
        //                    }
        //                }
        //                path = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "Agrin.OS.dll");
        //                if (System.IO.File.Exists(path))
        //                {
        //                    try
        //                    {
        //                        OSAssembly = Assembly.LoadFile(path);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        AutoLogger.LogError(ex, "ApplicationHelperMono 2");
        //                    }
        //                }
        //            };

        //        }

        //        static Action _closeApplication;

        //        public static Action CloseApplication
        //        {
        //            get { return ApplicationHelperMono._closeApplication; }
        //            set { ApplicationHelperMono._closeApplication = value; }
        //        }
        //        public static void EnterDispatcherThreadActionBegin(Action action)
        //        {
        //            if (wpfCoreAssembly == null)
        //                action();
        //            else
        //                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadActionBegin").Invoke(null, new object[] { action });
        //        }
        //        public static void EnterDispatcherThreadAction(Action action, object dispatcher = null)
        //        {
        //            if (wpfCoreAssembly == null)
        //                action();
        //            else
        //                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadAction").Invoke(null, new object[] { action, dispatcher });
        //        }
        //        public static void EnterDispatcherThreadActionForCollections(Action action, object dispatcher = null)
        //        {
        //            if (wpfCoreAssembly == null)
        //                action();
        //            else
        //                wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("EnterDispatcherThreadActionForCollections").Invoke(null, new object[] { action, dispatcher });
        //        }
        //        public static string GetAppResource(object key, bool nullable = false)
        //        {
        //            if (wpfCoreAssembly == null)
        //            {
        //                if (nullable)
        //                    return "";
        //                else
        //                    return "Not Found";
        //            }
        //            else
        //            {
        //                var type = wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper");
        //                var method = type.GetMethod("GetAppResource", new Type[] { key.GetType(), nullable.GetType() });
        //                return method.Invoke(null, new object[] { key, nullable }).ToString();
        //            }
        //        }

        //        public static object GetAppResourceObject(object key)
        //        {
        //            if (wpfCoreAssembly == null)
        //                return null;
        //            else
        //                return wpfCoreAssembly.GetType("Agrin.ViewModels.Helper.ComponentModel.ApplicationHelper").GetMethod("GetAppResourceObject").Invoke(null, new object[] { key });
        //        }

        //        public static void SetShutDown(int state)
        //        {
        //            if (state == 0)
        //                state = 1;
        //            else if (state == 1)
        //                state = 6;
        //            else
        //                state = 2;
        //            if (OSAssembly != null)
        //                OSAssembly.GetType("Agrin.OS.Management.WindowsController").GetMethod("SetPowerState").Invoke(null, new object[] { state });
        //        }
        //        public static void CheckInternetAndResetModem()
        //        {
        //            if (OSAssembly != null)
        //                OSAssembly.GetType("Agrin.OS.Management.Modem").GetMethod("CheckInternetAndResetModem").Invoke(null, null);
        //        }
        //        public static void CollectMemory()
        //        {
        //#if (MobileApp)
        //            GC.Collect();
        //#else

        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //            GC.Collect();
        //#endif
        //        }

        //        public static object DispatcherThread { get; set; }

    }
}
