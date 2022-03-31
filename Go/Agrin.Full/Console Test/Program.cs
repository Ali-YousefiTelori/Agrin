using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agrin.Framesoft.Helper;
using Agrin.Helper.Collections;
using System.Net;
using Agrin.LinkExtractor;
using System.Text.RegularExpressions;
using Agrin.Framesoft.String;
using System.IO;
using Agrin.Framesoft;
using System.Net.Sockets;

namespace Console_Test
{

    public static class SerializationData
    {

        public static string EncryptObject(object obj)
        {
            return EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        public static T DecryptObject<T>(string text)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(DecryptString(text));
        }

        public static string EncryptString(string json)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i]++;
            }
            var base64String = Convert.ToBase64String(bytes);
            var encode = StringEncoding.Base64Encode(base64String);
            return encode;
        }

        public static string DecryptString(string data)
        {
            var decode = StringEncoding.Base64Decode(data);
            var bytes = Convert.FromBase64String(decode);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i]--;
            }
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return json;
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
    }

    class Program
    {
        // static FastCollection<DateTime> sortTest = new FastCollection<DateTime>(null);
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    //var file = @"D:\BaseProjects\Agrin Download Manager\Agrin.Full\Agrin.Windows.UI\bin\Debug\";
                    //using (var stream = new FileStream(file+ "Error Logs.log", FileMode.Open, FileAccess.ReadWrite))
                    //{
                    //    stream.Seek(stream.Length - (1024 * 1024), SeekOrigin.Current);
                    //    byte[] bytes = new byte[1024 * 1024];

                    //    var readCount = stream.Read(bytes, 0, bytes.Length);
                    //    File.WriteAllBytes(file+ "Error LogsOK.log", bytes);
                    //}
                    //Func<string, string, bool, string[]> GetCredentialCache = (user, pass, isUTF8) =>
                    //{
                    //    string encoded = "";
                    //    if (isUTF8)
                    //        encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user + ":" + pass));
                    //    else
                    //        encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass));

                    //    //string testDecode = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(encoded));

                    //    return new string[] { "Authorization", "Basic " + encoded };
                    //};

                    //string username = "Milad";
                    //string password = "12345678";
                    //string[] auth = null;
                    //if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                    //{
                    //    auth = GetCredentialCache(username, password, true);
                    //}
                    //else
                    //{
                    //    auth = GetCredentialCache(username, password, true);
                    //}

                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Connect("dl.4-player.ir", 80);
                    var text = @"GET http://dl.4-player.ir/Stick_Fight_The_Game_v1.2.03_setup.exe HTTP/1.1
User-Agent: Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko
Range: bytes=19463832-20463833
Host: dl.4-player.ir
Connection: Keep-Alive

";
                    var bytes = Encoding.ASCII.GetBytes(text);
                    var stream = tcpClient.GetStream();
                    stream.Write(bytes, 0, bytes.Length);
                    bytes = new byte[1024];
                    while (true)
                    {
                        var readCount = stream.Read(bytes, 0, bytes.Length);
                        text = Encoding.ASCII.GetString(bytes, 0, readCount);
                    }
                    //HttpWebRequest _request = (HttpWebRequest)WebRequest.Create("http://dl.4-player.ir/Stick_Fight_The_Game_v1.2.03_setup.exe");
                    //_request.AllowAutoRedirect = true;
                    //_request.KeepAlive = true;
                    // _request.Headers.Add("Accept-Language", "en-US");
                    //_request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
                    //_request.AddRange(19463833, 20463833);
                    //_request.Headers.Add(auth[0], auth[1]);
                    //_request.CookieContainer = new CookieContainer();
                    //((HttpWebRequest)_request).CookieContainer = new CookieContainer();
                    //(((HttpWebRequest)_request).CookieContainer).Add(new Cookie("allow", "yes", "", _request.RequestUri.Host));
                    //_request.Credentials = new NetworkCredential(username, password);
                    //using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                    //{
                    //    long length = response.ContentLength;
                    //    var bytes = new byte[1024];
                    //    var readCount = response.GetResponseStream().Read(bytes, 0, bytes.Length);

                    //}
                }
                catch (WebException ex)
                {
                    var response = ex.Response;
                }
                catch (Exception ex)
                {

                }
                //var p = System.Diagnostics.Process.GetProcessesByName("Agrin.Windows.UI");
                //p[0].Kill();
                //var mil = new TimeSpan(0, 1, 0).Ticks / TimeSpan.TicksPerMillisecond;
                //Agrin.Download.Helper.LinkHelper.ExtractLinkReport("g:\\ReportLink.agn");

                //var msg = Agrin.Framesoft.Helper.FeedBackHelper.GetUserMessageReplays(new DateTime(635857293284970000), new Guid("198b6ed3-260b-4b33-a660-8a5f4ce9372a"));
                //Console.WriteLine(msg == null ? "null" : msg.Count.ToString());
                //Console.ReadKey();
                //string uri = "http://framesoft.ir/GetUpdate/Android";
                ////string uri = "http://www.framesoft.ir/GetUpdate/Android";
                ////string uri = "http://frameapp.ir/GetUpdate/Android";
                //using (System.Net.WebClient client = new System.Net.WebClient())
                //{
                //    client.Headers.Add("Language", null);
                //    client.Headers.Add("AppVersion", "1.8.8.3");
                //    client.Headers.Add("Guid", "d28f9ba0-e3a1-4a82-9cc5-4fd71693f932");

                //    string jsonString = client.DownloadString(uri);

                //}

                //sortTest.Add(new DateTime(2014, 05, 06));
                //sortTest.Add(new DateTime(2014, 05, 07));
                //sortTest.Add(new DateTime(2014, 05, 08));
                //sortTest.Add(new DateTime(2014, 05, 09));
                //sortTest.Add(new DateTime(2014, 05, 10));

                //sortTest.SortBy<DateTime>(x => x);
                //var items = sortTest.ToList();
                //sortTest.SortByAscending<DateTime>(x => x);
                //items = sortTest.ToList();

                //var link = "https://youtu.be/FrG4TEcSuRg";
                //var guid = Guid.NewGuid();
                //System.Net.ServicePointManager.Expect100Continue = false;
                //var loginData = Agrin.Framesoft.Helper.UserManagerHelper.LoginUser(new Agrin.Framesoft.UserInfoData() { UserName = "ali yousefi", Password = "".Sha1Hash(), ApplicationGuid = guid });
                //var linkInfoes1 = DownloadUrlResolver.GetDownloadUrls("https://youtu.be/uz30vx2zxZc").ToList();
                //string text = SerializationData.EncryptObject(linkInfoes1);
                //DataSerializationHelper.ShowStackTrace = true;
                //var list = Agrin.Framesoft.Helper.UserManagerHelper.GetYoutubeVideoList(new Agrin.Framesoft.UserFileInfoData() { UserName = "ali yousefi", Password = "".Sha1Hash(), Link = "http://youtu.be/fSeKW_yloKM" });

                //var link = "https://www.youtube.com/watch?v=jhyZYjmEJ24&feature=youtu.be";
                //var link = "https://youtu.be/uz30vx2zxZc";
                ////var link = "https://r17---sn-o097znld.googlevideo.com:443/videoplayback?itag=22&ratebypass=yes&ip=65.49.68.200&sver=3&requiressl=yes&ipbits=0&upn=0-3pKkD4p4I&pl=24&source=youtube&mv=m&mt=1449316280&gcr=us&ms=au&mn=sn-o097znld&mm=31&id=o-AA5W7zgu_kyhF7JA3InC22gOetF4dNa6aBOfAmT87Kxy&initcwndbps=16860000&key=yt6&mime=video/mp4&sparams=dur,gcr,id,initcwndbps,ip,ipbits,itag,lmt,mime,mm,mn,ms,mv,nh,pl,ratebypass,requiressl,source,upn,expire&expire=1449337948&fexp=9407117,9408210,9408506,9408710,9416126,9417683,9418203,9419891,9420310,9420452,9421461,9422342,9422540,9422596,9422618,9423662,9423991,9425670&nh=IgpwcjAxLnNqYzA3KgkxMjcuMC4wLjE&lmt=1449035687131232&dur=245.063&signature=050B5B5D720EF3C5E7C0F8125CD297982C5113B75CBF8.1E2BB1431E511BF79F919720E2F495930AC8163838&fallback_host=tc.v5.cache5.googlevideo.com";
                //var linkInfoes = Agrin.LinkExtractor.DownloadUrlResolver.GetDownloadUrls(link).ToList();
                //string fileName = "";
                //var size = GetYoutubeFileSize(link, 0, out fileName);
                //var guid = Guid.NewGuid();

                //DirectoryMoveHelper move = new DirectoryMoveHelper(@"D:\TestForCopy\ADM", @"D:\MoveToCopy\" + "ADM");
                //int count = move.FileCount;
                //move.MovedAction = (source, target, pos) =>
                //{

                //};
                //move.Move();
                //return;
                //var link = "https://youtu.be/Uesq5lXt7tM";
                //var link = "https://www.youtube.com/watch?v=UxxajLWwzqY";
                //string pass = "".Sha1Hash();

                ////var loginData = Agrin.Framesoft.Helper.UserManagerHelper.LoginUser(new Agrin.Framesoft.UserInfoData() { UserName = "ali yousefi", Password = pass, ApplicationGuid = guid });
                ////var loginData = Agrin.Framesoft.Helper.UserManagerHelper.LoginUser(new Agrin.Framesoft.UserInfoData() { UserName = "ehsan.20", Password = pass, ApplicationGuid = guid });
                //var links = DownloadUrlResolver.GetDownloadUrls(link).ToList();
                ////var links = Agrin.Framesoft.Helper.UserManagerHelper.GetYoutubeVideoList(new Agrin.Framesoft.UserFileInfoData() { UserName = "ali yousefi", Password = pass, Link = link });
                ////var ali = GetUrl(link);
                //string fileName = "";
                //var size = GetYoutubeFileSize(link, links.FirstOrDefault().FormatCode, out fileName);

                ////send to server for download:

                //ResponseData<UserFileInfoData> file = null;
                //file = UserManagerHelper.DownloadYoutubeLink(new UserFileInfoData() { UserName = "ali yousefi", Password = pass, Link = link, FormatCode = links.FirstOrDefault().FormatCode });
            }
            catch// (Exception e)
            {

            }
        }

        public static void CopyDir(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            // Get Files & Copy
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);

                // ADD Unique File Name Check to Below!!!!
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            // Get dirs recursively and copy files
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyDir(folder, dest);
            }
        }



        public static List<string> GetUrl(string PageURL)
        {

            string HTML = new WebClient().DownloadString(PageURL);//s.ytimg.com/yts/jsbin/player-en_US-vflGaNMBw/base.js
            string temp = "\"\\\\/\\\\/s.ytimg.com\\\\/yts\\\\/jsbin\\\\/html5player-(.+?)\\.js\"";
            Match m = Regex.Match(HTML, temp);
            Group g = m.Groups[1];
            string Player_Version = g.ToString();
            temp = "http://s.ytimg.com/yts/jsbin/" + "html5player-" + Player_Version + ".js";
            string Player_Code = new WebClient().DownloadString(temp);
            temp = "\"url_encoded_fmt_stream_map\":\\s+\"(.+?)\"";
            HTML = Uri.UnescapeDataString(Regex.Match(HTML, temp, RegexOptions.Singleline).Groups[1].ToString());

            MatchCollection Streams = Regex.Matches(HTML, "(^url=|(\\\\u0026url=|,url=))(.+?)(\\\\u0026|,|$)");
            MatchCollection Signatures = Regex.Matches(HTML, "(^s=|(\\\\u0026s=|,s=))(.+?)(\\\\u0026|,|$)");
            List<string> URLs = new List<string>();
            for (int i = 0; i < Streams.Count; i++)
            {
                string URL = Streams[i].Groups[3].ToString();
                if (Signatures.Count > 0)
                {
                    string sign = Sign_Decipher(Signatures[i].Groups[3].ToString(), Player_Code);
                    URL += "&signature=" + sign;
                }
                URLs.Add(URL.Trim());
            }

            return URLs;
        }

        private static string Sign_Decipher(string s, string code)
        {
            string Function_Name = Regex.Match(code, "signature=(\\w+)\\(\\w+\\)").Groups[1].ToString();
            Match Function_Match = Regex.Match(code, "function " + Function_Name + "\\((\\w+)\\)\\{(.+?)\\}", RegexOptions.Singleline);
            string var = Function_Match.Groups[1].ToString();
            string Decipher = Function_Match.Groups[2].ToString();
            string[] Lines = Decipher.Split(';');
            for (int i = 0; i < Lines.Length; i++)
            {
                string Line = Lines[i].Trim();
                if (Regex.IsMatch(Line, var + "=" + var + "\\.reverse\\(\\)"))
                {
                    char[] charArray = s.ToCharArray();
                    Array.Reverse(charArray);
                    s = new string(charArray);
                }
                else if (Regex.IsMatch(Line, var + "=" + var + "\\.slice\\(\\d+\\)"))
                {
                    s = Slice(s, Convert.ToInt32(Regex.Match(Line, var + "=" + var + "\\.slice\\((\\d+)\\)").Groups[1].ToString()));
                }
                else if (Regex.IsMatch(Line, var + "=\\w+\\(" + var + ",\\d+\\)"))
                {
                    s = Swap(s, Convert.ToInt32(Regex.Match(Line, var + "=\\w+\\(" + var + ",(\\d+)\\)").Groups[1].ToString()));
                }
                else if (Regex.IsMatch(Line, var + "\\[0\\]=" + var + "\\[\\d+%" + var + "\\.length\\]"))
                {
                    s = Swap(s, Convert.ToInt32(Regex.Match(Line, var + "\\[0\\]=" + var + "\\[(\\d+)%" + var + "\\.length\\]").Groups[1].ToString()));
                }
            }
            return s;
        }

        private static string Slice(string input, int length)
        {
            return input.Substring(length);
        }

        private static string Swap(string input, int position)
        {
            StringBuilder Str = new StringBuilder(input);
            char SwapChar = Str[position];
            Str[position] = Str[0];
            Str[0] = SwapChar;
            return Str.ToString();
        }


        //public static long GetYoutubeFileSize(string address, int formatCode, out string outFileName)
        //{
        //    try
        //    {
        //        string normalURL = address;
        //        //List<VideoInfo> videos = null;
        //        //if (DownloadUrlResolver.TryNormalizeYoutubeUrl(address, out normalURL))
        //        //    videos = DownloadUrlResolver.GetDownloadUrls(normalURL).ToList();
        //        //else
        //        //    videos = DownloadUrlResolver.GetDownloadUrls(address).ToList();

        //        //var video = DownloadUrlResolver.GetVideoInfoByFormatCode(videos, formatCode);
        //        //address = video.DownloadUrl;
        //        //outFileName = video.Title + video.VideoExtension;
        //        outFileName = address;
        //        HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(address);
        //        _request.AllowAutoRedirect = true;
        //        _request.KeepAlive = true;
        //        _request.Headers.Add("Accept-Language", "en-US");
        //        _request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0) like Gecko";
        //        using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
        //        {
        //            long length = response.ContentLength;
        //            if (length <= 0)
        //            {
        //                return -1;
        //            }
        //            return length;
        //        }
        //    }
        //    catch (Exception ef)
        //    {
        //        if (ef is WebException)
        //        {
        //            var w = ef as WebException;
        //            var stream = new System.IO.StreamReader(w.Response.GetResponseStream());
        //            var text = stream.ReadToEnd();
        //        }
        //        outFileName = null;
        //        return -2;
        //    }
        //}

    }
}
