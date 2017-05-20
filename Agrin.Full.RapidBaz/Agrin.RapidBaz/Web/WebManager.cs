using Agrin.RapidBaz.Models;
using Agrin.RapidBaz.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Web
{
    public static class WebManager
    {
        static IWSDLapiwebService CurrentService { get; set; }
        public static void Initialize(IWSDLapiwebService WSDLapiwebService)
        {
            CurrentService = WSDLapiwebService;
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

        public static List<string> UploadMultipeFile(List<string> urls)
        {
            CheckLogin();
            StringBuilder builder = new StringBuilder();
            foreach (var item in urls)
            {
                builder.Append(item + ";");
            }

            var values = CurrentService.MultiQueue(UserManager.CurrentSession, builder.ToString());
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(values);
            return list;
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
            if (string.IsNullOrEmpty(value))
                return new List<RapidItemInfo>();
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

        public static List<RapidItemInfo> GetQueueList(int start,int len)
        {
            CheckLogin();
            //var value = CurrentService.QueueGet(UserManager.CurrentSession, int.MaxValue.ToString(), "0");
            var value = CurrentService.QueueGet(UserManager.CurrentSession, len.ToString(), start.ToString());
            if (string.IsNullOrEmpty(value))
                return new List<RapidItemInfo>();
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

        public static string Free(string id)
        {
            CheckLogin();
            return CurrentService.Free(UserManager.CurrentSession, id);
        }

        public static string QueueRemove(string queueID)
        {
            CheckLogin();
            return CurrentService.QueueRemove(UserManager.CurrentSession, queueID);
        }

        public static string GetStatusString(string status)
        {
            if (status == "0")
                return "آماده ی دانلود";
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

        public static bool IsErrorStatus(string status)
        {
            //status == "2" ||
            if (status == "3" || status == "4" || status == "5" || status == "6" || status == "9" || status == "10")
                return true;
            else
                return false;
        }

        public static bool IsStopStatus(string status)
        {
            if (status == "2")
                return true;
            else
                return false;
        }

        public static List<FolderInfo> FolderList()
        {
            //List<FolderInfo> items = new List<FolderInfo>();
            //items.Add(new FolderInfo() { Name = "Ali", Count = 6 });
            //items.Add(new FolderInfo() { Name = "Reza", Count = 11 });
            //items.Add(new FolderInfo() { Name = "Alsdvsdvi", Count = 18 });
            //items.Add(new FolderInfo() { Name = "dsvsdv", Count = 15 });
            //items.Add(new FolderInfo() { Name = "Alsdvsi", Count = 10 });
            //items.Add(new FolderInfo() { Name = "Revsdvsdza", Count = 55 });
            //items.Add(new FolderInfo() { Name = "Aldvsi", Count = 4 });
            //items.Add(new FolderInfo() { Name = "Revsdvsdza", Count = 2 }); 
            //return items;
            CheckLogin();
            var value = CurrentService.FolderList(UserManager.CurrentSession);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<FolderInfo>>(value);
        }

        public static List<RapidItemInfo> FolderGet(string folder)
        {
            CheckLogin();
            var value = CurrentService.FolderGet(UserManager.CurrentSession, folder);
            var yes = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<System.String, RapidItemInfo>>(value);

            return yes.Values.ToList();
        }

        public static string FolderMake(string name)
        {
            CheckLogin();
            return CurrentService.FolderMake(UserManager.CurrentSession, name);
        }

        public static string FolderRemove(string folderId)
        {
            CheckLogin();
            return CurrentService.FolderRemove(UserManager.CurrentSession, folderId);
        }

        public static string FolderReset(string folderId)
        {
            CheckLogin();
            return CurrentService.FolderReset(UserManager.CurrentSession, folderId);
        }

        public static string Repair(string id)
        {
            CheckLogin();
            return CurrentService.Repair(UserManager.CurrentSession, id);
        }

        public static string SetFolder(string folderID, string fileID)
        {
            CheckLogin();
            return CurrentService.SetFolder(UserManager.CurrentSession, folderID, fileID);
        }
    }
}
