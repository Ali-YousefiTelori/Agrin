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
using Agrin.Views;
using Agrin.Helpers;
using Android.Net.Wifi;
using Android.Net;
using System.Threading;
using Agrin.Log;

namespace Agrin.Services
{
    [Service]
    [IntentFilter(new String[] { "agrin.android.agrinservice" })]
    public class AgrinService : IntentService
    {
        static string serviceName = "agrin.android.agrinservice";
        public static AgrinService This;

        public override void OnDestroy()
        {
            StartTimeService();
            base.OnDestroy();
        }

        Alarm alarm = new Alarm();
        bool isSetedOnce = false;
        public void AlarmSetNow(TimeSpan time)
        {
            if (isSetedOnce)
                alarm.CancelAlarm(this);

            isSetedOnce = true;
            alarm.SetAlarm(this, time);
        }

        public void AlarmCancel()
        {
            if (isSetedOnce)
                alarm.CancelAlarm(this);
            isSetedOnce = false;
        }

        public void StartTimeService()
        {
            //if (CheckAppForeground())
            //{
            //    var context = CuurentContext;
            //    var intentFilter = new IntentFilter(AgrinService.StocksUpdatedAction) { Priority = (int)IntentFilterPriority.HighPriority };
            //    context.RegisterReceiver(stockReceiver, intentFilter);
            //    stockServiceConnection = new StockServiceConnection(MainActivity.This);
            //    context.BindService(stockServiceIntent, stockServiceConnection, Bind.AutoCreate);
            //    ScheduleStockUpdates();
            //}
            Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
        }


        public void RestartAllRunners()
        {
            foreach (var item in Agrin.Download.Manager.ApplicationTaskManager.Current.TaskInfoes.ToArray())
            {
                if (item.IsActive && item.State != Download.Web.TaskState.Working)
                {
                    Agrin.Download.Manager.ApplicationTaskManager.Current.DeActiveTask(item);
                    Agrin.Download.Manager.ApplicationTaskManager.Current.ActiveTask(item);
                }
            }
        }

        public override bool StopService(Intent name)
        {
            StartTimeService();
            return base.StopService(name);
        }

        public override void OnLowMemory()
        {
            InitializeApplication.GoException("AgrinService OnLowMemory");
            base.OnLowMemory();
        }

        public static ContextWrapper CuurentContext = null;
        public static bool MustRunApp { get; set; }
        public static bool MustInitializeUI { get; set; }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartCommandNow();
            return StartCommandResult.Sticky;
        }

