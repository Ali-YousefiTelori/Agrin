using FrameSoft.Agrin.DataBase.Models;
using FramwsoftSignalGoServices;
using SignalGo.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameSoftWeb.Engine.Amar
{
    public static class FramesoftServiceProvider
    {
        static ClientProvider provider = new ClientProvider();
        static IFramesoftService service = null;

        static FramesoftServiceProvider()
        {
            provider = new ClientProvider();
            TryConnect();
            //var cb = provider.RegisterServerCallback<CallbackTest>();
        }

        static object lockobj = new object();
        static void TryConnect()
        {
            lock (lockobj)
            {
                try
                {
                    if (!provider.IsConnected)
                    {
                        provider.Connect("http://localhost:9191/FramesoftService");
                        service = provider.RegisterClientServiceInterface<IFramesoftService>();
                    }
                }
                catch (Exception ex)
                {
                    Agrin.Log.AutoLogger.LogError(ex, "TryConnect");
                }
            }
        }

        public static Exception AddErrorLog(Exception error)
        {
            TryConnect();
            return service.AddErrorLog(error).Data;
        }

        public static FrameSoft.AmarGiri.DataBase.Heper.AmarInfo GetFullAmarInfo()
        {
            TryConnect();
            return service.GetFullAmarInfo().Data;
        }

        public static FrameSoft.AmarGiri.DataBase.Heper.AmarInfo GetWindowsAmarInfo()
        {
            TryConnect();
            return service.GetWindowsAmarInfo().Data;
        }

        public static bool IsBlackListGUID(Guid guid)
        {
            TryConnect();
            return service.IsBlackListGUID(guid).Data;
        }

        public static int GetAndUpdateGuidIDByGuid(Guid guid)
        {
            TryConnect();
            return service.GetAndUpdateGuidIDByGuid(guid).Data;
        }

        public static bool SendToBlackList(int guidID)
        {
            TryConnect();
            return service.SendToBlackList(guidID).Data;
        }

        public static FrameSoft.AmarGiri.DataBase.Models.GuidDetailsTable GetGuidDetails(int guidID)
        {
            TryConnect();
            return service.GetGuidDetails(guidID).Data;
        }

        public static string GetApplicationVersionByGuidId(FrameSoft.AmarGiri.DataBase.Models.GuidDetailsTable detail)
        {
            TryConnect();
            return service.GetApplicationVersionByGuidId(detail).Data;
        }

        public static string GetApplicationNameByGuidId(FrameSoft.AmarGiri.DataBase.Models.GuidDetailsTable detail)
        {
            TryConnect();
            return service.GetApplicationNameByGuidId(detail).Data;
        }

        public static string GetOSVersionByGuidId(FrameSoft.AmarGiri.DataBase.Models.GuidDetailsTable detail)
        {
            TryConnect();
            return service.GetOSVersionByGuidId(detail).Data;
        }

        public static string GetOSNameByGuidId(FrameSoft.AmarGiri.DataBase.Models.GuidDetailsTable detail)
        {
            TryConnect();
            return service.GetOSNameByGuidId(detail).Data;
        }

        public static void InsertAmar(string ipAddress, string applicationName, string applicationVersion, string OSName, string OSVersion, Guid applicationGuid)
        {
            TryConnect();
            service.InsertAmar(ipAddress, applicationName, applicationVersion, OSName, OSVersion, applicationGuid);
        }


        public static void AddUserMesage(UserMessage userMessage)
        {
            TryConnect();
            service.AddUserMesage(userMessage);
        }

        public static bool CheckMarkAnswerMessage(int[] messageIDs)
        {
            TryConnect();
            return service.CheckMarkAnswerMessage(messageIDs).Data;
        }

        public static bool ReplayToUserMessage(int[] messageIDs, string message)
        {
            TryConnect();
            return service.ReplayToUserMessage(messageIDs, message).Data;
        }

        public static UserMessage GetUserReplay(int guidID, int lastUserMessageID)
        {
            TryConnect();
            return service.GetUserReplay(guidID, lastUserMessageID).Data;
        }

        public static List<UserMessage> GetLastNoAswerMessage(bool returnAll)
        {
            TryConnect();
            return service.GetLastNoAswerMessage(returnAll).Data;
        }

        public static List<UserMessage> GetNoAswerMessageById(int messageID, bool returnAll)
        {
            TryConnect();
            return service.GetNoAswerMessageById(messageID, returnAll).Data;
        }

        public static List<UserMessageReceiveInfo> GetNoReplayMessages(bool getFull)
        {
            TryConnect();
            return service.GetNoReplayMessages(getFull).Data;
        }

        public static int GetGuidIdByMessageID(int messageID)
        {
            TryConnect();
            return service.GetGuidIdByMessageID(messageID).Data;
        }

        public static bool SendReplayForMessages(int[] ids, string msg, int userID, string url)
        {
            TryConnect();
            return service.SendReplayForMessages(ids, msg, userID, url).Data;
        }

        public static bool SkeepReplayMessages(int[] ids)
        {
            TryConnect();
            return service.SkeepReplayMessages(ids).Data;
        }

        public static void SendUserFeedbackMessage(string msg, int userID)
        {
            TryConnect();
            service.SendUserFeedbackMessage(msg, userID);
        }

        public static List<PublicMessageInfo> GetMessagesByDataTime(DateTime dt, Agrin.Framesoft.Messages.LimitMessageEnum limit)
        {
            TryConnect();
            return service.GetMessagesByDataTime(dt, limit).Data;
        }

        public static void AddPublicMessage(string message, string title, Agrin.Framesoft.Messages.LimitMessageEnum limit)
        {
            TryConnect();
            service.AddPublicMessage(message, title, limit);
        }

        public static void AddApplicationErrorLogReport(ApplicationErrorReport error)
        {
            TryConnect();
            service.AddApplicationErrorLogReport(error);
        }

        public static bool ExistUserName(string userName)
        {
            TryConnect();
            return service.ExistUserName(userName).Data;
        }

        public static void RegisterUser(UserInfo user)
        {
            TryConnect();
            service.RegisterUser(user);
        }

        public static bool ExistEmail(string email)
        {
            TryConnect();
            return service.ExistEmail(email).Data;
        }

        public static int LoginUser(string userName, string password)
        {
            TryConnect();
            return service.LoginUser(userName, password).Data;
        }

        public static UserInfo GetUserPropertiesInfo(int userID)
        {
            TryConnect();
            return service.GetUserPropertiesInfo(userID).Data;
        }

        public static UserFileInfo GetLeechFileInfoByGUID(int userID, string guid)
        {
            TryConnect();
            return service.GetLeechFileInfoByGUID(userID, guid).Data;
        }

        public static void SubUserStorageSize(int userID, long size)
        {
            TryConnect();
            service.SubUserStorageSize(userID, size);
        }

        public static void UpdateLeechFile(UserFileInfo fileInfo)
        {
            TryConnect();
            service.UpdateLeechFile(fileInfo);
        }

        public static List<UserFileInfo> GetListLeechFileInfoes(int userID)
        {
            TryConnect();
            return service.GetListLeechFileInfoes(userID).Data;
        }

        public static bool ExistPurchaseByToken(int userID, string token)
        {
            TryConnect();
            return service.ExistPurchaseByToken(userID, token).Data;
        }

        public static void BuyStorageData(UserPurchase buy)
        {
            TryConnect();
            service.BuyStorageData(buy);
        }

        public static void SumUserStorageSize(int userID, long size)
        {
            TryConnect();
            service.SumUserStorageSize(userID, size);
        }

        public static List<UserMessageReplayInfo> GetUserMessageReplaysByDataTime(DateTime dt, int userGuidID)
        {
            TryConnect();
            return service.GetUserMessageReplaysByDataTime(dt, userGuidID).Data;
        }

        public static UserFileInfo GetLeechFileInfo(int userID, int fileID)
        {
            TryConnect();
            return service.GetLeechFileInfo(userID, fileID).Data;
        }

        public static List<UserFileInfo> GetCanDownloadLeechFileInfoes()
        {
            TryConnect();
            return service.GetCanDownloadLeechFileInfoes().Data;
        }

        public static int CreateLeechFile(int userID, long size, string path, string link, int formatCode, string fileName)
        {
            TryConnect();
            return service.CreateLeechFile(userID, size, path, link, formatCode, fileName).Data;
        }
        public static void LogError(string log)
        {
            TryConnect();
            service.LogError(log);
        }
    }
    //public static class AmarGiriUpdater
    //{
    //    public static List<AmarLocalInfo> MemoryInsertedAmar = new List<AmarLocalInfo>();
    //    public static void InsertAmar(string ipAddress, string applicationName, string applicationVersion, string OSName
    //        , string OSVersion, Guid applicationGuid)
    //    {
    //        MemoryInsertedAmar.Add(new AmarLocalInfo() { ApplicationGuid = applicationGuid, ApplicationName = applicationName, ApplicationVersion = applicationVersion, IPAddress = ipAddress, OSName = OSName, OSVersion = OSVersion });
    //        //var ipID = AmarContext.GetAndUpdateIPIDByIP(ipAddress);
    //        //var guidID = AmarContext.AddUpdateApplicationDetails(ipID, applicationName, applicationVersion,
    //        //     OSName, OSVersion, applicationGuid);
    //        //AmarContext.AddAmar(guidID, ipID);
    //        CountAdded = MemoryInsertedAmar.Count.ToString() + " " + (MemoryInsertedAmar.Count > 2000);
    //        if (MemoryInsertedAmar.Count > 2000)
    //        {
    //            PlayInsertEngine(MemoryInsertedAmar.ToList());
    //            MemoryInsertedAmar.Clear();
    //        }
    //    }

    //    public static string LastError = "";
    //    public static string CountAdded = "";
    //    static object lockOBJ = new object();
    //    static void PlayInsertEngine(List<AmarLocalInfo> items)
    //    {
    //        LastError = "PlayInsertEngine ";
    //        Task task = new Task(() =>
    //        {
    //            try
    //            {
    //                lock (lockOBJ)
    //                {
    //                    LastError = "Play For Add " + items.Count;
    //                    AmarContext.GetAndUpdateIPIDByIP(items);
    //                    AmarContext.AddUpdateApplicationDetails(items);
    //                    AmarContext.AddAmar(items);
    //                    LastError = "OK";
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                LastError = e.Message + " newLine " + e.StackTrace;
    //            }
    //        });
    //        task.Start();
    //    }

    //    public static AmarInfo GetFullAmarInfo()
    //    {
    //        var amar = AmarDataBaseHelper.GetFullAmarInfo();
    //        var on = AmarLocalHelper.GetFullAmarInfo(MemoryInsertedAmar.ToList());
    //        amar.OnlineApplicationCount = on.OnlineApplicationCount;
    //        amar.OnlineCount = on.OnlineCount;
    //        return amar;
    //    }
    //}

    //public class AmarLocalHelper
    //{
    //    int GetTotalApplicationAndInstallCount(List<AmarLocalInfo> items)
    //    {
    //        List<Guid> newList = new List<Guid>();
    //        foreach (var item in items)
    //        {
    //            if (!newList.Contains(item.ApplicationGuid))
    //            {
    //                newList.Add(item.ApplicationGuid);
    //            }
    //        }
    //        return items.Count;
    //    }

    //    static long GetTotalCount(List<AmarLocalInfo> items)
    //    {
    //        return items.Count;
    //    }

    //    static int GetTotalIPCount(List<AmarLocalInfo> items)
    //    {
    //        List<string> newList = new List<string>();
    //        foreach (var item in items)
    //        {
    //            if (!newList.Contains(item.IPAddress))
    //            {
    //                newList.Add(item.IPAddress);
    //            }
    //        }
    //        return items.Count;
    //    }

    //    static long GetAmarsCount(DateTime dt, List<AmarLocalInfo> items)
    //    {
    //        var byTimed = from x in items where (x.DateTime.Year > dt.Year) || (x.DateTime.Year >= dt.Year && x.DateTime.Month > dt.Month) || (x.DateTime.Year >= dt.Year && x.DateTime.Month >= dt.Month && x.DateTime.Day >= dt.Day) select x;
    //        //var item = byTimed.FirstOrDefault();
    //        //if (item == null)
    //        //    return 0;
    //        return byTimed.Count();
    //    }

    //    static DateTime GetLastMonthDateTime()
    //    {
    //        DateTime now = DateTime.Now;
    //        DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
    //        return dt;
    //    }

    //    static DateTime GetTodayDateTime()
    //    {
    //        DateTime now = DateTime.Now;
    //        DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
    //        return dt;
    //    }

    //    static DateTime[] GetYesterdayBetweenDateTime()
    //    {
    //        DateTime now = DateTime.Now;
    //        DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
    //        DateTime enddt = dt.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
    //        DateTime[] dates = new DateTime[] { dt, enddt };
    //        return dates;
    //        //return amars.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= enddt.Ticks).ToList();
    //        //return dt;
    //    }

    //    //static DateTime GetYesterdayDateTime()
    //    //{
    //    //    DateTime now = DateTime.Now;
    //    //    DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
    //    //    return dt;
    //    //    //return amars.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= enddt.Ticks).ToList();
    //    //    //return dt;
    //    //}

    //    static DateTime GetLastWeekDateTime()
    //    {
    //        DateTime now = DateTime.Now;
    //        DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
    //        return dt;
    //    }

    //    static DateTime GetOnlineDateTime()
    //    {
    //        return DateTime.Now.AddMinutes(-20);
    //    }

    //    static long GetCount(DateTime dt, List<AmarLocalInfo> items)
    //    {
    //        return GetAmarsCount(dt, items);
    //    }

    //    public static int GetApplicationCount(DateTime dt, List<AmarLocalInfo> items)
    //    {
    //        //Func<AmarLocalInfo, IEnumerable<AmarLocalInfo>> getItems = (item) =>
    //        //{
    //        //    var geted = (from x in items where x.ApplicationGuid == item.ApplicationGuid select x);
    //        //    return geted;
    //        //};
    //        var grpDupes = from f in items
    //                       group f by f.ApplicationGuid into grps
    //                       select grps;
    //        int count = 0;
    //        foreach (var item in grpDupes)
    //        {
    //            if (item.First().DateTime >= dt)
    //                count++;
    //        }
    //        return count;
    //    }

    //    static int GetInstallCount(DateTime dt)
    //    {
    //        using (var ctx = new AmarContext())
    //        {
    //            var byTimed = (from x in ctx.GuidIDs where x.InstallDateTime > dt select x);
    //            return byTimed.Count();
    //        }
    //    }

    //    static int GetIPCount(DateTime dt, List<AmarLocalInfo> items)
    //    {
    //        var grpDupes = from f in items
    //                       group f by f.IPAddress into grps
    //                       select grps;
    //        int count = 0;
    //        foreach (var item in grpDupes)
    //        {
    //            if (item.First().DateTime >= dt)
    //                count++;
    //        }
    //        return count;
    //    }

    //    static long GetAmarsCount(DateTime dt1, DateTime dt2, List<AmarLocalInfo> items)
    //    {
    //        var byTimed = from x in items where ((x.DateTime.Year > dt1.Year) || (x.DateTime.Year >= dt1.Year && x.DateTime.Month > dt1.Month) || (x.DateTime.Year >= dt1.Year && x.DateTime.Month >= dt1.Month && x.DateTime.Day >= dt1.Day)) && ((x.DateTime.Year < dt2.Year) || (x.DateTime.Year <= dt2.Year && x.DateTime.Month < dt2.Month) || (x.DateTime.Year <= dt2.Year && x.DateTime.Month <= dt2.Month && x.DateTime.Day <= dt2.Day)) select x;
    //        return byTimed.Count();
    //    }

    //    static long GetCount(DateTime dt1, DateTime dt2, List<AmarLocalInfo> items)
    //    {
    //        return GetAmarsCount(dt1, dt2, items);
    //    }

    //    public static int GetApplicationCount(DateTime dt1, DateTime dt2)
    //    {
    //        using (var ctx = new AmarContext())
    //        {
    //            var byTimed = (from x in ctx.GuidDetails where x.LastVisitTime >= dt1 && x.LastVisitTime <= dt2 select x);
    //            return byTimed.Count();
    //        }
    //    }

    //    static int GetInstallCount(DateTime dt1, DateTime dt2)
    //    {
    //        using (var ctx = new AmarContext())
    //        {
    //            var byTimed = (from x in ctx.GuidIDs where x.InstallDateTime >= dt1 && x.InstallDateTime <= dt2 select x);
    //            return byTimed.Count();
    //        }
    //    }

    //    static int GetIPCount(DateTime dt1, DateTime dt2)
    //    {
    //        using (var ctx = new AmarContext())
    //        {
    //            var byTimed = (from x in ctx.IPs where x.LastVisitTime >= dt1 && x.LastVisitTime <= dt2 select x);
    //            return byTimed.Count();
    //        }
    //    }


    //    public static AmarInfo GetWindowsAmarInfo()
    //    {
    //        return null;
    //    }

    //    public static AmarInfo GetFullAmarInfo(List<AmarLocalInfo> items)
    //    {
    //        //var now = DateTime.Now.Ticks;
    //        //var amars = GetWindowsTotal(lasDatetime);

    //        //windowslenght += amars.Count;
    //        //lasDatetime = now;
    //        AmarInfo info = new AmarInfo();

    //        //info.TotalApplicationAndInstallCount = GetTotalApplicationAndInstallCount();
    //        //info.TotalCount = GetTotalCount();
    //        //info.TotalIPCount = GetTotalIPCount();

    //        //DateTime dt = GetLastMonthDateTime();//ماه گذشته
    //        //info.LastMonthApplicationCount = GetApplicationCount(dt);//تعداد بازدید نرم افزار ها ثلاض من و علی و حسین بازدید کردیم
    //        //info.LastMonthCount = GetCount(dt);//تعداد بازدید کل
    //        //info.LastMonthInstallCount = GetInstallCount(dt);//تعداد نصب در این ماه ای دی های قبلی شمرده نمی شن
    //        //info.LastMonthIPCount = GetIPCount(dt);//تعداد ای پی های بازدید شده

    //        //dt = GetLastWeekDateTime();//هفته ی گذشته
    //        //info.LastWeekApplicationCount = GetApplicationCount(dt);
    //        //info.LastWeekCount = GetCount(dt);
    //        //info.LastWeekInstallCount = GetInstallCount(dt);
    //        //info.LastWeekIPCount = GetIPCount(dt);

    //        //var dts = GetYesterdayBetweenDateTime();//روز گذشته
    //        //info.YesterdayApplicationCount = GetApplicationCount(dts[0], dts[1]);
    //        //info.YesterdayCount = GetCount(dts[0], dts[1]);
    //        //info.YesterdayInstallCount = GetInstallCount(dts[0], dts[1]);
    //        //info.YesterdayIPCount = GetIPCount(dts[0], dts[1]);

    //        ////dt = GetYesterdayDateTime();//روز گذشته
    //        ////info.YesterdayApplicationCount = GetApplicationCount(dt);
    //        ////info.YesterdayCount = GetCount(dt);
    //        ////info.YesterdayInstallCount = GetInstallCount(dt);
    //        ////info.YesterdayIPCount = GetIPCount(dt);

    //        //dt = GetTodayDateTime();//امروز
    //        //info.TodayApplicationCount = GetApplicationCount(dt);
    //        //info.TodayCount = GetCount(dt);
    //        //info.TodayInstallCount = GetInstallCount(dt);
    //        //info.TodayIPCount = GetIPCount(dt);
    //        if (items.Count > 0)
    //        {
    //            var dt = GetOnlineDateTime();//آنلاین بیست دقیقه ی گذشته
    //            var list = AmarDataBaseHelper.GetListOfApplicationCount(dt);
    //            items.AddRange(list);
    //            info.OnlineApplicationCount = GetApplicationCount(dt, items);
    //            info.OnlineCount = GetIPCount(dt, items);
    //        }
    //        return info;
    //    }
    //}
}
