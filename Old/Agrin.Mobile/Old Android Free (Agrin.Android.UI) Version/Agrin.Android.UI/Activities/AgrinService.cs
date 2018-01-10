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
using System.Threading;

namespace Agrin.MonoAndroid.UI.Activities
{
    [Service]
    [IntentFilter(new String[] { "agrin.monoandroid.ui.agrinservice" })]
    public class AgrinService : IntentService
    {
        public static AgrinService This;

        public override void OnDestroy()
        {
            StartTimeService();
            base.OnDestroy();
        }

        public void StartTimeService()
        {
            var context = CuurentContext;
            var intentFilter = new IntentFilter(AgrinService.StocksUpdatedAction) { Priority = (int)IntentFilterPriority.HighPriority };
            context.RegisterReceiver(stockReceiver, intentFilter);
            stockServiceConnection = new StockServiceConnection(MainActivity.This);
            context.BindService(stockServiceIntent, stockServiceConnection, Bind.AutoCreate);
            ScheduleStockUpdates();
        }

        public override bool StopService(Intent name)
        {
            StartTimeService();
            return base.StopService(name);
        }

        public override void OnLowMemory()
        {
            StartTimeService();
            base.OnLowMemory();
        }

        public static Context CuurentContext = null;
        public static bool MustRunApp { get; set; }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                This = this;
                stockServiceIntent = new Intent("agrin.monoandroid.ui.agrinservice");
                stockReceiver = new StockReceiver();
                if (this.ApplicationContext != null)
                    CuurentContext = this.ApplicationContext;
                else
                {
                    if (this.BaseContext != null)
                        CuurentContext = this.BaseContext;
                    else
                    {
                        CuurentContext = this.CreatePackageContext("Agrin.MonoAndroid.UI", PackageContextFlags.IgnoreSecurity);
                    }
                }


                if (InitializeApplication.Inited)
                {
                    if (MustRunApp)
                    {
                        RunApplication();
                        return StartCommandResult.Sticky;
                    }
                    else
                        return StartCommandResult.Sticky;
                }

