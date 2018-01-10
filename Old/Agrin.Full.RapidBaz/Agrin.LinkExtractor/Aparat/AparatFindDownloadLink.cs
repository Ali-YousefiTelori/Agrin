using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.LinkExtractor.Aparat
{
    public class AparatLinkInfo
    {
        public string Title { get; set; }
        public string Link { get; set; }
    }

    public static class AparatFindDownloadLink
    {
        public static bool IsAparatLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Host.ToLower() == "www.aparat.com" || uri.Host.ToLower() == "aparat.com")
                    return true;
            }
            else
                return false;
            return false;
        }

        public static AparatLinkInfo FindLinkFromSite(string link, IWebProxy proxy)
        {
            var _request = (HttpWebRequest)WebRequest.Create(link);
            _request.Timeout = 60000;
            _request.AllowAutoRedirect = true;
            _request.Proxy = proxy;
            _request.ServicePoint.ConnectionLimit = int.MaxValue;

            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

            using(var reader = new System.IO.StreamReader(_request.GetResponse().GetResponseStream()))
            {
                var data = reader.ReadToEnd();
                var linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "embedBtn", "qrCode");
                
                var title = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "DC.Title", "/>",false);
                title = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(title, "content=", "", false).Trim(new char[]{'"'});
                return new AparatLinkInfo() { Title = title, Link = Agrin.IO.Strings.HtmlPage.ExtractFirstLinkFromHtml(linkString, 1) };
            }
        }
    }
}
