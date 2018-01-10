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

namespace Agrin.MonoAndroid.UI.Models
{
    public class FolderInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

    }
}