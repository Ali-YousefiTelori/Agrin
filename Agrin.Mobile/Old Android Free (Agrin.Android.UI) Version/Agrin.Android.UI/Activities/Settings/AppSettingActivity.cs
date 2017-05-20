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
using Agrin.MonoAndroid.UI.ViewModels.Settings;

namespace Agrin.MonoAndroid.UI.Activities.Settings
{
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class AppSettingActivity : Activity
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
                    ViewUtility.SetRightToLeftLayout(this, new List<int>() { Resource.AppSetting.LinearLayoutReverce1 });
                    ViewUtility.SetRightToLeftViews(this, new List<int>() { Resource.AppSetting.txtConnectionCountTitle, Resource.AppSetting.txtDownloadsPathTitle, Resource.AppSetting.LinearLayoutRightToLeft, Resource.AppSetting.chkIsLimitTitle,Resource.AppSetting.chkEnableNotifyTitle });
                }
                else
                {
                    ViewUtility.SetLeftToRightCheckBox(this, Resource.AppSetting.chkIsLimitTitle);
                    ViewUtility.SetLeftToRightCheckBox(this, Resource.AppSetting.chkEnableNotifyTitle);
                }

                ViewUtility.SetTextLanguage(this, new List<int>() { Resource.AppSetting.txtDownloadsPathTitle, Resource.AppSetting.txtConnectionCountTitle, Resource.AppSetting.btn_browseDownloadsPath, Resource.AppSetting.btnCancel, Resource.AppSetting.btnSaveSetting, Resource.AppSetting.btnSelectLanguage, Resource.AppSetting.chkIsLimitTitle, Resource.AppSetting.chkEnableNotifyTitle, Resource.AppSetting.btnUserAuthorization });

                var appSetting = new AppSettingViewModel(this);
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Error App Setting", true);
            }
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            ActivitesManager.ToolbarActive(null);
            base.Finish();
        }
    }
}