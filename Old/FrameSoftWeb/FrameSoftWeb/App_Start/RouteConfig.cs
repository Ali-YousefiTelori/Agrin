using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FrameSoftWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
"DownloadOneFile",
"Download/DownloadOneFile/{fileName}",
new { controller = "Download", action = "DownloadOneFile" }
);

            routes.MapRoute(
"DownloadOneFileForUser",
"UserManager/DownloadOneFileForUser/{fileGuid}",
new { controller = "UserManager", action = "DownloadOneFileForUser" }
);
            routes.MapRoute(
"GetIpProperties",
"Client/GetIpProperties/{ipOrDomain}",
new { controller = "Client", action = "GetIpProperties" }
);
            routes.MapRoute(
"GetFlagByCountryCode",
"Client/GetFlagByCountryCode/{code}",
new { controller = "Client", action = "GetFlagByCountryCode" }
);
            routes.MapRoute(
"GetIconByFileExtention",
"Design/GetIconByFileExtention/{fileExtention}",
new { controller = "Design", action = "GetIconByFileExtention" }
);
            routes.MapRoute(
"Images",
"Design/Images/{folder}",
new { controller = "Design", action = "Images" }
);
            routes.MapRoute(
"AndroidPro",
"Learning/AndroidPro/{fileName}",
new { controller = "Learning", action = "AndroidPro" }
);

            //هرچی میخوای اضافه کنی قبل از این خط اضافه کن وگرنه کار نمی کنه
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


            //    routes.MapRoute("AgrinUpdate",
            //"Agrin/Update/{controller}/{action}/{id}",
            //new { controller = "Agrin/Update/GetAndroidUpdate", action = "Index", id = "" }, new string[] { "FrameSoftWeb.Controllers.Agrin.Update" });
        }
    }
}