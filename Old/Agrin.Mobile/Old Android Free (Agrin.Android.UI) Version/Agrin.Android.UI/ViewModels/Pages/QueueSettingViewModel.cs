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

namespace Agrin.MonoAndroid.UI
{
    public class QueueSettingViewModel : IBaseViewModel
    {
        public Activity CurrentActivity { get; set; }
        public QueueSettingViewModel(Activity currentActivity)
        {
            CurrentActivity = currentActivity;
        }
    }
}