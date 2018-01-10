using Agrin.Helper.ComponentModel;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace Agrin.ViewModels.Helper.ComponentModel
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
            var disp = DispatcherThread;
            if (DispatcherThread == null && dispatcher == null)
                return;
            if (dispatcher != null)
                disp = dispatcher;
            ((System.Windows.Threading.Dispatcher)disp).Invoke(System.Windows.Threading.DispatcherPriority.DataBind, action);
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        public void EnterDispatcherThreadActionBegin(Action action)
        {
            if (DispatcherThread == null)
                return;
            ((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                action();
            });
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        public void EnterDispatcherThreadActionBegin(Action action, object dispatcher = null)
        {
            var disp = DispatcherThread;
            if (DispatcherThread == null && dispatcher == null)
                return;
            if (dispatcher != null)
                disp = dispatcher;

            ((System.Windows.Threading.Dispatcher)disp).BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                action();
            });
            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }
        /// <summary>
        /// من اینو BeginInvoke کردم چون گاهی اوقات هنگ میکرد و لاک میشد.
        /// </summary>
        /// <param name="action"></param>
        public void EnterDispatcherThreadAction(Action action, object dispatcher = null)
        {
            var disp = DispatcherThread;
            if (DispatcherThread == null && dispatcher == null)
                return;
            if (dispatcher != null)
                disp = dispatcher;
            if (((System.Windows.Threading.Dispatcher)disp).Thread == System.Threading.Thread.CurrentThread)
            {
                //Log.AutoLogger.LogText("Lock To Lock Skeep");
                action();
            }
            else
            {
                ((System.Windows.Threading.Dispatcher)disp).BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
                {
                    action();
                });
            }

            //((System.Windows.Threading.Dispatcher)DispatcherThread).BeginInvoke(action, new TimeSpan(0, 0, 2));
        }

        Dictionary<object, string> items = new Dictionary<object, string>();
        object lockObj = new object();
        public string GetAppResource(object key, bool nullable = false)
        {
            lock (lockObj)
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
        }

        public object GetAppResourceObject(object key)
        {
            return Application.Current.Resources[key];
        }

        public T GetAppResourceStyle<T>(object key) where T : class
        {
            var obj = Application.Current.Resources[key];
            if (obj is Style)
                return (T)obj;
            return null;
        }

        public T GetAppResource<T>(object key) where T : class
        {
            var obj = Application.Current.Resources[key];
            if (obj is T)
                return (T)obj;
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
