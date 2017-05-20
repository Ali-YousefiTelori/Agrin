using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace FrameSoftWeb.Helper
{
    public static class ControllerHelper
    {
        public static string SetResultTextValue(this Controller controller, string text)
        {
            controller.HttpContext.Response.AddHeader("Content-Length", text.Length.ToString());
            controller.HttpContext.Response.AddHeader("Message", "1");
            return text;
        }

        public static string SetResultTextData(this Controller controller, string text)
        {
            controller.HttpContext.Response.AddHeader("Content-Length", text.Length.ToString());
            controller.HttpContext.Response.AddHeader("Message", "0");
            return text;
        }

        public static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
