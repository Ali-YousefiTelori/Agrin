using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.LinkExtractor.Aparat
{
    public class AparatQualityInfo
    {
        public string Link { get; set; }
        public string Quality { get; set; }
    }

    public class AparatLinkInfo
    {
        public AparatLinkInfo()
        {
            Qualities = new List<AparatQualityInfo>();
        }

        public string Title { get; set; }
        public List<AparatQualityInfo> Qualities { get; set; }
    }

    public static class AparatFindDownloadLink
    {
        public static bool IsAparatLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Uri uri = null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                return false;
            if (uri.Host.ToLower() == "www.aparat.com" || uri.Host.ToLower() == "aparat.com")
                return true;
            else
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

            using (var reader = new System.IO.StreamReader(_request.GetResponse().GetResponseStream()))
            {
                var data = reader.ReadToEnd();
                var linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "setVideoVisit", "</div>");
                linkString = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(linkString, ">", "</ul>");
                var title = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(data, "DC.Title", "/>", false);
                title = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(title, "content=\"", "\"", false).Trim(new char[] { '"' });

                var items = Agrin.IO.Strings.Text.GetListTextBetweenTwoValue(linkString, "<a", "</li>");
                return new AparatLinkInfo() { Title = title, Qualities = GetItems(items).ToList() };// Link = Agrin.IO.Strings.HtmlPage.ExtractFirstLinkFromHtml(linkString, 1)
            }
        }

        static IEnumerable<AparatQualityInfo> GetItems(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                AparatQualityInfo quality = new AparatQualityInfo();
                quality.Link = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(item, "href=\"", "\"").Trim();
                quality.Quality = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(item, "class=\"label\">", "</span>").Trim();
                yield return quality;
            }
        }

        public static AparatQualityInfo GetQualityByText(IEnumerable<AparatQualityInfo> items, string qualityText)
        {
            foreach (var item in items)
            {
                if (item.Quality == qualityText)
                    return item;
            }
            return null;
        }
    }
}
