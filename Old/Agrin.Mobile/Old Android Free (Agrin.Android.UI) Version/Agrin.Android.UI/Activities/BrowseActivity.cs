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
using Agrin.Download.Manager;
using Agrin.Download.Data.Settings;
using Android.Content.PM;
using Agrin.MonoAndroid.UI.Activities;

namespace Agrin.MonoAndroid.UI
{
    [Activity(Name = "agrin.monoandroid.ui.browseactivity", Theme = "@style/Theme.Splash", LaunchMode = LaunchMode.SingleTask, AlwaysRetainTaskState = true)]
    public class BrowseActivity : Activity
    {
        public static BrowseActivity This;
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                AgrinService.MustRunApp = true;
                this.Title = ViewUtility.FindTextLanguage(this, "AgrinDownloadManager_Language");

                if (MainActivity.This == null)
                    MainActivity.This = this;
                else
                    ActivitesManager.FinishAllActivities();
                This = this;
                ActivitesManager.AddActivity(this);
                base.OnCreate(bundle);
                //SetContentView(new TextView(this));


                //if (Intent.GetBooleanExtra("EXIT", false))
                //{
                //    //Agrin.Log.AutoLogger.LogError(null, "finished run 2", true);
                //    Finish();
                //    System.Environment.Exit(0);
                //    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                //    return;
                //}
                //if (ActivityManager.ExitedApp)
                //{
                //    //Agrin.Log.AutoLogger.LogError(null, "ExitedApp 2", true);
                //    ActivityManager.Exit(this);
                //    return;
                //}
                InitializeApplication.ExtrasAllGeted = GetAllExtras(this);

                //ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
                //ApplicationGroupManager.Current = new ApplicationGroupManager();
                //ApplicationNotificationManager.Current = new ApplicationNotificationManager();
                //ApplicationBalloonManager.Current = new ApplicationBalloonManager();

                //Agrin.Download.Data.DeSerializeData.LoadApplicationData();
                //if (!ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme)
                //{
                //    ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme = true;
                //    ApplicationSetting.Current.IsSettingForAllLinks = true;
                //    ApplicationSetting.Current.IsSettingForNewLinks = true;
                //    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                //}

                //Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();
                ////ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(ApplicationLinkInfoManager.Current.LinkInfoes.ToList());
                //string text = null;
                //if (!string.IsNullOrEmpty(InitializeApplication.ExtrasAllGeted))
                //    text = InitializeApplication.ExtrasAllGeted;
                //if (!string.IsNullOrEmpty(text))
                //{
                //    AddYoutubeLinkViewModel.CurrentSelectedURL = text;
                //    if (InitializeApplication.IsYoutubeLink(text))
                //        ActivityManager.AddYoutubeLinkActive(null);
                //    else
                //        ActivityManager.AddNewLinkActive(null);
                //    return;
                //}

                //ActivityManager.ToolbarActive(this);

                //*******************************
                //if (InitializeApplication.Run(this))
                //{
                StartService(new Intent(this, typeof(AgrinService)));
                //if (InitializeApplication.IsSetLanguage)
                //    ActivityManager.ToolbarActive(this);
                //else
                //{
                //    ActivityManager.SelectLanguageActive(this, () =>
                //    {
                //        ActivityManager.ToolbarActive(this);
                //    });
                //}
                //}
                //ServiceRunner.Create();
            }
            catch (Exception error)
            {
                InitializeApplication.GoException(error);
            }

            //try
            //{
            //    This = this;
            //    base.OnCreate(bundle);
            //    InitializeApplication.ExtrasAllGeted = GetAllExtras(this);
            //    StartActivity(new Intent(this, typeof(MainActivity)));
            //}
            //catch(Exception e)
            //{
            //    MainActivity.This.GoException(e);
            //}
        }

        string GetAllExtras(Activity activity)
        {
            StringBuilder builder = new StringBuilder();
            if (activity.Intent.HasExtra(Intent.ExtraText) && !String.IsNullOrEmpty(activity.Intent.GetStringExtra(Intent.ExtraText)))
                builder.AppendLine(activity.Intent.GetStringExtra(Intent.ExtraText));
            else if (activity.Intent.HasExtra(Intent.ExtraTitle) && !String.IsNullOrEmpty(activity.Intent.GetStringExtra(Intent.ExtraTitle)))
                builder.AppendLine(activity.Intent.GetStringExtra(Intent.ExtraTitle));
            else if (activity.Intent.HasExtra(Intent.ExtraSubject) && !String.IsNullOrEmpty(activity.Intent.GetStringExtra(Intent.ExtraSubject)))
                builder.AppendLine(activity.Intent.GetStringExtra(Intent.ExtraSubject));
            else if (activity.Intent.ToUri(IntentUriType.None) != null)
            {
                builder.AppendLine(activity.Intent.ToUri(IntentUriType.None).Replace("#", " "));
            }
            Action<string> getItems = (name) =>
            {
                var text = activity.Intent.GetStringArrayExtra(name);
                if (text != null && text.Length > 0)
                    foreach (var item in text)
                    {
                        builder.AppendLine(item);
                    }
            };
            getItems(Intent.ExtraText);
            getItems(Intent.ExtraTitle);
            getItems(Intent.ExtraSubject);
            return builder.ToString();
        }

        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            base.Finish();
        }

        protected override void OnStart()
        {
            base.OnStart();
            //ServiceRunner.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            //ServiceRunner.Stop();
        }
    }
}

