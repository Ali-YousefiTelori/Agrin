using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Agrin.Helper.ComponentModel
{
    public class ThreadStackTrace
    {
        public Thread CurrentThread { get; set; }
        public StackTrace Stack { get; set; }
    }

    public static class AsyncActions
    {
        static AsyncActions()
        {
            Agrin.Log.AutoLogger.GetAllThreadStacks = () =>
            {
                var currentThread = Thread.CurrentThread;
                List<StackTrace> stacks = new List<StackTrace>();

                while (true)
                {
                    if (AllStackTasks.ContainsKey(currentThread))
                    {
                        stacks.Add(AllStackTasks[currentThread].Stack);
                        currentThread = AllStackTasks[currentThread].CurrentThread;
                    }
                    else
                        break;
                }
                return stacks.ToArray();
            };
        }
        //static object objLock = new object();
        //public static Task BeginAction(Action action, Action<Exception> errorReport, TimeSpan time)
        //{
        //    Task thread = null;
        //    thread = new Task(() =>
        //    {
        //        try
        //        {
        //            Thread.Sleep(time);
        //            lock (objLock)
        //                action();
        //        }
        //        catch (Exception e)
        //        {
        //            RunErrorReport(errorReport, e);
        //        }
        //    });
        //    thread.Start();
        //    return thread;
        //}


        public static void Action(Action action)
        {
            Action(action, null, true);
        }

        public static void Action(Action action, Action<Exception> errorReport)
        {
            Action(action, errorReport, true);
        }

        /// <summary>
        /// تسک اول تسکی هست که تازه ساخته شده و تسک دوم تسکی هست این تسک ساخته شده توشه یعنی پدرشه.
        /// </summary>
        public static Dictionary<Thread, ThreadStackTrace> AllStackTasks = new Dictionary<Thread, ThreadStackTrace>();
        static object addremoveLock = new object();
        public static void Action(Action action, Action<Exception> errorReport, bool isThread)
        {
            if (isThread)
            {
                Thread thread = null;
                thread = new Thread(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        RunErrorReport(errorReport, e);
                    }
                    lock (addremoveLock)
                        AllStackTasks.Remove(thread);
                });
                lock (addremoveLock)
                    AllStackTasks.Add(thread, new ThreadStackTrace() { CurrentThread = Thread.CurrentThread, Stack = new StackTrace(true) });
                thread.Start();
            }
            else
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    RunErrorReport(errorReport, e);
                }
            }
        }

        static void RunErrorReport(Action<Exception> errorReport, Exception e)
        {
            if (errorReport != null)
            {
                try
                {
                    if (errorReport == null)
                        Agrin.Log.AutoLogger.LogError(e, "RunErrorReport 0");
                    else
                        errorReport(e);
                }
                catch (Exception c)
                {
                    Agrin.Log.AutoLogger.LogError(c, "RunErrorReport 1");
                }
            }
            else
                Agrin.Log.AutoLogger.LogError(e, "RunErrorReport 2");
        }
    }
}
