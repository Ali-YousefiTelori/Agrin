using Android.Util;
using System;
namespace Xamarin.InAppBilling.Utilities
{
    internal class Logger
    {
        private const string Tag = "InApp-Billing";

        public static void Debug(string format, params object[] args)
        {
            //Agrin.Log.AutoLogger.LogText("InApp-Billing Debug" + string.Format(format, args));
        }

        public static void Error(string format, params object[] args)
        {
            //Agrin.Log.AutoLogger.LogText("InApp-Billing" + string.Format(format, args));
            //Log.Error("InApp-Billing", string.Format(format, args));
        }

        public static void Info(string format, params object[] args)
        {
            //Agrin.Log.AutoLogger.LogText("InApp-Billing Info" + string.Format(format, args));
            //Log.Info("InApp-Billing", string.Format(format, args));
        }
    }
}

