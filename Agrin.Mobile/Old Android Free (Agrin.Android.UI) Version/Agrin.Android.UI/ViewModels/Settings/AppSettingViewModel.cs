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
using Agrin.MonoAndroid.UI.Activities.Settings;

namespace Agrin.MonoAndroid.UI.ViewModels.Settings
{
    public class AppSettingViewModel : IBaseViewModel
    {
        public Activity CurrentActivity { get; set; }

        EditText txtDownloadsPath = null;
        TextView txtConnectionCount = null, txtLimit = null;
        CheckBox chkIsLimit = null, chkIsEnableNotify = null;
        SeekBar seekBarConnectionCount = null, seekBarLimit = null;
        public AppSettingViewModel(Activity activity)
        {
            CurrentActivity = activity;
            var btnSelectLanguage = CurrentActivity.FindViewById<Button>(Resource.AppSetting.btnSelectLanguage);
            btnSelectLanguage.Click += btnSelectLanguage_Click;
            var btnCancel = CurrentActivity.FindViewById<Button>(Resource.AppSetting.btnCancel);
            btnCancel.Click += btnCancel_Click;
            var btnbrowseDownloadsPath = CurrentActivity.FindViewById<Button>(Resource.AppSetting.btn_browseDownloadsPath);
            btnbrowseDownloadsPath.Click += btnbrowseDownloadsPath_Click;
            var btnSaveSetting = CurrentActivity.FindViewById<Button>(Resource.AppSetting.btnSaveSetting);
            btnSaveSetting.Click += btnSaveSetting_Click;

            var btnUserAuthorization = CurrentActivity.FindViewById<Button>(Resource.AppSetting.btnUserAuthorization);
            btnUserAuthorization.Click += btnUserAuthorization_Click;

            txtDownloadsPath = CurrentActivity.FindViewById<EditText>(Resource.AppSetting.txt_DownloadsPath);
            txtConnectionCount = CurrentActivity.FindViewById<TextView>(Resource.AppSetting.txtConnectionCount);
            txtLimit = CurrentActivity.FindViewById<TextView>(Resource.AppSetting.txtLimit);

            chkIsLimit = CurrentActivity.FindViewById<CheckBox>(Resource.AppSetting.chkIsLimitTitle);
            chkIsEnableNotify = CurrentActivity.FindViewById<CheckBox>(Resource.AppSetting.chkEnableNotifyTitle);
            seekBarConnectionCount = CurrentActivity.FindViewById<SeekBar>(Resource.AppSetting.seekBarConnectionCount);
            seekBarConnectionCount.ProgressChanged += seekBarConnectionCount_ProgressChanged;

            seekBarLimit = CurrentActivity.FindViewById<SeekBar>(Resource.AppSetting.seekBarLimit);
            seekBarLimit.IncrementProgressBy(5);
            seekBarLimit.ProgressChanged += seekBarLimit_ProgressChanged;

            txtDownloadsPath.Text = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath;
            seekBarConnectionCount.Progress = 1;
            seekBarConnectionCount.Progress = Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.ConnectionCount - 1;
            seekBarLimit.Progress = (Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.SpeedSize / 1024) - 1;

            chkIsLimit.Checked = Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.IsLimit;
            chkIsEnableNotify.Checked = Agrin.Download.Data.Settings.ApplicationSetting.Current.IsShowNotification;

            seekBarLimit_ProgressChanged(null, new SeekBar.ProgressChangedEventArgs(seekBarLimit, seekBarLimit.Progress, false));
            seekBarConnectionCount_ProgressChanged(null, new SeekBar.ProgressChangedEventArgs(seekBarConnectionCount, seekBarConnectionCount.Progress, false));
        }

        void btnUserAuthorization_Click(object sender, EventArgs e)
        {
            ActivitesManager.UserAuthorizationActivity(CurrentActivity);
        }

        void seekBarLimit_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            var prog = e.Progress + 1;
            prog = prog / 5;
            prog = prog * 5;
            if (prog == 0)
                prog = 1;
            //var progress = (prog) * 1024;
            //var size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(progress);
            var str = prog + " " + ViewUtility.FindTextLanguage(CurrentActivity, "KB_Language");
            //if (prog > 1024)
            //    prog /= 1024;
            txtLimit.Text = str;
        }

        void seekBarConnectionCount_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            txtConnectionCount.Text = (e.Progress + 1).ToString();
        }

        void btnSaveSetting_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(txtDownloadsPath.Text))
                Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath = txtDownloadsPath.Text;
            else
            {
                Toast.MakeText(CurrentActivity, ViewUtility.FindTextLanguage(CurrentActivity, "InvalidAddress_Language"), ToastLength.Short).Show();
                return;
            }
            //Agrin.Download.Manager.ApplicationGroupManager.Current.NoGroup.SavePath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath;
            Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.ConnectionCount = seekBarConnectionCount.Progress + 1;
            var prog = seekBarLimit.Progress + 1;
            prog = prog / 5;
            prog = prog * 5;
            if (prog == 0)
                prog = 1;
            Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.SpeedSize = prog * 1024;

            Agrin.Download.Data.Settings.ApplicationSetting.Current.SpeedSetting.IsLimit = chkIsLimit.Checked;
            Agrin.Download.Data.Settings.ApplicationSetting.Current.IsShowNotification = chkIsEnableNotify.Checked;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            ActivitesManager.ToolbarActive(CurrentActivity);
        }

        void btnbrowseDownloadsPath_Click(object sender, EventArgs e)
        {
            ActivitesManager.FolderBrowserDialogActive(CurrentActivity, txtDownloadsPath.Text, (path) =>
            {
                txtDownloadsPath.Text = path;
            });
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            ActivitesManager.ToolbarActive(CurrentActivity);
        }

        void btnSelectLanguage_Click(object sender, EventArgs e)
        {
            LanguageSettingActivity.CanBackToToolbar = true;
            LanguageSettingActivity.IsBaseSelect = false;
            ActivitesManager.SelectLanguageActive(CurrentActivity, () =>
                {
                    ActivitesManager.AppSettingActive();
                });
        }
    }
}