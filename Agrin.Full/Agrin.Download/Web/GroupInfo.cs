using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web
{
    public class GroupInfo : ANotifyPropertyChanged
    {
        public Action SelectionChanged;

        bool _IsSelected = true;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                if (SelectionChanged != null)
                    SelectionChanged();
                Engine.SearchEngine.Search();
                OnPropertyChanged("IsSelected");
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        string _saveFolderName = "";

        public string SaveFolderName
        {
            get { return _saveFolderName; }
            set { _saveFolderName = value; }
        }

        public string AppSecurityPath
        {
            get
            {
                return Agrin.IO.Helper.MPath.SecurityPath;
            }
        }

        string _UserSecurityPath;

        public string UserSecurityPath
        {
            get { return _UserSecurityPath; }
            set
            {
                _UserSecurityPath = value;
                OnPropertyChanged("UserSecurityPath");
            }
        }

        public string SecurityPath
        {
            get 
            {
                string path = "";
                if (string.IsNullOrEmpty(UserSecurityPath))
                {
                    path = AppSecurityPath;
                }
                else
                    path = UserSecurityPath;
                return path;
            }
        }

        public string AppSavePath
        {
            get { return Agrin.IO.Helper.MPath.DownloadsPath; }
        }

        string _userSavePath = "";
        public string UserSavePath
        {
            get { return _userSavePath; }
            set { _userSavePath = value; OnPropertyChanged("UserSavePath"); }
        }

        public string SavePath
        {
            get
            {
                string path = "";
                if (!string.IsNullOrEmpty(UserSecurityPath))
                    return UserSecurityPath;
                if (string.IsNullOrEmpty(UserSavePath))
                {
                    path = AppSavePath;
                }
                else
                    path = UserSavePath;
                if (!string.IsNullOrEmpty(SaveFolderName))
                    path = System.IO.Path.Combine(path, SaveFolderName);
                return path;
            }
            //set
            //{
            //    _SavePath = value;
            //    OnPropertyChanged("SavePath");
            //    if (Agrin.Download.Manager.ApplicationGroupManager.Current != null)
            //        Agrin.Download.Manager.ApplicationGroupManager.Current.ChangedGroupSavePath(this);
            //}
        }

        long _id;
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        List<string> _extentions = new List<string>();
        public List<string> Extentions
        {
            get { return _extentions; }
            set { _extentions = value; }
        }

        public string TextExtentions
        {
            get
            {
                string text = "";
                int i = 1;
                foreach (var item in Extentions)
                {
                    string v = i == Extentions.Count ? "" : ",";
                    text += item + v;
                    i++;
                }
                return text;
            }
        }

        bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                _IsExpanded = value;
                OnPropertyChanged("IsExpanded");
                Agrin.Download.Data.SerializeData.SaveGroupInfoesToFile();
            }
        }

        public void Validate()
        {
            OnPropertyChanged("TextExtentions");
        }
    }
}
