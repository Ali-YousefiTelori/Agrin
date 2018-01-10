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

namespace System
{
    class HolderHelper : Java.Lang.Object
    {
        public readonly object Value;

        public HolderHelper(object value)
        {
            this.Value = value;
        }
    }

    public static class ObjectTypeHelper
    {
        public static T Cast<T>(this Java.Lang.Object obj) where T : class
        {
            return (obj as HolderHelper).Value as T;
        }

        public static Java.Lang.Object Cast<T>(this T obj) where T : class
        {
            return new HolderHelper(obj);
        }
    }
}