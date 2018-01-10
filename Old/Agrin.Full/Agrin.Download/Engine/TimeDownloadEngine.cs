using Agrin.Data.Code;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agrin.Download.Engine
{
    public static class TimeDownloadEngine
    {
        public static Action<double, double, double, bool, bool, bool> TotalItemsChanged { get; set; }
        public static void StartTransferRate()
        {
            AsyncActions.Action(() =>
            {
                ApplicationTimer();
                Dictionary<LinkInfo, double> oldSizes = new Dictionary<LinkInfo, double>();
                while (true)
                {
                    try
                    {
                        double totalSpeed = 0;
                        double totalMaximumProgress = 0, totalProgressValue = 0;
                        var downdingLinks = ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToList();
                        bool isConnecting = downdingLinks.Count > 0;
                        bool isError = false;
                        foreach (var item in downdingLinks)
                        {
                            if (item.DownloadingProperty.State == ConnectionState.Downloading)
                            {
                                isConnecting = false;
                                long downloadedSize = item.DownloadingProperty.DownloadedSize;
                                double size = item.DownloadingProperty.Size;
                                if (!oldSizes.ContainsKey(item))
                                {
                                    oldSizes.Add(item, downloadedSize);
                                }
                                else if (size > 0)
                                {
                                    totalMaximumProgress += size;
                                    totalProgressValue += downloadedSize;
                                    var listSpeedByteDownloaded = item.DownloadingProperty.ListSpeedByteDownloaded.ToList();
                                    if (listSpeedByteDownloaded.Count > 0 && listSpeedByteDownloaded.Last() == 0 && downloadedSize == oldSizes[item])
                                        continue;
                                    item.DownloadingProperty.SpeedByteDownloaded = downloadedSize - oldSizes[item];
                                    oldSizes[item] = downloadedSize;
                                    totalSpeed += item.DownloadingProperty.SpeedByteDownloaded;
                                    if (listSpeedByteDownloaded.Count > 0)
                                    {
                                        int skipCount = listSpeedByteDownloaded.Count - 30;
                                        if (skipCount <= 0)
                                            skipCount = 0;
                                        double average = listSpeedByteDownloaded.Skip(skipCount).Average();
                                        double remainSize = size - downloadedSize;
                                        average = remainSize / average;
                                        item.DownloadingProperty.TimeRemaining = TimeSpan.FromSeconds(average);
                                    }
                                }
                            }
                            else if (item.DownloadingProperty.State == ConnectionState.Connecting)
                            {
                                totalMaximumProgress += item.DownloadingProperty.Size;
                                totalProgressValue += item.DownloadingProperty.DownloadedSize;
                            }
                            else if (item.IsError)
                            {
                                isConnecting = false;
                                isError = true;
                                totalMaximumProgress += item.DownloadingProperty.Size;
                                totalProgressValue += item.DownloadingProperty.DownloadedSize;
                            }
                            else if (item.IsComplete)
                            {
                                totalMaximumProgress += item.DownloadingProperty.Size;
                                totalProgressValue += item.DownloadingProperty.DownloadedSize;
                                ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Remove(item);
                            }
                        }
                        if (TotalItemsChanged != null)
                        {
                            TotalItemsChanged(totalSpeed, totalMaximumProgress, totalProgressValue, isConnecting, isError, downdingLinks.Count == 0);
                        }
                    }
                    catch
                    {

                    }
                    Thread.Sleep(1000);
                }
            });

        }
        public static void ApplicationTimer()
        {
            long secound = new TimeSpan(0, 0, 15).Ticks;
            long connectSecound = new TimeSpan(0, 1, 0).Ticks;
            AsyncActions.Action(() =>
            {
                while (true)
                {
                    try
                    {
                        List<LinkInfo> items = Manager.ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToList();
                        foreach (var item in items)
                        {
                            if (item.DownloadingProperty.State == ConnectionState.Complete || item.DownloadingProperty.State == ConnectionState.CopyingFile)
                                continue;
                            if ((item.DownloadingProperty.State == ConnectionState.Downloading || item.DownloadingProperty.State == ConnectionState.Connecting))
                            {
                                lock (item.lockReconnect)
                                {
                                    bool checkConnection = false;
                                    bool completed = true;
                                    foreach (var connectionInfo in item.Connections.ToList())
                                    {
                                        if (connectionInfo.State == ConnectionState.Downloading || connectionInfo.State == ConnectionState.Connecting || connectionInfo.State == ConnectionState.CreatingRequest)
                                        {
                                            if (connectionInfo.ReConnectTimer != null)
                                            {
                                                long watch = connectionInfo.ReConnectTimer.Elapsed.Ticks;
                                                if (watch > secound && (connectionInfo.State == ConnectionState.Connecting || connectionInfo.State == ConnectionState.CreatingRequest || (item.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes && connectionInfo.State == ConnectionState.Downloading)))
                                                {
                                                    lock (item.lockValue)
                                                        connectionInfo.ReConnect();
                                                }
                                            }
                                        }
                                        else if (!checkConnection && connectionInfo.State == ConnectionState.Error)
                                            checkConnection = true;
                                        if (connectionInfo.State != ConnectionState.Complete)
                                            completed = false;
                                    }

                                    if (completed && item.DownloadingProperty.State != ConnectionState.Complete && item.DownloadingProperty.State != ConnectionState.CopyingFile)
                                        Agrin.Helper.ComponentModel.AsyncActions.Action(() =>
                                        {
                                            item.CheckForCompleteLink(false);
                                        });
                                    if (checkConnection && (item.DownloadingProperty.State == ConnectionState.Downloading || item.DownloadingProperty.State == ConnectionState.Connecting))
                                        item.ConncetionsCheck(false);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Agrin.Log.AutoLogger.LogError(e, "Timing...");
                    }
                    Thread.Sleep(15000);
                }
            });
            AmarGiri();
            Update();
            GetUserMessage();
        }

#if !DEBUG
        static string GetedAmarAddress = "";
#endif
        public static void AmarGiri()
        {
#if !DEBUG
            Task thread = new Task(() =>
             {
                 while (true)
                 {
                     try
                     {
                         HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://framesoft.ir/amargiri/insert");
                         request.Headers["ApplicationNameAttrib"] = ApplicationSetting.Current.ApplicationOSSetting.Application;
                         request.Headers["ApplicationVersionAttrib"] = ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion;
                         request.Headers["OSNameAttrib"] = ApplicationSetting.Current.ApplicationOSSetting.OSName;
                         request.Headers["OSVersionAttrib"] = ApplicationSetting.Current.ApplicationOSSetting.OSVersion;
                         request.Headers["ApplicationGuid"] = ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid;
                         request.Headers["SystemIMEI"] = ApplicationSetting.Current.ApplicationOSSetting.SystemIMEI;
                         request.GetResponse().Close();

                         //Agrin.Helper.ComponentModel.ApplicationHelperMono.CheckInternetAndResetModem();
                         //#if(MobileApp)
                         //                        string html = GetedAmarAddress;
                         //                        if (string.IsNullOrEmpty(html))
                         //                            html = new System.Net.WebClient().DownloadString("http://framesoft.ir/page/agrinmobileappamargiri");
                         //                        if (html.Contains("</CSharpCode>"))
                         //                            GetedAmarAddress = Agrin.Data.Code.CodeLuncher.GetCode(html).Trim();

                         //                        if (!string.IsNullOrEmpty(GetedAmarAddress))
                         //                            new System.Net.WebClient().DownloadString(GetedAmarAddress);

                         //                        //CodeLuncher.LunchCSCode("http://framesoft.ir/page/AgrinMobileAppAmarGiri", "AmarGiri", "Main");
                         //#elif XamarinApp
                         //                        CodeLuncher.LunchCSCode("http://framesoft.ir/page/AgrinMonoAmarGiri", "AmarGiri", "Main");
                         //#else
                         //                        CodeLuncher.LunchCSCode("http://framesoft.ir/page/agrinamargiri", "AmarGiri", "Main");
                         //#endif
                     }
                     catch
                     {

                     }

                     Thread.Sleep(new TimeSpan(0, 20, 0));
                 }
             });
            thread.Start();
#endif

        }

        public static Action<UpdateInformation> UpdatedAction { get; set; }
        public static Action<UpdateInformation> GetLastMessageAction { get; set; }

        public static Action<MessageInformation> GetUserMessageAction { get; set; }

        public static void Update()
        {
            AsyncActions.Action(() =>
            {
                while (true)
                {
                    try
                    {
                        CheckLastVersion();

                        Thread.Sleep(new TimeSpan(5, 0, 0));
                    }
                    catch
                    {
                        Thread.Sleep(1000 * 60);
                    }

                }
            });
        }

        public static void CheckLastVersion(bool userManual = false)
        {
            if (ApplicationSetting.Current.NoCheckLastVersion && ApplicationSetting.Current.NoGetApplicationMessage && !userManual)
                return;
#if (MobileApp)
            string uri = "http://framesoft.ir/GetUpdate/Android";

#elif XamarinApp
            string uri = "http://framesoft.ir/GetUpdate/XamarinMono";
                        //CodeLuncher.LunchCSCode("http://framesoft.ir/page/AgrinMonoUpdate", "Update", "Main");
#else
            string uri = "http://framesoft.ir/GetUpdate/Windows";
#endif
            if (IsNewVersionOfAndroid)
            {
                var msg = Framesoft.Helper.FeedBackHelper.GetLastPublicMessages(ApplicationNoticeManager.Current.GetMaximumPublicMessageDateTime());
                if (msg == null)
                    return;
                foreach (var item in msg)
                {
                    ApplicationNoticeManager.Current.AddNotice(new NoticeInfo() { Data = item, Mode = NoticeMode.PublicMessage });
                }
            }
            else
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.Headers.Add("Language", ApplicationSetting.Current.ApplicationLanguage);
                    client.Headers.Add("AppVersion", ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion);
                    client.Headers.Add("Guid", ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid);
                    client.Headers.Add("SystemIMEI", ApplicationSetting.Current.ApplicationOSSetting.SystemIMEI);

                    string jsonString = client.DownloadString(uri);
                    UpdateInformation update = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateInformation>(jsonString);
                    if (!ApplicationSetting.Current.NoCheckLastVersion || userManual)
                    {
                        if (update.GetVersion > Version.Parse(ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion))
                        {
                            if (string.IsNullOrEmpty(update.DownloadUri))
                                return;
                            if (UpdatedAction != null)
                                UpdatedAction(update);
                        }
                    }
                    if (!ApplicationSetting.Current.NoGetApplicationMessage || userManual)
                    {
                        if (update.LastApplicationMessageID > ApplicationSetting.Current.LastApplicationMessageID)
                        {
                            //her code for new Application Message
                            if (string.IsNullOrEmpty(update.Message))
                                return;
                            GetLastMessageAction(update);
                        }
                    }
                }
            }

        }

        public static string GetLastUserMessage()
        {
            string uri = "http://framesoft.ir/Reporter/GetUserMessage";
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                client.Headers.Add("Guid", ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid);
                client.Headers.Add("LastUserMessageID", (ApplicationSetting.Current.LastUserMessageID - 1).ToString());
                client.Headers.Add("AppVersion", ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion);
                client.Headers.Add("SystemIMEI", ApplicationSetting.Current.ApplicationOSSetting.SystemIMEI);
                var msg = client.DownloadString(uri);
                if (msg != "Not Found")
                {
                    string jsonString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(msg));
                    MessageInformation data = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageInformation>(jsonString);
                    return data.Message;
                }
                else
                    return "Not Found";
            }
        }

        public static void RefrshUserMessage()
        {
            var msg = Framesoft.Helper.FeedBackHelper.GetUserMessageReplays(ApplicationNoticeManager.Current.GetMaximumUserMessageDateTime(), new Guid(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid));
            if (msg != null)
            {
                foreach (var item in msg)
                {
                    ApplicationNoticeManager.Current.AddNotice(new NoticeInfo() { Data = item, Mode = NoticeMode.UserMessage });
                }
            }
        }

        public static bool IsNewVersionOfAndroid = false;
        public static void GetUserMessage()
        {
            AsyncActions.Action(() =>
            {
                while (true)
                {
                    try
                    {
                        if (IsNewVersionOfAndroid)
                        {
                            RefrshUserMessage();
                        }
                        else
                        {
                            string uri = "http://framesoft.ir/Reporter/GetUserMessage";
                            using (System.Net.WebClient client = new System.Net.WebClient())
                            {
                                client.Headers.Add("Guid", ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid);
                                client.Headers.Add("LastUserMessageID", ApplicationSetting.Current.LastUserMessageID.ToString());
                                client.Headers.Add("AppVersion", ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion);
                                client.Headers.Add("SystemIMEI", ApplicationSetting.Current.ApplicationOSSetting.SystemIMEI);
                                var msg = client.DownloadString(uri);
                                if (msg != "Not Found")
                                {
                                    string jsonString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(msg));
                                    MessageInformation data = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageInformation>(jsonString);
                                    if (GetUserMessageAction != null)
                                        GetUserMessageAction(data);
                                }
                            }
                        }

                        Thread.Sleep(new TimeSpan(1, 0, 0));
                    }
                    catch
                    {
                        Thread.Sleep(1000 * 60);
                    }
                }
            });
        }
    }

    public class UpdateInformation
    {
        public Version GetVersion
        {
            get
            {
                try
                {
                    return Version.Parse(LastVersion);
                }
                catch
                {
                    return null;
                }
            }
        }

        public string DownloadUri { get; set; }
        public string LastVersion { get; set; }
        public int LastApplicationMessageID { get; set; }
        public string Message { get; set; }
    }

    public class MessageInformation
    {
        public string GUID { get; set; }
        public string Message { get; set; }
        public int LastUserMessageID { get; set; }
    }
}
