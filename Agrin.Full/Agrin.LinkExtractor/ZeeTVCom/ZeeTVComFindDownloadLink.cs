using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.LinkExtractor.ZeeTVCom
{
    public static class ZeeTVComFindDownloadLink
    {
        public static bool IsZeeTVLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Host.ToLower() == "www.zeetv.com" || uri.Host.ToLower() == "zeetv.com")
                    return true;
            }
            else
                return false;
            return false;
        }

        public static string FindLinkFromSite(string link, IWebProxy proxy)
        {
            var _request = (HttpWebRequest)WebRequest.Create(link);
            _request.Timeout = 60000;
            _request.AllowAutoRedirect = true;
            _request.Proxy = proxy;
            //_request.ServicePoint.ConnectionLimit = int.MaxValue;

            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

            using (var reader = new System.IO.StreamReader(_request.GetResponse().GetResponseStream()))
            {
                var data = reader.ReadToEnd();
                var linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "bigmumbai", "';",false);
                linkString = linkString.Trim().TrimStart(new char[] { '=' }).Trim().TrimStart(new char[] { '\'' });
                return linkString;
            }
        }
    }
}
