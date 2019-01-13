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
using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.Framesoft.Helper;
using Agrin.Views;
using Agrin.Download.Manager;
using Android.Content.PM;
using Agrin.Download.Engine;
using Agrin.Services;
using Agrin.BaseViewModels;
using Agrin.IO.Helper;
using Agrin.HardWare;
using Agrin.Log;
using Agrin.IO.Streams;

namespace Agrin.Helpers
{
    public static class InitializeApplication
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
                Agrin.Log.AutoLogger.LogText("Unhandle Java Exception Error!" + ex.Message);
                var baseEX = ex.GetBaseException();
                if (baseEX != null)
                    Agrin.Log.AutoLogger.LogError(baseEX, "Unhandle Java Base Exception");

                if (ex.InnerException != null)
                    Agrin.Log.AutoLogger.LogError(ex.InnerException, "Unhandle Java InnerException Exception");
                if (ex.StackTrace != null)
                    Agrin.Log.AutoLogger.LogText("Unhandle Java Stack Trace Exception Message:" + ex.StackTrace);
            }
        }

        public static void Initialize(Context active)
        {
            Agrin.Log.AutoLogger.ForceLog = true;
            System.AppDomain.CurrentDomain.UnhandledException += ErrorData;
            AndroidEnvironment.UnhandledExceptionRaiser += ErrorData2;
            Java.Lang.Thread.DefaultUncaughtExceptionHandler = new UnCaughtExceptionHandler(active);
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Agrin.Log.AutoLogger.LogError(args.Exception, "Unhandle UnobservedTaskException");
            };
        }

        public static void ErrorData(object sender, UnhandledExceptionEventArgs e)
        {
            Exception error = (Exception)e.ExceptionObject;
            GoException(error, "Unhandle ErrorData");
        }

        public static void ErrorData2(object sender, RaiseThrowableEventArgs e)
        {
            Exception error = (Exception)e.Exception;
            GoException(error, "Unhandle ErrorData2");
            e.Handled = true;
        }

        public static void GoException(Exception error, string title = "")
        {
            //if (error is Exception)
            //{
            //    MainActivity.notify(error.Message, title + " GoException EX");
            //}
            //else if (error is Java.Lang.Exception)
            //{
            //    MainActivity.notify(((Java.Lang.Exception)error).Message, "GoException Java");
            //}
            if (Agrin.Log.AutoLogger.ApplicationDirectory == null)
                Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");
            Agrin.Log.AutoLogger.LogError(error, title, true);
            //ActivitesManager.MessageBoxActive();
        }

        public static void GoException(string text)
        {
            if (Agrin.Log.AutoLogger.ApplicationDirectory == null)
                Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");

            Agrin.Log.AutoLogger.LogError(new Exception(text), text, true);
        }

        public static void SaveLanguage(string lang)
        {
            if (lang == "english")
            {
                IsSetLanguage = true;
                ViewsUtility.ProjectDirection = FlowDirection.LeftToRight;
                ViewsUtility.ApplicationLanguage = "_en";
            }
            else if (lang == "persian")
            {
                IsSetLanguage = true;
                ViewsUtility.ProjectDirection = FlowDirection.RightToLeft;
                ViewsUtility.ApplicationLanguage = "_fa";
            }
            ApplicationSetting.Current.ApplicationLanguage = lang;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
        }

        public static void LoadLanguage()
        {
            IsSetLanguage = true;
            string lang = ApplicationSetting.Current.ApplicationLanguage;
            if (lang == "english")
            {
                ViewsUtility.ProjectDirection = FlowDirection.LeftToRight;
                ViewsUtility.ApplicationLanguage = "_en";
            }
            else if (lang == "persian")
            {
                ViewsUtility.ProjectDirection = FlowDirection.RightToLeft;
                ViewsUtility.ApplicationLanguage = "_fa";
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

        //public static void StartNotify(Context context)
        //{
        //    if (context == null)
        //        return;
        //    var nMgr = (NotificationManager)context.GetSystemService(Context.NotificationService);

        //    var notification = new Notification(Resource.Drawable.smallIcon, ViewsUtility.FindTextLanguage(context, "AgrinDownloadManager_Language"));
        //    var mainActivity = new Intent(context, typeof(MainActivity));
        //    mainActivity.SetFlags(ActivityFlags.NewTask);
        //    mainActivity.SetFlags(ActivityFlags.ClearTop);
        //    var pendingIntent = PendingIntent.GetActivity(context, 0, mainActivity, 0);
        //    notification.SetLatestEventInfo(context, ViewsUtility.FindTextLanguage(context, "AgrinDownloadManager_Language"), ViewsUtility.FindTextLanguage(context, "CompleteDownloadNotify_Language"), pendingIntent);
        //    notification.Flags = NotificationFlags.AutoCancel;
        //    nMgr.Notify(0, notification);
        //}

        static void NotificationInit()
        {
            ApplicationNotificationManager.Current.NotificationInfoChanged = (mode, linkInfo) =>
            {
                if (mode == Download.Web.NotificationMode.Complete)
                {
                    if (linkInfo != null)
                    {
                        //CheckAddFrameSoftLink(linkInfo);
                        Agrin.Download.Data.ApplicationServiceData.RemoveItem(linkInfo.PathInfo.Id);
                        try
                        {
                            Android.Media.MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { linkInfo.PathInfo.FullAddressFileName }, null, null);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            };
        }

        public static void CancelNotify(int id)
        {
            if (_notifyManager != null)
                _notifyManager.Cancel(id);
        }

        public static void CleanUpManualStop(Context context)
        {
            foreach (var item in ManualNotifyStop.ToArray())
            {
                if (!IsNotificationVisible(item, context))
                    ManualNotifyStop.Remove(item);
            }
        }

        private static bool IsNotificationVisible(int id, Context context)
        {
            if (Agrin.Download.Manager.ApplicationLinkInfoManager.Current == null)
            {
                return false;
            }
            var linkID = id - _maxReserveRange;
            var linkInfo = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindLinkInfoByID(linkID);
            if (linkInfo != null && (linkInfo.IsComplete || linkInfo.CanStop))
                return false;
            Intent notificationIntent = new Intent(context, typeof(NotificationClickActivity));
            PendingIntent test = PendingIntent.GetActivity(context, id, notificationIntent, PendingIntentFlags.NoCreate);
            return test != null;
        }

        static List<int> downloadingIDS = new List<int>();
        static NotificationManager _notifyManager = null;
        static Notification _notification = null;
        public static int _maxReserveRange = 10;
        public static List<int> ManualNotifyStop = new List<int>();
        public static void RemoteNotify()
        {
            if (ApplicationLinkInfoManager.Current == null)
                return;
            if (_notifyManager == null)
            {
                Java.Lang.ICharSequence title = null;
                Java.Lang.ICharSequence content = null;
                title = new Java.Lang.String(ViewsUtility.FindTextLanguage(CurrentContext, "AgrinDownloadManager_Language"));
                content = new Java.Lang.String("");



                if ((int)Android.OS.Build.VERSION.SdkInt < 11)
                {
                    _notification = new Notification(Resource.Drawable.smallIcon, ViewsUtility.FindTextLanguage(CurrentContext, "AgrinDownloadManager_Language"), DateTime.Now.Ticks);
                    //Intent notificationIntent = new Intent(CurrentContext, typeof(MainActivity));
                    //notification.LargeIcon = GetLargeIconBySize();
                    _notification.SetLatestEventInfo(CurrentContext, title, content, null);
                }
                else
                {
                    var builder = new Notification.Builder(CurrentContext).SetContentTitle(title).SetContentText("").SetSmallIcon(Resource.Drawable.smallIcon);
                    try
                    {
                        builder = builder.SetLargeIcon(ViewsUtility.GetLargeIconBySize(CurrentContext));
                    }
                    catch
                    {
                    }
                    if (Build.VERSION.SdkInt >= Build.VERSION_CODES.JellyBean)
                    {
                        _notification = builder.Build();
                    }
                    else
                    {
                        _notification = builder.Notification;
                    }
                    //_notification = builder.Build(); // available from API level 11 and onwards
                }
                _notifyManager = (NotificationManager)CurrentContext.GetSystemService(Context.NotificationService);


                //var mainActivity = new Intent(CurrentContext, typeof(MainActivity));
                //mainActivity.SetFlags(ActivityFlags.NewTask);
                //mainActivity.SetFlags(ActivityFlags.ClearTop);
                //var pendingIntent = PendingIntent.GetActivity(CurrentContext, 0, mainActivity, 0);
                //_notifyManager = (NotificationManager)CurrentContext.GetSystemService(Context.NotificationService);
                //_notification = new Notification(Resource.Drawable.smallIcon, ViewsUtility.FindTextLanguage(CurrentContext, "AgrinDownloadManager_Language"));
                //_notification.Flags = NotificationFlags.OnlyAlertOnce;
                //_notification.SetLatestEventInfo(CurrentContext, ViewsUtility.FindTextLanguage(CurrentContext, "AgrinDownloadManager_Language"), "", pendingIntent);
            }
            if (ApplicationLinkInfoManager.Current.RemovedFromDownloadingLinkInfoes == null)
                ApplicationLinkInfoManager.Current.RemovedFromDownloadingLinkInfoes = (item) =>
                {
                    int linkId = item.PathInfo.Id + _maxReserveRange;
                    var state = item.DownloadingProperty.State;
                    if ((!item.IsManualStop && state != Download.Web.ConnectionState.Complete) || downloadingIDS.Contains(linkId))
                    {
                        if (ApplicationSetting.Current.IsShowNotification)
                        {
                            RemoteViews bigView = new RemoteViews(CurrentContext.PackageName, Resource.Layout.LinkInfoItemNotify);
                            string fileName = item.PathInfo.FileName;
                            bigView.SetTextViewText(Resource.LinkInfoItemNotify.txtFileName, fileName);

                            string[] size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.Size);
                            var text = size[0] + " " + ViewsUtility.FindTextLanguage(CurrentContext, size[1] + "_Language");
                            bigView.SetTextViewText(Resource.LinkInfoItemNotify.txtSize, text);

                            var downSize = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.DownloadedSize);
                            text = downSize[0] + " " + ViewsUtility.FindTextLanguage(CurrentContext, downSize[1] + "_Language");
                            bigView.SetTextViewText(Resource.LinkInfoItemNotify.txtDownloadedSize, text);

                            text = ViewsUtility.FindTextLanguage(CurrentContext, state.ToString() + "_Language");
                            bigView.SetTextViewText(Resource.LinkInfoItemNotify.txtState, text);

                            //SetProgressStyleByState(prgDownload, linearImage, item);
                            //SetTextViewTextColorByState(txtFileName, item);

                            text = ViewsUtility.TimeToString(CurrentContext, item.DownloadingProperty.TimeRemaining);
                            bigView.SetTextViewText(Resource.LinkInfoItemNotify.txtTimeRemaning, text);

                            text = item.DownloadingProperty.GetPercent;
                            bigView.SetTextViewText(Resource.LinkInfoItemNotify.txtPercent, text);

                            size = Agrin.Helper.Converters.MonoConverters.GetSizeStringSplit(item.DownloadingProperty.SpeedByteDownloaded);
                            text = size[0] + " " + ViewsUtility.FindTextLanguage(CurrentContext, size[1] + "_Language");
                            bigView.SetTextViewText(Resource.LinkInfoItemNotify.txtSpeed, text);


                            if (item.DownloadingProperty.Size >= 0.0)
                                bigView.SetProgressBar(Resource.LinkInfoItemNotify.prgDownload, 100, (int)(100 / (item.DownloadingProperty.Size / item.DownloadingProperty.DownloadedSize)), false);
                            else
                                bigView.SetProgressBar(Resource.LinkInfoItemNotify.prgDownload, 100, 0, false);

                            _notification.ContentView = bigView;

                            var activity = new Intent(CurrentContext, typeof(NotificationClickActivity));
                            activity.SetFlags(ActivityFlags.SingleTop);
                            activity.SetFlags(ActivityFlags.ClearTop);
                            activity.PutExtra("LinkId", linkId);
                            var pendingIntent = PendingIntent.GetActivity(CurrentContext, linkId, activity, PendingIntentFlags.UpdateCurrent);

                            _notification.ContentIntent = pendingIntent;

                            _notifyManager.Notify(linkId, _notification);
                        }

                        if ((item.IsManualStop || state == Download.Web.ConnectionState.Complete) && state != Download.Web.ConnectionState.CopyingFile && downloadingIDS.Contains(linkId))
                        {
                            downloadingIDS.Remove(linkId);
                            if (item.IsManualStop && !ManualNotifyStop.Contains(linkId))
                                CancelNotify(linkId);
                        }
                        else if (!downloadingIDS.Contains(linkId))
                            downloadingIDS.Add(linkId);
                    }
                };

            var downloadingItems = ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToArray();

            foreach (var item in downloadingItems)
            {
                ApplicationLinkInfoManager.Current.RemovedFromDownloadingLinkInfoes(item);
            }

            foreach (var linkID in downloadingIDS.ToArray())
            {
                var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID(linkID - _maxReserveRange);
                if (!downloadingItems.Contains(linkInfo))
                {
                    ApplicationLinkInfoManager.Current.RemovedFromDownloadingLinkInfoes(linkInfo);
                    if (linkInfo.IsManualStop)
                    {
                        if (!ManualNotifyStop.Contains(linkID))
                            CancelNotify(linkID);
                    }
                }
            }

            if (AgrinService.This != null)
            {
                AgrinService.This.CheckAppForeground();
                //if (!isNotify)
                //{
                //    AgrinService.This.ApplicationStopedDownloading();
                //    MainActivity.ReleaseWakeLock();
                //}
                //else
                //{
                //    AgrinService.This.ApplicationIsDownloading();
                //    MainActivity.AcquireWakeLock();
                //}
            }
        }

        static bool initLimit = false;
        static Context _CurrentContext = null;

        public static Context CurrentContext
        {
            get { return InitializeApplication._CurrentContext; }
            set { InitializeApplication._CurrentContext = value; }
        }

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
                        //ActivityManager am = (ActivityManager)CurrentContext.GetSystemService(Context.ActivityService);

                        //// get the info from the currently running task
                        //var taskInfo = am.GetRunningTasks(1);

                        //ComponentName componentInfo = taskInfo[0].TopActivity;
                        //if (!componentInfo.PackageName.Equals("Agrin.MonoAndroid"))
                        //{
                        //    //Do your stuff
                        //    //Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StopNotifyChanging();
                        //}
                        //else
                        //{
                        //    //Agrin.Helper.ComponentModel.ANotifyPropertyChanged.StartNotifyChanging();
                        //}

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
        static object lockObj = new object();
        //public static int MaxNotifyID = 569000;
        //static int nooooo = 6546464;
        public static bool Run(ContextWrapper currentActivity)
        {
            IOHelperBase.Current = new IOHelper() { Activity = currentActivity };
            if (string.IsNullOrEmpty(Agrin.Log.AutoLogger.ApplicationDirectory))
                Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");
            StreamCross.OpenFile = (path, fileMode, fileAccess) =>
            {
                return new AndroidStreamCross(path, fileMode, fileAccess);
            };
            //var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);
            //var builder = new Notification.Builder(currentActivity).SetContentTitle("title").SetContentText(Inited.ToString()).SetSmallIcon(Resource.Drawable.smallIcon);
            //try
            //{
            //    builder = builder.SetLargeIcon(ViewsUtility.GetLargeIconBySize(currentActivity));
            //}
            //catch
            //{
            //}
            //var not=builder.Build();
            //nMgr.Notify(nooooo, not);
            //nooooo++;
            lock (lockObj)
            {

                CurrentContext = currentActivity;
                BindingExpresionEventManager.CurrentActivity = MainActivity.This;

                //ClipboardHelper.InitializeClipboardChangedAction();
                //GoException("Run: " + Inited.ToString() + " " + DateTime.Now.ToString());
                if (!Inited)
                {
                    ApplicationHelperBase.Current = new ApplicationHelper();
                    //WindowsControllerBase.Current = new WindowsController();

                    Inited = true;
                    DriveHelper.Current = new DriveHelper();
                    Framesoft.Helper.FeedBackHelper.DefaultLimitMessage = Framesoft.Messages.LimitMessageEnum.Android;
                    Agrin.Download.Engine.TimeDownloadEngine.IsNewVersionOfAndroid = true;
                    TaskRunner.ChangeWifiStateAction = (value) =>
                    {
                        DeviceHelper.SetWifiEnable(currentActivity, value);
                    };
                    Agrin.Download.Engine.TimeDownloadEngine.UpdatedAction = (update) =>
                    {
                        ////her code for new Application Version Update
                        //var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                        //var notification = new Notification(Resource.Drawable.smallIcon, ViewsUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                        //var mainActivity = new Intent(currentActivity, typeof(BrowseActivity));
                        //mainActivity.SetFlags(ActivityFlags.NewTask);
                        //mainActivity.SetFlags(ActivityFlags.ClearTop);
                        //mainActivity.PutExtra(Intent.ExtraText, update.DownloadUri);
                        //var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                        //notification.SetLatestEventInfo(currentActivity, ViewsUtility.FindTextLanguage(currentActivity, "AgrinApplicationNewVersionTitle_Language"), ViewsUtility.FindTextLanguage(currentActivity, "AgrinApplicationNewVersionMessage_Language"), pendingIntent);
                        //notification.Flags = NotificationFlags.AutoCancel;

                        //nMgr.Notify(MaxNotifyID + 1, notification);
                    };

                    Agrin.Download.Engine.TimeDownloadEngine.GetLastMessageAction = (update) =>
                    {
                        //var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                        //var notification = new Notification(Resource.Drawable.smallIcon, ViewsUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                        //var mainActivity = new Intent(currentActivity, typeof(MainActivity));
                        //mainActivity.SetFlags(ActivityFlags.NewTask);
                        //mainActivity.SetFlags(ActivityFlags.ClearTop);
                        //mainActivity.PutExtra("AgrinApplicationMessage", Newtonsoft.Json.JsonConvert.SerializeObject(update));
                        //var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                        //notification.SetLatestEventInfo(currentActivity, ViewsUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageTitle_Language"), ViewsUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageMessage_Language"), pendingIntent);
                        //notification.Flags = NotificationFlags.AutoCancel;

                        //nMgr.Notify(MaxNotifyID + 2, notification);
                    };

                    Agrin.Download.Engine.TimeDownloadEngine.GetUserMessageAction = (userMessage) =>
                    {
                        //if (userMessage.GUID == ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid)
                        //{
                        //    if (userMessage.LastUserMessageID > ApplicationSetting.Current.LastUserMessageID)
                        //    {
                        //        //her code for new User Message
                        //        if (string.IsNullOrEmpty(userMessage.Message))
                        //            return;

                        //        var nMgr = (NotificationManager)currentActivity.GetSystemService(Context.NotificationService);

                        //        var notification = new Notification(Resource.Drawable.smallIcon, ViewsUtility.FindTextLanguage(currentActivity, "AgrinDownloadManager_Language"));
                        //        var mainActivity = new Intent(currentActivity, typeof(MainActivity));
                        //        mainActivity.SetFlags(ActivityFlags.NewTask);
                        //        mainActivity.SetFlags(ActivityFlags.ClearTop);
                        //        mainActivity.PutExtra("AgrinApplicationUserMessage", Newtonsoft.Json.JsonConvert.SerializeObject(userMessage));
                        //        var pendingIntent = PendingIntent.GetActivity(currentActivity, 0, mainActivity, PendingIntentFlags.UpdateCurrent);
                        //        notification.SetLatestEventInfo(currentActivity, ViewsUtility.FindTextLanguage(currentActivity, "AgrinApplicationUserMessageTitle_Language"), ViewsUtility.FindTextLanguage(currentActivity, "AgrinApplicationMessageMessage_Language"), pendingIntent);
                        //        notification.Flags = NotificationFlags.AutoCancel;

                        //        nMgr.Notify(MaxNotifyID + 3, notification);
                        //    }
                        //}
                    };

                    Agrin.Download.Web.Link.LinkInfoManagement.LinkInfoErrorAction = (error) =>
                    {
                        MainActivity.This.RunOnUI(() =>
                        {
                            try
                            {
                                if (Agrin.Download.Data.Settings.ApplicationSetting.Current.IsShowErrorMessageOnScreen)
                                {
                                    if ((bool)error?.Message?.ToLower()?.Contains("disk full"))
                                        Toast.MakeText(currentActivity, "خطا: " + System.Environment.NewLine + "حافظه ی گوشی شما پر شده است، لطفاً فضای ذخیره سازی را برای ادامه ی دانلود افزایش دهید.", ToastLength.Long).Show(); // For example
                                    else
                                        Toast.MakeText(currentActivity, "خطا: " + System.Environment.NewLine + "" + error.Message, ToastLength.Long).Show(); // For example
                                }
                            }
                            catch (Exception ex)
                            {
                                GoException(ex, "LinkInfoErrorAction Toast");
                            }
                        });
                    };

                    InitializeApplication.Initialize(currentActivity);
                    InitializeApplication.InitializeAppIO();
                    ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
                    ApplicationGroupManager.Current = new ApplicationGroupManager();
                    ApplicationNotificationManager.Current = new ApplicationNotificationManager();
                    ApplicationBalloonManager.Current = new ApplicationBalloonManager();


                    ApplicationTaskManager.Current = new ApplicationTaskManager((taskInfo) =>
                    {
                        if (taskInfo == null)
                        {
                            GoException("Start taskInfo == null");
                            return;
                        }
                        try
                        {
                            AutoLogger.LogTextTest($"android ApplicationTaskManager start {taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.WiFiOn)} {taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.InternetOn)}");
                            if (taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.WiFiOn))
                            {
                                DeviceHelper.SetWifiEnable(currentActivity, true);
                            }
                            if (taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.InternetOn))
                            {
                                DeviceHelper.SetMobileDataEnabled(currentActivity, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            GoException(ex, "ApplicationTaskManager.Current Start");
                        }
                    }, (taskInfo) =>
                    {
                        try
                        {
                            if (taskInfo == null)
                            {
                                GoException("Stop taskInfo == null");
                                return;
                            }
                            AutoLogger.LogTextTest($"android ApplicationTaskManager start {taskInfo.LinkCompleteTaskUtilityActions.Contains(TaskUtilityModeEnum.WiFiOff)} {taskInfo.LinkCompleteTaskUtilityActions.Contains(TaskUtilityModeEnum.InternetOff)}");
                            if (taskInfo.LinkCompleteTaskUtilityActions.Contains(TaskUtilityModeEnum.WiFiOff))
                            {
                                DeviceHelper.SetWifiEnable(currentActivity, false);
                            }
                            if (taskInfo.LinkCompleteTaskUtilityActions.Contains(TaskUtilityModeEnum.InternetOff))
                            {
                                DeviceHelper.SetMobileDataEnabled(currentActivity, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            GoException(ex, "ApplicationTaskManager.Current Stop");
                        }
                    });


                    ApplicationLinkInfoManager.Current.NotSupportResumableLinkAction = (link) =>
                    {
                        MainActivity.This.RunOnUI(() =>
                        {
                            ViewsUtility.ShowYesCancelMessageBox(MainActivity.This, "خطا در دانلود از ادامه", "مشکلی در دانلود از ادامه رخ داده است، بدین منظور که لینک مورد نظر شما قابلیت توقف از سوی سرور دریافت نمی کند اگر میخواهید لینک شما از ابتدا دانلود شود روی قبول کلیک کنید در غیر اینصورت انصراف زده و مجدد سعی نمایید.", () =>
                            {
                                ApplicationLinkInfoManager.Current.ClearDataForNotSupportResumableLink(link);
                                ApplicationLinkInfoManager.Current.PlayLinkInfo(link);
                            });
                        });
                    };

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

                    ApplicationBaseLoader.CreateGroups();
                    //set app osSetting
                    ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin Android";
                    if (string.IsNullOrEmpty(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid))
                    {
                        ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid().ToString();
                        mustSave = true;
                    }
                    PackageManager manager = currentActivity.PackageManager;
                    PackageInfo info = manager.GetPackageInfo(currentActivity.ApplicationContext.PackageName, 0);
                    ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = info.VersionName;
                    ApplicationSetting.Current.ApplicationOSSetting.OSName = "Android";
                    ApplicationSetting.Current.ApplicationOSSetting.OSVersion = Android.OS.Build.VERSION.Release + " Api Level " + Android.OS.Build.VERSION.Sdk;
                    Agrin.Log.AutoLogger.AppVersion = ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion;
                    Agrin.Log.AutoLogger.ApplicationGuid = ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid;
                    if (!ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme)
                    {
                        ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme = true;
                        ApplicationSetting.Current.IsSettingForAllLinks = true;
                        ApplicationSetting.Current.IsSettingForNewLinks = true;
                        Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    }
                    else if (mustSave)
                        Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    if (!ApplicationSetting.Current.DontShowHelpPage)
                    {
                        MainActivity.This.RunOnUI(() =>
                        {
                            ShowHelpPage(MainActivity.This);
                        });
                    }

                    InitializeApplication.LoadLanguage();
                    //Agrin.Download.Manager.ApplicationGroupManager.Current.NoGroup.SavePath = Agrin.Download.Data.Settings.ApplicationSetting.Current.PathSetting.DownloadsPath;

                    Agrin.Download.Engine.TimeDownloadEngine.StartTransferRate();


                }
                //storage access framework
                //IOHelperBase.Current.OpenFileStreamForWriteAction = (fileAddress, fileName, fileMode, newAddressAction, link) =>
                //{
                //    if (fileAddress.StartsWith("content://"))
                //    {
                //        //StringBuilder bulder = new StringBuilder();
                //        try
                //        {
                //            //bulder.AppendLine("child == null");
                //            //bulder.AppendLine("fileAddress : " + fileAddress ?? "null");
                //            //bulder.AppendLine("fileName : " + fileName ?? "null");

                //            //var linkInfo = link as LinkInfo;
                //            Android.Net.Uri fileUri = null;
                //            //if (linkInfo != null && string.IsNullOrEmpty(linkInfo.PathInfo.SecurityPath))
                //            //{
                //            //    //InitializeApplication.GoException("null linkInfo.PathInfo.SecurityPath" + bulder.ToString());
                //            //    return null;
                //            //}

                //            if (!string.IsNullOrEmpty(fileName))
                //            {
                //                Android.Net.Uri childUri = null;
                //                Android.Net.Uri path = Android.Net.Uri.Parse(fileAddress);
                //                if (ViewsUtility.GetApiVersion() >= 21)
                //                {
                //                    var treedocId = Android.Provider.DocumentsContract.GetTreeDocumentId(path);
                //                    Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUriUsingTree(path, treedocId);
                //                    childUri = Android.Provider.DocumentsContract.CreateDocument(currentActivity.ContentResolver, docUri, ViewsUtility.GetMimeType(fileName), fileName);
                //                    if (childUri == null)
                //                    {
                //                        throw new Exception("Could not create file! Please change save address...");
                //                    }
                //                }
                //                else
                //                {
                //                    throw new Exception("Could not create file! Please change save address...");
                //                    var id = Android.Provider.DocumentsContract.GetDocumentId(path);
                //                    //Android.Provider.
                //                    //return new Agrin.Streams.AndroidStreamWriter(new Java.IO.FileOutputStream(realPath));
                //                    var aq = currentActivity.ContentResolver.AcquireContentProviderClient(Android.Provider.ContactsContract.Authority);

                //                    childUri = FileUtils.createDocumentWithFlags(aq, id, ViewsUtility.GetMimeType(fileName), fileName, 0);
                //                    //Android.Net.Uri docUri = Android.Provider.DocumentsContract.BuildDocumentUri("com.example.documentsprovider.authority", id);
                //                    //childUri = docUri;
                //                }
                //                newAddressAction?.Invoke(childUri.ToString());
                //                fileUri = childUri;
                //            }
                //            else
                //                fileUri = Android.Net.Uri.Parse(fileAddress);
                //            ParcelFileDescriptor pfd = currentActivity.ContentResolver.OpenFileDescriptor(fileUri, "w");
                //            var fileD = pfd.FileDescriptor;
                //            Java.IO.FileOutputStream fileOutputStream = new Java.IO.FileOutputStream(fileD);
                //            //InitializeApplication.GoException("OK-o : " + bulder.ToString());

                //            return new Agrin.Streams.AndroidStreamWriter(fileOutputStream) { FileDescriptor = fileD, ParcelFileDescriptor = pfd };
                //        }
                //        catch (Exception ex)
                //        {
                //            GoException(ex);
                //        }
                //    }
                //    return null;
                //};

                if (Agrin.Download.Data.ApplicationServiceData.Current != null && Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks != null)
                {
                    foreach (var id in Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks.ToArray())
                    {
                        var link = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindLinkInfoByID(id);
                        if (link != null)
                        {
                            if (link.DownloadingProperty.State == Agrin.Download.Web.ConnectionState.Complete)
                            {
                                CheckAddFrameSoftLink(link);
                                //InitializeApplication.StartNotify(currentActivity);
                                Agrin.Download.Data.ApplicationServiceData.RemoveItem(id);
                            }
                            else
                            {
                                Agrin.Download.Manager.ApplicationLinkInfoManager.Current.PlayLinkInfo(link);
                            }
                        }
                    }
                }
                InitializeLimitDrawing();
                return true;
            }
        }

        private static void ShowHelpPage(Activity activity)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("سلام کاربر گرامی، ممنون از اینکه از نرم افزار آگرین استفاده می کنید، اینها نکات مهمی است که قبل از شروع باید بدانید تا استفاده از نرم افزار برایتان آسان شود، لطفا حداقل یکبار مطالعه کنید بعد از مطالعه میتوانید دکمه ی عدم نمایش را بزنید تا دیگر برایتان نمایش داده نشود:");
            text.AppendLine("1.از منوی اصلی نرم افزار می توانید آموزش نرم افزار را مطالعه کنید، منوی اصلی نرم افزار در بالای نرم افزار سمت چپ به صورت علامت سه نقطه مشخص شده است");
            text.AppendLine("2.هر لینکی که ایجاد میکنید را میتوانید مدیریت کنید، مثلا اگر لینکی تکمیل شد با نگه داشتن انگشت روی آن لینک میتوانید آن را اجرا کنید");
            text.AppendLine("3.مدیریت زمانبندی به صورت پیشفرض در منوی اصلی خالی می باشد برای اینکه پر شود شما باید لینک خود را زمانبندی کنید، هر لینکی که انتخاب کنید از نوار ابزار پایین صفحه یا روی آن انگشت را نگه داشته باشید میتوانید زمانبندی کنید");
            text.AppendLine("4.نرم افزار دارای مرورگر داخلی است که میتوانید از منوی اصلی وارد آن شوید و از آن استفاده کنید");
            text.AppendLine("5.هرگونه سوال و مشکل را در صفحه ی اعلانات میتوانید از ما بپرسید صفحه ی اعلانات در نوار ابزار بالای صفحه ی شما قرار دارد یا به گروه تلگرام آگرین بیایید و آنجا به صورت انلاین پاسخگوی شما خواهیم بود لینک گروه آگرین را میتوانید از بخش توضیحات اپ استوری که دانلود کردید یا از قسمت پیام های بخش اعلانات باز کنید");
            text.AppendLine("6.اگر از استفاده از نرم افزار راضی بودید لطفا با نظرات مثبت خود در اپ استور هایی که آگرین را از آن دانلود کردید برای ما نظر ارسال کنید آگرین تا به امروز به واسطه ی نظرات کاربران پیشرفت داشته است");
            text.AppendLine("7.ما آگرین را همیشه بروزرسانی میکنیم از بروزرسانی ها بی خبر نمانید و منتظر خبر های خوب در مورد دانلود منیجر آگرین باشید");
            text.AppendLine("");
            text.AppendLine("امیدوارم در استفاده از نرم افزار لذت ببرید، پشتیبانی آگرین در تلگرام، ایمیل،در نرم افزار همیشه پاسخگوی سوالات، مشکلات و پیشنهادات شماست.");
            
            TextView txtMessage = new TextView(activity);

            LinearLayout layout = new LinearLayout(activity);
            layout.Orientation = Orientation.Vertical;
            layout.SetPadding(10, 10, 10, 10);
            ScrollView scroll = new ScrollView(activity);
            scroll.AddView(txtMessage);
            ViewsUtility.SetBackground(activity, layout);
            ViewsUtility.SetTextViewTextColor(activity, txtMessage, Resource.Color.foreground);
            
            txtMessage.Text = text.ToString();
            
            layout.AddView(scroll);

            ViewsUtility.ShowCustomResultDialog(activity, "لطفا مطالعه کنید.", layout, () =>
            {

            }, () =>
            {

            }, ()=>
            {
                ApplicationSetting.Current.DontShowHelpPage = true;
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            }, "بعدا میخوانم", "خواندم، دیگر نمایش نده",false);
        }

        public static void TriggerStorageAccessFramework()
        {
            if ((int)Build.VERSION.SdkInt == 19)
            {
                Intent intent = new Intent(Intent.ActionCreateDocument)
                    .AddCategory(Intent.CategoryOpenable)
                    .SetType(Android.Provider.DocumentsContract.Document.MimeTypeDir);

                MainActivity.This.StartActivityForResult(intent, 42);
            }
            else
            {
                Android.Content.Intent intent = new Android.Content.Intent(Intent.ActionOpenDocumentTree);
                MainActivity.This.StartActivityForResult(intent, 42);
            }
        }

        public static void ShowMessageDialog(object updateInformation, Activity activity)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(activity);
            builder.SetTitle(ViewsUtility.FindTextLanguage(activity, "AgrinDownloadManager_Language"));
            LinearLayout layout = new LinearLayout(activity);
            layout.Orientation = Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

            layoutParams.SetMargins(5, 5, 5, 5);
            layout.LayoutParameters = layoutParams;

            TextView txtMessage = new TextView(activity);
            ViewsUtility.SetTextAppearance(activity, txtMessage, Android.Resource.Attribute.TextAppearanceSmall);
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
            builder.SetPositiveButton(ViewsUtility.FindTextLanguage(activity, "OK_Language"), (dialog, which) =>
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

            Agrin.IO.Helper.MPath.InitializePath(System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DataBase"), downloadsDirectory, System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "DownloadingSaveData"));
        }
    }
}