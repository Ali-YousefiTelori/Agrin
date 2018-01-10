using Android.Util;
using System;
namespace Xamarin.InAppBilling.Utilities
{
    internal class Logger
    {
        private const string Tag = "InApp-Billing";

        public static void Debug(string format, params object[] args)
        {
        }

        public static void Error(string format, params object[] args)
        {
            //Log.Error("InApp-Billing", string.Format(format, args));
        }

        public static void Info(string format, params object[] args)
        {
            //Log.Info("InApp-Billing", string.Format(format, args));
        }
    }
}

