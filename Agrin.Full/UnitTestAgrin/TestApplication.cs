using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using Agrin.IO.File;
using System.Net;
using Agrin.Download.Manager;
using Agrin.Download.Data;
using Agrin.LinkExtractor;
using System.IO;

namespace UnitTestAgrin
{
    public class Person
    {
        public string Name { get; set; }
        public string Value { get; set; }

    }
    [TestClass]
    public class TestApplication
    {
        [TestMethod]
        public void SortByHttp()
        {
            List<string> links = new List<string>();
            links.Add("www.ali.com");
            links.Add("www.reza.com");
            links.Add("http://alsdvsdvi.com");
            links.Add("http://www.tttt.com");
            links.Add("www.ttttt.com");
            links.Add("www.bbbbbb.com");
            var sort = links.OrderByDescending<string, bool>(x => x.ToLower().Contains("http")).ToList();
        
        }
        [TestMethod]
        public void GetNextWeekday()
        {
            DateTime start = DateTime.Now;
            List<DayOfWeek> days = new List<DayOfWeek>() { DayOfWeek.Tuesday, DayOfWeek.Saturday, DayOfWeek.Monday, DayOfWeek.Friday };
            var today = start.DayOfWeek;
            int daysToAdd = 7;
            foreach (var day in days)
            {
                var d = ((int)day - (int)start.DayOfWeek + 7) % 7;
                if (d <= daysToAdd)
                    daysToAdd = d;
            }

            var dt = start.AddDays(daysToAdd);
            var time = dt - DateTime.Now;
            var tday = DateTime.Now.AddHours(5).AddMinutes(50).TimeOfDay;
            time = time.Add(tday);
            var newD = dt.DayOfWeek;
            List<DateTime> dts = new List<DateTime>() { new DateTime(2014, 5, 3), new DateTime(2014, 5, 4), new DateTime(2014, 5, 1), new DateTime(2014, 4, 1), new DateTime(2014, 10, 1) };
            dts.Sort();
        }

