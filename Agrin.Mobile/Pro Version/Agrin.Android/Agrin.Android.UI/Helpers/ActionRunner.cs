using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Agrin.Helpers
{
    public static class ActionRunner
    {
        static ActionRunner()
        {
            RunEngine();
        }

        static object lockOBJ = new object();
        static Dictionary<object, Action> Items = new Dictionary<object, Action>();
        public static void AddAction(object obj, Action action)
        {
            lock (lockOBJ)
            {
                if (Items.ContainsKey(obj))
                    return;
                Items.Add(obj, action);
            }
        }

        static bool _isRun = false;
        static void RunEngine()
        {
            if (_isRun)
                return;
            _isRun = true;
            Task task = new Task(() =>
            {
                while (true)
                {
                    lock (lockOBJ)
                    {

                        if (Items.Count > 0)
                        {
                            var first = Items.First();
                            try
                            {
                                first.Value();
                            }
                            catch (Exception ex)
                            {
                                InitializeApplication.GoException(ex, "RunEngine : " + first.Key.GetType().FullName);
                            }
                            Items.Remove(first.Key);
                        }
                    }
                    Thread.Sleep(500);
                }
            });
            task.Start();
        }
    }
}