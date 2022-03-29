using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.WPFCore.ComponentModel
{
    public class ApplicationHelperMono
    {
        public static void EnterDispatcherThreadAction(object dispatcherThread, Action action)
        {
            if (dispatcherThread == null)
                action();
            else
                EnterAction(dispatcherThread, action);
        }
        static void EnterAction(object dispatcherThread, Action action)
        {
            Agrin.Helper.ComponentModel.ApplicationHelper.EnterDispatcherThreadAction(action);
        }
    }
}
