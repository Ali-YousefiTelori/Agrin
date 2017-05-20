using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrameSoft.AmarGiri.DataBase.Heper;
using FrameSoft.AmarGiri.DataBase.Contexts;
using FrameSoft.AmarGiri.DataBase.Models;
using System.Collections.Generic;
using System.Linq;

namespace LibraryCodeTest
{
    public class ali
    {
        public string A { get; set; }
        public int B { get; set; }
    }
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PerformanceTest()
        {
            try
            {
                List<AmarLocalInfo> amarInfoes = new List<AmarLocalInfo>();
                amarInfoes.Add(new AmarLocalInfo() { ApplicationGuid = Guid.Parse("1c0ed6d4-0a91-468e-9777-939060281fef") });
                amarInfoes.Add(new AmarLocalInfo() { ApplicationGuid = Guid.Parse("5e269494-27d6-43e4-8dc4-3344e1d1c8ea") });
                amarInfoes.Add(new AmarLocalInfo() { ApplicationGuid = Guid.NewGuid() });
                amarInfoes.Add(new AmarLocalInfo() { ApplicationGuid = Guid.NewGuid() });
                amarInfoes.Add(new AmarLocalInfo() { ApplicationGuid = Guid.NewGuid() });
                amarInfoes.Add(new AmarLocalInfo() { ApplicationGuid = amarInfoes.Last().ApplicationGuid });
                using (var ctx = new AmarContext())
                {
                    var guids = amarInfoes.GroupBy(ip => ip.ApplicationGuid).Select(g => g.First()).ToList();
                    var all = guids.Select(x => x.ApplicationGuid);

                    var items = (from x in ctx.GuidIDs where all.Contains(x.GuidData) select x).ToList();

                    var count = items.Count;
                    var exp1 = items.Select(x => x.GuidData);
                    var mustAdd = guids.Where(x => !exp1.Contains(x.ApplicationGuid)).ToList();//اضافه شده
                    var mustEdit = items.Where(x => all.Contains(x.GuidData)).ToList();//باید ویرایش شوند


                    ctx.SaveChanges();
                }
                //using (var ctx = new AmarContext())
                //{
                //    var ips = amars.GroupBy(ip => ip.IPAddress).Select(g => g.First()).ToList();
                //    var all = ips.Select(x => x.IPAddress);

                //    var items = (from x in ctx.IPs where all.Contains(x.IPValue) select x).ToList();

                //    var count = items.Count;
                //    var exp1 = items.Select(x => x.IPValue);
                //    var mustAdd = ips.Where(x => !exp1.Contains(x.IPAddress)).ToList();//اضافه شده
                //    var mustEdit = items.Where(x => all.Contains(x.IPValue)).ToList();//باید ویرایش شوند

                //    ctx.SaveChanges();
                //}
            //List<ali> aaaaa = new List<ali>() { new ali() { A = "aaa" }, new ali() { A = "aaa" }, new ali() { A = "bb" } };
            //var qqqqq = aaaaa.GroupBy(car => car.A).Select(g => g.First()).ToList(); //(from x in aaaaa group x.A by x into g select g.Key).ToArray();
            

                //List<IPsTable> ips = new List<IPsTable>();
                //using (var ctx = new AmarContext())
                //{
                //    ips = (from x in ctx.IPs select x).Take(100).ToList();
                //    ips.Add(new IPsTable() { IPValue = "154.254.111.333" });
                //}
                //using (var ctx = new AmarContext())
                //{
                //    foreach (var ip in ips)
                //    {
                //        var all = ips.Select(x => x.IPValue);
                //        var items = (from x in ctx.IPs where all.Contains(x.IPValue) select x).ToList();

                //        var count = items.Count;
                //        var exp1 = items.Select(x => x.IPValue);
                //        var expect = ips.Where(x => !exp1.Contains(x.IPValue)).ToList();//اضافه شده
                //        var noexpect = ips.Where(x => exp1.Contains(x.IPValue)).ToList();//باید ویرایش شوند

                //        //if (first != null)
                //        //{
                //        //    first.LastVisitTime = DateTime.Now;
                //        //    ip.ID = first.ID;
                //        //}
                //        //else
                //        //{
                //        //    var get = new IPsTable() { IPValue = ip.IPValue, LastVisitTime = DateTime.Now };
                //        //    ctx.IPs.Add(get);
                //        //    ip.ID = get.ID;
                //        //}
                //    }
                //    ctx.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                //DataBaseHelper.LoadDataBase();
                //BaseAmarDTO val = new BaseAmarDTO() { IP = "127.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = DateTime.Now.Ticks };
                //bool insert = val.InsertToDataBase();
                //foreach (var item in BaseAmarDTO.SelectAll())
                //{

                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void AddGUID()
        {
            try
            {
                // AmarDataBaseHelper.AddGuid(Guid.NewGuid().ToString());
            }
            catch
            {

            }
        }

        [TestMethod]
        public void TestDateTime()
        {
            try
            {
                //DataBaseHelper.LoadDataBase();
                //for (int i = 0; i < 100; i++)
                //{
                //    AmarDTO val = new AmarDTO() { IP = "127.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = DateTime.Now.Ticks};
                //    val.InsertToDataBase();
                //}
                //for (int i = 0; i < 100; i++)
                //{
                //    AmarDTO val = new AmarDTO() { IP = "127.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = DateTime.Now.AddDays(-1).Ticks };
                //    val.InsertToDataBase();
                //}

                //DateTime diroozStart = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                //DateTime dirooz = diroozStart;
                //AmarDTO val = new AmarDTO() { IP = "128.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = DateTime.Now.Ticks };
                //val.InsertToDataBase();
                //dirooz = diroozStart.AddHours(10);
                //val = new AmarDTO() { IP = "129.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = dirooz.Ticks };
                //val.InsertToDataBase();
                //dirooz = diroozStart.AddHours(23).AddMinutes(55);
                //val = new AmarDTO() { IP = "130.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = dirooz.Ticks };
                //val.InsertToDataBase();
                //dirooz = diroozStart.AddHours(24);
                //val = new AmarDTO() { IP = "131.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = dirooz.Ticks };
                //val.InsertToDataBase();
                //dirooz = diroozStart.AddHours(24).AddMinutes(1);
                //val = new AmarDTO() { IP = "132.0.0.1", Application = "Android Agrin", ApplicationVersion = "1.4", OSName = "Android", OSVersion = "1.4.3", DateTime = dirooz.Ticks };
                //val.InsertToDataBase();

                ////Get Today
                //DateTime now = DateTime.Now;
                //DateTime dt = now.AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                //var list = AmarDTO.SelectList(x => x.DateTime >= dt.Ticks);

                //Get Yesterday
                //DateTime now = DateTime.Now;
                //DateTime dt = now.AddDays(-1).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                //DateTime enddt = dt.AddDays(1);
                //var list = AmarDTO.SelectList(x => x.DateTime >= dt.Ticks && x.DateTime <= enddt.Ticks);

                //Get Last Week
                //DateTime now = DateTime.Now;
                //DateTime dt = now.AddDays(-7).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                //var list = AmarDTO.SelectList(x => x.DateTime >= dt.Ticks);

                //Get Last Month
                //DateTime now = DateTime.Now;
                //DateTime dt = now.AddDays(-30).AddHours(-now.Hour).AddMinutes(-now.Minute).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                //var list = AmarDTO.SelectList(x => x.DateTime >= dt.Ticks);

                //Get All
                //var list = AmarDTO.SelectAll();


                //Get Online
                //DateTime now = DateTime.Now;
                //DateTime dt = now.AddMinutes(-15);
                //var list = AmarDTO.SelectList(x => x.DateTime >= dt.Ticks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
