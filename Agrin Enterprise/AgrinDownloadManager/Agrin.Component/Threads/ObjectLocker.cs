using Agrin.Interfaces;
using Agrin.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Threads
{
    /// <summary>
    /// lock object by string key name
    /// </summary>
    public static class ObjectLocker
    {
        //static ConcurrentDictionary<IObjectDisposable, object> LockerList = new ConcurrentDictionary<IObjectDisposable, object>();
        //static object lockerObj = new object();

        /// <summary>
        /// take an object for lock
        /// </summary>
        /// <param name="key"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        public static void RunInLock(this IObjectDisposable key, Action run)
        {
            if (key == null || key.IsDispose)
            {
                return;
            }
            //object obj = null;
            //lock (lockerObj)
            //{
            //    if (key.IsDispose)
            //        return;
            //    if (!LockerList.ContainsKey(key))
            //        LockerList.TryAdd(key, new object());
            //    obj = LockerList[key];
            //}

            lock (key)
            {
                if (key.IsDispose)
                    return;
                try
                {
                    run();
                }
                catch (Exception ex)
                {
                    if (!key.IsDispose)
                    {
                        AutoLogger.LogError(ex, "RunInLock");
                    }
                }
            }
        }

        /// <summary>
        /// take an object for lock
        /// </summary>
        /// <param name="key"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        public static T RunInLock<T>(this IObjectDisposable key, Func<T> run)
        {
            if (key.IsDispose)
                return default(T);
            //object obj = null;
            //lock (lockerObj)
            //{
            //    if (!LockerList.ContainsKey(key))
            //        LockerList.TryAdd(key, new object());
            //    obj = LockerList[key];
            //}

            lock (key)
            {
                if (key.IsDispose)
                    return default(T);
                return run();
            }
        }

        /// <summary>
        /// when object disposed object is going to remove from list
        /// </summary>
        /// <param name="key"></param>
        public static void ObjectDisposed(IObjectDisposable key)
        {
            //if (LockerList.ContainsKey(key))
            //    LockerList.TryRemove(key, out object value);
        }
    }
}
