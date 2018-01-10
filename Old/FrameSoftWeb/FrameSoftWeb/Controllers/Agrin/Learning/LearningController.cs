using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers.Agrin.Learning
{
    public class LearningController : Controller
    {
        //
        // GET: /Learning/

        public ActionResult Android()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                string html = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Resources/Agrin/Learning/Android.html"));
                return Content(html);
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Learning/Android");
#endif
            }
            catch (Exception e)
            {
                return Content("Error: " + e.Message);
            }
        }

        public ActionResult AndroidPro(string fileName)
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                string html = null;
                if (string.IsNullOrEmpty(fileName))
                    html = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Resources/Agrin/Learning/AndroidPro.html"));
                else
                    html = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Resources/Agrin/Learning/" + fileName));

                return Content(html);
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Learning/AndroidPro");
#endif
            }
            catch (Exception e)
            {
                return Content("Error: " + e.Message);
            }
        }

        public ActionResult Android2()
        {
            try
            {
#if (FrameAppVPS || DEBUG)
                string html = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Resources/Agrin/Learning/Android2.html"));
                return Content(html);
#elif (FrameSoftIR)
                return Redirect("http://www.frameapp.ir/Learning/Android2");
#endif
            }
            catch (Exception e)
            {
                return Content("Error: " + e.Message);
            }
        }

        public ActionResult ImageAndroid(string id)
        {
            var dir = Server.MapPath("~/Resources/Agrin/Learning/Images/Android");
            var path = Path.Combine(dir, id + ".jpg");
            return base.File(path, "image/jpeg");
        }
    }
}
