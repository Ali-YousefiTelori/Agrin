
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
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class ApplicationLogsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);

            // Create your application here

            SetContentView(Resource.Layout.MessageBox);
            this.Title = "ApplicationLogs_Language";
            ViewUtility.SetTextLanguage(this, new List<int>() { });
            ApplicationLogsViewModel logs = new ApplicationLogsViewModel(this);

        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            base.Finish();
        }
    }
}

