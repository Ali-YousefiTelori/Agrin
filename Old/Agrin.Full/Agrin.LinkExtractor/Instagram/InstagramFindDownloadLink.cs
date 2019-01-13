using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.LinkExtractor.Instagram
{
    public class InstagramLinkInfo
    {
        public string Title { get; set; }
        public string Link { get; set; }
    }

    public static class InstagramFindDownloadLink
    {
        public static bool IsInstagramLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Host.ToLower() == "www.instagram.com" || uri.Host.ToLower().Contains("instagram.com"))
                    return true;
            }
            else
                return false;
            return false;
        }

        public static InstagramLinkInfo FindLinkFromSite(string link, IWebProxy proxy)
        {
            var _request = (HttpWebRequest)WebRequest.Create(link);
            _request.Timeout = 60000;
            _request.AllowAutoRedirect = true;
            _request.Proxy = proxy;
            _request.ServicePoint.ConnectionLimit = int.MaxValue;

            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-US");
            _request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";

            using (var reader = new System.IO.StreamReader(_request.GetResponse().GetResponseStream()))
            {
                var data = reader.ReadToEnd();
                if (Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "og:type\"", "/>", false).Contains("profile"))
                {
                    throw new Exception("توانایی دانلود از این آدرس وجود ندارد");
                }
                var linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data,  "og:video\"", "/>",false);
                if (string.IsNullOrEmpty(linkString))
                    linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "og:image\"", "/>", false);

                return new InstagramLinkInfo() { Link = Agrin.IO.Strings.HtmlPage.ExtractFirstLinkFromHtml(linkString, 1) };
            }
        }
    }
}
