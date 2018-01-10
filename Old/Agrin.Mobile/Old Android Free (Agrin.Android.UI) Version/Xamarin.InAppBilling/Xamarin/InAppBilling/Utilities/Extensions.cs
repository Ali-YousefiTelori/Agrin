using Android.Content;
using Android.OS;
using Java.Lang;
using System;
using System.Runtime.CompilerServices;

namespace Xamarin.InAppBilling.Utilities
{
    public static class Extensions
    {
        public static int GetReponseCodeFromIntent(this Intent intent)
        {
            object obj2 = intent.Extras.Get("RESPONSE_CODE");
            if (obj2 == null)
            {
                return 0;
            }
            if (obj2 is Number)
            {
                return ((Number) obj2).IntValue();
            }
            return 6;
        }

        public static int GetResponseCodeFromBundle(this Bundle bunble)
        {
            object obj2 = bunble.Get("RESPONSE_CODE");
            if (obj2 == null)
            {
                return 0;
            }
            if (obj2 is Number)
            {
                return ((Number) obj2).IntValue();
            }
            return 6;
        }
    }
}

