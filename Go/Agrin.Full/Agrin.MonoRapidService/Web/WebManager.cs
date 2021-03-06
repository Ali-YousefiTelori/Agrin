using Agrin.MonoRapidService.RapidBazWebService;
using Agrin.RapidService.Models;
using Agrin.RapidService.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidService.Web
{
    public static class WebManager
    {
        static WSDLapiwebService CurrentService { get; set; }
        static WebManager()
        {
            CurrentService = new WSDLapiwebService();
        }

        public static void CheckFile()
        {

        }

        public static void CheckLogin()
        {
            if (!UserManager.IsLogin)
            {
                UserManager.Login(UserManager.CurrentUser.UserName, UserManager.CurrentUser.Password);
            }
        }

        public static string UploadFile(string url)
        {
            CheckLogin();
            return CurrentService.Queue(UserManager.CurrentSession, url);
        }

        public static FileStatus FileStatus(string queueID)
        {
            CheckLogin();
            var value = CurrentService.Status(UserManager.CurrentSession, queueID);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<FileStatus>(value);
        }

        public static List<RapidItemInfo> GetCompleteList()
        {
            CheckLogin();
            var value = CurrentService.Get(UserManager.CurrentSession, int.MaxValue.ToString(), "0");
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RapidItemInfo>>(value);
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.Link))
                {
                    item.Link = Agrin.IO.Strings.UrlDecoder.DecodeUrlString(item.Link);
                }
                if (!string.IsNullOrEmpty(item.Name))
                {
                    item.Name = Agrin.IO.Strings.UrlDecoder.DecodeUrlString(item.Name);
                }
                if (!string.IsNullOrEmpty(item.Url))
                {
                    item.Url = Agrin.IO.Strings.UrlDecoder.DecodeUrlString(item.Url);
                }
            }
            return list;
        }

        public static List<RapidItemInfo> GetQueueList()
        {
            CheckLogin();
            var value = CurrentService.QueueGet(UserManager.CurrentSession, int.MaxValue.ToString(), "0");
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RapidItemInfo>>(value);
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.Link))
                {
                    item.Link = Agrin.IO.Strings.UrlDecoder.DecodeUrlString(item.Link);
                }
                if (!string.IsNullOrEmpty(item.Name))
                {
                    item.Name = Agrin.IO.Strings.UrlDecoder.DecodeUrlString(item.Name);
                }
                if (!string.IsNullOrEmpty(item.Url))
                {
                    item.Url = Agrin.IO.Strings.UrlDecoder.DecodeUrlString(item.Url);
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        item.Name = Agrin.IO.Helper.MPath.GetFileName(new Uri(item.Url));
                    }
                }
            }
            return list;
        }

        public static string GetStatusString(string status)
        {
            if (status == "0")
                return "فایل با موفقیت ارسال شد";
            else if (status == "1")
                return "فایل در نوبت انتقال است";
            else if (status == "2")
                return "در هنگام دانلود با مشکل مواجه شده است";
            else if (status == "3")
                return "درخواست تکراری است";
            else if (status == "4")
                return "فایل غیر مجاز است";
            else if (status == "5")
                return "رپیدباز نمی تواند این فایل را منتقل کند";
            else if (status == "6")
                return "درخواست داده شده از حجم باقی مانده بیشتر است";
            else if (status == "7")
                return "فایل در حال بررسی است";
            else if (status == "8")
                return "فایل در حال انتقال است";
            else if (status == "9")
                return "سرور مبدا آدرس درخواستی را معتبر نمی داند و فایل را در اختیار ما قرار نمی دهد";
            else if (status == "10")
                return "انتقال فایل با مشکل مواجه شده است";
            else
                return "نامشخص";
        }
    }
}
