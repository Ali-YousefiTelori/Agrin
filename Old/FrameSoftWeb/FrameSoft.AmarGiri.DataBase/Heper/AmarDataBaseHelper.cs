using FrameSoft.AmarGiri.DataBase.Contexts;
using FrameSoft.AmarGiri.DataBase.Migrations;
using FrameSoft.AmarGiri.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Heper
{
    //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<AmarContext, Configuration>());
    //var str=ctx.Database.Connection.ConnectionString;
    //var items = ctx.GuidIDs.ToArray();
    // ctx.GuidIDs.RemoveRange(ctx.GuidIDs);
    public static class AmarDataBaseHelper
    {
        //public static int AddGuid(string guid)
        //{
        //    using (var ctx = new AmarContext())
        //    {
        //        GuidIDTable guidIDTable = new GuidIDTable() { Guid = guid };
        //        ctx.GuidIDs.Add(guidIDTable);
        //        ctx.SaveChanges();
        //        return guidIDTable.ID;
        //    }
        //}

        static int GetTotalApplicationAndInstallCount()
        {
            using (var ctx = new AmarContext())
            {
                return ctx.GuidIDs.Count();
            }
        }

        static long GetTotalCount()
        {
            using (var ctx = new AmarContext())
            {
                return (from x in ctx.Amars select x.Count).DefaultIfEmpty(0).Sum();
            }
        }

        static int GetTotalIPCount()
        {
            using (var ctx = new AmarContext())
            {
                return ctx.IPs.Count();
            }
        }

        static long GetAmarsCount(DateTime dt)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = from x in ctx.Amars where (x.YearDate > dt.Year) || (x.YearDate >= dt.Year && x.MonthDate > dt.Month) || (x.YearDate >= dt.Year && x.MonthDate >= dt.Month && x.DayDate >= dt.Day) select x;
                var item = byTimed.FirstOrDefault();
                if (item == null)
                    return 0;
                return item.Count;
            }
        }

        static DateTime GetLastMonthDateTime()
        {
            DateTime now = DateTime.Now;
            DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            return dt;
        }

        static DateTime GetTodayDateTime()
        {
            DateTime now = DateTime.Now;
            DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            return dt;
        }

        static DateTime[] GetYesterdayBetweenDateTime()
        {
            DateTime now = DateTime.Now;
            DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            DateTime enddt = dt.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
            DateTime[] dates = new DateTime[] { dt, enddt };
            return dates;
            //return amars.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= enddt.Ticks).ToList();
            //return dt;
        }

        //static DateTime GetYesterdayDateTime()
        //{
        //    DateTime now = DateTime.Now;
        //    DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    return dt;
        //    //return amars.Where(x => x.DateTime >= dt.Ticks && x.DateTime <= enddt.Ticks).ToList();
        //    //return dt;
        //}

        static DateTime GetLastWeekDateTime()
        {
            DateTime now = DateTime.Now;
            DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            return dt;
        }

        static DateTime GetOnlineDateTime()
        {
            return DateTime.Now.AddMinutes(-20);
        }

        static long GetCount(DateTime dt)
        {
            return GetAmarsCount(dt);
        }

        public static int GetApplicationCount(DateTime dt)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = (from x in ctx.GuidDetails where x.LastVisitTime >= dt select x);
                return byTimed.Count();
            }
        }

        public static List<AmarLocalInfo> GetListOfApplicationCount(DateTime dt)
        {
            using (var ctx = new AmarContext())
            {
                List<AmarLocalInfo> items = new List<AmarLocalInfo>();
                var byTimed = (from x in ctx.GuidDetails where x.LastVisitTime >= dt select x);
                foreach (var item in byTimed)
                {
                    items.Add(new AmarLocalInfo() { ApplicationGuid = AmarContext.GetGuidByID(item.GuidID), IPAddress = AmarContext.GetIpAddressByIpID(item.IPID) });
                }
                return items;
            }
        }

        static int GetInstallCount(DateTime dt)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = (from x in ctx.GuidIDs where x.InstallDateTime > dt select x);
                return byTimed.Count();
            }
        }

        static int GetIPCount(DateTime dt)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = (from x in ctx.IPs where x.LastVisitTime > dt select x);
                return byTimed.Count();
            }
        }

        static long GetAmarsCount(DateTime dt1, DateTime dt2)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = from x in ctx.Amars where ((x.YearDate > dt1.Year) || (x.YearDate >= dt1.Year && x.MonthDate > dt1.Month) || (x.YearDate >= dt1.Year && x.MonthDate >= dt1.Month && x.DayDate >= dt1.Day)) && ((x.YearDate < dt2.Year) || (x.YearDate <= dt2.Year && x.MonthDate < dt2.Month) || (x.YearDate <= dt2.Year && x.MonthDate <= dt2.Month && x.DayDate <= dt2.Day)) select x;
                var item = byTimed.FirstOrDefault();
                if (item == null)
                    return 0;
                return item.Count;
            }
        }

        static long GetCount(DateTime dt1, DateTime dt2)
        {
            return GetAmarsCount(dt1, dt2);
        }

        public static int GetApplicationCount(DateTime dt1, DateTime dt2)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = (from x in ctx.GuidDetails where x.LastVisitTime >= dt1 && x.LastVisitTime <= dt2 select x);
                return byTimed.Count();
            }
        }

        static int GetInstallCount(DateTime dt1, DateTime dt2)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = (from x in ctx.GuidIDs where x.InstallDateTime >= dt1 && x.InstallDateTime <= dt2 select x);
                return byTimed.Count();
            }
        }

        static int GetIPCount(DateTime dt1, DateTime dt2)
        {
            using (var ctx = new AmarContext())
            {
                var byTimed = (from x in ctx.IPs where x.LastVisitTime >= dt1 && x.LastVisitTime <= dt2 select x);
                return byTimed.Count();
            }
        }


        public static AmarInfo GetWindowsAmarInfo()
        {
            return null;
        }

        public static AmarInfo GetFullAmarInfo()
        {
            //var now = DateTime.Now.Ticks;
            //var amars = GetWindowsTotal(lasDatetime);

            //windowslenght += amars.Count;
            //lasDatetime = now;
            AmarInfo info = new AmarInfo();

            info.TotalApplicationAndInstallCount = GetTotalApplicationAndInstallCount();
            info.TotalCount = GetTotalCount();
            info.TotalIPCount = GetTotalIPCount();

            DateTime dt = GetLastMonthDateTime();//ماه گذشته
            info.LastMonthApplicationCount = GetApplicationCount(dt);//تعداد بازدید نرم افزار ها ثلاض من و علی و حسین بازدید کردیم
            info.LastMonthCount = GetCount(dt);//تعداد بازدید کل
            info.LastMonthInstallCount = GetInstallCount(dt);//تعداد نصب در این ماه ای دی های قبلی شمرده نمی شن
            info.LastMonthIPCount = GetIPCount(dt);//تعداد ای پی های بازدید شده

            dt = GetLastWeekDateTime();//هفته ی گذشته
            info.LastWeekApplicationCount = GetApplicationCount(dt);
            info.LastWeekCount = GetCount(dt);
            info.LastWeekInstallCount = GetInstallCount(dt);
            info.LastWeekIPCount = GetIPCount(dt);

            var dts = GetYesterdayBetweenDateTime();//روز گذشته
            info.YesterdayApplicationCount = GetApplicationCount(dts[0], dts[1]);
            info.YesterdayCount = GetCount(dts[0], dts[1]);
            info.YesterdayInstallCount = GetInstallCount(dts[0], dts[1]);
            info.YesterdayIPCount = GetIPCount(dts[0], dts[1]);

            //dt = GetYesterdayDateTime();//روز گذشته
            //info.YesterdayApplicationCount = GetApplicationCount(dt);
            //info.YesterdayCount = GetCount(dt);
            //info.YesterdayInstallCount = GetInstallCount(dt);
            //info.YesterdayIPCount = GetIPCount(dt);

            dt = GetTodayDateTime();//امروز
            info.TodayApplicationCount = GetApplicationCount(dt);
            info.TodayCount = GetCount(dt);
            info.TodayInstallCount = GetInstallCount(dt);
            info.TodayIPCount = GetIPCount(dt);

            dt = GetOnlineDateTime();//آنلاین بیست دقیقه ی گذشته
            info.OnlineApplicationCount = GetApplicationCount(dt);
            info.OnlineCount = GetIPCount(dt);

            return info;
        }
    }
}
