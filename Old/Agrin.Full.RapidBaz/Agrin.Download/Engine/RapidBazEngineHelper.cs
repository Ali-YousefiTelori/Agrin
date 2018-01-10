using Agrin.Download.Data.Settings;
using Agrin.RapidBaz.Models;
using Agrin.RapidBaz.Users;
using Agrin.RapidBaz.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Engine
{
    public static class RapidBazEngineHelper
    {
        public static string UserName
        {
            get
            {
                return UserManager.CurrentUser.UserName;
            }
        }

        public static string Password
        {
            get
            {
                return UserManager.CurrentUser.Password;
            }
        }

        public static string CurrentSession
        {
            get
            {
                return UserManager.CurrentSession;
            }
        }

        public static bool IsLogin
        {
            get
            {
                return UserManager.IsLogin;
            }
        }
        static object lockOBJ = new object();

        public static bool Login()
        {
            lock (lockOBJ)
            {
                if (!UserManager.IsLogin)
                {
                    if (!Login(ApplicationSetting.Current.RapidBazSetting.UserName, ApplicationSetting.Current.RapidBazSetting.Password))
                        return false;
                }
                return true;
            }
        }

        public static bool Login(string userName, string password)
        {
            ApplicationSetting.Current.RapidBazSetting.UserName = userName;
            ApplicationSetting.Current.RapidBazSetting.Password = password;
            UserManager.CurrentUser.UserName = userName;
            UserManager.CurrentUser.Password = password;

            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();

            return UserManager.Login(userName, password);
        }

        public static string SendFile(string url)
        {
            if (!UserManager.IsLogin)
            {
                if (!Login(ApplicationSetting.Current.RapidBazSetting.UserName, ApplicationSetting.Current.RapidBazSetting.Password))
                    return "-2";
            }

            return WebManager.UploadFile(url);
        }

        public static List<string> SendMultipeFile(List<string> urls)
        {
            if (!UserManager.IsLogin)
            {
                if (!Login(ApplicationSetting.Current.RapidBazSetting.UserName, ApplicationSetting.Current.RapidBazSetting.Password))
                    return null;
            }

            return WebManager.UploadMultipeFile(urls);
        }

        public static List<RapidItemInfo> GetCompleteList()
        {
            return WebManager.GetCompleteList();
        }

        public static List<RapidItemInfo> GetQueueList(int start,int len)
        {
            return WebManager.GetQueueList(start, len);
        }

        public static FileStatus FileStatus(string queueID)
        {
            return WebManager.FileStatus(queueID);
        }

        public static string Free(string queueID)
        {
            return WebManager.Free(queueID);
        }

        public static string QueueRemove(string queueID)
        {
            return WebManager.QueueRemove(queueID);
        }

        public static string Retry(string id)
        {
            return WebManager.Repair(id);
        }

        public static string SetFolder(string folderID, string fileID)
        {
            if (string.IsNullOrEmpty(folderID))
                return null;
            return WebManager.SetFolder(folderID, fileID);
        }

        public static string UserInfo()
        {
            WebManager.CheckLogin();
            var str = UserManager.UserInfo();
            return "";
        }
    }
}