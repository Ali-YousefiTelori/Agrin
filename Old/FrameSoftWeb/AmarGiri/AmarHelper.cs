using AmarGiri.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmarGiri
{
    public static class AmarHelper
    {
        //#region Online
        //public static List<BaseAmarDTO> GetOnline(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddMinutes(-20);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars.Where(x => x.DateTime >= dt.Ticks))
        //    {
        //        if (item.IP != null && !items.ContainsKey(item.IP))
        //            items.Add(item.IP, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetOnlineApplication(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddMinutes(-20);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars.Where(x => x.DateTime >= dt.Ticks))
        //    {
        //        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid))
        //            items.Add(item.ApplicationGuid, item);
        //    }
        //    return items.Values.ToList();
        //}
        //#endregion

        //#region Today

        //public static List<BaseAmarDTO> GetTodayIPCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        //    {
        //        if (item.IP != null && !items.ContainsKey(item.IP))
        //            items.Add(item.IP, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetTodayApplicationCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        //    {
        //        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid))
        //            items.Add(item.ApplicationGuid, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetTodayInstall(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    //List<string> keys = amars.Where(x => x.DateTime < dt.Ticks).Select<AmarDTO, string>(x => x.ApplicationGuid).ToList();
        //    List<string> keys = amars.Select<BaseAmarDTO, string>(x => x.ApplicationGuid).ToList();//.Where(x => x.DateTime < dt.Ticks)

        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        //    {
        //        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid) && !keys.Contains(item.ApplicationGuid))
        //            items.Add(item.ApplicationGuid, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetToday(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    return amars.Where(x => x.DateTime >= dt.Ticks).ToList();
        //}

        //#endregion

        //#region Yesterday
        //public static List<BaseAmarDTO> GetYesterday(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    DateTime today = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    DateTime enddt = dt.AddDays(1);
        //    return amars.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= enddt.Ticks).ToList();
        //}

        //public static List<BaseAmarDTO> GetYesterdayIPCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    //DateTime today = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= today.Ticks)
        //    {
        //        if (item.IP != null && !items.ContainsKey(item.IP))
        //            items.Add(item.IP, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetYesterdayApplicationCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    //DateTime today = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= today.Ticks)
        //    {
        //        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid))
        //            items.Add(item.ApplicationGuid, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetYesterdayInstall(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    //DateTime today = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    //List<string> keys = amars.Where(x => x.DateTime < dt.Ticks).Select<AmarDTO, string>(x => x.ApplicationGuid).ToList();
        //    List<string> keys = amars.Select<BaseAmarDTO, string>(x => x.ApplicationGuid).ToList();//.Where(x => x.DateTime < dt.Ticks)
        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= today.Ticks)
        //    {
        //        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid) && !keys.Contains(item.ApplicationGuid))
        //            items.Add(item.ApplicationGuid, item);
        //    }
        //    return items.Values.ToList();
        //}
        //#endregion


        //#region Weekday
        //public static List<BaseAmarDTO> GetLastWeek(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    return amars.Where(x => x.DateTime >= dt.Ticks).ToList();
        //}

        //public static List<BaseAmarDTO> GetLastWeekIPCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        //    {
        //        if (item.IP != null && !items.ContainsKey(item.IP))
        //            items.Add(item.IP, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetLastWeekApplicationCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        //    {
        //        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid))
        //            items.Add(item.ApplicationGuid, item);
        //    }
        //    return items.Values.ToList();
        //}

        //public static List<BaseAmarDTO> GetLastWeekInstall(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    //DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    //List<string> keys = amars.Where(x => x.DateTime < dt.Ticks).Select<AmarDTO, string>(x => x.ApplicationGuid).ToList();
        //    List<string> keys = amars.Select<BaseAmarDTO, string>(x => x.ApplicationGuid).ToList();

        //    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        //    {
        //        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid) && !keys.Contains(item.ApplicationGuid))
        //            items.Add(item.ApplicationGuid, item);
        //    }
        //    return items.Values.ToList();
        //}

        //#endregion


        //#region Month
        //public static List<BaseAmarDTO> GetLastMonth(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    var list = amars.Where(x => x.DateTime >= dt.Ticks);
        //    //var expect = amars.Except(list);
        //    //var guids = expect.Select<AmarDTO, string>(x => x.ApplicationGuid);
        //    //var installed = new List<AmarDTO>();
        //    //foreach (var item in list)
        //    //{
        //    //    if (!guids.Contains(item.ApplicationGuid))
        //    //        installed.Add(item);
        //    //}
        //    return list.ToList();
        //}

        //public static List<BaseAmarDTO> GetIPCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)
        //    {
        //        if (item.IP != null && !items.ContainsKey(item.IP))
        //            items.Add(item.IP, item);
        //    }
        //    return items.Values.ToList();
        //}
        ////public static List<AmarDTO> GetLastMonthIPCount(List<AmarDTO> amars)
        ////{
        ////    DateTime now = DateTime.Now;
        ////    //DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        ////    Dictionary<string, AmarDTO> items = new Dictionary<string, AmarDTO>();
        ////    foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        ////    {
        ////        if (item.IP != null && !items.ContainsKey(item.IP))
        ////            items.Add(item.IP, item);
        ////    }
        ////    return items.Values.ToList();
        ////}

        ////public static List<AmarDTO> GetLastMonthApplicationCount(List<AmarDTO> amars)
        ////{
        ////    //DateTime now = DateTime.Now;
        ////    ////DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        ////    //Dictionary<string, AmarDTO> items = new Dictionary<string, AmarDTO>();
        ////    //foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        ////    //{
        ////    //    if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid))
        ////    //        items.Add(item.ApplicationGuid, item);
        ////    //}
        ////    //return items.Values.ToList();
        ////    List<AmarDTO> items = new List<AmarDTO>();
        ////    foreach (var item in amars)
        ////    {
        ////        if (guidItems.ContainsKey(item.ApplicationGuid))
        ////            items.Add(item);
        ////    }
        ////    return items;
        ////}

        //public static List<BaseAmarDTO> GetApplicationCount(List<BaseAmarDTO> amars)
        //{
        //    List<BaseAmarDTO> items = new List<BaseAmarDTO>();
        //    HashSet<string> adedItems = new HashSet<string>();
        //    foreach (var item in amars)
        //    {
        //        if (!adedItems.Contains(item.ApplicationGuid))
        //        {
        //            items.Add(item);
        //            adedItems.Add(item.ApplicationGuid);
        //        }
        //    }
        //    return items.ToList();
        //}

        ////public static List<AmarDTO> GetApplicationCount(List<AmarDTO> amars)
        ////{
        ////    List<AmarDTO> items = new List<AmarDTO>();
        ////    HashSet<string> adedItems = new HashSet<string>();
        ////    foreach (var item in amars)
        ////    {
        ////        if (!adedItems.Contains(item.ApplicationGuid))
        ////        {
        ////            items.Add(item);
        ////            adedItems.Add(item.ApplicationGuid);
        ////        }
        ////    }
        ////    return items.ToList();
        ////}

        //public static Dictionary<string, long> GetLastMonthInstall(Dictionary<string, long> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);

        //    //Dictionary<string, AmarDTO> items = new Dictionary<string, AmarDTO>();
        //    var byTimed = from x in amars where x.Value < dt.Ticks select x.Key;
        //    var expect = amars.Keys.Except(byTimed);
        //    Dictionary<string, long> items = new Dictionary<string, long>();
        //    foreach (var item in byTimed)
        //    {
        //        if (!expect.Contains(item))
        //            items.Add(item, amars[item]);
        //    }
        //    return items;
        //    //List<string> keys = byTimed.Select<AmarDTO, string>(x => x.ApplicationGuid).ToList();
        //    //List<string> keys = amars.Select<AmarDTO, string>(x => x.ApplicationGuid).ToList();

        //    //foreach (var item in amars)//.Where(x => x.DateTime >= dt.Ticks)
        //    //{
        //    //    if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid) && !keys.Contains(item.ApplicationGuid))
        //    //        items.Add(item.ApplicationGuid, item);
        //    //}
        //    //return items.Values.ToList();
        //}

        //#endregion

        //#region Total
        //public static List<BaseAmarDTO> GetTotal(long lastDT)
        //{
        //    var list = BaseAmarDTO.SelectList((x) => x.DateTime > lastDT);
        //    foreach (var item in list)
        //    {
        //        if (string.IsNullOrEmpty(item.ApplicationGuid))
        //            continue;
        //        if (!guidItems.ContainsKey(item.ApplicationGuid))
        //        {
        //            guidItems.Add(item.ApplicationGuid, item.DateTime);
        //            if (item.Application == "Agrin WPF Windows" && !guidWindowsItems.ContainsKey(item.ApplicationGuid))
        //                guidWindowsItems.Add(item.ApplicationGuid, item.DateTime);
        //        }
        //        else if (item.DateTime < guidItems[item.ApplicationGuid])
        //        {
        //            guidItems[item.ApplicationGuid] = item.DateTime;
        //            if (item.Application == "Agrin WPF Windows" && guidWindowsItems.ContainsKey(item.ApplicationGuid))
        //                guidWindowsItems[item.ApplicationGuid] = item.DateTime;
        //        }

        //        if (!ipItems.Contains(item.IP))
        //            ipItems.Add(item.IP);
        //        if (!ipwindowsItems.Contains(item.IP) && item.Application == "Agrin WPF Windows")
        //            ipwindowsItems.Add(item.IP);
        //    }
        //    return list;
        //}

        //public static List<BaseAmarDTO> GetWindowsTotal(long lastDT)
        //{
        //    return GetTotal(lastDT).Where((amar) => amar.Application == "Agrin WPF Windows").ToList();
        //}

        //public static List<BaseAmarDTO> GetTotalIPCount(List<BaseAmarDTO> amars)
        //{
        //    DateTime now = DateTime.Now;
        //    Dictionary<string, BaseAmarDTO> items = new Dictionary<string, BaseAmarDTO>();
        //    foreach (var item in amars)
        //    {
        //        if (item.IP != null && !items.ContainsKey(item.IP))
        //            items.Add(item.IP, item);
        //    }
        //    return items.Values.ToList();
        //}

        ////public static List<AmarDTO> GetTotalApplicationAndInstallCount(List<AmarDTO> amars)
        ////{
        ////    DateTime now = DateTime.Now;
        ////    Dictionary<string, AmarDTO> items = new Dictionary<string, AmarDTO>();
        ////    foreach (var item in amars)
        ////    {
        ////        if (item.ApplicationGuid != null && !items.ContainsKey(item.ApplicationGuid))
        ////            items.Add(item.ApplicationGuid, item);
        ////    }
        ////    return items.Values.ToList();
        ////}
        //public static Dictionary<string, long> GetInstall(Dictionary<string, long> amars, long ticks, bool isYesterday = false)
        //{
        //    IEnumerable<string> byTimed = null;
        //    if (isYesterday)
        //    {
        //        DateTime now = DateTime.Now;
        //        DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //        DateTime today = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //        DateTime enddt = dt.AddDays(1);
        //        byTimed = from x in amars where x.Value >= dt.Ticks && x.Value <= enddt.Ticks select x.Key;
        //    }
        //    else
        //        byTimed = (from x in amars where x.Value > ticks select x.Key).ToList();
        //    var intersect = amars.Keys.Intersect(byTimed).ToList();
        //    var except = amars.Keys.Except(byTimed).ToList();
        //    //DateTime dtgggg = new DateTime(amars[except[0]]);
        //    Dictionary<string, long> items = new Dictionary<string, long>();
        //    foreach (var item in intersect)
        //    {
        //        if (!except.Contains(item))
        //            items.Add(item, amars[item]);
        //    }
        //    return items;
        //}

        //static long GetLastMonthTick()
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    return dt.Ticks;
        //}
        //static long GetLastWeekTick()
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    return dt.Ticks;
        //}
        ////static long GetYesterdayTick()
        ////{
        ////    DateTime now = DateTime.Now;
        ////    DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        ////    return dt.Ticks;
        ////}
        //static long GetTodayTick()
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    return dt.Ticks;
        //}

        //#endregion

        static AmarInfo lastAmar = null;
        static long lasDatetime = DateTime.MinValue.Ticks;
        static List<BaseAmarDTO> lastMonthAmar = new List<BaseAmarDTO>();
        static List<BaseAmarDTO> lastWeekAmar = new List<BaseAmarDTO>();
        static List<BaseAmarDTO> yesterdayAmar = new List<BaseAmarDTO>();
        static Dictionary<string, long> guidItems = new Dictionary<string, long>();
        static HashSet<string> ipItems = new HashSet<string>();
        static int lenght = 0;

        public static AmarInfo GetAllAmarInfo()
        {
            //var now = DateTime.Now.Ticks;
            //var amars = GetTotal(lasDatetime);
            //lenght += amars.Count;
            //lasDatetime = now;
            AmarInfo info = new AmarInfo();

            //info.TotalApplicationAndInstallCount = guidItems.Count;//GetTotalApplicationAndInstallCount(amars).Count;
            //info.TotalCount = lenght;
            //info.TotalIPCount = ipItems.Count;

            //lastMonthAmar.AddRange(amars);
            //lastMonthAmar = GetLastMonth(lastMonthAmar);

            //var installItems = GetInstall(guidItems, GetLastMonthTick());
            //info.LastMonthApplicationCount = GetApplicationCount(lastMonthAmar).Count;
            //info.LastMonthCount = lastMonthAmar.Count;
            //info.LastMonthInstallCount = installItems.Count;
            //info.LastMonthIPCount = GetIPCount(lastMonthAmar).Count;

            //lastWeekAmar = GetLastWeek(lastMonthAmar);
            //installItems = GetInstall(installItems, GetLastWeekTick());
            //info.LastWeekApplicationCount = GetApplicationCount(lastWeekAmar).Count;
            //info.LastWeekCount = lastWeekAmar.Count;
            //info.LastWeekInstallCount = installItems.Count;
            //info.LastWeekIPCount = GetIPCount(lastWeekAmar).Count;

            //yesterdayAmar = GetYesterday(lastWeekAmar);
            //installItems = GetInstall(guidItems, 0, true);
            //info.YesterdayApplicationCount = GetApplicationCount(yesterdayAmar).Count;
            //info.YesterdayCount = yesterdayAmar.Count;
            //info.YesterdayInstallCount = installItems.Count;
            //info.YesterdayIPCount = GetIPCount(yesterdayAmar).Count;

            //var todayList = GetToday(lastWeekAmar);
            //installItems = GetInstall(guidItems, GetTodayTick());
            //info.TodayApplicationCount = GetApplicationCount(todayList).Count;
            //info.TodayCount = todayList.Count;
            //info.TodayInstallCount = installItems.Count;
            //info.TodayIPCount = GetIPCount(todayList).Count;

            //info.OnlineApplicationCount = GetOnlineApplication(todayList).Count;
            //info.OnlineCount = GetOnline(todayList).Count;

            //lastAmar = info;
            //amars.Clear();
            return info;
        }

        static Dictionary<string, long> guidWindowsItems = new Dictionary<string, long>();
        static List<BaseAmarDTO> windowsLastMonthAmar = new List<BaseAmarDTO>();
        static int windowslenght = 0;
        static HashSet<string> ipwindowsItems = new HashSet<string>();
        public static AmarInfo GetWindowsAmarInfo()
        {
            //var now = DateTime.Now.Ticks;
            //var amars = GetWindowsTotal(lasDatetime);

            //windowslenght += amars.Count;
            //lasDatetime = now;
            AmarInfo info = new AmarInfo();

            //info.TotalApplicationAndInstallCount = guidWindowsItems.Count;//GetTotalApplicationAndInstallCount(amars).Count;
            //info.TotalCount = windowslenght;
            //info.TotalIPCount = ipwindowsItems.Count;

            //windowsLastMonthAmar.AddRange(amars);
            //windowsLastMonthAmar = GetLastMonth(windowsLastMonthAmar);

            //var installItems = GetInstall(guidWindowsItems, GetLastMonthTick());
            //info.LastMonthApplicationCount = GetApplicationCount(windowsLastMonthAmar).Count;
            //info.LastMonthCount = windowsLastMonthAmar.Count;
            //info.LastMonthInstallCount = installItems.Count;
            //info.LastMonthIPCount = GetIPCount(windowsLastMonthAmar).Count;

            //lastWeekAmar = GetLastWeek(windowsLastMonthAmar);
            //installItems = GetInstall(installItems, GetLastWeekTick());
            //info.LastWeekApplicationCount = GetApplicationCount(lastWeekAmar).Count;
            //info.LastWeekCount = lastWeekAmar.Count;
            //info.LastWeekInstallCount = installItems.Count;
            //info.LastWeekIPCount = GetIPCount(lastWeekAmar).Count;

            //yesterdayAmar = GetYesterday(lastWeekAmar);
            //installItems = GetInstall(guidWindowsItems, 0, true);
            //info.YesterdayApplicationCount = GetApplicationCount(yesterdayAmar).Count;
            //info.YesterdayCount = yesterdayAmar.Count;
            //info.YesterdayInstallCount = installItems.Count;
            //info.YesterdayIPCount = GetIPCount(yesterdayAmar).Count;

            //var todayList = GetToday(lastWeekAmar);
            //installItems = GetInstall(guidWindowsItems, GetTodayTick());
            //info.TodayApplicationCount = GetApplicationCount(todayList).Count;
            //info.TodayCount = todayList.Count;
            //info.TodayInstallCount = installItems.Count;
            //info.TodayIPCount = GetIPCount(todayList).Count;

            //info.OnlineApplicationCount = GetOnlineApplication(todayList).Count;
            //info.OnlineCount = GetOnline(todayList).Count;

            //lastAmar = info;
            //amars.Clear();
            return info;
        }
    }
}
