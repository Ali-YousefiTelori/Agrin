using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Threading;

namespace Agrin.ViewModels
{
    public static class ApplicationLoader
    {
        public static void LoadApplicationData(Dispatcher dispatcher, Version version)
        {
            ApplicationHelper.DispatcherThread = dispatcher;
            Agrin.IO.Helper.MPath.InitializePath();
            ApplicationLinkInfoManager.Current = new ApplicationLinkInfoManager();
            ApplicationGroupManager.Current = new ApplicationGroupManager();
            ApplicationNotificationManager.Current = new ApplicationNotificationManager();
            ApplicationBalloonManager.Current = new ApplicationBalloonManager();

            DeSerializeData.LoadApplicationData();
            //Agrin.UI.ViewModels.Popups.BalloonViewModel.CreateBalloon();

            //notify.Click += notify_Click;
            //System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Agrin.UI;component/Project1.ico")).Stream;
            //notify.Icon = new System.Drawing.Icon(iconStream);

            //downloadManager.linksListData.CurrentToolbox = mainTopToolBox;

            //ser app setting
            ApplicationSetting.Current.ApplicationOSSetting.Application = "Agrin WPF Windows";
            if (string.IsNullOrEmpty(ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid))
            {
                ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid = Guid.NewGuid().ToString();
                SerializeData.SaveApplicationSettingToFile();
            }
            ApplicationSetting.Current.ApplicationOSSetting.ApplicationVersion = version.ToString();
            ApplicationSetting.Current.ApplicationOSSetting.OSName = Agrin.OS.Management.OSSystemInfo.GetSystemInformation();
            ApplicationSetting.Current.ApplicationOSSetting.OSVersion = Environment.OSVersion.VersionString;


            ApplicationHelperMono.DispatcherThread = dispatcher;
            ApplicationRapidbazStatusChecker.Start();
            //IsShowToolbar = true;
            //Agrin.UI.ViewModels.Toolbox.ToolbarViewModel.This.PinCommand.Execute();
            //var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //txtVersion.Text = "نسخه آزمایشی " + version.ToString() + " بهمن 1393";
            CreateGroups();
        }

        private static void CreateGroups()
        {
            if (ApplicationGroupManager.Current.GroupInfoes.Count == 0)
            {
                string path = Agrin.IO.Helper.MPath.DownloadsPath;
                GroupInfo group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Music"), Name = "موسیقی", Extentions = new List<string>() { "mp3", "wav", "wma", "mpa", "ram", "ra", "aac", "aif", "m4a", "m4p", "msv", "oga" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Videos"), Name = "ویدئو", Extentions = new List<string>() { "avi", "mpg", "mpe", "mpeg", "asf", "wmv", "mov", "qt", "rm", "mp4", "flv", "m4v", "webm", "ogv", "ogg", "mkv", "webm", "vob", "3g2", "svi" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Images"), Name = "تصاویر", Extentions = new List<string>() { "JPEG", "jpg", "jpe", "JFIF", "Exif", "TIFF", "tif", "RIF", "gif", "bmp", "png", "psd", "flv", "m4v", "webm", "ogv", "ogg", "mkv" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Document"), Name = "سند", Extentions = new List<string>() { "doc", "pdf", "ppt", "pps", "xls", "xlsx" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Application"), Name = "نرم افزار", Extentions = new List<string>() { "exe", "msi", "apk" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SavePath = System.IO.Path.Combine(path, "Compress"), Name = "فشرده", Extentions = new List<string>() { "zip", "rar", "arj", "gz", "sit", "sitx", "sea", "ace", "bz2", "7z" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);
                Agrin.Download.Data.SerializeData.SaveGroupInfoesToFile();
            }
        }
    }
}
