using FrameSoft.AmarGiri.DataBase.Migrations;
using FrameSoft.AmarGiri.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading;

namespace FrameSoft.AmarGiri.DataBase.Contexts
{
    public class AmarContext : DbContext
    {


        //static AmarContext()
        //{
        //    //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<AmarGiri.DataBase.Contexts.AmarContext, Configuration>());
        //}
        //public AmarContext()
        //    : base("AmarGiriEntities")
        //{

        //}
        public AmarContext()
              : base("data source=(LOCAL)\\SQLEXPRESS;initial catalog=FrameSoftAmar;User ID=Ali;Password=.;App=EntityFramework")
        {
            Database.SetInitializer<AmarContext>(null);
            Database.SetInitializer(new CreateDatabaseIfNotExists<AmarContext>());
            Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<AmarContext, Migrations.Configuration>());
        }

#if (FrameAppVPS || DEBUG)

#elif (FrameSoftIR)
 public AmarContext()
            : base("Data Source=localhost;Integrated Security=False;Initial Catalog=FrameSoftAmar;User ID=ali;Password=.;Connect Timeout=15;Encrypt=False;Packet Size=4096")
        {
        }
#endif


        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<AmarTable>()
        //        .HasIndex("IX_Customers_Name",          // Provide the index name.
        //            e => e.Property(x => x.LastName),   // Specify at least one column.
        //            e => e.Property(x => x.FirstName))  // Multiple columns as desired.

        //        .HasIndex("IX_Customers_EmailAddress",  // Supports fluent chaining for more indexes.
        //            IndexOptions.Unique,                // Supports flags for unique and clustered.
        //            e => e.Property(x => x.EmailAddress));
        //}

        public DbSet<AmarTable> Amars { get; set; }
        public DbSet<GuidDetailsTable> GuidDetails { get; set; }
        public DbSet<GuidIDTable> GuidIDs { get; set; }
        public DbSet<IPsTable> IPs { get; set; }

        public DbSet<ApplicationNamesTable> ApplicationNames { get; set; }
        public DbSet<ApplicationVersionTable> ApplicationVersions { get; set; }
        public DbSet<OSNameTable> OSNames { get; set; }
        public DbSet<OSVersionTable> OSVersions { get; set; }

        public DbSet<ErrorLogTable> ErrorLogs { get; set; }
        /// <summary>
        /// یوزر های بی تربیت
        /// </summary>
        public DbSet<BlackUserIgnoreInfo> BlackList { get; set; }

        static object lockOBJ = new object();
        static object innerLockOBJ = new object();

