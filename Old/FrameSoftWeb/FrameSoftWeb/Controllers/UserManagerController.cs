using Agrin.Framesoft;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FrameSoftWeb.Helper;
using YoutubeExtractor;
using FrameSoft.Agrin.DataBase.Helper;
using FrameSoft.Agrin.DataBase.Models;

namespace FrameSoftWeb.Controllers
{
    public class UserManagerController : Controller
    {
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
                        string csrftoken = SerializationData.GetTextBetweenTwoValue(strings[0], "csrftoken=", "");
                        string sessionid = SerializationData.GetTextBetweenTwoValue(strings[1], "sessionid=", "");
                        cooks.Add(new Cookie("csrftoken", csrftoken) { Domain = _request.RequestUri.Host });
                        cooks.Add(new Cookie("sessionid", sessionid) { Domain = _request.RequestUri.Host });
                        var text = SerializationData.GetTextBetweenTwoValue(stream.ReadToEnd(), "<form", "submit");
                        var name1 = SerializationData.GetTextBetweenTwoValue(text, "name='", "value='").Trim().TrimEnd(new char[] { '\'' });
                        var value1 = SerializationData.GetTextBetweenTwoValue(text, "value='", "'");
                        var name2 = SerializationData.GetTextBetweenTwoValue(text, "name=\"", "value=\"").Trim().TrimEnd(new char[] { '\"' });
                        var value2 = SerializationData.GetTextBetweenTwoValue(text, "value=\"", "\"/>");

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

        public static string refreshToken = "";
        public ActionResult CafeBazaarAuth()
        {
            try
            {
                string param1 = this.Request.QueryString["code"];
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
                return Content("OK");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        public string PurchaseValidateState { get; set; }
        public bool PurchaseValidate(UserPurchaseData userPurchaseData, bool redirect = false)
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
                        if (redirect)
                            PurchaseValidateState = "Two";
                        else
                            PurchaseValidateState = "One";
                        if (text == "{}")
                            return false;
                    }
                }
            }
            catch (Exception e)
            {
                if (!redirect)
                {
                    CreateNewCode();
                    return PurchaseValidate(userPurchaseData, true);
                }
                PurchaseValidateState = "Error CafeBazaar:" + e.Message;
                return false;
            }