                RunApplication();
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Starting Service", true);
            }
            return StartCommandResult.Sticky;
        }

        IBinder binder2 = null;
        List<Stock> stocks;
        public const string StocksUpdatedAction = "StocksUpdated";

        public override IBinder OnBind(Intent intent)
        {
            binder2 = new StockServiceBinder(this);
            return binder2;
        }

        public List<Stock> GetStocks()
        {
            return stocks;
        }

        List<Stock> UpdateStocks(List<string> symbols)
        {
            List<Stock> results = null;

            //string[] array = symbols.ToArray();
            //string symbolsString = String.Join("%22%2C%22", array);

            //string uri = String.Format(
            //    "http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%22{0}%22)%0A%09%09&diagnostics=false&format=json&env=http%3A%2F%2Fdatatables.org%2Falltables.env",
            //    symbolsString);

            //var httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(uri));

            //try
            //{
            //    using (HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse())
            //    {

            //        var response = httpRes.GetResponseStream();
            //        var json = (JsonObject)JsonObject.Load(response);

            //        results = (from result in (JsonArray)json["query"]["results"]["quote"]
            //                   let jResult = result as JsonObject
            //                   select new Stock { Symbol = jResult["Symbol"], LastPrice = (float)jResult["LastTradePriceOnly"] }).ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.Debug("StockService", "error connecting to web service: " + ex.Message);
            //}

            return results;
        }

        bool started = false;
        public void ApplicationIsDownloading()
        {
            if (started)
                return;
            started = true;
            Notification notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(this, "AgrinDownloadManager_Language"), DateTime.Now.Ticks);
            Intent notificationIntent = new Intent(this, typeof(MainActivity));
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
            notification.SetLatestEventInfo(this, ViewUtility.FindTextLanguage(this, "AgrinDownloadManager_Language"),
                  ViewUtility.FindTextLanguage(this, "ApplicationIsDownloading_Language"), pendingIntent);
            StartForeground(56900, notification);
        }

        public void ApplicationStopedDownloading()
        {
            if (!started)
                return;
            started = false;
            StopForeground(true);
        }

        public void RunApplication()
        {
            try
            {
                //MainActivity.AcquireWakeLock();
                if (!InitializeApplication.Inited || MustRunApp)
                {
                    MustRunApp = false;

                    //var nMgr = (NotificationManager)CuurentContext.GetSystemService(Context.NotificationService);
                    //var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(CuurentContext, "AgrinDownloadManager_Language"));
                    //var mainActivity = new Intent(CuurentContext, typeof(MainActivity));
                    //var pendingIntent = PendingIntent.GetActivity(CuurentContext, 0, mainActivity, 0);
                    //notification.SetLatestEventInfo(CuurentContext, "Started New Service", InitializeApplication.Inited.ToString(), pendingIntent);
                    //nMgr.Notify(0, notification);
                    //var task = new System.Threading.Tasks.Task(() =>
                    //{
                    //    var aT = new System.Threading.Tasks.Task(() =>
                    //    {
                    //        while (true)
                    //        {
                    //            nMgr = (NotificationManager)CuurentContext.GetSystemService(Context.NotificationService);
                    //            notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(CuurentContext, "AgrinDownloadManager_Language"));
                    //            mainActivity = new Intent(CuurentContext, typeof(MainActivity));
                    //            pendingIntent = PendingIntent.GetActivity(CuurentContext, 0, mainActivity, 0);
                    //            notification.SetLatestEventInfo(CuurentContext, "success Inner", "Inner Thread", pendingIntent);
                    //            nMgr.Notify(12, notification);
                    //            Thread.Sleep(5000);
                    //        }
                    //    });
                    //    aT.Start();
                    //    while (true)
                    //    {
                    //        nMgr = (NotificationManager)CuurentContext.GetSystemService(Context.NotificationService);
                    //        notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(CuurentContext, "AgrinDownloadManager_Language"));
                    //        mainActivity = new Intent(CuurentContext, typeof(MainActivity));
                    //        pendingIntent = PendingIntent.GetActivity(CuurentContext, 0, mainActivity, 0);
                    //        notification.SetLatestEventInfo(CuurentContext, "success Main", InitializeApplication.Inited.ToString() + " Thread Main", pendingIntent);
                    //        nMgr.Notify(11, notification);
                    //        Thread.Sleep(10000);
                    //    }
                    //});
                    //task.Start();
                    //if (InitializeApplication.Run(this.ApplicationContext))
                    //{
                    //    //ServiceRunner.Create();
                    //    //ServiceRunner.Start();
                    //}
                    if (InitializeApplication.Run(this.ApplicationContext))
                    {
                        //if (InitializeApplication.IsSetLanguage)
                        //    ActivityManager.ToolbarActive(MainActivity.This);
                        //else
                        //{
                        //    ActivityManager.SelectLanguageActive(MainActivity.This, () =>
                        //    {
                        //        ActivityManager.ToolbarActive(MainActivity.This);
                        //    });
                        //}
                    }
                }
                else
                {
                    //if (InitializeApplication.IsSetLanguage)
                    //    ActivityManager.ToolbarActive(MainActivity.This);
                    //else
                    //{
                    //    ActivityManager.SelectLanguageActive(MainActivity.This, () =>
                    //    {
                    //        ActivityManager.ToolbarActive(MainActivity.This);
                    //    });
                    //}
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
                                    InitializeApplication.StartNotify(CuurentContext);
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
                //var nMgr = (NotificationManager)CuurentContext.GetSystemService(Context.NotificationService);
                //var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(CuurentContext, "AgrinDownloadManager_Language"));
                //var mainActivity = new Intent(CuurentContext, typeof(MainActivity));
                //var pendingIntent = PendingIntent.GetActivity(CuurentContext, 0, mainActivity, 0);
                //notification.SetLatestEventInfo(CuurentContext, "Runned App", "OK", pendingIntent);
                //nMgr.Notify(0, notification);
            }
            catch (Exception e)
            {
                //var nMgr = (NotificationManager)CuurentContext.GetSystemService(Context.NotificationService);
                //var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(CuurentContext, "AgrinDownloadManager_Language"));
                //var mainActivity = new Intent(CuurentContext, typeof(MainActivity));
                //var pendingIntent = PendingIntent.GetActivity(CuurentContext, 0, mainActivity, 0);
                //notification.SetLatestEventInfo(CuurentContext, "exception", e.Message, pendingIntent);
                //nMgr.Notify(0, notification);
                if (e is Java.Lang.Exception)
                {
                    Agrin.Log.AutoLogger.LogText("ApplicationServiceData: " + ((Java.Lang.Exception)e).Message);
                }
                Agrin.Log.AutoLogger.LogError(e, "ApplicationServiceData: ", true);
            }
        }

        static bool isBound = false;
        static StockServiceBinder binder;
        static StockServiceConnection stockServiceConnection;
        static StockReceiver stockReceiver;
        static Intent stockServiceIntent;

        protected override void OnHandleIntent(Intent intent)
        {

        }

        static void ScheduleStockUpdates()
        {
            Context context = CuurentContext;
            if (context == null)
                context = StockNotificationReceiver.CuurentContext;
            if (context == null)
            {
                //var nMgr = (NotificationManager)context.GetSystemService(Context.NotificationService);
                //var notification = new Notification(Resource.Drawable.smallIcon, ViewUtility.FindTextLanguage(context, "AgrinDownloadManager_Language"));
                //var mainActivity = new Intent(context, typeof(MainActivity));
                //var pendingIntent = PendingIntent.GetActivity(context, 0, mainActivity, 0);
                //notification.SetLatestEventInfo(context, "No Context Found", InitializeApplication.Inited.ToString(), pendingIntent);
                //notification.Flags = NotificationFlags.AutoCancel;
                //nMgr.Notify(1368, notification);
                return;
            }
            if (!IsAlarmSet())
            {
                var alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);

                var pendingServiceIntent = PendingIntent.GetService(context, 0, stockServiceIntent, PendingIntentFlags.UpdateCurrent);
                alarm.SetRepeating(AlarmType.Rtc, 0, 10000, pendingServiceIntent);

                //alarm.SetRepeating (AlarmType.Rtc, 0, AlarmManager.IntervalHalfHour, pendingServiceIntent);
            }
            else
            {

            }
        }

        static bool IsAlarmSet()
        {
            Context context = CuurentContext;
            if (context == null)
                context = StockNotificationReceiver.CuurentContext;
            if (context == null)
                return false;
            return PendingIntent.GetBroadcast(context, 0, stockServiceIntent, PendingIntentFlags.NoCreate) != null;
        }


        class StockReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Android.Content.Intent intent)
            {
                CuurentContext = context;
                InvokeAbortBroadcast();
            }
        }

        class StockServiceConnection : Java.Lang.Object, IServiceConnection
        {
            Activity activity;

            public StockServiceConnection(Activity activity)
            {
                this.activity = activity;
            }

            public void OnServiceConnected(ComponentName name, IBinder service)
            {
                var stockServiceBinder = service as StockServiceBinder;
                if (stockServiceBinder != null)
                {
                    var binder = (StockServiceBinder)service;
                    AgrinService.binder = binder;
                    AgrinService.isBound = true;
                }
            }

            public void OnServiceDisconnected(ComponentName name)
            {
                AgrinService.isBound = false;
            }
        }

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
    public class StockNotificationReceiver : BroadcastReceiver
    {
        public StockNotificationReceiver()
        {

        }

        public static Context CuurentContext = null;

        public override void OnReceive(Context context, Intent intent)
        {
            CuurentContext = context;
        }
    }

    public class StockServiceBinder : Binder
    {
        AgrinService service;

        public StockServiceBinder(AgrinService service)
        {
            this.service = service;
        }

        public AgrinService GetStockService()
        {
            return service;
        }
    }
}