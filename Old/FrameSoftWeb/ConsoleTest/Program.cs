using Agrin.Framesoft;
using Agrin.Framesoft.Helper;
using Agrin.Framesoft.String;
using FrameSoft.Agrin.DataBase.Models;
using Gita.Infrastructure.UI.IO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ConsoleTest
{

    static class Program
    {

        //hex encoding of the hash, in uppercase.
        public static string Sha1Hash(this string str)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(str);
            data = Sha1Hash(data);
            return BitConverter.ToString(data).Replace("-", "");
        }
        // Do the actual hashing
        public static byte[] Sha1Hash(this byte[] data)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(data);
            }
        }

        public static string refreshToken = "";
        public static void CafeBazaarAuth(string querycode)
        {
            try
            {
                string param1 = querycode;
                var parameters = new StringBuilder();
                var nameValueCollection = new NameValueCollection() {
    { "grant_type", "authorization_code" },
                    { "code", param1 },
                     { "client_id", "Rgq6Dg6RoLYPVOExT1hSCFTVZ0ijFM54w1nf2JQg" },
                      { "client_secret", "JeIPSVD13LneB5lDvnPG9vh74yOhgGMnY2E42EVkbBNI7xlHeyRaqJjMZBX7" },
                       { "redirect_uri", "http://framesoft.ir/UserManager/CafeBazaarAuth" }
};
                foreach (string key in nameValueCollection.Keys)
                {
                    parameters.AppendFormat("{0}={1}&",
                        HttpUtility.UrlEncode(key),
                        HttpUtility.UrlEncode(nameValueCollection[key]));
                }

                parameters.Length -= 1;

                // Here we create the request and write the POST data to it.
                var request = (HttpWebRequest)HttpWebRequest.Create("https://pardakht.cafebazaar.ir/auth/token/");
                request.Method = "POST";
                request.AllowAutoRedirect = true;
                request.KeepAlive = true;
                request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                request.ContentType = "application/x-www-form-urlencoded";
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(parameters.ToString());
                }


                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var length = response.ContentLength;
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        dynamic text = Newtonsoft.Json.JsonConvert.DeserializeObject(stream.ReadToEnd());
                        refreshToken = text.refresh_token;
                    }
                }

            }
            catch (Exception e)
            {

            }
        }


        public static bool PurchaseValidate(UserPurchaseData userPurchaseData)
        {
            try
            {
                var parameters = new StringBuilder();
                var nameValueCollection = new NameValueCollection() {
    { "grant_type", "refresh_token" },
     { "client_id", "Rgq6Dg6RoLYPVOExT1hSCFTVZ0ijFM54w1nf2JQg" },
      { "client_secret", "JeIPSVD13LneB5lDvnPG9vh74yOhgGMnY2E42EVkbBNI7xlHeyRaqJjMZBX7" },
       { "refresh_token", refreshToken }
};
                foreach (string key in nameValueCollection.Keys)
                {
                    parameters.AppendFormat("{0}={1}&",
                        HttpUtility.UrlEncode(key),
                        HttpUtility.UrlEncode(nameValueCollection[key]));
                }

                parameters.Length -= 1;

                // Here we create the request and write the POST data to it.
                var request = (HttpWebRequest)HttpWebRequest.Create("https://pardakht.cafebazaar.ir/auth/token/");
                request.Method = "POST";
                request.AllowAutoRedirect = true;
                request.KeepAlive = true;
                request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                request.ContentType = "application/x-www-form-urlencoded";
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(parameters.ToString());
                }

                string accessCode = "";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var length = response.ContentLength;
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        dynamic text = Newtonsoft.Json.JsonConvert.DeserializeObject(stream.ReadToEnd());
                        accessCode = text.access_token;
                    }
                }

                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create("https://pardakht.cafebazaar.ir/api/validate/Agrin.MonoAndroid.UI/inapp/" + userPurchaseData.ProductId + "/purchases/" + userPurchaseData.PurchaseToken + "/?access_token=" + accessCode);//LoginUser
                _request.AllowAutoRedirect = true;
                _request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";

                using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                {
                    var length = response.ContentLength;
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        var text = stream.ReadToEnd();
                        if (text == "{}")
                            return false;
                    }
                }
            }
            catch
            {

            }
            return true;
        }

        public static string GetTextBetweenTwoValue(string content, string str1, string str2, bool singleLine = true)
        {
            RegexOptions pattern;
            if (singleLine)
            {
                pattern = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline;
            }
            else
            {
                pattern = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace;
            }
            Regex regex = new Regex(str1 + "(.*)" + str2, pattern);
            return regex.Match(content).Groups[1].ToString();
        }

        public static void CreateNewCode()
        {
            try
            {
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create("https://pardakht.cafebazaar.ir/auth/authorize/?response_type=code&access_type=offline&redirect_uri=http://framesoft.ir/UserManager/CafeBazaarAuth&client_id=Rgq6Dg6RoLYPVOExT1hSCFTVZ0ijFM54w1nf2JQg");//LoginUser
                _request.AllowAutoRedirect = true;
                _request.KeepAlive = true;
                _request.ContentType = "application/x-www-form-urlencoded";
                _request.Headers.Add("Accept-Language", "fa-IR");
                _request.Accept = "text/html, application/xhtml+xml, */*";
                _request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";

                string postdata = "";
                string referer = "";
                string setCookie = "";
                CookieCollection cooks = new CookieCollection();
                using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                {
                    var length = response.ContentLength;
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        referer = response.ResponseUri.OriginalString;
                        setCookie = response.Headers["Set-Cookie"];
                        var strings = setCookie.Split(new char[] { ';' }).ToList();
                        foreach (var item in strings.ToList())
                        {
                            if (!item.Contains("csrftoken") && !item.Contains("sessionid"))
                            {
                                strings.Remove(item);
                            }
                        }
                        string csrftoken = GetTextBetweenTwoValue(strings[0], "csrftoken=", "");
                        string sessionid = GetTextBetweenTwoValue(strings[1], "sessionid=", "");
                        cooks.Add(new Cookie("csrftoken", csrftoken) { Domain = _request.RequestUri.Host });
                        cooks.Add(new Cookie("sessionid", sessionid) { Domain = _request.RequestUri.Host });
                        var text = GetTextBetweenTwoValue(stream.ReadToEnd(), "<form", "submit");
                        var name1 = GetTextBetweenTwoValue(text, "name='", "value='").Trim().TrimEnd(new char[] { '\'' });
                        var value1 = GetTextBetweenTwoValue(text, "value='", "'");
                        var name2 = GetTextBetweenTwoValue(text, "name=\"", "value=\"").Trim().TrimEnd(new char[] { '\"' });
                        var value2 = GetTextBetweenTwoValue(text, "value=\"", "\"/>");

                        NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                        outgoingQueryString.Add(name1, value1);
                        outgoingQueryString.Add(name2, value2);
                        postdata = outgoingQueryString.ToString();
                    }
                }

                _request = (HttpWebRequest)WebRequest.Create("https://pardakht.cafebazaar.ir/auth/authorize/");//LoginUser
                _request.AllowAutoRedirect = true;
                _request.KeepAlive = true;
                _request.ContentType = "application/x-www-form-urlencoded";
                _request.Headers.Add("Accept-Language", "fa-IR");
                _request.Accept = "text/html, application/xhtml+xml, */*";
                _request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";

                _request.Method = "POST";
                _request.Referer = referer;
                _request.CookieContainer = new CookieContainer();
                _request.CookieContainer.Add(cooks);
                using (var reqStream = _request.GetRequestStream())
                {
                    var bytes = Encoding.ASCII.GetBytes(postdata);
                    reqStream.Write(bytes, 0, bytes.Length);
                    using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                    {
                        var length = response.ContentLength;
                        using (var stream = new StreamReader(response.GetResponseStream()))
                        {
                            var text = stream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        static void Main(string[] args)
        {
            try
            {
                ////CafeBazaarAuth("sXd2MOIGidNHQRJs5uyLYKuzrGtCAC");
                var data = DataSerializationHelper.GetRequestData<IpProperties>("http://framesoft.ir/client/GetIpProperties/codeload.github.com");
                return;
            }
            catch (Exception ex)
            {
                //byte[] buffer = new byte[999999];
                WebException wex = (WebException)ex;
                var s = wex.Response.GetResponseStream();
                string ss = "";
                int lastNum = 0;
                do
                {
                    lastNum = s.ReadByte();
                    ss += (char)lastNum;
                } while (lastNum != -1);
                s.Close();
                s = null;

                var ali = ss;
            }
        }
    }

    public class MessageInformation
    {
        public string GUID { get; set; }
        public string Message { get; set; }
        public int LastUserMessageID { get; set; }
    }
}
