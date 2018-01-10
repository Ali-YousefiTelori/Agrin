using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Helper.Threads
{
    /// <summary>
    /// اجرای اکشن در مواقع مورد نظر بدون وقفه
    /// </summary>
    public class NormalActionRunner
    {
        /// <summary>
        /// اکشن مورد نظر
        /// </summary>
        Action runAction = null;
        /// <summary>
        /// کانستراکتور کلاس
        /// </summary>
        /// <param name="run">اکشن جاری</param>
        public NormalActionRunner(Action run)
        {
            runAction = run;
        }

        /// <summary>
        /// اجرا
        /// </summary>
        public void Run()
        {
            if (!_isDispose)
                runAction();
        }

        object lockOBJ = new object();

        bool _isDispose = false;
        /// <summary>
        /// حذف از حافظه
        /// </summary>
        public void Dispose()
        {
            lock (lockOBJ)
            {
                _isDispose = true;
            }
        }
    }

    public class ActionRunner : IDisposable
    {
        bool _IsDispose = false;
        object lockObj = new object();

        public void Start(TimeSpan sleepTime, Action run, Action disposeRun)
        {
            Task task = new Task(() =>
            {
                System.Threading.Thread.Sleep(sleepTime);
                lock (lockObj)
                {
                    if (_IsDispose)
                    {
                        disposeRun?.Invoke();
                        return;
                    }
                    run();
                }
                Dispose();
            });
            task.Start();
        }

        public void StartCondition(TimeSpan sleepTime, Action run, Action disposeRun, bool skeep)
        {
            if (skeep)
            {
                lock (lockObj)
                {
                    if (_IsDispose)
                    {
                        disposeRun?.Invoke();
                        return;
                    }
                    run();
                }
                Dispose();
                return;
            }

            Task task = new Task(() =>
            {
                System.Threading.Thread.Sleep(sleepTime);
                lock (lockObj)
                {
                    if (_IsDispose)
                    {
                        disposeRun?.Invoke();
                        return;
                    }
                    run();
                }
                Dispose();
            });
            task.Start();
        }

        public void Dispose()
        {
            lock (lockObj)
            {
                _IsDispose = true;
            }
        }

        public static ActionRunner Run(TimeSpan sleepTime, Action run, Action disposeRun, bool skeep = false)
        {
            ActionRunner runner = new ActionRunner();
            if (skeep)
            {
                runner.StartCondition(sleepTime, run, disposeRun, true);
            }
            else
            {
                runner.Start(sleepTime, run, disposeRun);
            }
            return runner;
        }

    }
}
