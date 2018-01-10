using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Agrin.Download.Manager;
using Agrin.Download.Data.Settings;
using Android.Content.PM;
using Agrin.MonoAndroid.UI.Models;
using Android.App;
using Agrin.MonoAndroid.UI.Activities;
using Agrin.Download.Engine;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.Framesoft.Helper;

namespace Agrin.MonoAndroid.UI
{
    public class UnCaughtExceptionHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
    {
        readonly Context context;
        public UnCaughtExceptionHandler(Context context)
        {
            this.context = context;
        }

        public void UncaughtException(Java.Lang.Thread thread, Java.Lang.Throwable ex)
        {
            Agrin.Log.AutoLogger.LogError(new Exception("Unhandle Java Exception Error!"), "");
            Agrin.Log.AutoLogger.LogError(new Exception(ex.Message), "Unhandle Java Exception Message");
            if (ex.StackTrace != null)
                Agrin.Log.AutoLogger.LogError(new Exception(ex.StackTrace), "Unhandle Java Stack Trace Exception Message");
        }
    }

    public static class InitializeApplication
    {
        public static void Initialize(Context active)
        {
            Agrin.Log.AutoLogger.ForceLog = true;
            System.AppDomain.CurrentDomain.UnhandledException += ErrorData;
            AndroidEnvironment.UnhandledExceptionRaiser += ErrorData2;
            Java.Lang.Thread.DefaultUncaughtExceptionHandler = new UnCaughtExceptionHandler(active);
        }

        public static string ExtrasAllGeted { get; set; }
        public static void ErrorData(object sender, UnhandledExceptionEventArgs e)
        {
            Exception error = (Exception)e.ExceptionObject;
            GoException(error);
        }

        public static void ErrorData2(object sender, RaiseThrowableEventArgs e)
        {
            Exception error = (Exception)e.Exception;
            GoException(error);
            e.Handled = true;
        }
        public static void GoException(Exception error)
        {
            Agrin.Log.AutoLogger.LogError(error, "Unhandle: ", true);
            ActivitesManager.MessageBoxActive();
            //			SetContentView (Resource.Layout.MessageBox);
            //			EditText textBox = FindViewById<EditText> (Resource.ApplicationLog.TxTmessageBox);
            //			string stack = "";
            //			if (error == null) {
            //				textBox.Text = "Error!";
            //				return;
            //			}
            //			if (error.StackTrace != null)
            //				stack = error.StackTrace;

            //textBox.Text = "نرم افزار بسته می شود! لطفاً این پیغام خطا را به ما گزارش دهید: " + System.Environment.NewLine + error.Message + " Stack: " + stack;

        }

        public static void SaveLanguage(string lang)
        {
            if (lang == "english")
            {
                IsSetLanguage = true;
                ViewUtility.ProjectDirection = FlowDirection.LeftToRight;
                ViewUtility.ApplicationLanguage = "_en";
            }
            else if (lang == "persian")
            {
                IsSetLanguage = true;
                ViewUtility.ProjectDirection = FlowDirection.RightToLeft;
                ViewUtility.ApplicationLanguage = "_fa";
            }
            ApplicationSetting.Current.ApplicationLanguage = lang;
            //Agrin.Log.AutoLogger.LogError(null, "save Lang: " + ApplicationSetting.Current.ApplicationLanguage == null ? "null" : ApplicationSetting.Current.ApplicationLanguage);
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
        }

        public static void LoadLanguage()
        {
            IsSetLanguage = true;
            //Agrin.Log.AutoLogger.LogError(null, "load Lang: " + ApplicationSetting.Current.ApplicationLanguage == null ? "null" : ApplicationSetting.Current.ApplicationLanguage);
            string lang = ApplicationSetting.Current.ApplicationLanguage;
            if (lang == "english")
            {
                ViewUtility.ProjectDirection = FlowDirection.LeftToRight;
                ViewUtility.ApplicationLanguage = "_en";
            }
            else if (lang == "persian")
            {
                ViewUtility.ProjectDirection = FlowDirection.RightToLeft;
                ViewUtility.ApplicationLanguage = "_fa";
            }
            else
                IsSetLanguage = false;
        }

