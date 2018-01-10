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
using Android.Content.PM;
using Agrin.Views.List;
using Agrin.Download.Web;
using Agrin.Services;
using Agrin.Helpers;

namespace Agrin.Views
{
    [Activity(Name = "agrin.android.notificationclickactivity", Theme = "@style/Theme.IconSplash", LaunchMode = LaunchMode.SingleTask, AlwaysRetainTaskState = true, ExcludeFromRecents = true, TaskAffinity = "", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden)]
    public class NotificationClickActivity : Activity
    {
        public static bool IsDestroy = true;
        public static bool IsBrowse { get; set; }
        public static NotificationClickActivity This { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                Title = ViewsUtility.FindTextLanguage(this, "App_Name_Language");

                IsDestroy = false;
                This = this;
                IsBrowse = true;
                base.OnCreate(bundle);
                if (MainActivity.This == null)
                {
                    MainActivity.This = this;
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MainActivity.HandleExceptions);
                    AndroidEnvironment.UnhandledExceptionRaiser += MainActivity.HandleAndroidException;
                }

                if (AgrinService.This == null)
                    StartService(new Intent(this, typeof(AgrinService)));
                else
                    Initialize();
            }
            catch (Exception e)
            {
                Helpers.InitializeApplication.GoException(e, "NotificationClickActivity");
            }
        }
        protected override void OnDestroy()
        {
            IsDestroy = true;
            Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
            AgrinService.StopServiceIfNotNeed();
            base.OnDestroy();
        }

        public void Initialize()
        {
            IsBrowse = false;
            var ex = this.Intent.Extras;
            if (ex.ContainsKey("LinkId"))
            {
                // extract the extra-data in the Notification
                var id = ex.GetInt("LinkId");
                if (Agrin.Helpers.InitializeApplication.Inited)
                {
                    var linkID = id - Agrin.Helpers.InitializeApplication._maxReserveRange;
                    var linkInfo = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindLinkInfoByID(linkID);
                    if (linkInfo != null)
                    {
                        if (linkInfo.CanPlay)
                        {
                            Agrin.Download.Manager.ApplicationLinkInfoManager.Current.PlayLinkInfo(linkInfo);
                            Toast.MakeText(this, "لینک شما شروع شد", ToastLength.Long).Show();
                        }
                        else if (linkInfo.CanStop)
                        {
                            bool isNotSupportResumable = false;

                            if (linkInfo.IsDownloading && (linkInfo.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.No || linkInfo.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Unknown) && linkInfo.DownloadingProperty.DownloadedSize > 0)
                            {
                                isNotSupportResumable = true;
                            }

                            if (isNotSupportResumable)
                            {
                                Toast.MakeText(this, "لینک شما قابلیت توقف ندارد لطفاً برای توقف وارد نرم افزار شوید", ToastLength.Long).Show();
                                return;
                            }
                            if (!Agrin.Helpers.InitializeApplication.ManualNotifyStop.Contains(id))
                                Agrin.Helpers.InitializeApplication.ManualNotifyStop.Add(id);
                            Agrin.Download.Manager.ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
                            Toast.MakeText(this, "لینک شما متوقف شد", ToastLength.Long).Show();
                        }
                        else if (linkInfo.IsComplete)
                        {
                            if (LinksViewModel.CanOpenFile(linkInfo))
                                LinksViewModel.OpenFile(linkInfo, this);
                        }
                    }
                    else
                    {
                        Agrin.Helpers.InitializeApplication.CancelNotify(id);
                    }
                }
            }

            Finish();
        }
    }
}