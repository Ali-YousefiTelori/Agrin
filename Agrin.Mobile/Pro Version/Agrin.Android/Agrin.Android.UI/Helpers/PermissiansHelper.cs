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

namespace Agrin.Helpers
{
    public static class PermissiansHelper
    {
        public static List<string> NeedPermissians { get; set; }
        public static readonly int MultiperPermissionsCode = 10;
        public static void CheckPermissionsNeed(Activity activity)
        {
            if ((int)Android.OS.Build.VERSION.SdkInt >= 23 && activity != null && NeedPermissians != null && NeedPermissians.Count > 0)
            {
                foreach (var permission in NeedPermissians)
                {
                    if (activity.CheckSelfPermission(permission) != Android.Content.PM.Permission.Granted)
                    {
                        activity.RequestPermissions(new string[] { permission }, MultiperPermissionsCode);
                    }
                }
            }
        }
    }
}