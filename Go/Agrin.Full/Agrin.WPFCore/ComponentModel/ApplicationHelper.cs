using System;
using System.Collections.Generic;
using System.Windows;

namespace Agrin.Helper.ComponentModel
{
    public static class ApplicationHelper
    {
        static Action _closeApplication;

        public static Action CloseApplication
        {
            get { return ApplicationHelper._closeApplication; }
            set { ApplicationHelper._closeApplication = value; }
        }

        static object _dispatcherThread;

        public static object DispatcherThread
        {
            get { return ApplicationHelper._dispatcherThread; }
            set { ApplicationHelper._dispatcherThread = value; }
        }
        public static void EnterDispatcherThreadActionForCollections(Action action)
        {
            if (DispatcherThread == null)
                return;
            ((System.Windows.Threading.Dispatcher)DispatcherThread).Invoke(System.Windows.Threading.DispatcherPriority.Background, action);
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        public static void EnterDispatcherThreadActionBegin(Action action)
        {
            if (DispatcherThread == null)
                return;
            ((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                action();
            });
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        public static void EnterDispatcherThreadAction(Action action)
        {
            if (DispatcherThread == null)
                return;
            ((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                action();
            });
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        static Dictionary<object, string> items = new Dictionary<object, string>();
        public static string GetAppResource(object key, bool nullable = false)
        {
            if (items.ContainsKey(key))
                return items[key];
            var obj = Application.Current.Resources[key];
            
            if (obj != null)
            {
                items.Add(key, obj.ToString());
                return items[key];
            }
            if (nullable)
                return "";
            return "Not Found";
        }

        public static object GetAppResourceObject(object key)
        {
            return Application.Current.Resources[key];
        }

        public static Style GetAppResourceStyle(object key)
        {
            var obj = Application.Current.Resources[key];
            if (obj is Style)
                return (Style)obj;
            return null;
        }

        public static T GetAppResource<T>(object key) where T : class
        {
            var obj = Application.Current.Resources[key];
            if (obj is T)
                return (T)obj;
            return null;
        }

        public static void CollectMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
