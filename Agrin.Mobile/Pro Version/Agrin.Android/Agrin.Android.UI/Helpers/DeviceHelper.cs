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
using Android.Net;
using Java.Lang;
using Java.Lang.Reflect;
using Android.Net.Wifi;
using Android.Telephony;
using System.IO;

namespace Agrin.Helpers
{
    public static class DeviceHelper
    {
        public static bool IsRootMode(Context context)
        {
            try
            {
                ConnectivityManager conman = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                Class conmanClass = Class.ForName(conman.Class.Name);
                Field iConnectivityManagerField = conmanClass.GetDeclaredField("mService");
                iConnectivityManagerField.Accessible = true;
                Java.Lang.Object iConnectivityManager = iConnectivityManagerField.Get(conman);
                Class iConnectivityManagerClass = Class.ForName(iConnectivityManager.Class.Name);
                bool exist = false;
                foreach (var method in iConnectivityManagerClass.GetDeclaredMethods())
                {
                    if (method.Name.ToLower() == "setmobiledataenabled")
                    {
                        exist = true;
                        break;
                    }
                }
                return !exist;
            }
            catch (System.Exception e)
            {
                InitializeApplication.GoException(e, "IsRootMode Check");
                return true;
            }
        }

        public static void SetMobileDataEnabled(Context context, bool enabled)
        {
            if (IsRootMode(context))
            {
                SetMobileDataEnabledRoot(context, enabled);
            }
            else
            {
                SetMobileDataEnabledNormal(context, enabled);
            }
        }

        static void SetMobileDataEnabledNormal(Context context, bool enabled)
        {
            try
            {
                ConnectivityManager conman = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                Class conmanClass = Class.ForName(conman.Class.Name);
                Field iConnectivityManagerField = conmanClass.GetDeclaredField("mService");
                iConnectivityManagerField.Accessible = true;
                Java.Lang.Object iConnectivityManager = iConnectivityManagerField.Get(conman);
                Class iConnectivityManagerClass = Class.ForName(iConnectivityManager.Class.Name);
                bool exist = false;
                foreach (var method in iConnectivityManagerClass.GetDeclaredMethods())
                {
                    if (method.Name.ToLower() == "setmobiledataenabled")
                    {
                        exist = true;
                        break;
                    }
                }
                Method setMobileDataEnabledMethod = null;
                if (exist)
                    setMobileDataEnabledMethod = iConnectivityManagerClass.GetDeclaredMethod("setMobileDataEnabled", Java.Lang.Boolean.Type);
                else
                    setMobileDataEnabledMethod = iConnectivityManagerClass.GetDeclaredMethod("setMobileDataEnabled", Java.Lang.Boolean.Type);
                setMobileDataEnabledMethod.Accessible = true;
                setMobileDataEnabledMethod.Invoke(iConnectivityManager, enabled);
            }
            catch (System.Exception e)
            {
                InitializeApplication.GoException(e, "SetMobileDataEnabled");
            }
        }

        //static void SetMobileDataEnabledRoot(Context context, bool mobileDataEnabled)
        //{
        //    try
        //    {
        //        TelephonyManager telephonyService = (TelephonyManager)context.GetSystemService(Context.TelephonyService);

