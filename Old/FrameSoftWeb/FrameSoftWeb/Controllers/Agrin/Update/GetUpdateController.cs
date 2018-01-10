using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers.Agrin.Update
{
    public class GetUpdateController : Controller
    {
        //
        // GET: /GetUpdate/
        DateTime androidLastFileUpdate = DateTime.MinValue;
        public string AndroidLastUpdateFileContent { get; set; }

        public ActionResult Android()
        {
            try
            {
//#if (FrameAppVPS || DEBUG)
                string fileName = HttpContext.Server.MapPath("~/Resources/Agrin/Update/Android.txt");
                DateTime lastModified = System.IO.File.GetLastWriteTime(fileName);
                if (lastModified != androidLastFileUpdate)
                {
                    androidLastFileUpdate = lastModified;
                    AndroidLastUpdateFileContent = System.IO.File.ReadAllText(fileName);
                }
                return Content(AndroidLastUpdateFileContent);
//#elif (FrameSoftIR)
//                return Redirect("http://www.frameapp.ir/GetUpdate/Android");
//#endif

            }
            catch (Exception e)
            {
                return Content("Error: " + e.Message);
            }
        }

        public ActionResult Windows()
        {
            try
            {
//#if (FrameAppVPS || DEBUG)
                string html = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Resources/Agrin/Update/Windows.txt"));
                return Content(html);
//#elif (FrameSoftIR)
//                return Redirect("http://www.frameapp.ir/GetUpdate/Windows");
//#endif

            }
            catch (Exception e)
            {
                return Content("Error: " + e.Message);
            }
        }

        public ActionResult Xamarin()
        {
            try
            {
                string html = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Resources/Agrin/Update/Xamarin.txt"));
                return Content(html);
            }
            catch (Exception e)
            {
                return Content("Error: " + e.Message);
            }
        }
    }
}
