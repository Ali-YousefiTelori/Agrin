using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows;

namespace UnitTestAgrin.Link.FaceBook
{
    public class JTest
    {
        public string hd_src { get; set; }
        public string sd_src { get; set; }
        public string sd_src_no_ratelimit { get; set; }
        public string hd_src_no_ratelimit { get; set; }
    }

    [TestClass]
    public class FaceBook
    {
        [TestMethod]
        public void FaceBookVideoExtractor()
        {
            try
            {
                //string page = "https://www.facebook.com/groups/240499956136374/permalink/408360322683669/";

                //HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(page);
                //_request.AllowAutoRedirect = true;
                //_request.KeepAlive = true;
                //_request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                //_request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                //_request.Proxy = WebProxy.GetDefaultProxy();
                //HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                //TextReader reader = new StreamReader(response.GetResponseStream());
                //var text = reader.ReadToEnd();
                var text = Clipboard.GetText();
                Regex regExp = new Regex("\"videoData\":(.*?)]", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
                var match = regExp.Match(text);

                List<string> retText = new List<string>();
                while (match.Length > 0)
                {
                    string str = match.Groups[1].Value + "]";
                    foreach (var item in Newtonsoft.Json.JsonConvert.DeserializeObject<JTest[]>(str))
                    {
                        if (!string.IsNullOrEmpty(item.hd_src))
                            retText.Add(item.hd_src);
                        if (!string.IsNullOrEmpty(item.hd_src_no_ratelimit))
                            retText.Add(item.hd_src_no_ratelimit);
                        if (!string.IsNullOrEmpty(item.sd_src))
                            retText.Add(item.sd_src);
                        if (!string.IsNullOrEmpty(item.sd_src_no_ratelimit))
                            retText.Add(item.sd_src_no_ratelimit);
                    }
                    match = match.NextMatch();
                }
            }
            catch
            {

            }

        }
    }
}
