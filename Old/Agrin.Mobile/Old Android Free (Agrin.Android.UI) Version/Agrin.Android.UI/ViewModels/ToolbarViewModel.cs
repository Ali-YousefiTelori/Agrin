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
using Android.Webkit;
using Agrin.Download.Data.Settings;
using Agrin.Download.Data;

namespace Agrin.MonoAndroid.UI
{
    public class ToolbarViewModel : IBaseViewModel
    {
        public Activity CurrentActivity { get; set; }

        public ToolbarViewModel(Activity currentActivity)
        {
            CurrentActivity = currentActivity;
        }

        public void Initialize()
        {

            Button button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btn_AddLinks);
            button.Click += new EventHandler(btnAddLinksClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btn_AddYoutubeLinks);
            button.Click += new EventHandler(btnAddYoutubeClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btn_Links);
            button.Click += new EventHandler(btnLinksClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btn_applicationLog);
            button.Click += new EventHandler(btnApplicationLogClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btnGoToHome);
            button.Click += new EventHandler(btnGoToHomeClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btn_GroupManager);
            button.Click += new EventHandler(btnGroupManagerClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btnAbout);
            button.Click += new EventHandler(btnAboutClick);
            //button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btn_QueueManager);
            //button.Click += new EventHandler(btnQueueManagerClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btn_Setting);
            button.Click += new EventHandler(btnSettingClick);
            button = CurrentActivity.FindViewById<Button>(Resource.Toolbars.btnVIP);
            button.Click += new EventHandler(btnVIPClick);
        }

        private void btnVIPClick(object sender, EventArgs e)
        {
            bool userPermisions = ApplicationSetting.Current.FramesoftSetting.ConfirmUserPermissions;
            if (!userPermisions)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(CurrentActivity);
                builder.SetTitle(ViewUtility.FindTextLanguage(CurrentActivity, "VIPAccount_Language"));
                LinearLayout layout = new LinearLayout(CurrentActivity);
                layout.Orientation = Orientation.Vertical;
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

                layoutParams.SetMargins(5, 5, 5, 5);
                layout.LayoutParameters = layoutParams;

                TextView txt = new TextView(CurrentActivity);
                txt.Text = ViewUtility.FindTextLanguage(CurrentActivity, "PermisionFirstMessage_Language");
                txt.SetTextAppearance(CurrentActivity, Android.Resource.Attribute.TextAppearanceMedium);

                layout.AddView(txt);

                builder.SetView(layout);
                AlertDialog dialogW = null;

                builder.SetNegativeButton(ViewUtility.FindTextLanguage(CurrentActivity, "OK_Language"), (EventHandler<DialogClickEventArgs>)null);

                dialogW = builder.Create();
                dialogW.Show();

                // Get the buttons.
                var noBtn = dialogW.GetButton((int)DialogButtonType.Negative);

                noBtn.Click += (s, args) =>
                {
                    ShowPermisionDialog();
                    dialogW.Dismiss();
                };
            }
            else
            {
                ShowVIPMenu();
            }
        }

        void ShowPermisionDialog()
        {
            ViewUtility.ShowPageDialog("http://framesoft.ir/UserManager/PermissionReadMe", "VIPAccount_Language", () =>
            {
                ApplicationSetting.Current.FramesoftSetting.ConfirmUserPermissions = true;
                SerializeData.SaveApplicationSettingToFile();
                ShowVIPMenu();
            }, CurrentActivity, true, "AgreePermissions_Language");
        }

        void ShowVIPMenu()
        {
            ActivitesManager.VIPToolbarActive(CurrentActivity);
        }

        private void btnAddLinksClick(object sender, EventArgs e)
        {
            ActivitesManager.AddNewLinkActive(CurrentActivity);
        }

        private void btnAddYoutubeClick(object sender, EventArgs e)
        {
            ActivitesManager.AddYoutubeLinkActive(CurrentActivity);
        }

        private void btnLinksClick(object sender, EventArgs e)
        {
            ActivitesManager.DownloadListActive(CurrentActivity);
        }

        private void btnApplicationLogClick(object sender, EventArgs e)
        {
            ActivitesManager.MessageBoxActive(CurrentActivity);
        }

        private void btnGroupManagerClick(object sender, EventArgs e)
        {
            ActivitesManager.GroupManagerListActive(CurrentActivity);
        }

        private void btnGoToHomeClick(object sender, EventArgs e)
        {
            ActivitesManager.GoToHomeApplication(CurrentActivity);
        }

        private void btnAboutClick(object sender, EventArgs e)
        {
            ActivitesManager.AboutActive(CurrentActivity);
        }

        private void btnQueueManagerClick(object sender, EventArgs e)
        {
            ActivitesManager.QueueSettingActive(CurrentActivity);
        }

        private void btnSettingClick(object sender, EventArgs e)
        {
            ActivitesManager.AppSettingActive(CurrentActivity);
        }
    }
}

