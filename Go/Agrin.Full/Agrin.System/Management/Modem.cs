using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.OS.Management
{
    public static class Modem
    {
        public static void CheckInternetAndResetModem()
        {
            if (!CheckForInternetConnection())
                ResetModem();
        }

        static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        static void ResetModem()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://192.168.1.1/rebootinfo.cgi");
                request.Method = "GET";
                request.Referer = @"http://192.168.1.1/wancfg.cmd";
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.3; WOW64; rv:27.0) Gecko/20100101 Firefox/27.0";
                request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.KeepAlive = true;
                request.Credentials = new NetworkCredential("admin", "admin");
                //var requestBody = Encoding.UTF8.GetBytes("Reboot = Reboot");
                request.Host = "192.168.1.1";

                request.Headers["Authorization"] = "Basic YWRtaW46YWRtaW4=";
                //request.Headers["Origin"] = @"http://192.168.0.1";
                request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.Headers["Accept-Language"] = "en-US";
                //var requestBody = Encoding.UTF8.GetBytes("Reboot = Reboot");
                //using (var requestStream = request.GetRequestStream())
                //{
                //    requestStream.Write(requestBody, 0, requestBody.Length);
                //}
                string output = string.Empty;
                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                    {
                        output = stream.ReadToEnd();
                    }
                }
            }
            catch
            {
            }
        }
    }
}
