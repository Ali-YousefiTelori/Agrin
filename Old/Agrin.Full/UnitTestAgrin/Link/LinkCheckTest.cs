using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Agrin.LinkExtractor;
using Agrin.LinkExtractor.RadioJavan;
using Agrin.IO.Strings;
using YoutubeExtractor;

namespace UnitTestAgrin.Link
{
    [TestClass]
    public class LinkCheckTest
    {
        [TestMethod]
        public void CheckRangeLink()
        {
            //var check = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(new Uri("http://tinyez.tv/dl/t/The.Wolverine.2013.EXTENDED.720p.YIFY.mp4?hash=8b076709f7d919d9d275ad9335c7cc34_187730_8997&s=2"));
        }

        [TestMethod]
        public void TestRequestHttp()
        {
            try
            {
                MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (getSyntax != null && flagsField != null)
                {
                    foreach (string scheme in new[] { "http", "https" })
                    {
                        UriParser parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                        if (parser != null)
                        {
                            int flagsValue = (int)flagsField.GetValue(parser);
                            // Clear the CanonicalizeAsFilePath attribute
                            if ((flagsValue & 0x1000000) != 0)
                                flagsField.SetValue(parser, flagsValue & ~0x1000000);
                        }
                    }
                }
                //"http://framesoft.s29.rapidbaz.com/premium/2ee59d16318af2a14520d37709e32128/554cb842/s29.us./697059/My.Moms.New.Boyfriend.2008.DVDSCR.XviD-VoMiT.part3.rar");
                var uri = new Uri(Decodings.UrlDecode("http://framesoft.dl.rapidbaz.com/I2qJ./%282%29+Dreamworld+-+Leo+Rojas+-+El+Condor+Pasa.mp4"));
                //string uri = ;
                // while (true)
                //{
                var _request = (HttpWebRequest)WebRequest.Create(uri);
                _request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)";
                //_request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //_request.ServicePoint.ConnectionLimit = 2;
                //_request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                //_request.Headers.Add("Accept-Encoding", "gzip, deflate");
                _request.Headers.Add("Authorization", "Basic YWdyaW46ZnJhbWVzb2Z0");
                _request.Accept = "*/*";


                //_request.KeepAlive = true;
                //_request.Headers.Add("Accept-Language: en-US,en;q=0.5");
                //_request.CookieContainer = new CookieContainer();
                //Uri target = new Uri(uri);
                //Cookie ck = new Cookie("session", "F4%2BaCBtu1CCT1LH7bZPiOoQomGHKajFJja%2BpWXHn6iU%3D") { Domain = target.Host };
                //_request.Headers.Add("Cookie:", "session=F4%2BaCBtu1CCT1LH7bZPiOoQomGHKajFJja%2BpWXHn6iU%3D");
                //_request.Headers.Add("Authorization", "Basic YWdyaW46ZnJhbWVzb2Z0");
                //_request.CookieContainer.Add(ck);
                //_request.Timeout = 60000;
                //_request.AllowAutoRedirect = false;
                //_request.ServicePoint.ConnectionLimit = int.MaxValue;
                //_request.CookieContainer = new CookieContainer();
                _request.AllowAutoRedirect = true;
                var response = _request.GetResponse();
                //uri = response.Headers["location"];

                //}


            }
            catch
            {

            }

        }
        [TestMethod]
        public void ExtractLinkReport()
        {
            Agrin.Download.Helper.LinkHelper.ExtractLinkReport("d:\\67.agn");
        }

        [TestMethod]
        public void TestLinkHttp()
        {
            try
            {
                // string link = "http://dl.asramusic125.org/persian%20full%20video/Yas/Yas%20-%20Az%20Chi%20Begam%20%28%20AsraMusic%20%29.mp4";
                //string link = "http://free2.iranfilmdl.net/dl/9d5fbfe7b98782d2754a10f4741495d6/5532946f/tohi/Movie/1392/10/Bezan.Bahador/Bezan.Bahador.2010_Iran-Film.rar";
                string link = "https://www.youtube.com/watch?v=vqAK8jKKI5A";
                var pp = Assembly.GetExecutingAssembly().Location;
                var videos = DownloadUrlResolver.GetDownloadUrls(link);
                //WebClient client = new WebClient();

                //client.Headers.Add("Range", "Range: bytes=500-999");
                //var stream1 = client.OpenRead("");

                //byte[] bytes3 = new byte[1024];
                //var readCount3 = stream1.Read(bytes3, 0, bytes3.Length);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(link);
                request.UserAgent = "com.google.android.youtube/10.14.56(Linux; U; Android 4.2.2; fa_IR_LNum; MediaPad X1 7.0 Build/HuaweiMediaPad) gzip";

                var response = request.GetResponse();
                var stream = response.GetResponseStream();


                byte[] bytes = new byte[1024];
                var readCount = stream.Read(bytes, 0, bytes.Length);

                var headers = response.Headers;

                //request = (HttpWebRequest)HttpWebRequest.Create(link);
                //request.AddRange(0);
                //response = request.GetResponse();
                //stream = response.GetResponseStream();
                //byte[] bytes2 = new byte[1024];
                //var readCount2 = stream.Read(bytes2, 0, bytes2.Length);

                //var headers2 = response.Headers;
            }
            catch
            {

            }

        }
        [TestMethod]
        public void TestRadioJavan()
        {
            try
            {
                var link = "https://m.radiojavan.com/mobile/mp3/Ali-Azimi-Ahange-Shoari"; //"https://www.radiojavan.com/mp3s/mp3/Magico-Mar";
                if (RadioJavanFindDownloadLink.IsRadioJavanLink(link))
                {
                    var find = RadioJavanFindDownloadLink.FindLinkFromSite(link, WebRequest.GetSystemWebProxy());
                }
            }
            catch// (Exception ex)
            {

            }
        }

    }


    public static class HttpWebRequestExtensions
    {
        static string[] RestrictedHeaders = new string[] {
            "Accept",
            "Connection",
            "Content-Length",
            "Content-Type",
            "Date",
            "Expect",
            "Host",
            "If-Modified-Since",
            "Keep-Alive",
            "Proxy-Connection",
            "Range",
            "Referer",
            "Transfer-Encoding",
            "User-Agent"
        };

        static Dictionary<string, PropertyInfo> HeaderProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        static HttpWebRequestExtensions()
        {
            Type type = typeof(HttpWebRequest);
            foreach (string header in RestrictedHeaders)
            {
                string propertyName = header.Replace("-", "");
                PropertyInfo headerProperty = type.GetProperty(propertyName);
                HeaderProperties[header] = headerProperty;
            }
        }

        public static void SetRawHeader(this HttpWebRequest request, string name, string value)
        {
            if (HeaderProperties.ContainsKey(name))
            {
                PropertyInfo property = HeaderProperties[name];
                if (property.PropertyType == typeof(DateTime))
                    property.SetValue(request, DateTime.Parse(value), null);
                else if (property.PropertyType == typeof(bool))
                    property.SetValue(request, Boolean.Parse(value), null);
                else if (property.PropertyType == typeof(long))
                    property.SetValue(request, Int64.Parse(value), null);
                else
                    property.SetValue(request, value, null);
            }
            else
            {
                request.Headers[name] = value;
            }
        }
    }
}