            return true;
        }

        static object lockOBJ = new object();
        public ActionResult RegisterUser()
        {
            try
            {
                lock (lockOBJ)
                {
                    var allkeys = this.Request.Headers.AllKeys;
                    string data = null;
                    if (allkeys.Contains("DataJson"))
                    {
                        data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                    }
                    else
                        data = SerializationData.DecryptStream(Request.InputStream);

                    var register = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoData>(data);

                    if (string.IsNullOrEmpty(register.UserName) || string.IsNullOrEmpty(register.Password) || string.IsNullOrEmpty(register.Email))
                        return Content(this.SetResultTextValue("Null"));
                    else
                    {
                        register.UserName = register.UserName.Trim();
                        register.Email = register.Email.Trim();
                        if (Engine.Amar.FramesoftServiceProvider.ExistUserName(register.UserName))
                            return Content(this.SetResultTextValue("UserName"));
                        if (Engine.Amar.FramesoftServiceProvider.ExistEmail(register.Email))
                            return Content(this.SetResultTextValue("Email"));
                        Engine.Amar.FramesoftServiceProvider.RegisterUser(new UserInfo() { Email = register.Email, UserName = register.UserName, Password = register.Password, ApplicationGuid = register.ApplicationGuid });
                        return Content(this.SetResultTextValue("OK"));
                    }
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        public ActionResult LoginUser()
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoData>(data);
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    user.UserName = user.UserName.Trim();
                    if (!string.IsNullOrEmpty(user.Email))
                        user.Email = user.Email.Trim();
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(user.UserName, user.Password);
                    if (login != -1)
                    {
                        var userI = Engine.Amar.FramesoftServiceProvider.GetUserPropertiesInfo(login);
                        return Content(this.SetResultTextData(SerializationData.EncryptObject(new UserInfoData() { Size = userI.UserSize, UserName = userI.UserName })));
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        public ActionResult RefreshDownloadingLinks()
        {
            try
            {
                return Content(this.SetResultTextValue("OK Started: " + SmallDownloadManager.RefreshDownloadLinks() + " Downloading Count: " + SmallDownloadManager.DownloadingCount()));
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        public ActionResult PermissionReadMe()
        {
            try
            {
                string html = System.IO.File.ReadAllText(HttpContext.Server.MapPath("~/Resources/Agrin/Learning/AndroidUserPermission.html"));
                return Content(html);
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        /// <summary>
        /// دریافت مشخصات کامل یوزر از جمله کل حجم خریداری شده و حجم باقی مانده
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserInfo()
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoData>(data);
                if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(user.UserName, user.Password);
                    if (login != -1)
                    {
                        var prop = Engine.Amar.FramesoftServiceProvider.GetUserPropertiesInfo(login);
                        if (prop != null)
                            return Content(this.SetResultTextData(SerializationData.EncryptObject(new UserInfoData() { Size = prop.UserSize, UserName = prop.UserName })));
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        //hex encoding of the hash, in uppercase.
        public static string Sha1Hash(string str)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(str);
            data = Sha1Hash(data);
            return BitConverter.ToString(data).Replace("-", "");
        }
        // Do the actual hashing
        public static byte[] Sha1Hash(byte[] data)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(data);
            }
        }

        static object lockSize = new object();
        /// <summary>
        ///  دانلود یک فایل فقط برای یک یوزر خاص
        /// </summary>
        /// <returns></returns>
        public DownloadResult DownloadOneFileForUser(string fileGuid)
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                if (!allkeys.Contains("Authorization"))
                    return new DownloadResult(this, "", 403);
                var items = ParseAuthHeader(this.Request.Headers["Authorization"]);
                if (items == null)
                    return new DownloadResult(this, "", 403);

                string userName = items[0];
                string password = Sha1Hash(items[1]);

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    return new DownloadResult(this, "", 403);
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(userName, password);
                    if (login != -1)
                    {
                        var file = Engine.Amar.FramesoftServiceProvider.GetLeechFileInfoByGUID(login, fileGuid);
                        if (!file.IsComplete)
                            return new DownloadResult(this, "", 403);
                        HttpContext.Response.AddHeader("HideFN", SerializationData.EncryptString(file.FileName));
                        return new DownloadResult(this, file.DiskPath);
                    }
                    return new DownloadResult(this, "", 403);
                }
            }
            catch (Exception e)
            {
                return new DownloadResult(this, "", 500);
            }
        }

        private string[] ParseAuthHeader(string authHeader)
        {
            // Check this is a Basic Auth header
            if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic")) return null;

            // Pull out the Credentials with are seperated by ':' and Base64 encoded
            string base64Credentials = authHeader.Substring(6);
            string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials)).Split(new char[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0])) return null;

            // Okay this is the credentials
            return credentials;
        }

        /// <summary>
        /// آپلود یک لینک
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadOneLink()
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var uFile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserFileInfoData>(data);
                if (string.IsNullOrEmpty(uFile.UserName) || string.IsNullOrEmpty(uFile.Password) || string.IsNullOrEmpty(uFile.Link))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(uFile.UserName, uFile.Password);
                    if (login != -1)
                    {
                        var prop = Engine.Amar.FramesoftServiceProvider.GetUserPropertiesInfo(login);
                        if (prop != null)
                        {
                            if (prop.UserSize <= 0)
                                return Content(this.SetResultTextValue("NoStorage"));
                            else
                            {

                                var size = SmallDownloadManager.GetFileSize(uFile.Link);
                                var fileName = SmallDownloadManager.GetFileName(uFile.Link);
                                if (size > 0)
                                {
                                    lock (lockSize)
                                    {
                                        if (prop.UserSize >= size)
                                        {
                                            Engine.Amar.FramesoftServiceProvider.SubUserStorageSize(login, size);
                                            try
                                            {
                                                var fileInfo = SmallDownloadManager.CreateNewFileStorageForDownload(login, uFile.Link, size, fileName);
                                                return Content(this.SetResultTextData(SerializationData.EncryptObject(ConvertToFileInfoData(fileInfo))));
                                            }
                                            catch (Exception e)
                                            {
                                                Engine.Amar.FramesoftServiceProvider.SumUserStorageSize(login, size);
                                                return Content(this.SetResultTextValue("StorageBackContactMe"));
                                            }

                                        }
                                        else
                                            return Content(this.SetResultTextValue("NoStorageFileSize"));
                                    }
                                }
                                else if (size == -1)
                                {
                                    return Content(this.SetResultTextValue("NoFileSize"));
                                }
                                else if (size == -2)
                                {
                                    return Content(this.SetResultTextValue("CannotDownload"));
                                }
                            }
                        }
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        UserFileInfoData ConvertToFileInfoData(UserFileInfo fileInfo)
        {
            return new UserFileInfoData() { FileName = fileInfo.FileName, FileGuid = fileInfo.FileGuid, ID = fileInfo.ID, Link = fileInfo.Link, Status = fileInfo.Status, Size = fileInfo.Size };
        }

        /// <summary>
        /// دریافت وضعیت یک فایل
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOneFileStatus()
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var uFile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserFileInfoData>(data);
                if (string.IsNullOrEmpty(uFile.UserName) || string.IsNullOrEmpty(uFile.Password))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(uFile.UserName, uFile.Password);
                    if (login != -1)
                    {
                        long downloaded = 0;
                        var fileInfo = SmallDownloadManager.GetFileStatus(login, uFile.ID, out downloaded, false);
                        if (fileInfo != null)
                        {
                            var con = ConvertToFileInfoData(fileInfo);
                            con.DownloadedSize = downloaded;
                            return Content(this.SetResultTextData(SerializationData.EncryptObject(con)));
                        }
                        else
                        {
                            return Content(this.SetResultTextValue("FileNotFound"));
                        }
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        /// <summary>
        /// ارسال تکمیل شدن از طرف کلاینت و حذف فایل
        /// </summary>
        /// <returns></returns>
        public ActionResult SetCompleteUserFiles()
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var uFile = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserFileInfoData>>(data);
                if (string.IsNullOrEmpty(uFile.First().UserName) || string.IsNullOrEmpty(uFile.First().Password))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(uFile.First().UserName, uFile.First().Password);
                    if (login != -1)
                    {
                        foreach (var item in uFile)
                        {
                            var fileInfo = Engine.Amar.FramesoftServiceProvider.GetLeechFileInfoByGUID(login, item.FileGuid.ToString());

                            if (fileInfo != null)
                            {
                                if (fileInfo.IsComplete)
                                {
                                    fileInfo.IsUserDownloadedThis = true;
                                    try
                                    {
                                        System.IO.File.Delete(fileInfo.DiskPath);
                                        fileInfo.IsDeletedByApplication = true;
                                    }
                                    catch (Exception e)
                                    {
                                        Engine.Amar.FramesoftServiceProvider.UpdateLeechFile(fileInfo);
                                        return Content(this.SetResultTextValue(e.Message));
                                    }
                                }
                                else
                                {
                                    if (System.IO.File.Exists(fileInfo.DiskPath))
                                    {
                                        var fileSize = new FileInfo(fileInfo.DiskPath).Length;
                                        if (fileSize < fileInfo.Size)
                                        {
                                            Engine.Amar.FramesoftServiceProvider.SumUserStorageSize(login, fileInfo.Size - fileSize);
                                        }

                                        fileInfo.IsUserDownloadedThis = true;

                                        try
                                        {
                                            SmallDownloadManager.DisposeAndStopDownloadManager(fileInfo.ID);
                                            System.IO.File.Delete(fileInfo.DiskPath);
                                            fileInfo.IsDeletedByApplication = true;
                                        }
                                        catch (Exception e)
                                        {
                                            Engine.Amar.FramesoftServiceProvider.UpdateLeechFile(fileInfo);
                                            return Content(this.SetResultTextValue(e.Message));
                                        }
                                    }

                                }
                                Engine.Amar.FramesoftServiceProvider.UpdateLeechFile(fileInfo);
                            }
                        }
                        return Content(this.SetResultTextValue("OK"));
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        /// <summary>
        /// دریافت لیست فایل ها
        /// </summary>
        /// <returns></returns>
        public ActionResult GetListOfFiles()
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var uFile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfoData>(data);
                if (string.IsNullOrEmpty(uFile.UserName) || string.IsNullOrEmpty(uFile.Password))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(uFile.UserName, uFile.Password);
                    if (login != -1)
                    {
                        var fileInfoes = Engine.Amar.FramesoftServiceProvider.GetListLeechFileInfoes(login);
                        if (fileInfoes != null)
                        {
                            List<UserFileInfoData> items = new List<UserFileInfoData>();
                            foreach (var item in fileInfoes)
                            {
                                var con = ConvertToFileInfoData(item);
                                long downloaded = 0;
                                var getNew = SmallDownloadManager.GetFileStatus(login, item.ID, out downloaded, true);
                                if (getNew != null)
                                {
                                    con.DownloadedSize = downloaded;
                                    con.Status = getNew.Status;
                                }
                                items.Add(con);
                            }
                            return Content(this.SetResultTextData(SerializationData.EncryptObject(items)));
                        }
                        else
                        {
                            return Content(this.SetResultTextValue("FileNotFound"));
                        }
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        /// <summary>
        /// خرید محصول از طرف کاربر
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyStorageFromUser()
        {
            try
            {
                var allkeys = this.Request.Headers.AllKeys;
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var udata = Newtonsoft.Json.JsonConvert.DeserializeObject<UserPurchaseData>(data);
                if (string.IsNullOrEmpty(udata.UserName) || string.IsNullOrEmpty(udata.Password))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(udata.UserName, udata.Password);
                    if (login != -1)
                    {
                        lock (lockSize)
                        {
                            bool exist = Engine.Amar.FramesoftServiceProvider.ExistPurchaseByToken(login, udata.PurchaseToken);
                            if (exist)
                            {
                                return Content(this.SetResultTextValue("Repeat"));
                            }
                            else
                            {
                                if (PurchaseValidate(udata))
                                {
                                    UserPurchase up = new UserPurchase() { InsertDateTime = DateTime.Now, PurchaseTime = udata.PurchaseTime, PurchaseToken = udata.PurchaseToken, UserID = login, ProductId = udata.ProductId };
                                    Engine.Amar.FramesoftServiceProvider.BuyStorageData(up);
                                    Engine.Amar.FramesoftServiceProvider.SumUserStorageSize(login, GetSizeByProductID(up.ProductId));
                                    return Content(this.SetResultTextValue("OK"));
                                }
                                else
                                {
                                    return Content(this.SetResultTextValue("InvalidUserPurchase"));//+ PurchaseValidateState
                                }
                            }
                        }
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        public long GetSizeByProductID(string id)
        {
            switch (id)
            {
                case "buystorage100mb":
                    {
                        return 104857600;
                    }
                case "buystorage500mb":
                    {
                        return 524288000;
                    }
                case "buystorage1gb":
                    {
                        return 1073741824;
                    }
                case "buystorage2gb":
                    {
                        return 2147483648;
                    }
                case "buystorage5gb":
                    {
                        return 5368709120;
                    }
                case "buystorage10gb":
                    {
                        return 10737418240;
                    }
            }
            return 0;
        }

        public ActionResult GetYoutubeVideoList()
        {
            var allkeys = this.Request.Headers.AllKeys;
            try
            {
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var uFile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserFileInfoData>(data);
                if (string.IsNullOrEmpty(uFile.UserName) || string.IsNullOrEmpty(uFile.Password))
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(uFile.UserName, uFile.Password);
                    if (login != -1)
                    {
                        var linkInfoes = DownloadUrlResolver.GetDownloadUrls(uFile.Link).ToList();

                        if (linkInfoes != null)
                        {
                            return Content(this.SetResultTextData(SerializationData.EncryptObject(linkInfoes)));
                        }
                        else
                        {
                            return Content(this.SetResultTextValue("FileNotFound"));
                        }
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                if (allkeys.Contains("stacktrace"))
                    return Content(this.SetResultTextValue("ex: " + e.Message + " stackT: " + e.StackTrace));
                else
                    return Content(this.SetResultTextValue(e.Message));
            }
        }



        public ActionResult DownloadYoutubeLink()
        {
            var allkeys = this.Request.Headers.AllKeys;
            try
            {
                string data = null;
                if (allkeys.Contains("DataJson"))
                {
                    data = SerializationData.DecryptDataString(this.Request.Headers["DataJson"]);
                }
                else
                    data = SerializationData.DecryptStream(Request.InputStream);
                var uFile = Newtonsoft.Json.JsonConvert.DeserializeObject<UserFileInfoData>(data);
                if (string.IsNullOrEmpty(uFile.UserName) || string.IsNullOrEmpty(uFile.Password) || string.IsNullOrEmpty(uFile.Link) || uFile.FormatCode == 0)
                    return Content(this.SetResultTextValue("Null"));
                else
                {
                    var login = Engine.Amar.FramesoftServiceProvider.LoginUser(uFile.UserName, uFile.Password);
                    if (login != -1)
                    {
                        var prop = Engine.Amar.FramesoftServiceProvider.GetUserPropertiesInfo(login);
                        if (prop != null)
                        {
                            if (prop.UserSize <= 0)
                                return Content(this.SetResultTextValue("NoStorage"));
                            else
                            {
                                string fileName = null;
                                var size = SmallDownloadManager.GetYoutubeFileSize(uFile.Link, uFile.FormatCode, out fileName);
                                if (size > 0)
                                {
                                    lock (lockSize)
                                    {
                                        if (prop.UserSize >= size)
                                        {
                                            Engine.Amar.FramesoftServiceProvider.SubUserStorageSize(login, size);
                                            try
                                            {
                                                var fileInfo = SmallDownloadManager.CreateNewFileStorageForDownload(login, uFile.Link, size, fileName, true, uFile.FormatCode);

                                                return Content(this.SetResultTextData(SerializationData.EncryptObject(ConvertToFileInfoData(fileInfo))));
                                            }
                                            catch (Exception e)
                                            {
                                                Engine.Amar.FramesoftServiceProvider.SumUserStorageSize(login, size);
                                                return Content(this.SetResultTextValue("StorageBackContactMe"));
                                            }

                                        }
                                        else
                                            return Content(this.SetResultTextValue("NoStorageFileSize"));
                                    }
                                }
                                else if (size == -1)
                                {
                                    return Content(this.SetResultTextValue("NoFileSize"));
                                }
                                else if (size == -2)
                                {
                                    return Content(this.SetResultTextValue("CannotDownload"));
                                }
                            }
                        }
                    }
                    return Content(this.SetResultTextValue("No"));
                }
            }
            catch (Exception e)
            {
                if (allkeys.Contains("stacktrace"))
                    return Content(this.SetResultTextValue("ex: " + e.Message + " stackT: " + e.StackTrace));
                else
                    return Content(this.SetResultTextValue("ex: " + e.Message));
            }
        }
    }
}
