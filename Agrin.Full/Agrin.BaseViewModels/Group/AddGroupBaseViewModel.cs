using Agrin.Download.Manager;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Group
{
    public class AddGroupBaseViewModel : ANotifyPropertyChanged
    {
        public static Action BackItemClick { get; set; }

        string _GroupName;
        public string GroupName
        {
            get { return _GroupName; }
            set { _GroupName = value; OnPropertyChanged("GroupName"); }
        }

        string _Extentions;
        public string Extentions
        {
            get { return _Extentions; }
            set { _Extentions = value; OnPropertyChanged("Extentions"); }
        }

        string _SaveToPath;
        public string SaveToPath
        {
            get { return _SaveToPath; }
            set
            {
                _SaveToPath = value;
                OnPropertyChanged("SaveToPath");
            }
        }

        bool _IsEditMode = false;
        public bool IsEditMode
        {
            get { return _IsEditMode; }
            set { _IsEditMode = value; OnPropertyChanged("IsEditMode"); }
        }

        List<string> GetExtentions()
        {
            List<string> extentions = new List<string>();
            foreach (var item in Extentions.ToLower().Trim().Split(new char[] { ',' }))
            {
                var ext = item.Trim();
                if (!extentions.Contains(ext) && !string.IsNullOrWhiteSpace(ext))
                    extentions.Add(ext);
            }
            return extentions;
        }

        public void AddGroup()
        {
            var group = new Download.Web.GroupInfo() { Name = GroupName, Extentions = GetExtentions() };
            if (!MPath.EqualPath(SaveToPath, group.SavePath) && !string.IsNullOrEmpty(SaveToPath))
                group.UserSavePath = SaveToPath;
            ApplicationGroupManager.Current.AddGroupInfo(group);
            BackItem();
        }

        public bool CanAddGroup()
        {
            return !IsEditMode && MPath.IsValidDirectoryPath(SaveToPath) && !string.IsNullOrWhiteSpace(GroupName);
        }

        public void Clear()
        {
            IsEditMode = false;
            //EditGroupInfo = null;
            GroupName = "";
            SaveToPath = "";
            Extentions = "";
        }



        public virtual void BrowseFile()
        {

        }

        public void BackItem()
        {
            Clear();
            if (BackItemClick != null)
                BackItemClick();
        }
    }
}
