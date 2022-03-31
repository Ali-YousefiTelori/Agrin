using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        static string State = "";
        static string ErrorMessage = "";
        public ActionResult RestoreDataBaseNow()
        {
            try
            {
                return Content("Restore OK");
            }
            catch (Exception e)
            {
                ErrorMessage = "2 State " + State + " Error: " + e.Message + " stack " + (e.StackTrace == null ? "" : e.StackTrace);
                return Content(ErrorMessage);
            }
        }
    }
}