        //        Method setMobileDataEnabledMethod = telephonyService.Class.GetDeclaredMethod("setDataEnabled", Java.Lang.Boolean.Type);
        //        if (null != setMobileDataEnabledMethod)
        //        {
        //            setMobileDataEnabledMethod.Invoke(telephonyService, mobileDataEnabled);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        InitializeApplication.GoException(ex, "SetMobileDataEnabledRoot");
        //    }
        //}
        static void SetMobileDataEnabledRoot(Context context, bool mobileDataEnabled)
        {
            try
            {
                // Requires: android.permission.CHANGE_NETWORK_STATE
                if ((int)Build.VERSION.SdkInt < 9)
                {
                    // pre-Gingerbread sucks!
                    TelephonyManager telMgr = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
                    Method getITelephony = telMgr.Class.GetDeclaredMethod("getITelephony");
                    getITelephony.Accessible = true;
                    Java.Lang.Object objITelephony = getITelephony.Invoke(telMgr);
                    Method toggleDataConnectivity = objITelephony.Class.GetDeclaredMethod(mobileDataEnabled ? "enableDataConnectivity" : "disableDataConnectivity");
                    toggleDataConnectivity.Accessible = true;
                    toggleDataConnectivity.Invoke(objITelephony);
                    return;
                }
                string state = mobileDataEnabled ? "1" : "0";
                var transactionCode = getTransactionCode(context);
                if ((int)Build.VERSION.SdkInt > 21)
                {
                    SubscriptionManager mSubscriptionManager = (SubscriptionManager)context.GetSystemService(Context.TelephonySubscriptionService);
                    // Loop through the subscription list i.e. SIM list.
                    for (int i = 0; i < mSubscriptionManager.ActiveSubscriptionInfoCountMax; i++)
                    {
                        if (transactionCode != null && transactionCode.Length > 0)
                        {
                            if (mSubscriptionManager.ActiveSubscriptionInfoList[i] == null)
                                continue;
                            // Get the active subscription ID for a given SIM card.
                            int subscriptionId = mSubscriptionManager.ActiveSubscriptionInfoList[i].SubscriptionId;
                            // Execute the command via `su` to turn off
                            // mobile network for a subscription service.
                            var command = "service call phone " + transactionCode + " i32 " + subscriptionId + " i32 " + state;
                            executeCommandViaSu(context, "-c", command);
                        }
                    }
                }
                else if ((int)Build.VERSION.SdkInt == 21)
                {
                    // Android 5.0 (API 21) only.
                    if (transactionCode != null && transactionCode.Length > 0)
                    {
                        // Execute the command via `su` to turn off mobile network.                     
                        var command = "service call phone " + transactionCode + " i32 " + state;
                        executeCommandViaSu(context, "-c", command);
                    }
                }

                //// Requires: android.permission.CHANGE_NETWORK_STATE
                //else if ((int)Build.VERSION.SdkInt < 21)
                //{
                //    ConnectivityManager connMgr = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                //    // Gingerbread to KitKat inclusive
                //    Field serviceField = connMgr.Class.GetDeclaredField("mService");
                //    serviceField.Accessible = true;
                //    Java.Lang.Object connService = serviceField.Get(connMgr);
                //    try
                //    {
                //        Method setMobileDataEnabled = connService.Class.GetDeclaredMethod("setMobileDataEnabled", Java.Lang.Boolean.Type);
                //        setMobileDataEnabled.Accessible = true;
                //        setMobileDataEnabled.Invoke(connService, Java.Lang.Boolean.ValueOf(mobileDataEnabled));
                //    }
                //    catch (NoSuchMethodException e)
                //    {
                //        // Support for CyanogenMod 11+
                //        var str = new Java.Lang.String();
                //        Method setMobileDataEnabled = connService.Class.GetDeclaredMethod("setMobileDataEnabled", str.Class, Java.Lang.Boolean.Type);
                //        setMobileDataEnabled.Accessible = true;
                //        setMobileDataEnabled.Invoke(connService, context.PackageName, Java.Lang.Boolean.ValueOf(mobileDataEnabled));
                //    }
                //}
                //// Requires: android.permission.MODIFY_PHONE_STATE (System only, here for completions sake)
                //else
                //{
                //    // Lollipop and into the Future!
                //    TelephonyManager telMgr = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
                //    Method setDataEnabled = telMgr.Class.GetDeclaredMethod("setDataEnabled", Java.Lang.Boolean.Type);
                //    setDataEnabled.Accessible = true;
                //    setDataEnabled.Invoke(telMgr, Java.Lang.Boolean.ValueOf(mobileDataEnabled));
                //}
            }
            catch (System.Exception ex)
            {
                InitializeApplication.GoException(ex, "SetMobileDataEnabledRoot");
            }
            //bool isEX = false;
            //try
            //{
            //    ConnectivityManager conman = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            //    Class conmanClass = Class.ForName(conman.Class.Name);
            //    Field connectivityManagerField = conmanClass.GetDeclaredField("mService");
            //    connectivityManagerField.Accessible = true;
            //    Java.Lang.Object connectivityManager = connectivityManagerField.Get(conman);
            //    Class connectivityManagerClass = Class.ForName(connectivityManager.Class.Name);
            //    Method setMobileDataEnabledMethod = connectivityManagerClass.GetDeclaredMethod("setMobileDataEnabled", Java.Lang.Boolean.Type);
            //    setMobileDataEnabledMethod.Accessible = true;

            //    setMobileDataEnabledMethod.Invoke(connectivityManager, mobileDataEnabled);
            //}
            //catch (System.Exception ex)
            //{
            //    isEX = true;
            //    InitializeApplication.GoException(ex, "SetMobileDataEnabledRoot 1");
            //}
            //if (isEX)
            //{
            //    try
            //    {
            //        ConnectivityManager conman = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            //        Class conmanClass = Class.ForName(conman.Class.Name);
            //        Field connectivityManagerField = conmanClass.GetDeclaredField("mService");
            //        connectivityManagerField.Accessible = true;
            //        Java.Lang.Object connectivityManager = connectivityManagerField.Get(conman);
            //        Class connectivityManagerClass = Class.ForName(connectivityManager.Class.Name);
            //        Method setMobileDataEnabledMethod = connectivityManagerClass.GetDeclaredMethod("setDataEnabled", Java.Lang.Boolean.Type);
            //        setMobileDataEnabledMethod.Accessible = true;

            //        setMobileDataEnabledMethod.Invoke(connectivityManager, mobileDataEnabled);
            //    }
            //    catch (System.Exception ex)
            //    {
            //        isEX = true;
            //        InitializeApplication.GoException(ex, "SetMobileDataEnabledRoot 2");
            //    }
            //}
        }

