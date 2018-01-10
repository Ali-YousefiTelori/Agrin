using FrameSoftWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers
{
    public class AmarGiriController : Controller
    {
//#if (FrameAppVPS)
//        private static volatile Type _dependency;

//        static AmarGiriController()
//        {
//            _dependency = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
//        }
//#endif

        //static string _ApplicationDirectory;
        //public static string ApplicationDirectory
        //{
        //    get
        //    {
        //        return _ApplicationDirectory;
        //    }
        //    set
        //    {
        //        _ApplicationDirectory = value;
        //    }
        //}

        //public void LoadDataBase()
        //{
        //    if (ApplicationDirectory == null && HttpContext != null)
        //    {
        //        string path = HttpContext.Server.MapPath("~/");
        //        AutoLogger.ApplicationDirectory = ApplicationDirectory = path;
        //        DataBaseHelper.LoadDataBase(path);
        //    }
        //}

        //public ActionResult ConnectToDB()
        //{
        //    try
        //    {
        //        AutoLogger.MemoryLog = "";
        //        string path = HttpContext.Server.MapPath("~/");
        //        AutoLogger.MemoryLog += path + System.Environment.NewLine;
        //        AutoLogger.ApplicationDirectory = ApplicationDirectory = path;
        //        //ApplicationDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        //        // @"D:\irdms-src\irandmstest\newPRG\FrameSoftWeb\Library";
        //        var db = new Gita.DataBase.RDMS.SQLiteDataBase(System.IO.Path.Combine(ApplicationDirectory, "DataBase", "AmarDB.db"));
        //        AutoLogger.MemoryLog += (db == null) + System.Environment.NewLine;
        //        Gita.DataBase.RDMS.SQLiteDataBase.AddDataBaseKey("AmarDataBase", db);
        //        return Content("OK");
        //    }
        //    catch (Exception e)
        //    {
        //        AutoLogger.LogError(e, "Insert");
        //        return Content("Exception: " + e.Message);
        //    }
        //}

        //
        // GET: /AmarGiri/

        public ActionResult Insert()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("ApplicationNameAttrib") || !allkeys.Contains("ApplicationVersionAttrib") || !allkeys.Contains("OSNameAttrib") || !allkeys.Contains("OSVersionAttrib"))
                {
                    return Content("Not Found");
                }
                var headers = this.Request.Headers;
                Engine.Amar.FramesoftServiceProvider.InsertAmar(ControllerHelper.GetIPAddress(), headers["ApplicationNameAttrib"], headers["ApplicationVersionAttrib"],
                     headers["OSNameAttrib"], headers["OSVersionAttrib"], Guid.Parse(headers["ApplicationGuid"]));
                //var ipID = AmarContext.GetAndUpdateIPIDByIP(ControllerHelper.GetIPAddress());
                //var guidID = AmarContext.AddUpdateApplicationDetails(ipID, headers["ApplicationNameAttrib"], headers["ApplicationVersionAttrib"],
                //     headers["OSNameAttrib"], headers["OSVersionAttrib"], Guid.Parse(headers["ApplicationGuid"]));
                //AmarContext.AddAmar(guidID, ipID);
                //amar.Application = ;//Agrin Android
                //amar.ApplicationVersion = ;//1.4
                //amar.OSName = ;//Android
                //amar.OSVersion = ;//4.3
                //amar.ApplicationGuid = ;//Application Guid
                return Content("I Af");// "Error To Insert Database"    
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/AmarGiri/Insert");
#endif
            }
            catch (Exception e)
            {
                Exception ee = Engine.Amar.FramesoftServiceProvider.AddErrorLog(e);
                return Content("Exception: " + e.Message);
            }
        }

        public ActionResult GetAmarMessage()
        {
            return Content("wtf");/// AmarGiriUpdater.CountAdded + " New Line " + AmarGiriUpdater.LastError);
        }

        public ActionResult GetAmar()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var amarInfo = Engine.Amar.FramesoftServiceProvider.GetFullAmarInfo();// AmarGiriUpdater.GetFullAmarInfo();
                ViewData["OnlineCount"] = amarInfo.OnlineCount;
                ViewData["OnlineApplicationCount"] = amarInfo.OnlineApplicationCount;

                ViewData["TodayApplicationCount"] = amarInfo.TodayApplicationCount;
                ViewData["TodayCount"] = amarInfo.TodayCount;
                ViewData["TodayIPCount"] = amarInfo.TodayIPCount;
                ViewData["TodayInstallCount"] = amarInfo.TodayInstallCount;

                ViewData["YesterdayApplicationCount"] = amarInfo.YesterdayApplicationCount;
                ViewData["YesterdayCount"] = amarInfo.YesterdayCount;
                ViewData["YesterdayIPCount"] = amarInfo.YesterdayIPCount;
                ViewData["YesterdayInstallCount"] = amarInfo.YesterdayInstallCount;

                ViewData["LastWeekApplicationCount"] = amarInfo.LastWeekApplicationCount;
                ViewData["LastWeekCount"] = amarInfo.LastWeekCount;
                ViewData["LastWeekIPCount"] = amarInfo.LastWeekIPCount;
                ViewData["LastWeekInstallCount"] = amarInfo.LastWeekInstallCount;

                ViewData["LastMonthApplicationCount"] = amarInfo.LastMonthApplicationCount;
                ViewData["LastMonthCount"] = amarInfo.LastMonthCount;
                ViewData["LastMonthIPCount"] = amarInfo.LastMonthIPCount;
                ViewData["LastMonthInstallCount"] = amarInfo.LastMonthInstallCount;

                ViewData["TotalApplicationAndInstallCount"] = amarInfo.TotalApplicationAndInstallCount;
                ViewData["TotalCount"] = amarInfo.TotalCount;
                ViewData["TotalIPCount"] = amarInfo.TotalIPCount;

                return View();
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/AmarGiri/GetAmar");
#endif

            }
            catch (Exception e)
            {
                //AutoLogger.LogError(e, "GetAmar");
                // "Error To Insert Database";
                return Content("Exception: " + e.ToString());
            }
        }

        public ActionResult GetWindowsAmar()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                var amarInfo = Engine.Amar.FramesoftServiceProvider.GetWindowsAmarInfo();
                ViewData["OnlineCount"] = amarInfo.OnlineCount;
                ViewData["OnlineApplicationCount"] = amarInfo.OnlineApplicationCount;

                ViewData["TodayApplicationCount"] = amarInfo.TodayApplicationCount;
                ViewData["TodayCount"] = amarInfo.TodayCount;
                ViewData["TodayIPCount"] = amarInfo.TodayIPCount;
                ViewData["TodayInstallCount"] = amarInfo.TodayInstallCount;

                ViewData["YesterdayApplicationCount"] = amarInfo.YesterdayApplicationCount;
                ViewData["YesterdayCount"] = amarInfo.YesterdayCount;
                ViewData["YesterdayIPCount"] = amarInfo.YesterdayIPCount;
                ViewData["YesterdayInstallCount"] = amarInfo.YesterdayInstallCount;

                ViewData["LastWeekApplicationCount"] = amarInfo.LastWeekApplicationCount;
                ViewData["LastWeekCount"] = amarInfo.LastWeekCount;
                ViewData["LastWeekIPCount"] = amarInfo.LastWeekIPCount;
                ViewData["LastWeekInstallCount"] = amarInfo.LastWeekInstallCount;

                ViewData["LastMonthApplicationCount"] = amarInfo.LastMonthApplicationCount;
                ViewData["LastMonthCount"] = amarInfo.LastMonthCount;
                ViewData["LastMonthIPCount"] = amarInfo.LastMonthIPCount;
                ViewData["LastMonthInstallCount"] = amarInfo.LastMonthInstallCount;

                ViewData["TotalApplicationAndInstallCount"] = amarInfo.TotalApplicationAndInstallCount;
                ViewData["TotalCount"] = amarInfo.TotalCount;
                ViewData["TotalIPCount"] = amarInfo.TotalIPCount;

                return View("GetAmar");
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/AmarGiri/GetWindowsAmar");
#endif
            }
            catch (Exception e)
            {
                return Content("Exception: " + e.Message);// "Error To Insert Database";
            }
        }


    }
}
