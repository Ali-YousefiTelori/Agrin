using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrin.LinkExtractor.Facebook
{
    public class FacebookQualityInfo
    {
        public FaceBookVideoTypeEnum LinkType { get; set; }
        public string Link { get; set; }
    }

    public class FacebookModel
    {
        public string hd_src { get; set; }
        public string sd_src { get; set; }
        public string sd_src_no_ratelimit { get; set; }
        public string hd_src_no_ratelimit { get; set; }
    }

    public enum FaceBookVideoTypeEnum
    {
        HD = 0,
        HDRateLimit = 1,
        SD = 2,
        SDRateLimit = 3
    }

    public static class FacebookFindDownloadLink
    {
        public static bool IsFacebookLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Host.ToLower() == "www.facebook.com" || uri.Host.ToLower().Contains("facebook.com"))
                    return true;
            }
            else
                return false;
            return false;
        }

        public static List<FacebookQualityInfo> FindLinkFromSite(string link, IWebProxy proxy)
        {
            List<FacebookQualityInfo> items = new List<Facebook.FacebookQualityInfo>();
            //string videoId =Path.GetFileName( link.TrimEnd('/'));
            //link = "https://graph.facebook.com/" + videoId;
            var _request = (HttpWebRequest)WebRequest.Create(link);
            _request.Timeout = 60000;
            _request.AllowAutoRedirect = true;
            _request.Proxy = proxy;
            _request.ServicePoint.ConnectionLimit = int.MaxValue;

            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
            TextReader reader = new StreamReader(response.GetResponseStream());
            var text = reader.ReadToEnd();

            Regex regExp = new Regex("videoData(.*?)}]", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            //Regex regExp = new Regex(",videoData:(.*?)]", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            var match = regExp.Match(text);

            while (match.Length > 0)
            {
                string str = match.Groups[1].Value + "}]";
                str = str.TrimStart('"');
                str = str.TrimStart(':');
                foreach (var item in Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookModel[]>(str))
                {
                    if (!string.IsNullOrEmpty(item.hd_src))
                        items.Add(new FacebookQualityInfo() { Link = item.hd_src, LinkType = FaceBookVideoTypeEnum.HD });
                    if (!string.IsNullOrEmpty(item.hd_src_no_ratelimit))
                        items.Add(new FacebookQualityInfo() { Link = item.hd_src_no_ratelimit, LinkType = FaceBookVideoTypeEnum.HDRateLimit });
                    if (!string.IsNullOrEmpty(item.sd_src))
                        items.Add(new FacebookQualityInfo() { Link = item.sd_src, LinkType = FaceBookVideoTypeEnum.SD });
                    if (!string.IsNullOrEmpty(item.sd_src_no_ratelimit))
                        items.Add(new FacebookQualityInfo() { Link = item.sd_src_no_ratelimit, LinkType = FaceBookVideoTypeEnum.SDRateLimit });
                }
                break;
                match = match.NextMatch();
            }
            return items;
        }

        public static FacebookQualityInfo GetQualityByType(List<FacebookQualityInfo> items, int quality)
        {
            return (from x in items where (int)x.LinkType == quality select x).FirstOrDefault();
        }
    }
}