        public static int GetAndUpdateApplicationIDByName(string name)
        {
            lock (innerLockOBJ)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.ApplicationNames where x.Name == name select x;
                    var first = items.FirstOrDefault();
                    if (first != null)
                        return first.ID;
                    else
                    {
                        var get = new ApplicationNamesTable() { Name = name };
                        ctx.ApplicationNames.Add(get);
                        ctx.SaveChanges();
                        return get.ID;
                    }
                }
            }
        }

        public static int GetAndUpdateApplicationVersionsIDByVersion(string version)
        {
            lock (innerLockOBJ)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.ApplicationVersions where x.Version == version select x;
                    var first = items.FirstOrDefault();
                    if (first != null)
                        return first.ID;
                    else
                    {
                        var get = new ApplicationVersionTable() { Version = version };
                        ctx.ApplicationVersions.Add(get);
                        ctx.SaveChanges();
                        return get.ID;
                    }
                }
            }
        }

        public static int GetAndUpdateOSNamesIDByName(string name)
        {
            lock (innerLockOBJ)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.OSNames where x.Name == name select x;
                    var first = items.FirstOrDefault();
                    if (first != null)
                        return first.ID;
                    else
                    {
                        var get = new OSNameTable() { Name = name };
                        ctx.OSNames.Add(get);
                        ctx.SaveChanges();
                        return get.ID;
                    }
                }
            }
        }

        public static int GetAndUpdateOSVersionsIDByVersion(string version)
        {
            lock (innerLockOBJ)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.OSVersions where x.Version == version select x;
                    var first = items.FirstOrDefault();
                    if (first != null)
                        return first.ID;
                    else
                    {
                        var get = new OSVersionTable() { Version = version };
                        ctx.OSVersions.Add(get);
                        ctx.SaveChanges();
                        return get.ID;
                    }
                }
            }
        }

        public static int GetAndUpdateGuidIDByGuid(Guid guid, AmarContext ctx = null)
        {
            Func<AmarContext, int> run = (_ctx) =>
            {
                var items = from x in _ctx.GuidIDs where x.GuidData == guid select x;
                var first = items.FirstOrDefault();
                if (first != null)
                {
                    return first.ID;
                }
                else
                {
                    var get = new GuidIDTable() { GuidData = guid, InstallDateTime = DateTime.Now };
                    _ctx.GuidIDs.Add(get);
                    _ctx.SaveChanges();
                    return get.ID;
                }
            };
            if (ctx != null)
            {
                return run(ctx);
            }
            else
            {
                using (var _ctx = new AmarContext())
                {
                    return run(_ctx);
                }
            }

        }

        public static bool IsBlackListGUID(Guid guid)
        {
            lock (lockOBJ)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.GuidIDs where x.GuidData == guid select x;
                    var first = items.FirstOrDefault();
                    if (first != null)
                    {
                        var blackList = from x in ctx.BlackList where x.GuidID == first.ID select x;
                        if (blackList.FirstOrDefault() != null)
                            return true;
                    }

                }
                return false;
            }
        }

        public static bool SendToBlackList(int guidID)
        {
            lock (lockOBJ)
            {
                using (var ctx = new AmarContext())
                {
                    var blackList = from x in ctx.BlackList where x.GuidID == guidID select x;
                    if (blackList.FirstOrDefault() != null)
                        return false;
                    ctx.BlackList.Add(new BlackUserIgnoreInfo() { GuidID = guidID });
                    ctx.SaveChanges();
                    return true;
                }
            }
        }

        public static string GetIpAddressByIpID(int ipID)
        {
            using (var ctx = new AmarContext())
            {
                //var sss = ctx.Database.Connection.ConnectionString;
                var items = from x in ctx.IPs where x.ID == ipID select x;
                var first = items.FirstOrDefault();
                if (first != null)
                {
                    return first.IPValue;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int GetAndUpdateIPIDByIP(string ip)
        {
            lock (lockOBJ)
            {
                using (var ctx = new AmarContext())
                {
                    //var sss = ctx.Database.Connection.ConnectionString;
                    var items = from x in ctx.IPs where x.IPValue == ip select x;
                    var first = items.FirstOrDefault();
                    if (first != null)
                    {
                        first.LastVisitTime = DateTime.Now;
                        ctx.SaveChanges();
                        return first.ID;
                    }
                    else
                    {
                        var get = new IPsTable() { IPValue = ip, LastVisitTime = DateTime.Now };
                        ctx.IPs.Add(get);
                        ctx.SaveChanges();
                        return get.ID;
                    }
                }
            }
        }

        public static void GetAndUpdateIPIDByIP(List<AmarLocalInfo> amars)
        {
            using (var ctx = new AmarContext())
            {
                var ips = amars.GroupBy(ip => ip.IPAddress).Select(g => g.First()).ToList();
                var all = ips.Select(x => x.IPAddress);

                var items = (from x in ctx.IPs where all.Contains(x.IPValue) select x).ToList();

                var count = items.Count;
                var exp1 = items.Select(x => x.IPValue);
                var mustAdd = ips.Where(x => !exp1.Contains(x.IPAddress)).ToList();//اضافه شده
                var mustEdit = items.Where(x => all.Contains(x.IPValue)).ToList();//باید ویرایش شوند
                foreach (var item in mustAdd)
                {
                    var get = new IPsTable() { IPValue = item.IPAddress, LastVisitTime = DateTime.Now };
                    ctx.IPs.Add(get);
                }

                foreach (var item in mustEdit)
                {
                    item.LastVisitTime = DateTime.Now;
                }

                ctx.SaveChanges();
            }
            //using (var ctx = new AmarContext())
            //{
            //    foreach (var ip in ips)
            //    {
            //        var items = from x in ctx.IPs where x.IPValue == ip.IPAddress select x;
            //        var first = items.FirstOrDefault();
            //        if (first != null)
            //        {
            //            first.LastVisitTime = DateTime.Now;
            //            ip.IpId = first.ID;
            //        }
            //        else
            //        {
            //            var get = new IPsTable() { IPValue = ip.IPAddress, LastVisitTime = DateTime.Now };
            //            ctx.IPs.Add(get);
            //            ip.IpId = get.ID;
            //        }
            //        Thread.Sleep(10);
            //    }
            //    ctx.SaveChanges();
            //}
        }

        public static GuidDetailsTable GetGuidDetails(int guidID)
        {
            using (var ctx = new AmarContext())
            {
                var items = from x in ctx.GuidDetails where x.GuidID == guidID select x;
                var detail = items.FirstOrDefault();
                if (detail == null)
                    return null;
                return detail;
            }
        }

        public static Guid GetGuidByID(int guidID)
        {
            using (var ctx = new AmarContext())
            {
                var items = from x in ctx.GuidIDs where x.ID == guidID select x;
                if (items.FirstOrDefault() == null)
                    return Guid.Empty;
                var detail = items.FirstOrDefault().GuidData;
                return detail;
            }
        }

        public static string GetApplicationVersionByGuidId(GuidDetailsTable detail)
        {
            if (detail != null)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.ApplicationVersions where x.ID == detail.ApplicationVersionID select x;
                    var version = items.FirstOrDefault();
                    if (version == null)
                        return null;
                    return version.Version;
                }
            }
            return null;
        }

        public static string GetOSVersionByGuidId(GuidDetailsTable detail)
        {
            if (detail != null)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.OSVersions where x.ID == detail.OSVersionID select x;
                    var version = items.FirstOrDefault();
                    if (version == null)
                        return null;
                    return version.Version;
                }
            }
            return null;
        }

        public static string GetOSNameByGuidId(GuidDetailsTable detail)
        {
            if (detail != null)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.OSNames where x.ID == detail.OSNameID select x;
                    var os = items.FirstOrDefault();
                    if (os == null)
                        return null;
                    return os.Name;
                }
            }
            return null;
        }

        public static string GetApplicationNameByGuidId(GuidDetailsTable detail)
        {
            if (detail != null)
            {
                using (var ctx = new AmarContext())
                {
                    var items = from x in ctx.ApplicationNames where x.ID == detail.ApplicationNameID select x;
                    var version = items.FirstOrDefault();
                    if (version == null)
                        return null;
                    return version.Name;
                }
            }
            return null;
        }

        public static int AddUpdateApplicationDetails(AmarContext ctx, string applicationName, string applicationVersion, string OSName, string OSVersion, Guid applicationGuid)
        {
            int guidID = GetAndUpdateGuidIDByGuid(applicationGuid, ctx);

            var items = from x in ctx.GuidDetails where x.GuidID == guidID select x;
            var detail = items.FirstOrDefault();

            if (detail != null)
            {
                //detail.IPID = ipID;
                detail.LastVisitTime = DateTime.Now;
                detail.ApplicationVersionID = GetAndUpdateApplicationVersionsIDByVersion(applicationVersion);
            }
            else
            {
                GuidDetailsTable guidDetailsTable = new GuidDetailsTable();
                guidDetailsTable.LastVisitTime = DateTime.Now;
                guidDetailsTable.ApplicationNameID = GetAndUpdateApplicationIDByName(applicationName);
                guidDetailsTable.GuidID = guidID;
                guidDetailsTable.ApplicationVersionID = GetAndUpdateApplicationVersionsIDByVersion(applicationVersion);
                //guidDetailsTable.IPID = ipID;
                guidDetailsTable.OSNameID = GetAndUpdateOSNamesIDByName(OSName);
                guidDetailsTable.OSVersionID = GetAndUpdateOSVersionsIDByVersion(OSVersion);
                ctx.GuidDetails.Add(guidDetailsTable);
            }
            return guidID;
        }

        public static void AddUpdateApplicationDetails(List<AmarLocalInfo> amarInfoes)
        {
            //using (var ctx = new AmarContext())
            //{
            //    var guids = amarInfoes.GroupBy(ip => ip.ApplicationGuid).Select(g => g.First()).ToList();
            //    var all = guids.Select(x => x.ApplicationGuid);

            //    var items = (from x in ctx.GuidIDs where all.Contains(x.GuidData) select x).ToList();

            //    var count = items.Count;
            //    var exp1 = items.Select(x => x.GuidData);
            //    var mustAdd = guids.Where(x => !exp1.Contains(x.ApplicationGuid)).ToList();//اضافه شده
            //    var mustEdit = items.Where(x => all.Contains(x.GuidData)).ToList();//باید ویرایش شوند

            //    foreach (var item in mustAdd)
            //    {
            //        GuidDetailsTable guidDetailsTable = new GuidDetailsTable();
            //        guidDetailsTable.LastVisitTime = DateTime.Now;
            //        guidDetailsTable.ApplicationNameID = GetAndUpdateApplicationIDByName(item.ApplicationName);
            //        guidDetailsTable.GuidID = item.GuidID;
            //        guidDetailsTable.ApplicationVersionID = GetAndUpdateApplicationVersionsIDByVersion(item.ApplicationVersion);
            //        guidDetailsTable.OSNameID = GetAndUpdateOSNamesIDByName(item.OSName);
            //        guidDetailsTable.OSVersionID = GetAndUpdateOSVersionsIDByVersion(item.OSVersion);
            //        ctx.GuidDetails.Add(guidDetailsTable);
            //    }
            //    ctx.SaveChanges();
            //}

            using (var ctx = new AmarContext())
            {
                foreach (var amar in amarInfoes)
                {
                    int guidID = GetAndUpdateGuidIDByGuid(amar.ApplicationGuid, ctx);
                    var items = from x in ctx.GuidDetails where x.GuidID == guidID select x;
                    var detail = items.FirstOrDefault();

                    if (detail != null)
                    {
                        detail.IPID = amar.IpId;
                        detail.LastVisitTime = DateTime.Now;
                        detail.ApplicationVersionID = GetAndUpdateApplicationVersionsIDByVersion(amar.ApplicationVersion);
                    }
                    else
                    {
                        GuidDetailsTable guidDetailsTable = new GuidDetailsTable();
                        guidDetailsTable.LastVisitTime = DateTime.Now;
                        guidDetailsTable.ApplicationNameID = GetAndUpdateApplicationIDByName(amar.ApplicationName);
                        guidDetailsTable.GuidID = guidID;
                        guidDetailsTable.ApplicationVersionID = GetAndUpdateApplicationVersionsIDByVersion(amar.ApplicationVersion);
                        guidDetailsTable.IPID = amar.IpId;
                        guidDetailsTable.OSNameID = GetAndUpdateOSNamesIDByName(amar.OSName);
                        guidDetailsTable.OSVersionID = GetAndUpdateOSVersionsIDByVersion(amar.OSVersion);
                        ctx.GuidDetails.Add(guidDetailsTable);
                    }
                    Thread.Sleep(10);
                }
                ctx.SaveChanges();
            }

        }

        public static void ClearAmar()
        {
            using (var ctx = new AmarContext())
            {
                ctx.Amars.RemoveRange(ctx.Amars);
                ctx.SaveChanges();
            }
        }

        public static void AddAmar(int guidID, AmarContext ctx)
        {
            DateTime dt = DateTime.Now;
            int day = dt.Day;
            int month = dt.Month;
            int year = dt.Year;
            var items = from x in ctx.Amars where x.DayDate == day && x.MonthDate == month && x.YearDate == dt.Year select x;
            var detail = items.FirstOrDefault();

            if (detail != null)
            {
                detail.Count++;
            }
            else
            {
                detail = new AmarTable();
                detail.DayDate = (byte)day;
                detail.MonthDate = (byte)month;
                detail.YearDate = year;
                detail.Count = 1;
                ctx.Amars.Add(detail);
            }
        }

        public static void AddAmar(List<AmarLocalInfo> amars)
        {
            using (var ctx = new AmarContext())
            {
                foreach (var item in amars)
                {
                    DateTime dt = item.DateTime;
                    int day = dt.Day;
                    int month = dt.Month;
                    int year = dt.Year;
                    var items = from x in ctx.Amars where x.DayDate == day && x.MonthDate == month && x.YearDate == dt.Year select x;
                    var detail = items.FirstOrDefault();

                    if (detail != null)
                    {
                        detail.Count++;
                    }
                    else
                    {
                        detail = new AmarTable();
                        detail.DayDate = (byte)day;
                        detail.MonthDate = (byte)month;
                        detail.YearDate = year;
                        detail.Count = 1;
                        ctx.Amars.Add(detail);
                    }
                    Thread.Sleep(10);
                }
                ctx.SaveChanges();
            }
        }

        public static Exception AddErrorLog(Exception error)
        {
            try
            {
                lock (lockOBJ)
                {
                    string msg = "";
                    if (error is System.Data.Entity.Validation.DbEntityValidationException)
                    {
                        var ex = error as System.Data.Entity.Validation.DbEntityValidationException;
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            msg += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                msg += string.Format("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                       ve.PropertyName,
                                       eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                       ve.ErrorMessage);
                            }
                        }
                    }
                    else
                    {
                        while (error != null)
                        {
                            msg += "Start " + error.Message + " END ";
                            error = error.InnerException;
                        }
                    }

                    using (var ctx = new AmarContext())
                    {
                        var get = new ErrorLogTable() { InsertDateTime = DateTime.Now, Message = msg };
                        ctx.ErrorLogs.Add(get);
                        ctx.SaveChanges();
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