        private static void executeCommandViaSu(Context context, string option, string command)
        {
            bool success = false;
            string su = "su";
            for (int i = 0; i < 3; i++)
            {
                // Default "su" command executed successfully, then quit.
                if (success)
                {
                    break;
                }
                // Else, execute other "su" commands.
                if (i == 1)
                {
                    su = "/system/xbin/su";
                }
                else if (i == 2)
                {
                    su = "/system/bin/su";
                }
                try
                {
                    // Execute command as "su".
                    Runtime.GetRuntime().Exec(new string[] { su, option, command });
                }
                catch (IOException e)
                {
                    success = false;
                    // Oops! Cannot execute `su` for some reason.
                    // Log error here.
                }
                finally
                {
                    success = true;
                }
            }
        }

        private static string getTransactionCode(Context context)
        {
            try
            {
                TelephonyManager mTelephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
                var mTelephonyClass = Class.ForName(mTelephonyManager.Class.Name);
                Method mTelephonyMethod = mTelephonyClass.GetDeclaredMethod("getITelephony");
                mTelephonyMethod.Accessible = true;
                var mTelephonyStub = mTelephonyMethod.Invoke(mTelephonyManager);
                var mTelephonyStubClass = Class.ForName(mTelephonyStub.Class.Name);
                var mClass = mTelephonyStubClass.DeclaringClass;
                Field field = mClass.GetDeclaredField("TRANSACTION_setDataEnabled");
                field.Accessible = true;
                return Java.Lang.String.ValueOf(field.GetInt(null));
            }
            catch (System.Exception e)
            {
                // The "TRANSACTION_setDataEnabled" field is not available,
                // or named differently in the current API level, so we throw
                // an exception and inform users that the method is not available.
                throw e;
            }
        }


        public static void SetWifiEnable(Context context, bool enabled)
        {
            try
            {
                Log.AutoLogger.LogText($"SetWifiEnable called {enabled}");
                WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
                var result = wifiManager.SetWifiEnabled(enabled);
                Log.AutoLogger.LogText($"SetWifiEnable finished {enabled} {result}");
            }
            catch (System.Exception e)
            {
                InitializeApplication.GoException(e, "SetWifiEnable");
            }
        }
    }
}