        public static void CheckAddFrameSoftLink(LinkInfo link)
        {
            try
            {
                List<Uri> uri = new List<Uri>();
                uri.Add(new Uri(link.PathInfo.Address));
                foreach (var item in link.Management.MultiLinks)
                {
                    uri.Add(new Uri(item.Address));
                }

                List<string> guids = new List<string>();
                foreach (var item in uri)
                {
                    if (item.Host.ToLower().Contains(Framesoft.Helper.UserManagerHelper.domain.ToLower()))
                    {
                        string guidString = item.Segments.LastOrDefault();
                        Guid guid = Guid.Empty;
                        if (Guid.TryParse(guidString, out guid))
                        {
                            guids.Add(guid.ToString());
                        }
                    }
                }

                if (guids.Count > 0)
                {
                    foreach (var item in ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.ToList())
                    {
                        if (guids.Contains(item))
                            guids.Remove(item);
                    }
                    if (guids.Count > 0)
                    {
                        ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.AddRange(guids);
                        AsyncActions.Action(() =>
                            {
                                System.Threading.Thread.Sleep(2000);
                                List<Framesoft.UserFileInfoData> sendList = new List<Framesoft.UserFileInfoData>();
                                var sendingItems = ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.ToList();
                                foreach (var item in sendingItems)
                                {
                                    sendList.Add(new Framesoft.UserFileInfoData() { FileGuid = Guid.Parse(item) });
                                }
                                if (sendList.Count > 0)
                                {
                                    sendList.First().UserName = ApplicationSetting.Current.FramesoftSetting.UserName;
                                    sendList.First().Password = ApplicationSetting.Current.FramesoftSetting.Password.Sha1Hash();

                                    var data = UserManagerHelper.SetCompleteUserFiles(sendList);
                                    if (data.Message == "OK")
                                    {
                                        ApplicationSetting.Current.FramesoftSetting.CompleteFileAddress.RemoveAll(x => sendingItems.Contains(x));
                                        Agrin.MonoAndroid.UI.ViewModels.Lists.FramesoftLinksListDataViewModel.MustRefresh = true;
                                        Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                                    }
                                }
                            });
                        Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    }
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "CheckAddFrameSoftLink");
            }

        }

        public static void StartNotify(Context context)
        {
            if (context == null)
                return;
            var nMgr = (NotificationManager)context.GetSystemService(Context.NotificationService);

            var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(context, "AgrinDownloadManager_Language"));
            var mainActivity = new Intent(context, typeof(MainActivity));
            mainActivity.SetFlags(ActivityFlags.NewTask);
            mainActivity.SetFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(context, 0, mainActivity, 0);
            notification.SetLatestEventInfo(context, ViewUtility.FindTextLanguage(context, "AgrinDownloadManager_Language"), ViewUtility.FindTextLanguage(context, "CompleteDownloadNotify_Language"), pendingIntent);
            notification.Flags = NotificationFlags.AutoCancel;
            nMgr.Notify(0, notification);
        }

