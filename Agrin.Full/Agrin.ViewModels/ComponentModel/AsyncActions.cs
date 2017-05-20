using System;
using System.Threading;

namespace Agrin.Helper.ComponentModel
{
    public static class AsyncActions
    {
        public static void Action(Action action)
        {
            Action(action, null, true);
        }

        public static void Action(Action action, Action<Exception> errorReport)
        {
            Action(action, errorReport, true);
        }

        public static void Action(Action action, Action<Exception> errorReport, bool isThread)
        {
            if (isThread)
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        if (errorReport != null)
                            errorReport(e);
                    }
                });
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
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
                    if (errorReport != null)
                        errorReport(e);
                }
            }
        }
    }
}
