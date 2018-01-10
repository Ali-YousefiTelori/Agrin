using FrameSoftWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers
{
    public class DesignController : Controller
    {
        public ActionResult GetIconByFileExtention(string fileExtention)
        {
            try
            {
                fileExtention = fileExtention.Trim(new char[] { '.' });
                string path = this.HttpContext.Server.MapPath("~/Design/ExtensionIcons/");
                string fileName = System.IO.Path.Combine(path, fileExtention) + ".png";
                byte[] icon = null;
                if (System.IO.File.Exists(fileName))
                {
                    icon = System.IO.File.ReadAllBytes(fileName);
                }

                string noAdd = System.IO.Path.Combine(path, "notAded.txt");
                if (icon == null)
                {
                    List<string> lines = new List<string>();
                    if (System.IO.File.Exists(noAdd))
                        lines = System.IO.File.ReadAllLines(noAdd).ToList();
                    if (!lines.Contains(fileExtention))
                    {
                        lines.Add(fileExtention);
                        System.IO.File.WriteAllLines(noAdd, lines.ToArray());
                    }
                }
                return Content(this.SetResultTextData(SerializationData.EncryptObject(icon)));
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }


        public ActionResult Images(string folder, string fileName)
        {
            try
            {
                string path = this.HttpContext.Server.MapPath("~/Resources/Agrin/Design/");
                string address = System.IO.Path.Combine(path, folder, fileName);
                if (!System.IO.File.Exists(address))
                {
                    this.HttpContext.Response.StatusCode = 404;
                    return Content(this.SetResultTextValue("Not Found"));
                }
                return base.File(address, "image/jpeg");
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        //
        // GET: /Design/

        //public ActionResult Index()
        //{
        //    return View();
        //}

    }
}
