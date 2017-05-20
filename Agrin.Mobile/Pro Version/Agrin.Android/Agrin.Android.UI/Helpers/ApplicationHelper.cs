using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Agrin.Helper.ComponentModel;
using System.Threading;
using Agrin.Log;
using Agrin.Views;

namespace Agrin.Helpers
{
    public class ApplicationHelper : IApplicationHelper
    {
        Action _closeApplication;

        public Action CloseApplication
        {
            get { return _closeApplication; }
            set { _closeApplication = value; }
        }

        object _dispatcherThread;

        public object DispatcherThread
        {
            get { return _dispatcherThread; }
            set { _dispatcherThread = value; }
        }

        public Action RefreshCommandAction { get; set; }
        public Action RunOnUIThread { get; set; }
        public Dictionary<Thread, object> Dispatchers { get; set; }

        public void AddThreadDispather(object dispatcher)
        {
            if (Dispatchers == null)
                Dispatchers = new Dictionary<Thread, object>();
            Dispatchers.Add(Thread.CurrentThread, dispatcher);
        }

        public void EnterDispatcherThreadActionForCollections(Action action, object dispatcher = null)
        {
            action();
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        public void EnterDispatcherThreadActionBegin(Action action)
        {
            action();
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }

        public void EnterDispatcherThreadActionBegin(Action action, object dispatcher = null)
        {
            action();
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        /// <summary>
        /// من اینو BeginInvoke کردم چون گاهی اوقات هنگ میکرد و لاک میشد.
        /// </summary>
        /// <param name="action"></param>
        public void EnterDispatcherThreadAction(Action action, object dispatcher = null)
        {
            action();
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }

        Dictionary<object, string> items = new Dictionary<object, string>();
        object lockObj = new object();
        public string GetAppResource(object key, bool nullable = false)
        {
            lock (lockObj)
            {
                return ViewsUtility.FindTextLanguage(MainActivity.This, key.ToString());
                //if (items.ContainsKey(key))
                //    return items[key];
                //var obj = Application.Current.Resources[key];

                //if (obj != null)
                //{
                //    items.Add(key, obj.ToString());
                //    return items[key];
                //}
                //if (nullable)
                //    return "";
                //return "Not Found";
            }
        }

        public object GetAppResourceObject(object key)
        {
            AutoLogger.LogText("AppHelper GetAppResourceObject " + key == null ? "null" : key.ToString());
            return null;
        }

        public T GetAppResourceStyle<T>(object key) where T : class
        {
            AutoLogger.LogText("AppHelper GetAppResourceStyle " + key == null ? "null" : key.ToString());
            return null;
        }

        public T GetAppResource<T>(object key) where T : class
        {
            AutoLogger.LogText("AppHelper GetAppResource " + key == null ? "null" : key.ToString());
            return null;
        }

        public void CollectMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public object GetDispatcherByThread(Thread thread)
        {
            try
            {
                if (Dispatchers == null)
                    return null;
                if (Dispatchers.ContainsKey(thread))
                    return Dispatchers[thread];
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "GetDispatcherByThread");
            }
            return null;
        }

        public void SetShutDown(int state)
        {
            if (state == 0)
                state = 1;
            else if (state == 1)
                state = 6;
            else
                state = 2;
            WindowsControllerBase.Current.SetPowerState((ComputerPowerState)state);
        }
    }
}
