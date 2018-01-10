using System;
using Android.Content;
using Android.App;
using Agrin.MonoAndroid.UI.Activities.Pages;
using Agrin.MonoAndroid.UI.Activities.Download;
using Agrin.MonoAndroid.UI.Activities.Lists;
using Agrin.MonoAndroid.UI.ViewModels.Lists;
using Agrin.Download.Manager;
using Agrin.Log;
using Agrin.MonoAndroid.UI.Activities.Settings;
using System.Collections.Generic;
using Agrin.MonoAndroid.UI.Activities.Toolbox;

namespace Agrin.MonoAndroid.UI
{
    public static class ActivitesManager
    {
        public static bool FinishingAllActivities { get; set; }
        public static bool ExitedApp = false;
        static List<Activity> AllActivities = new List<Activity>();
        static object lockobj = new object();

        public static void AddActivity(Activity activity)
        {
            lock (lockobj)
            {
                AllActivities.Add(activity);
            }
        }

        public static void RemoveActivity(Activity activity)
        {
            lock (lockobj)
            {
                AllActivities.Remove(activity);
            }
        }

        public static void FinishAllActivities()
        {
            FinishingAllActivities = true;
            foreach (var item in AllActivities.ToArray())
            {
                if (item != null && item != MainActivity.This)
                    item.Finish();
            }
            FinishingAllActivities = false;
        }

        public static void Exit(Activity finishActivity)
        {
            ExitedApp = true;
            foreach (var item in ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToList())
            {
                ApplicationLinkInfoManager.Current.StopLinkInfo(item);
            }
            FinishAllActivities();
            finishActivity.Finish();

            //Intent intent = new Intent(MainActivity.This.ApplicationContext, typeof(MainActivity));
            //intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            //intent.PutExtra("EXIT", true);
            //MainActivity.This.StartActivity(intent);

            if (BrowseActivity.This != null)
                BrowseActivity.This.Finish();
            if (MainActivity.This != BrowseActivity.This)
                MainActivity.This.Finish();
            //for (int i = 0; i < 10; i++)
            //{
            //    FinishAllActivities();
            //    finishActivity.Finish();
            //    if (BrowseActivity.This != null)
            //        BrowseActivity.This.Finish();
            //    if (MainActivity.This != BrowseActivity.This)
            //        MainActivity.This.Finish();
            //    System.Threading.Thread.Sleep(500);
            //}


            System.Environment.Exit(0);
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public static void GoToHomeApplication(Activity finishActivity)
        {
            try
            {
                Intent startMain = new Intent(Intent.ActionMain);
                startMain.AddCategory(Intent.CategoryHome);
                startMain.SetFlags(ActivityFlags.NewTask);
                startMain.SetFlags(ActivityFlags.ClearTop);
                MainActivity.This.StartActivity(startMain);
                //var builder = new AlertDialog.Builder(finishActivity);
                //builder.SetMessage(ViewUtility.FindTextLanguage(finishActivity, "ExitApplicationQuestion_Language"));
                //builder.SetPositiveButton(ViewUtility.FindTextLanguage(finishActivity, "Yes_Language"), (s, ee) =>
                //{

                //    //Exit(finishActivity);
                //});
                //builder.SetNegativeButton(ViewUtility.FindTextLanguage(finishActivity, "No_Language"), (s, ee) => { }).Create();
                //builder.Show();
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "ExitApplication");
            }
        }

        static int main = 23;
        public static void ToolbarActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;

            var intent = new Intent(MainActivity.This, typeof(ToolbarActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            if (MainActivity.This == null)
                Agrin.MonoAndroid.UI.Activities.AgrinService.CuurentContext.StartActivity(intent);
            else
                MainActivity.This.StartActivity(intent);

            if (finishActivity != null)
                finishActivity.Finish();
        }
        public static void SelectLanguageActive(Activity finishActivity, Action selected)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null && MainActivity.This != finishActivity)
                finishActivity.Finish();
            LanguageSettingActivity.SelectAction = selected;
            var intent = new Intent(MainActivity.This, typeof(LanguageSettingActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }

        public static void AddNewLinkActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(AddLinkActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            MainActivity.This.StartActivity(intent);

        }

        public static void AddYoutubeLinkActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(AddYoutubeLinkActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);

        }

        public static void DownloadListActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(LinksListDataActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }
        public static void AddGroupActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(AddGroupActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }

        public static void GroupManagerListActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(GroupListActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }

        public static void AboutActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(AboutActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }

        public static void VIPToolbarActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(VIPToolbarActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }

        public static void VIPLinksActive(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(FramesoftLinksListDataActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            MainActivity.This.StartActivity(intent);
        }


        public static void MessageBoxActive(Activity finishActivity = null)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(ApplicationLogsActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }

        public static void AppSettingActive(Activity finishActivity = null)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(AppSettingActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }

        public static void QueueSettingActive(Activity finishActivity = null)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(QueueSettingActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }
        public static void FolderBrowserDialogActive(Activity currentActivity, string path, Action<string> selectFolder)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            FolderBrowserDialogActivity.CurrentPath = path;
            FolderBrowserDialogViewModel.SelectedFolder = selectFolder;
            var intent = new Intent(currentActivity, typeof(FolderBrowserDialogActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            currentActivity.StartActivity(intent);
        }

        public static void LinkInfoDetailActivity(Agrin.Download.Web.LinkInfo linkInfo, Activity currentActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            LinkInfoDownloadDetailActivity.CurrentLinkInfo = linkInfo;
            var intent = new Intent(currentActivity, typeof(LinkInfoDownloadDetailActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            currentActivity.StartActivity(intent);
        }


        //public static void LinkInfoDetailActivity(Agrin.Download.Web.LinkInfo linkInfo, Activity currentActivity)
        //{
        //    if (currentActivity != null)
        //        currentActivity.Finish();
        //    LinkInfoDownloadDetailActivity.CurrentLinkInfo = linkInfo;
        //    currentActivity.StartActivity(new Intent(MainActivity.This, typeof(LinkInfoDownloadDetailActivity)));
        //}

        public static void UserAuthorizationActivity(Activity finishActivity)
        {
            if (FinishingAllActivities || ExitedApp)
                return;
            if (finishActivity != null)
                finishActivity.Finish();
            var intent = new Intent(MainActivity.This, typeof(UserAuthorizationListActivity));
            intent.SetFlags(ActivityFlags.ClearTop);
            //intent.SetFlags(ActivityFlags.NewTask);
            MainActivity.This.StartActivity(intent);
        }
    }
}