        static void NotificationInit()
        {
            ApplicationNotificationManager.Current.NotificationInfoChanged = (mode, linkInfo) =>
            {
                if (mode == Download.Web.NotificationMode.Complete)
                {
                    StartNotify(MainActivity.This);
                    if (linkInfo != null)
                    {
                        CheckAddFrameSoftLink(linkInfo);
                        Agrin.Download.Data.ApplicationServiceData.RemoveItem(linkInfo.PathInfo.Id);
                        CancelNotify(linkInfo.PathInfo.Id);
                        try
                        {
                            Android.Media.MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { linkInfo.PathInfo.FullAddressFileName }, null, null);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    //// Set up an intent so that tapping the notifications returns to this app:
                    //Intent intent = new Intent(MainActivity.This, typeof(LinksListDataActivity));
                    //intent.AddFlags(ActivityFlags.ClearTop);
                    //// Create a PendingIntent; we're only using one PendingIntent (ID = 0):
                    //const int pendingIntentId = 0;
                    //PendingIntent pendingIntent =
                    //    PendingIntent.GetActivity(MainActivity.This, pendingIntentId, intent, PendingIntentFlags.OneShot);

                    //// Instantiate the builder and set notification elements, including pending intent:
                    //NotificationCompat.Builder builder = new NotificationCompat.Builder(MainActivity.This)
                    //    .SetContentIntent(pendingIntent)
                    //    .SetContentTitle(ViewUtility.FindTextLanguage(MainActivity.This, "AgrinDownloadManager_Language"))
                    //    .SetContentText(ViewUtility.FindTextLanguage(MainActivity.This, "CompleteDownloadNotify_Language"))
                    //    .SetSmallIcon(Resource.Drawable.smallIcon);

                    //// Build the notification:
                    //Notification notification = builder.Build();

                    //// Get the notification manager:
                    //NotificationManager notificationManager =
                    //    MainActivity.This.GetSystemService(Context.NotificationService) as NotificationManager;

                    //// Publish the notification:
                    //const int notificationId = 0;
                    //notificationManager.Notify(notificationId, notification);


                }
            };
        }

        public static void CancelNotify(int id)
        {
            if (_notifyManager != null)
                _notifyManager.Cancel(id);
        }

        static List<int> downloadingIDS = new List<int>();
        static NotificationManager _notifyManager = null;
        static Notification _notification = null;
        static void RemoteNotify()
        {
            if (ApplicationLinkInfoManager.Current == null)
                return;
            if (_notifyManager == null)
            {
                var mainActivity = new Intent(_context, typeof(MainActivity));
                mainActivity.SetFlags(ActivityFlags.NewTask);
                mainActivity.SetFlags(ActivityFlags.ClearTop);
                var pendingIntent = PendingIntent.GetActivity(_context, 0, mainActivity, 0);
                _notifyManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
                _notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(_context, "AgrinDownloadManager_Language"));
                _notification.Flags = NotificationFlags.OnlyAlertOnce;
                _notification.SetLatestEventInfo(_context, ViewUtility.FindTextLanguage(_context, "AgrinDownloadManager_Language"), "", pendingIntent);
            }
            bool isNotify = false;

            ApplicationLinkInfoManager.Current.RemovedFromDownloadingLinkInfoes = (item) =>
            {
                if ((!item.IsManualStop && item.DownloadingProperty.State != Download.Web.ConnectionState.Complete) || downloadingIDS.Contains(item.PathInfo.Id))
                {
                    isNotify = true;
                    if (ApplicationSetting.Current.IsShowNotification)
                    {
                        RemoteViews bigView = new RemoteViews(_context.PackageName, Resource.Layout.NotifyLinkDownloading);
                        StringBuilder textBuilder = new StringBuilder();
                        string fileName = item.PathInfo.FileName;
                        textBuilder.Append(ViewUtility.FindTextLanguage(_context, item.DownloadingProperty.State.ToString() + "_Language"));

                        textBuilder.Append(" | " + ViewUtility.FindTextLanguage(_context, "SpeedTitle_Language") + " ");
                        var size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.SpeedByteDownloaded);
                        textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(_context, size[1] + "_Language"));

                        bigView.SetTextViewText(Resource.NotifyLinkDownloading.txtSizeData, textBuilder.ToString());

                        textBuilder.Clear();

                        textBuilder.Append(ViewUtility.FindTextLanguage(_context, "Size_Language") + " ");
                        size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.Size);
                        textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(_context, size[1] + "_Language"));
                        textBuilder.Append(" | " + ViewUtility.FindTextLanguage(_context, "Downloaded_Language") + " ");
                        size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.DownloadedSize);
                        textBuilder.Append(size[0] + " " + ViewUtility.FindTextLanguage(_context, size[1] + "_Language"));

                        bigView.SetTextViewText(Resource.NotifyLinkDownloading.txtStateData, textBuilder.ToString());

                        bigView.SetTextViewText(Resource.NotifyLinkDownloading.txtPercentData, item.DownloadingProperty.GetPercent);

                        bigView.SetTextViewText(Resource.NotifyLinkDownloading.chkFileName, fileName);


                        if (item.DownloadingProperty.Size >= 0.0)
                            bigView.SetProgressBar(Resource.NotifyLinkDownloading.mainProgress, 100, (int)(100 / (item.DownloadingProperty.Size / item.DownloadingProperty.DownloadedSize)), false);
                        else
                            bigView.SetProgressBar(Resource.NotifyLinkDownloading.mainProgress, 100, 0, true);

                        _notification.ContentView = bigView;
                        _notifyManager.Notify(item.PathInfo.Id, _notification);
                    }

                    if ((item.IsManualStop || item.DownloadingProperty.State == Download.Web.ConnectionState.Complete) && item.DownloadingProperty.State != Download.Web.ConnectionState.CopyingFile && downloadingIDS.Contains(item.PathInfo.Id))
                    {
                        downloadingIDS.Remove(item.PathInfo.Id);
                        CancelNotify(item.PathInfo.Id);
                    }
                    else if (!downloadingIDS.Contains(item.PathInfo.Id))
                        downloadingIDS.Add(item.PathInfo.Id);
                }
            };

            foreach (var item in ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToArray())
            {
                ApplicationLinkInfoManager.Current.RemovedFromDownloadingLinkInfoes(item);
            }

            if (AgrinService.This != null)
            {
                if (!isNotify)
                {
                    AgrinService.This.ApplicationStopedDownloading();
                    MainActivity.ReleaseWakeLock();
                }
                else
                {
                    AgrinService.This.ApplicationIsDownloading();
                    MainActivity.AcquireWakeLock();
                }
            }
        }

        static bool initLimit = false;
        static Context _context = null;
        static void InitializeLimitDrawing()
        {
            if (initLimit)
                return;
            initLimit = true;
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            {
                while (true)
                {
                    try
                    {
                        ActivityManager am = (ActivityManager)_context.GetSystemService(Context.ActivityService);

                        // get the info from the currently running task
                        var taskInfo = am.GetRunningTasks(1);

                        ComponentName componentInfo = taskInfo[0].TopActivity;
                        if (!componentInfo.PackageName.Equals("Agrin.MonoAndroid.UI"))
                        {
                            //Do your stuff
                            Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StopNotifyChanging();
                        }
                        else
                        {
                            Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StartNotifyChanging();
                        }

                        if (ApplicationSetting.Current != null)
                            RemoteNotify();
                    }
                    catch (Java.Lang.Exception er)
                    {
                        Agrin.Log.AutoLogger.LogText("Java EXP InitializeLimitDrawing()" + er.Message);
                    }
                    catch (Exception e)
                    {
                        Agrin.Log.AutoLogger.LogError(e, "InitializeLimitDrawing()", true);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            });
            task.Start();
        }

        public static bool Inited = false;
        public static bool IsSetLanguage { get; set; }
        public static int MaxNotifyID = 569000;
        public static bool Run(Context currentActivity)
        {
            _context = currentActivity;
            InitializeLimitDrawing();
            if (!Inited)
            {
                Inited = true;


                Agrin.Download.Engine.TimeDownloadEngine.UpdatedAction = (update) =>
                {
                    //her code for new Application Version Update
                    var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                    var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                    var mainActivity = new Intent(currentActivity, typeof(BrowseActivity));
                    mainActivity.SetFlags(ActivityFlags.NewTask);
                    mainActivity.SetFlags(ActivityFlags.ClearTop);
                    mainActivity.PutExtra(Intent.ExtraText, update.DownloadUri);
                    var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                    notification.SetLatestEventInfo(currentActivity, ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationNewVersionTitle_Language"), ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationNewVersionMessage_Language"), pendingIntent);
                    notification.Flags = NotificationFlags.AutoCancel;

                    nMgr.Notify(MaxNotifyID + 1, notification);
                };

                Agrin.Download.Engine.TimeDownloadEngine.GetLastMessageAction = (update) =>
                {
                    var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                    var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                    var mainActivity = new Intent(currentActivity, typeof(MainActivity));
                    mainActivity.SetFlags(ActivityFlags.NewTask);
                    mainActivity.SetFlags(ActivityFlags.ClearTop);
                    mainActivity.PutExtra("AgrinApplicationMessage", Newtonsoft.Json.JsonConvert.SerializeObject(update));
                    var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                    notification.SetLatestEventInfo(currentActivity, ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageTitle_Language"), ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageMessage_Language"), pendingIntent);
                    notification.Flags = NotificationFlags.AutoCancel;

                    nMgr.Notify(MaxNotifyID + 2, notification);
                };

                Agrin.Download.Engine.TimeDownloadEngine.GetUserMessageAction = (userMessage) =>
                {
                    if (userMessage.GUID == ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid)
                    {
                        if (userMessage.LastUserMessageID > ApplicationSetting.Current.LastUserMessageID)
                        {
                            //her code for new User Message
                            if (string.IsNullOrEmpty(userMessage.Message))
                                return;

                            var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                            var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                            var mainActivity = new Intent(currentActivity, typeof(MainActivity));
                            mainActivity.SetFlags(ActivityFlags.NewTask);
                            mainActivity.SetFlags(ActivityFlags.ClearTop);
                            mainActivity.PutExtra("AgrinApplicationUserMessage", Newtonsoft.Json.JsonConvert.SerializeObject(userMessage));
                            var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                            notification.SetLatestEventInfo(currentActivity, ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationUserMessageTitle_Language"), ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageMessage_Language"), pendingIntent);
                            notification.Flags = NotificationFlags.AutoCancel;

                            nMgr.Notify(MaxNotifyID + 3, notification);
                        }
                    }
                };

                InitializeApplication.Initialize(currentActivity);
                InitializeApplication.InitializeAppIO();
                ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
                ApplicationGroupManager.Current = new ApplicationGroupManager();
                ApplicationNotificationManager.Current = new ApplicationNotificationManager();
                ApplicationBalloonManager.Current = new ApplicationBalloonManager();
                ApplicationTaskManager.Current = new ApplicationTaskManager((taskInfo) =>
                {

                }, (taskInfo) =>
                {

                });

                ApplicationLinkInfoManager.Current.NotSupportResumableLinkAction = (link) =>
                {
                    ApplicationLinkInfoManager.Current.StopLinkInfo(link);
                    var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                    var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                    var mainActivity = new Intent(currentActivity, typeof(MainActivity));
                    mainActivity.SetFlags(ActivityFlags.NewTask);
                    mainActivity.SetFlags(ActivityFlags.ClearTop);
                    mainActivity.PutExtra("NotSupportResumableLinkAction", link.PathInfo.Id);
                    var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                    notification.SetLatestEventInfo(currentActivity, ViewUtility.FindTextLanguage(currentActivity, "WariningYourLinkNotSupportResumable_Language"), ViewUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageMessage_Language"), pendingIntent);
                    notification.Flags = NotificationFlags.AutoCancel;
                    nMgr.Notify(MaxNotifyID + 4, notification);
                };

                //Agrin.RapidService.Helper.InitializerHelper.Initialize();
                NotificationInit();

                Agrin.Download.Data.DeSerializeData.LoadApplicationData();
                bool mustSave = false;
                if (System.IO.Directory.Exists(ApplicationSetting.Current.PathSetting.DownloadsPath))
                    Agrin.IO.Helper.MPath.DownloadsPath = ApplicationSetting.Current.PathSetting.DownloadsPath;
                else
                {
                    mustSave = true;
                    ApplicationSetting.Current.PathSetting.DownloadsPath = Agrin.IO.Helper.MPath.DownloadsPath;
                }

                CreateGroups();
                //set app osSetting
                ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin Android";
                if (string.IsNullOrEmpty(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid))
                {
                    ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid().ToString();
                    mustSave = true;
                }
                PackageManager manager = currentActivity.PackageManager;
                PackageInfo info = manager.GetPackageInfo(currentActivity.PackageName, 0);
                ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = info.VersionName;
                ApplicationSetting.Current.ApplicationOSSetting.OSName = "Android";
                ApplicationSetting.Current.ApplicationOSSetting.OSVersion = Android.OS.Build.VERSION.Release + " Api Level " + Android.OS.Build.VERSION.Sdk;

                if (!ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme)
                {
                    ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme = true;
                    ApplicationSetting.Current.IsSettingForAllLinks = true;
                    ApplicationSetting.Current.IsSettingForNewLinks = true;
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                }
                else if (mustSave)
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                InitializeApplication.LoadLanguage();
                //Agrin.Download.Manager.ApplicationGroupManager.Current.NoGroup.SavePath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath;

                Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();
            }

            if (Agrin.Download.Data.ApplicationServiceData.Current != null && Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks != null)
            {
                //int count = Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks.Count;
                //if (count == 0)
                //{
                //    MainActivity.ReleaseWakeLock();
                //}
                //else
                foreach (var id in Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks.ToArray())
                {
                    var link = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindLinkInfoByID(id);
                    if (link != null)
                    {
                        if (link.DownloadingProperty.State == Agrin.Download.Web.ConnectionState.Complete)
                        {
                            CheckAddFrameSoftLink(link);
                            InitializeApplication.StartNotify(currentActivity);
                            Agrin.Download.Data.ApplicationServiceData.RemoveItem(id);
                        }
                        else
                        {
                            Agrin.Download.Manager.ApplicationLinkInfoManager.Current.PlayLinkInfo(link);
                        }
                    }
                }
            }
            //if (Agrin.Download.Data.ApplicationServiceData.Current != null && Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks != null)
            //{
            //    try
            //    {
            //        foreach (var id in Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks.ToArray())
            //        {
            //            var link = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindById(id);
            //            if (link != null)
            //            {
            //                if (link.DownloadingProperty.State != Agrin.Download.Web.ConnectionState.Complete)
            //                    Agrin.Download.Manager.ApplicationLinkInfoManager.Current.PlayLinkInfo(link);
            //                else
            //                {
            //                    Agrin.Download.Data.ApplicationServiceData.RemoveItem(id);
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Agrin.Log.AutoLogger.LogError(e, "ApplicationServiceData: ", true);
            //    }
            //}
            //ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(ApplicationLinkInfoManager.Current.LinkInfoes.ToList());

            Func<bool> run = () =>
                {
                    string text = null;
                    if (!string.IsNullOrEmpty(InitializeApplication.ExtrasAllGeted))
                        text = InitializeApplication.ExtrasAllGeted;
                    InitializeApplication.ExtrasAllGeted = "";

                    if (!string.IsNullOrEmpty(text))
                    {
                        AddYoutubeLinkViewModel.CurrentSelectedURL = text;
                        if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(text))
                            ActivitesManager.AddYoutubeLinkActive(BrowseActivity.This);
                        else
                            ActivitesManager.AddNewLinkActive(BrowseActivity.This);
                        return false;
                    }
                    else
                    {
                        ActivitesManager.ToolbarActive(MainActivity.This);
                    }
                    return true;
                };

            if (InitializeApplication.IsSetLanguage)
                return run();
            else
            {
                Agrin.MonoAndroid.UI.Activities.Settings.LanguageSettingActivity.IsBaseSelect = true;
                ActivitesManager.SelectLanguageActive(MainActivity.This, () =>
                {
                    run();
                });
            }

            return true;
        }

        private static void CreateGroups()
        {
            if (ApplicationGroupManager.Current.GroupInfoes.Count == 0)
            {

                Agrin.BaseViewModels.ApplicationBaseLoader.CreateGroups();
                //Agrin.IO.
                //string path = Agrin.IO.Helper.MPath.DownloadsPath;
                //GroupInfo group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Music"), Name = "موسیقی", Extentions = new List<string>() { "mp3", "wav", "wma", "mpa", "ram", "ra", "aac", "aif", "m4a", "m4p", "msv", "oga" } };
                //ApplicationGroupManager.Current.AddGroupInfo(group);

                //group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Videos"), Name = "ویدئو", Extentions = new List<string>() { "avi", "mpg", "mpe", "mpeg", "asf", "wmv", "mov", "qt", "rm", "mp4", "flv", "m4v", "webm", "ogv", "ogg", "mkv", "webm", "vob", "3g2", "svi" } };
                //ApplicationGroupManager.Current.AddGroupInfo(group);

                //group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Images"), Name = "تصاویر", Extentions = new List<string>() { "JPEG", "jpg", "jpe", "JFIF", "Exif", "TIFF", "tif", "RIF", "gif", "bmp", "png", "psd", "flv", "m4v", "webm", "ogv", "ogg", "mkv" } };
                //ApplicationGroupManager.Current.AddGroupInfo(group);

                //group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Document"), Name = "سند", Extentions = new List<string>() { "doc", "pdf", "ppt", "pps", "xls", "xlsx" } };
                //ApplicationGroupManager.Current.AddGroupInfo(group);

                //group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Application"), Name = "نرم افزار", Extentions = new List<string>() { "exe", "msi", "apk" } };
                //ApplicationGroupManager.Current.AddGroupInfo(group);

                //group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Compress"), Name = "فشرده", Extentions = new List<string>() { "zip", "rar", "arj", "gz", "sit", "sitx", "sea", "ace", "bz2", "7z" } };
                //ApplicationGroupManager.Current.AddGroupInfo(group);
                //Agrin.Download.Data.SerializeData.SaveGroupInfoesToFile();
            }
        }

        public static void ShowMessageDialog(object updateInformation, Activity activity)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(ViewUtility.FindTextLanguage(activity, "AgrinDownloadManager_Language"));
            LinearLayout layout = new LinearLayout(activity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.FillParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            TextView txtMessage = new TextView(activity);
            txtMessage.SetTextAppearance(activity, global::Android.Resource.Style.TextAppearanceSmall);
            txtMessage.SetSingleLine(false);
            txtMessage.VerticalScrollBarEnabled = true;
            if (updateInformation is UpdateInformation)
                txtMessage.Text = ((UpdateInformation)updateInformation).Message;
            else if (updateInformation is MessageInformation)
                txtMessage.Text = ((MessageInformation)updateInformation).Message;

            layout.AddView(txtMessage);
            ScrollView scroll = new ScrollView(activity);
            scroll.AddView(layout);
            builder.SetView(scroll);
            // Set up the buttons
            builder.SetPositiveButton(ViewUtility.FindTextLanguage(activity, "OK_Language"), (dialog, which) =>
            {
                if (updateInformation is UpdateInformation)
                    ApplicationSetting.Current.LastApplicationMessageID = ((UpdateInformation)updateInformation).LastApplicationMessageID;
                else if (updateInformation is MessageInformation)
                    ApplicationSetting.Current.LastUserMessageID = ((MessageInformation)updateInformation).LastUserMessageID;
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                builder.Dispose();
            });

            builder.Show();
        }

        public static void InitializeAppIO()
        {
            Agrin.Log.AutoLogger.ApplicationDirectory = Agrin.IO.Helper.MPath.CurrentAppDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");

            string downloadsDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path;
            try
            {
                System.IO.Directory.CreateDirectory(Agrin.IO.Helper.MPath.CurrentAppDirectory);
            }
            catch (System.UnauthorizedAccessException e)
            {
                Agrin.Log.AutoLogger.ApplicationDirectory = downloadsDirectory = Agrin.IO.Helper.MPath.CurrentAppDirectory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Agrin");
                Agrin.Log.AutoLogger.LogError(e, "Storage 1: ", true);
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Storage 2: ", true);
            }
            //try
            //{
            //    //System.IO.Directory.CreateDirectory (Agrin.IO.Helper.MPath.CurrentAppDirectory);
            //    System.IO.File.Create(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory , "/test.txt");
            //}
            //catch (System.UnauthorizedAccessException e)
            //{
            //    Agrin.Log.AutoLogger.LogError(e, "Storage 3: ", true);
            //}
            //catch (Exception e)
            //{
            //    Agrin.Log.AutoLogger.LogError(e, "Storage 4: ", true);
            //}
            Agrin.IO.Helper.MPath.InitializePath(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DataBase"), downloadsDirectory, System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DownloadingSaveData"));
        }
    }

    [Activity(MainLauncher = true, Theme = "@style/Theme.Splash", AlwaysRetainTaskState = true, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        public static Activity This;
        //		public override void OnBackPressed ()
        //		{
        //			SetContentView (Resource.Layout.Toolbar);
        //			ToolbarViewModel toolbar = new ToolbarViewModel ();
        //			toolbar.Initialize ();
        //		}

        public static string AgrinApplicationMessage { get; set; }
        public static string AgrinApplicationUserMessage { get; set; }
        public static int NotSupportResumableLinkAction { get; set; }
        protected override void OnCreate(Bundle bundle)
        {

            //var clipboard = (Android.Text.ClipboardManager)GetSystemService(ClipboardService);
            //var clip = Android.Text.ClipData.NewPlainText("your_text_to_be_copied");
            //this.eti.commander.RelayAPIModel$NativeCalls.InitRelayJava(Native Method)
            try
            {
                AgrinService.MustRunApp = true;
                ActivitesManager.AddActivity(this);
                this.Title = ViewUtility.FindTextLanguage(this, "AgrinDownloadManager_Language");
                This = this;
                base.OnCreate(bundle);
                try
                {
                    //if (this.Intent.HasExtra("AgrinApplicationMessage"))
                    //{

                    AgrinApplicationMessage = this.Intent.GetStringExtra("AgrinApplicationMessage");
                    AgrinApplicationUserMessage = this.Intent.GetStringExtra("AgrinApplicationUserMessage");
                    NotSupportResumableLinkAction = this.Intent.GetIntExtra("NotSupportResumableLinkAction", -1);
                    //}
                }
                catch (Exception ex)
                {
                    Agrin.Log.AutoLogger.LogError(ex, "AgrinApplicationMessage");
                }
                //if (Intent.GetBooleanExtra("EXIT", false))
                //{
                //    //Agrin.Log.AutoLogger.LogError(null, "finished run", true);
                //    Finish();
                //    System.Environment.Exit(0);
                //    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                //    return;
                //}
                //if (ActivityManager.ExitedApp)
                //{
                //    //Agrin.Log.AutoLogger.LogError(null, "ExitedApp", true);
                //    ActivityManager.Exit(this);
                //    return;
                //}
                //if (Intent.GetBooleanExtra("EXIT", false))
                //{
                //    Finish();
                //    return;
                //}

                //**************************************
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
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
        }

        static PowerManager.WakeLock wakeLock;
        static bool isOn = false;
        public static void AcquireWakeLock()
        {
            if (isOn)
                return;
            isOn = true;
            PowerManager pm = (PowerManager)MainActivity.This.GetSystemService(Context.PowerService);
            //wakeLock = pm.NewWakeLock(WakeLockFlags.Full | WakeLockFlags.AcquireCausesWakeup | WakeLockFlags.OnAfterRelease,
            //                          "Agrin Wake Look On");
            wakeLock = pm.NewWakeLock(WakeLockFlags.Full,
                                      "Agrin Wake Look On");
            wakeLock.Acquire();
            //			Toast acquire = Toast.MakeText(getApplicationContext(), "Wake Lock ON",
            //			                               Toast.LENGTH_SHORT);
            //acquire.Show();
        }

        public static void ReleaseWakeLock()
        {
            if (!isOn || wakeLock == null)
                return;
            isOn = false;
            wakeLock.Release();
            //			Toast release = Toast.MakeText(getApplicationContext(),
            //			                               "Wake Lock OFF", Toast.LENGTH_SHORT);
            //release.Show();
        }


        public override void Finish()
        {
            ActivitesManager.RemoveActivity(this);
            base.Finish();
        }

        public override void OnContentChanged()
        {
            base.OnContentChanged();
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

