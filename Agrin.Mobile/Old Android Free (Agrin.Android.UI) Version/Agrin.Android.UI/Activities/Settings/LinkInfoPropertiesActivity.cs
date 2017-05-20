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
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class LinkInfoPropertiesActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                ActivitesManager.AddActivity(this);
                base.OnCreate(bundle);
                // Create your application here
                SetContentView(Resource.Layout.AppSetting);
                this.Title = "AppSetting_Language";
                if (ViewUtility.ProjectDirection == FlowDirection.RightToLeft)
                {
                    ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.LinkInfoPropertiesGeneral.LinearLayoutReverce1 });
                    ViewUtility.SetRightToLeftViews(this, new List<int>() { Resource.LinkInfoPropertiesGeneral.txt_SaveLinkPath, Resource.LinkInfoPropertiesGeneral.txtSaveLinkPathTitle});
                }

                ViewUtility.SetTextLanguage(this, new List<int>() { Resource.LinkInfoPropertiesGeneral.txtSaveLinkPathTitle });

                var appSetting = new Agrin.MonoAndroid.UI.ViewModels.Settings.AppSettingViewModel(this);
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Error App Setting", true);
            }
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.DownloadListActive(null);
            base.Finish();
        }
    }
}