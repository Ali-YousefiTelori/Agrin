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

namespace Agrin.MonoAndroid.UI.Activities.Pages
{
    [Activity(Label = "QueueSettingActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class QueueSettingActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.QueueSetting);
            var vm = new QueueSettingViewModel(this);
            // Create your application here
        }
        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.ToolbarActive(null);
            base.Finish();
        }
    }
}