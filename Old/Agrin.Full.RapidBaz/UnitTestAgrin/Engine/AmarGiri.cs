using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Net;
using Agrin.Download.Data.Settings;
using System.IO;

namespace UnitTestAgrin.Engine
{
    [TestClass]
    public class AmarGiri
    {
        [TestMethod]
        public void AmarTest()
        {
            AmarGiriFrameSoft();
        }

        void AmarGiriFrameSoft()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://framesoft.ir/amargiri/insert");
            request.Headers["ApplicationNameAttrib"] = "ApplicationSetting.Current.ApplicationOSSetting.Application";
            request.Headers["ApplicationVersionAttrib"] = "ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion";
            request.Headers["OSNameAttrib"] = "ApplicationSetting.Current.ApplicationOSSetting.OSName";
            request.Headers["OSVersionAttrib"] = "ApplicationSetting.Current.ApplicationOSSetting.OSVersion";
            request.Headers["ApplicationGuid"] = Guid.NewGuid().ToString();
            var response = request.GetResponse().GetResponseStream();
            StreamReader streamReader = new StreamReader(response, true);
            try
            {
                var target = streamReader.ReadToEnd();
            }
            finally
            {
                streamReader.Close();
            }
        }
        static string GetedAmarAddress = "";
        public static void AmarGiriTest()
        {
            while (true)
            {
                try
                {
                    string html = GetedAmarAddress;
                    if (string.IsNullOrEmpty(html))
                        html = new System.Net.WebClient().DownloadString("http://framesoft.ir/page/agrinmobileappamargiri");
                    if (html.Contains("</CSharpCode>"))
                        GetedAmarAddress = Agrin.Data.Code.CodeLuncher.GetCode(html).Trim();

                    if (!string.IsNullOrEmpty(GetedAmarAddress))
                        new System.Net.WebClient().DownloadString(GetedAmarAddress);
                }
                catch
                {

                }

                Thread.Sleep(new TimeSpan(0, 15, 0));
            }
        }
    }
}
