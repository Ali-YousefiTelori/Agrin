using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels
{
    public static class ApplicationBaseLoader
    {
        public static void CreateGroups()
        {
            if (ApplicationGroupManager.Current.GroupInfoes.Count == 0)
            {
                GroupInfo group = new GroupInfo() { SaveFolderName = "Music", Name = "موسیقی", Extentions = new List<string>() { "mp3", "wav", "wma", "mpa", "ram", "ra", "aac", "aif", "m4a", "m4p", "msv", "oga" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SaveFolderName = "Videos", Name = "ویدئو", Extentions = new List<string>() { "avi", "mpg", "mpe", "mpeg", "asf", "wmv", "mov", "qt", "rm", "mp4", "flv", "m4v", "webm", "ogv", "ogg", "mkv", "webm", "vob", "3g2", "svi" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SaveFolderName = "Images", Name = "تصاویر", Extentions = new List<string>() { "JPEG", "jpg", "jpe", "JFIF", "Exif", "TIFF", "tif", "RIF", "gif", "bmp", "png", "psd", "flv", "m4v", "webm", "ogv", "ogg", "mkv" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SaveFolderName = "Document", Name = "سند", Extentions = new List<string>() { "doc", "pdf", "ppt", "pps", "xls", "xlsx" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SaveFolderName = "Application", Name = "نرم افزار", Extentions = new List<string>() { "exe", "msi", "apk" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);

                group = new GroupInfo() { SaveFolderName = "Compress", Name = "فشرده", Extentions = new List<string>() { "zip", "rar", "arj", "gz", "sit", "sitx", "sea", "ace", "bz2", "7z" } };
                ApplicationGroupManager.Current.AddGroupInfo(group);
                Agrin.Download.Data.SerializeData.SaveGroupInfoesToFile();
            }
            else
            {
                bool save = false;
                foreach (var item in ApplicationGroupManager.Current.GroupInfoes)
                {
                    if (string.IsNullOrEmpty(item.SaveFolderName))
                    {
                        save = true;
                        if (item.Name == "موسیقی")
                            item.SaveFolderName = "Music";
                        else if (item.Name == "ویدئو")
                            item.SaveFolderName = "Videos";
                        else if (item.Name == "تصاویر")
                            item.SaveFolderName = "Images";
                        else if (item.Name == "سند")
                            item.SaveFolderName = "Document";
                        else if (item.Name == "نرم افزار")
                            item.SaveFolderName = "Application";
                        else if (item.Name == "فشرده")
                            item.SaveFolderName = "Compress";
                        else
                            item.SaveFolderName = item.Name;
                    }
                }
                if (save)
                    Agrin.Download.Data.SerializeData.SaveGroupInfoesToFile();
            }
        }
    }
}
