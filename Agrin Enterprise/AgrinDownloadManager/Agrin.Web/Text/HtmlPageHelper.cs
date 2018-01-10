using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrin.Web.Text
{
    public static class HtmlPage
    {
        static List<string> SortLinkByHttp(List<string> links)
        {
            return links.OrderByDescending<string, bool>(x => x.ToLower().Contains("http")).ToList();
        }

        public static List<string> ExtractLinksFromHtml(string html)
        {
            try
            {
                Regex linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string rawString = html;
                List<string> links = new List<string>();
                foreach (Match m in linkParser.Matches(rawString))
                    links.Add(m.Value);
                return SortLinkByHttp(links);
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "ExtractLinksFromHtml");
                return new List<string>() { html };
            }
        }

        public static List<string> ExtractLinksFromHtmlTwo(string html)
        {
            try
            {
                html = Decodings.UrlDecode(html);
                Regex r;
                Match m;
                html = Regex.Replace(html, "http|ftp|https", new MatchEvaluator((match) =>
                {
                    return System.Environment.NewLine + match.Value;
                }), RegexOptions.Compiled);
                //html = ;
                r = new Regex(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^()\[\]{}=%&amp; :/~\+#]*[\w\-\@?^=%&amp; /~\+#])",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                List<string> str = new List<string>();

                for (m = r.Match(html); m.Success; m = m.NextMatch())
                {
                    string txt = m.Groups[0].Value;
                    if (!str.Contains(txt))
                        str.Add(txt);
                }
                return SortLinkByHttp(str);
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, "ExtractLinksFromHtmlTwo");
                return new List<string>() { };
            }
        }

        public static string ExtractFirstLinkFromHtml(string html, int algoritm = 0)
        {
            if (algoritm == 0)
                return ExtractLinksFromHtml(html).FirstOrDefault();
            else
                return ExtractLinksFromHtmlTwo(html).FirstOrDefault();
        }

        public static string ExtractOneLinkFromHtml(string html, int algoritm = 0)
        {
            string value = null;
            if (algoritm == 0)
            {
                value = ExtractLinksFromHtml(html).FirstOrDefault();
            }
            else
            {
                value = ExtractLinksFromHtmlTwo(html).FirstOrDefault();
            }
            if (value == null)
                return html;
            else
                return value;
        }
    }
}