        public void StartCommandNow()
        {
            //Agrin.Download.Data.DeSerializeData.ChangeState(Download.Data.LoadingStateEnum.InitializeServicesAndUI);
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    This = this;
                    //stockServiceIntent = new Intent("agrin.android.agrinservice");
                    //stockReceiver = new StockReceiver();
                    CuurentContext = this;
                    //if (this.ApplicationContext != null)
                    //    CuurentContext = this.ApplicationContext;
                    //else
                    //{
                    //    if (this.BaseContext != null)
                    //        CuurentContext = this.BaseContext;
                    //    else
                    //    {
                    //        CuurentContext = this.CreatePackageContext("Agrin.Android", PackageContextFlags.IgnoreSecurity);
                    //    }
                    //}

                    if (CuurentContext == null)
                        InitializeApplication.GoException("CuurentContext == null");

                    //if (InitializeApplication.Inited)
                    //{
                    //    if (MustRunApp)
                    //        RunApplication();
                    //    MustInitializeUI = true;
                    //    return;
                    //}

                    RunApplication();
                    Agrin.Download.Data.DeSerializeData.ChangeState(Download.Data.LoadingStateEnum.InitializeUI);

                    if (BrowseActivity.IsBrowse)
                    {
                        if (!((Activity)BrowseActivity.This).IsFinishing)
                            BrowseActivity.This.RunOnUI(() =>
                            {
                                BrowseActivity.This.RunDialog();
                            });
                    }
                    else if (NotificationClickActivity.IsBrowse)
                    {
                        if (!((Activity)BrowseActivity.This).IsFinishing)
                            NotificationClickActivity.This.RunOnUI(() =>
                            {
                                NotificationClickActivity.This.Initialize();
                            });
                    }
                    //else if (MainActivity.This != null && MainActivity.This is MainActivity)
                    //    ((MainActivity)MainActivity.This).InitializeMain();
                    MustInitializeUI = true;
                }
                catch (Exception e)
                {
                    InitializeApplication.GoException(e, "OnStartCommand");
                    //Agrin.Log.AutoLogger.LogError(e, "Starting Service", true);
                }
            });

            task.Start();
        }

        //IBinder binder2 = null;
        //List<Stock> stocks;
        public const string StocksUpdatedAction = "StocksUpdated";

        public override IBinder OnBind(Intent intent)
        {
            //binder2 = new StockServiceBinder(this);
            return null;
        }

        //public List<Stock> GetStocks()
        //{
        //    return stocks;
        //}

        List<Stock> UpdateStocks(List<string> symbols)
        {
            List<Stock> results = null;
            return results;
        }

        System.Globalization.Calendar cal = new System.Globalization.PersianCalendar();
        string DateTimeToText(DateTime dt)
        {
            var now = DateTime.Now;
            if (dt.Year == now.Year && dt.Month == now.Month && dt.Day == now.Day)
            {
                return dt.Hour + ":" + dt.Minute;
            }
            else
            {
                return cal.GetYear(dt).ToString().Substring(2, 2) + "/" + cal.GetMonth(dt) + "/" + cal.GetDayOfMonth(dt) + " " + dt.Hour + ":" + dt.Minute;
            }
        }

        public static void StopServiceIfNotNeed()
        {
            try
            {
                if (!This.CheckAppForeground())
                {
                    if (MainActivity.IsDestroy && BrowseActivity.IsDestroy && NotificationClickActivity.IsDestroy)
                    {
                        Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
                        This.StopService(new Intent(serviceName));
                    }
                }
            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "AgrinService StopServiceIfNotNeed");
            }
        }

        DateTime lastMinDateTieSet = DateTime.MaxValue;
        public bool CheckAppForeground()
        {
            try
            {
                InitializeApplication.CleanUpManualStop(this);
                if (Agrin.Download.Manager.ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Count > 0 || IsClipboardOn || (Agrin.Download.Data.Settings.ApplicationSetting.Current.IsTurnOnScreenWhenQueueIsActivated && CanScreeenOnWhenIsInQueue()))// || Agrin.Helpers.InitializeApplication.ManualNotifyStop.Count > 0)
                {
                    ApplicationIsDownloading();
                    return true;
                }
                else
                {
                    return SetAlarmWithClosedTaskTime();
                }
            }
            catch (Exception e)
            {
                InitializeApplication.GoException(e, "CheckAppForeground");
            }
            return true;
        }

        /// <summary>
        /// روشن شدن گوشی در زمان نزدیک یکی از تسک ها که نزدیک ترین زمان برای ان ست شده است
        /// </summary>
        public bool SetAlarmWithClosedTaskTime()
        {
            try
            {
                bool finded = false;
                DateTime minDateTime = DateTime.MaxValue;
                foreach (var task in Agrin.Download.Manager.ApplicationTaskManager.Current.TaskInfoes.ToArray())
                {
                    if (task.State == Download.Web.TaskState.WaitingForWork || task.State == Download.Web.TaskState.Started || task.State == Download.Web.TaskState.Working)
                    {
                        if (task.DateTimes.Count > 0)
                        {
                            var dt = task.DateTimes.Min();
                            if (dt < minDateTime)
                                minDateTime = dt;
                            finded = true;
                        }
                    }
                }
                if (finded)
                {
                    TimeSpan time = (minDateTime - DateTime.Now).Add(new TimeSpan(0, -1, 0));
                    if (time < new TimeSpan(0, 1, 30))
                    {
                        ApplicationIsDownloading();
                        return true;
                    }

                    if (lastMinDateTieSet == minDateTime)
                        return false;
                    lastMinDateTieSet = minDateTime;
                    AlarmCancel();
                    AlarmSetNow(time);
                    SetCreenOff();
                }
                else
                {
                    lastMinDateTieSet = DateTime.MaxValue;
                    AlarmCancel();
                    ApplicationStopedDownloading();
                    return false;
                }
            }
            catch (Exception e)
            {
                InitializeApplication.GoException(e, "SetAlarmWithClosedTaskTime");
            }
            return false;
        }

        public void HoldScreenOn()
        {
            if (wakeLock == null && Agrin.Download.Data.Settings.ApplicationSetting.Current.IsTurnOnScreenWhenDownloading)
            {
                PowerManager mgr = (PowerManager)this.GetSystemService(Context.PowerService);
                wakeLock = mgr.NewWakeLock(WakeLockFlags.ScreenDim, "AgrinDownloadManager");
            }

            if (wakeLock != null && !wakeLock.IsHeld)
                wakeLock.Acquire();

            if (wifiLock == null)
            {
                WifiManager wifiManager = (WifiManager)this.GetSystemService(Context.WifiService);
                wifiLock = wifiManager.CreateWifiLock(WifiMode.Full, "AgrinDownloadManager");
            }

            if (!wifiLock.IsHeld)
                wifiLock.Acquire();
        }

        public void SetCreenOff()
        {
            if (wakeLock != null && wakeLock.IsHeld)
                wakeLock.Release();
            if (wifiLock != null && wifiLock.IsHeld)
                wifiLock.Release();
        }

        public static bool IsClipboardOn { get; set; }

        Android.OS.PowerManager.WakeLock wakeLock = null;
        WifiManager.WifiLock wifiLock = null;
        bool isStarted = false;
        string lastcontent = "";
        public void ApplicationIsDownloading()
        {
            HoldScreenOn();
            Java.Lang.ICharSequence title = null;
            Java.Lang.ICharSequence content = null;
            if (IsClipboardOn)
            {
                title = new Java.Lang.String("مدیریت کلیپ برد روشن");
                content = new Java.Lang.String("هنگامی که لینکی را کپی کنید آگرین بالا می آید.");
            }
            else if (Agrin.Download.Data.Settings.ApplicationSetting.Current.IsTurnOnScreenWhenQueueIsActivated && CanScreeenOnWhenIsInQueue())
            {
                title = new Java.Lang.String("در صف دانلود");
                content = new Java.Lang.String("روشن نگه داشتن صفحه هنگام استفاده از صف فعال است.");
            }
            else if (Agrin.Download.Manager.ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Count > 0 || Agrin.Helpers.InitializeApplication.ManualNotifyStop.Count > 0)
            {
                title = new Java.Lang.String(ViewsUtility.FindTextLanguage(this, "AgrinDownloadManagerWorkingForDownload_Language"));
                string text = ViewsUtility.FindTextLanguage(this, "DownloadingCount_Language") + (Agrin.Download.Manager.ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Count + Agrin.Helpers.InitializeApplication.ManualNotifyStop.Count);
                content = new Java.Lang.String(text);
                if (isStarted && lastcontent == text)
                    return;
                lastcontent = text;
            }
            else
            {
                title = new Java.Lang.String(ViewsUtility.FindTextLanguage(this, "AgrinDownloadManagerWaitingForDownload_Language"));
                string times = "";

                foreach (var task in Agrin.Download.Manager.ApplicationTaskManager.Current.TaskInfoes.ToArray())
                {
                    if (task.State == Download.Web.TaskState.WaitingForWork)
                    {
                        if (task.DateTimes.Count > 0)
                            times += ViewsUtility.TimeSpanToShortText(task.DateTimes.First() - DateTime.Now) + "-";
                        //var runner = Agrin.Download.Manager.ApplicationTaskManager.Current.GetTaskRunnerOfTaskInfo(task);
                        //if (runner != null)
                        //{
                        //    times += "(" + ViewsUtility.TimeSpanToShortText(runner.EndWaitingDateTime - DateTime.Now) + ")";
                        //}
                        //else
                        //{
                        //    times += "null";
                        //}
                        //times += DateTimeToText(task.DateTimes.First()) + "-";
                    }
                }
                times = times.Trim(new char[] { '-' });
                content = new Java.Lang.String(times);
                if (isStarted && lastcontent == times)
                    return;
                lastcontent = times;
            }
            isStarted = true;

            Notification notification = null;
            if ((int)Build.VERSION.SdkInt >= 26)
            {
                var channelId = CreateNotificationChannel("download_service_Agrin2", "Agrin Download Notification");
                notification = new Notification.Builder(this, channelId).Build();
            }
            else if ((int)Android.OS.Build.VERSION.SdkInt < 11)
            {
                notification = new Notification(Resource.Drawable.smallIcon, ViewsUtility.FindTextLanguage(this, "AgrinDownloadManager_Language"), DateTime.Now.Ticks);
                Intent notificationIntent = new Intent(this, typeof(MainActivity));
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
                //notification.LargeIcon = GetLargeIconBySize();
                notification.SetLatestEventInfo(this, title, content, pendingIntent);
            }
            else
            {
                var builder = new Notification.Builder(this).SetContentTitle(title).SetContentText(content).SetSmallIcon(Resource.Drawable.smallIcon);
                try
                {
                    builder = builder.SetLargeIcon(ViewsUtility.GetLargeIconBySize(this));
                }
                catch
                {
                }
                if ((int)Build.VERSION.SdkInt >= 16)
                {
                    notification = builder.Build();
                }
                else
                {
                    notification = builder.Notification;
                }
                // = builder.Build(); // available from API level 11 and onwards
            }
            StartForeground(1, notification);
        }

        private string CreateNotificationChannel(string channelId, string channelName)
        {
            NotificationChannel chan = new NotificationChannel(channelId, channelName, NotificationManager.ImportanceNone);
            chan.LockscreenVisibility = NotificationVisibility.Private;
            NotificationManager service = GetSystemService(Context.NotificationService) as NotificationManager;
            service.CreateNotificationChannel(chan);
            return channelId;
        }

        public bool CanScreeenOnWhenIsInQueue()
        {
            foreach (var task in Agrin.Download.Manager.ApplicationTaskManager.Current.TaskInfoes.ToArray())
            {
                if (task.State == Download.Web.TaskState.WaitingForWork)
                {
                    return true;
                }
            }
            return false;
        }

        public void ApplicationStopedDownloading()
        {
            if (!isStarted)
                return;
            isStarted = false;
            try
            {
                InitializeApplication.RemoteNotify();
            }
            catch (Exception e)
            {
                InitializeApplication.GoException(e);
            }

            Agrin.Download.Data.SerializeData.CloseApplicationWaitForSavingAllComplete();
            SetCreenOff();
            StopForeground(true);
        }

        public void RunApplication()
        {
            try
            {
                if (!InitializeApplication.Inited || MustRunApp)
                {
                    MustRunApp = false;

                    if (InitializeApplication.Run(CuurentContext))
                    {

                    }
                }
                else
                {
                    if (Agrin.Download.Data.ApplicationServiceData.Current != null && Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks != null)
                    {
                        foreach (var id in Agrin.Download.Data.ApplicationServiceData.Current.IsPlayLinks.ToArray())
                        {
                            var link = Agrin.Download.Manager.ApplicationLinkInfoManager.Current.FindLinkInfoByID(id);
                            if (link != null)
                            {
                                if (link.DownloadingProperty.State == Agrin.Download.Web.ConnectionState.Complete)
                                {
                                    //InitializeApplication.StartNotify(CuurentContext);
                                    InitializeApplication.CheckAddFrameSoftLink(link);
                                    Agrin.Download.Data.ApplicationServiceData.RemoveItem(id);
                                }
                                else
                                {

                                    Agrin.Download.Manager.ApplicationLinkInfoManager.Current.PlayLinkInfo(link);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //var nMgr = (NotificationManager)CuurentContext.GetSystemService(Context.NotificationService);
                //var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(CuurentContext, "AgrinDownloadManager_Language"));
                //var mainActivity = new Intent(CuurentContext, typeof(MainActivity));
                //var pendingIntent = PendingIntent.GetActivity(CuurentContext, 0, mainActivity, 0);
                //notification.SetLatestEventInfo(CuurentContext, "exception", e.Message, pendingIntent);
                //nMgr.Notify(0, notification);
                //if (e is Java.Lang.Exception)
                //{
                //    Agrin.Log.AutoLogger.LogText("ApplicationServiceData: " + ((Java.Lang.Exception)e).Message);
                //}
                //Agrin.Log.AutoLogger.LogError(e, "ApplicationServiceData: ", true);
                InitializeApplication.GoException(e, "RunApplication");
            }
        }

        //static bool isBound = false;
        //static StockServiceBinder binder;
        //static StockServiceConnection stockServiceConnection;
        //static StockReceiver stockReceiver;
        //static Intent stockServiceIntent;

        protected override void OnHandleIntent(Intent intent)
        {

        }

        static void ScheduleStockUpdates()
        {
            //Context context = CuurentContext;
            //if (context == null)
            //    context = StockNotificationReceiver.CuurentContext;
            //if (context == null)
            //{
            //    //var nMgr = (NotificationManager)context.GetSystemService(Context.NotificationService);
            //    //var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(context, "AgrinDownloadManager_Language"));
            //    //var mainActivity = new Intent(context, typeof(MainActivity));
            //    //var pendingIntent = PendingIntent.GetActivity(context, 0, mainActivity, 0);
            //    //notification.SetLatestEventInfo(context, "No Context Found", InitializeApplication.Inited.ToString(), pendingIntent);
            //    //notification.Flags = NotificationFlags.AutoCancel;
            //    //nMgr.Notify(1368, notification);
            //    return;
            //}
            //if (!IsAlarmSet())
            //{
            //    var alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);

            //    var pendingServiceIntent = PendingIntent.GetService(context, 0, stockServiceIntent, PendingIntentFlags.UpdateCurrent);
            //    alarm.SetRepeating(AlarmType.Rtc, 0, 20000, pendingServiceIntent);

            //    //alarm.SetRepeating (AlarmType.Rtc, 0, AlarmManager.IntervalHalfHour, pendingServiceIntent);
            //}
            //else
            //{

            //}
        }

        //static bool IsAlarmSet()
        //{
        //    //Context context = CuurentContext;
        //    //if (context == null)
        //    //    context = StockNotificationReceiver.CuurentContext;
        //    //if (context == null)
        //    //    return false;
        //    //return PendingIntent.GetBroadcast(context, 0, stockServiceIntent, PendingIntentFlags.NoCreate) != null;
        //}


        //class StockReceiver : BroadcastReceiver
        //{
        //    public override void OnReceive(Context context, Android.Content.Intent intent)
        //    {
        //        CuurentContext = context;
        //        InvokeAbortBroadcast();
        //    }
        //}

        //class StockServiceConnection : Java.Lang.Object, IServiceConnection
        //{
        //    Activity activity;

        //    public StockServiceConnection(Activity activity)
        //    {
        //        this.activity = activity;
        //    }

        //    public void OnServiceConnected(ComponentName name, IBinder service)
        //    {
        //        var stockServiceBinder = service as StockServiceBinder;
        //        if (stockServiceBinder != null)
        //        {
        //            var binder = (StockServiceBinder)service;
        //            AgrinService.binder = binder;
        //            AgrinService.isBound = true;
        //        }
        //    }

        //    public void OnServiceDisconnected(ComponentName name)
        //    {
        //        AgrinService.isBound = false;
        //    }
        //}

    }

    public class Stock
    {
        public Stock()
        {
        }

        public string Symbol { get; set; }

        public float LastPrice { get; set; }

        public override string ToString()
        {
            return string.Format("[Stock: Symbol={0}, LastPrice={1}]", Symbol, LastPrice);
        }
    }

    [BroadcastReceiver]
    [IntentFilter(new string[] { AgrinService.StocksUpdatedAction }, Priority = (int)IntentFilterPriority.LowPriority)]
    public class Alarm : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (string.IsNullOrEmpty(Agrin.Log.AutoLogger.ApplicationDirectory))
                    Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");
                AutoLogger.LogTextTest($"Alarm OnReceive {AgrinService.This == null} {!InitializeApplication.Inited}");
                PowerManager powerManager = (PowerManager)context.GetSystemService(Context.PowerService);
                var WakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenBright | WakeLockFlags.AcquireCausesWakeup, "AgrinBroadcastReceiver");
                WakeLock.Acquire();
                if (AgrinService.This == null || !InitializeApplication.Inited)
                {
                    var i = new Intent(context, typeof(MainActivity));
                    i.SetFlags(ActivityFlags.NewTask);
                    context.StartActivity(i);
                }
                else
                    AgrinService.This.RestartAllRunners();
                //WakeLock.Release();
                //PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
                //PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.Partial, "");
                //wl.Acquire();

                //// Put here YOUR code.
                ////Toast.MakeText(context, "Alarm !!!!!!!!!!", Toast.LENGTH_LONG).show(); // For example

            }
            catch (Exception ex)
            {
                InitializeApplication.GoException(ex, "BroadcastReceiver OnReceive");
            }
        }

        public void SetAlarm(Context context, TimeSpan time)
        {
            if (string.IsNullOrEmpty(Agrin.Log.AutoLogger.ApplicationDirectory))
                Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");
            AutoLogger.LogTextTest($"Alarm SetAlarm {time.ToString()}");
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            Intent i = new Intent(context, typeof(Alarm));
            PendingIntent pi = PendingIntent.GetBroadcast(context, 0, i, 0);
            var cm = CurrentTimeMillis();
            var mils = time.Ticks / TimeSpan.TicksPerMillisecond;
            AutoLogger.LogTextTest($"Alarm SetAlarm2 {cm + mils}");
            am.Set(AlarmType.RtcWakeup, cm + mils, pi); // Millisec * Second * Minute
            // am.SetRepeating(AlarmType.RtcWakeup, cm, 1000 * 20 * 1, pi); // Millisec * Second * Minute
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        public void CancelAlarm(Context context)
        {
            if (string.IsNullOrEmpty(Agrin.Log.AutoLogger.ApplicationDirectory))
                Agrin.Log.AutoLogger.ApplicationDirectory = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.RootDirectory.Path).Path, "AgrinData");
            AutoLogger.LogTextTest($"Alarm CancelAlarm");
            Intent intent = new Intent(context, typeof(Alarm));
            PendingIntent sender = PendingIntent.GetBroadcast(context, 0, intent, 0);
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(sender);
        }
    }

    //[BroadcastReceiver]
    //[IntentFilter(new string[] { AgrinService.StocksUpdatedAction }, Priority = (int)IntentFilterPriority.LowPriority)]
    //public class StockNotificationReceiver : BroadcastReceiver
    //{
    //    public StockNotificationReceiver()
    //    {

    //    }

    //    public static Context CuurentContext = null;

    //    public override void OnReceive(Context context, Intent intent)
    //    {
    //        CuurentContext = context;
    //    }
    //}

    //public class StockServiceBinder : Binder
    //{
    //    AgrinService service;

    //    public StockServiceBinder(AgrinService service)
    //    {
    //        this.service = service;
    //    }

    //    public AgrinService GetStockService()
    //    {
    //        return service;
    //    }
    //}
}