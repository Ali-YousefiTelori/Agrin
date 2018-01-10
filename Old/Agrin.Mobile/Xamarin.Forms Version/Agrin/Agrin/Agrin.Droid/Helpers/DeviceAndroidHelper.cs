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
using Agrin.ViewModels.Helpers;
using Android.Content.PM;

namespace Agrin.Droid.Helpers
{
    public class DeviceAndroidHelper : DeviceHelper
    {
        public static DeviceAndroidHelper CurrentAndroid { get; set; }
        Activity CurrentActivity { get; set; }
        public static void CreateInstance(Activity activity)
        {
            Current = CurrentAndroid = new DeviceAndroidHelper();
            CurrentAndroid.CurrentActivity = activity;
            PackageManager manager = activity.PackageManager;
            PackageInfo info = manager.GetPackageInfo(activity.PackageName, 0);
            CurrentAndroid._ApplicationVersion = info.VersionName;
            CurrentAndroid._OSVersion = Android.OS.Build.VERSION.Release + " Api Level " + Android.OS.Build.VERSION.Sdk;
        }

        string _ApplicationVersion;
        string _OSVersion;

        public override string ApplicationVersion
        {
            get
            {
                return _ApplicationVersion;
            }
        }

        public override string OSVersion
        {
            get
            {
                return _OSVersion;
            }
        }

        public override bool IsRootMode()
        {
            return false;
        }

        public override void SetMobileDataEnabled(bool enabled)
        {

        }

        public override void SetMobileDataEnabledNormal(bool enabled)
        {
        }

        public override void SetMobileDataEnabledRoot(bool mobileDataEnabled)
        {
        }

        public override void SetWifiEnable(bool enabled)
        {
        }
    }
}