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

namespace Agrin.MonoAndroid.UI.Activities.Settings
{
    [Activity(Label = "Select Application Language", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class LanguageSettingActivity : Activity
    {
        public static Action SelectAction { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            ActivitesManager.AddActivity(this);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LanguageSetting);
            // Create your application here
            var btnPersian = FindViewById<Button>(Resource.LanguageSetting.btnPersian);
            btnPersian.Click += btnPersian_Click;
            var btnEnglish = FindViewById<Button>(Resource.LanguageSetting.btnEnglish);
            btnEnglish.Click += btnEnglish_Click;
        }

        void SelectLanguage()
        {
            CanBackToToolbar = false;
            SelectAction();
            this.Finish();
        }

        public static bool IsBaseSelect { get; set; }

        void btnEnglish_Click(object sender, EventArgs e)
        {
            InitializeApplication.SaveLanguage("english");
            SelectLanguage();
        }

        void btnPersian_Click(object sender, EventArgs e)
        {
            InitializeApplication.SaveLanguage("persian");
            SelectLanguage();
        }

        public static bool CanBackToToolbar { get; set; }
        public override void OnBackPressed()
        {
            if (CanBackToToolbar && !IsBaseSelect)
                base.OnBackPressed();
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            CanBackToToolbar = false;
            if (!IsBaseSelect)
                ActivitesManager.AppSettingActive(null);
            base.Finish();
        }
    }
}