        [TestMethod]
        public void TestHttp()
        {
            try
            {
                //string alieeeee = "";

                //var item = GetFormat("http://r6---sn-ab5l6n7e.googlevideo.com/videoplayback?source=youtube&ipbits=0&ip=162.223.94.95&mime=video/mp4&expire=1419267159&itag=22&upn=vwLvZUsbRV8&id=o-AH63dch-j7zqh7_CjiaN4Sj5MkwwfqN9si-bdSR6qFJ1&key=yt5&fexp=900718,905639,913438,927622,931351,932404,9405768,9406077,941004,943917,945084,947209,947218,948124,952302,952605,952901,955301,957103,957105,957201&sparams=dur,id,initcwndbps,ip,ipbits,itag,mime,mm,ms,mv,ratebypass,source,upn,expire&dur=185.945&mm=31&ratebypass=yes&initcwndbps=1292500&sver=3&signature=82634D76EF850CA1F5B85BA63FBBD7DA973D66B2.6F0B7F5C4B339B5E149F656DED4368D1BABFD180&ms=au&mv=m&mt=1419245540");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3| (SecurityProtocolType)3072;
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create("https://my.mci.ir/app/MyMCI.apk");
                //_request.AllowAutoRedirect = true;
                //_request.KeepAlive = true;Accept-Language: 
                _request.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
                _request.Headers.Add("Accept-Language", "en-US,en;q=0.7,fa;q=0.3");
                _request.Headers.Add("Accept-Encoding", "gzip, deflate");
                _request.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                //_request.Proxy = null; //WebProxy.GetDefaultProxy();
                //_request.AddRange(10);
                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                //var alia = response.ResponseUri.ToString();
                //bool normal = Agrin.LinkExtractor.DownloadUrlResolver.TryNormalizeYoutubeUrl("https://www.youtube.com/watch?v=AUgFOpESl1Y", out alieeeee);
                //bool normal = Agrin.LinkExtractor.DownloadUrlResolver.TryNormalizeYoutubeUrl("http://r8---sn-hpjx-hn2e.googlevideo.com/videoplayback?signature=10AC6CEB43F4F1F66022C0B9EF049A0381259F57.BC34E61406D31744B8612FF6CE17ED0873A3F7D7&lmt=1379592595380036&sparams=clen,dur,gir,id,initcwndbps,ip,ipbits,itag,keepalive,lmt,mime,mm,ms,mv,source,upn,expire&sver=3&id=o-AAKXVRPvFwrZn-i1DUPp9wgpWP-oRwaafUdXHON0gCj3&initcwndbps=1407500&fexp=900245,900718,919149,927622,931357,932404,9405588,943917,947209,947218,948124,952302,952605,952901,955301,957103,957105,957201&key=yt5&itag=135&mime=video/mp4&ip=65.49.68.155&ipbits=0&keepalive=yes&dur=528.600&gir=yes&mt=1419276888&ms=au&clen=41421390&mm=31&source=youtube&mv=m&expire=1419298534&upn=hNhGHADAKro", out alieeeee);

                //var ali = DownloadUrlResolver.GetDownloadUrls("https://youtu.be/dUmzgkGFnIo").ToList();
                //var reza = "dad";
            }
            catch (WebException e)
            {
                try
                {
                    var reader = new StreamReader(e.Response.GetResponseStream());
                    var text = reader.ReadToEnd();
                    var memory = Agrin.IO.Helper.SerializeStream.Serialize(Agrin.Download.Data.Serializition.ExceptionSerializable.ExceptionToSerializable(e));
                    //var sf = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
                    //MemoryStream stream = new MemoryStream();
                    //sf.Serialize(stream, e);
                    //stream.Seek(0, SeekOrigin.Begin);
                    //sf.Deserialize(stream);
                    var qqqq = Agrin.IO.Helper.SerializeStream.Deserialize(memory);
                }
                catch (Exception aaa)
                {

                }
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Forbidden)
                    {

                    }
                }
            }
            catch (AggregateException c)
            {
                if (c.InnerExceptions != null && c.InnerExceptions.Count > 0)
                {
                    if (c.InnerExceptions[0].InnerException != null)
                    {
                        var qqq = c.InnerExceptions[0].InnerException.Message;
                    }
                }
                var q = c.Message;

            }
            catch //(Exception e)
            {

            }

        }

        [TestMethod]
        public void TestFileStream()
        {
            TestStream testS = new TestStream();
            int len = 1024 * 512;
            //testS.CreateFile(len);
            testS.ReadFile(len);
        }


        public System.Reflection.MethodBase QQQ()
        {
            return System.Reflection.MethodBase.GetCurrentMethod();
        }

        [TestMethod]
        public void TestYoutubeLinkDownloader()
        {
            try
            {
                Agrin.IO.Helper.MPath.InitializePath();
                ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
                ApplicationGroupManager.Current = new ApplicationGroupManager();
                ApplicationNotificationManager.Current = new ApplicationNotificationManager();
                ApplicationBalloonManager.Current = new ApplicationBalloonManager();

                DeSerializeData.LoadApplicationData();
                var info = new Agrin.Download.Web.LinkInfo("http://www.youtube.com/watch?v=Db7gcgwtuRA");
                ApplicationLinkInfoManager.Current.AddLinkInfo(info, ApplicationGroupManager.Current.NoGroup, false);
                info.Management.UserMustSelectItemAction = (list) =>
                    {
                        Agrin.Helper.ComponentModel.AsyncActions.Action(() =>
                            {
                                System.Threading.Thread.Sleep(5000);
                                info.Management.UserSelectedItemAction(list.First());
                            });
                    };
                info.Management.MultiProxy.Add(new Agrin.Download.Web.Link.ProxyInfo() { IsSystemProxy = true, IsSelected = true });
                ApplicationLinkInfoManager.Current.PlayLinkInfo(info);
                while (true)
                {
                    System.Threading.Thread.Sleep(5000);
                }
            }
            catch //(Exception ve)
            {

            }
        }
    }
}
