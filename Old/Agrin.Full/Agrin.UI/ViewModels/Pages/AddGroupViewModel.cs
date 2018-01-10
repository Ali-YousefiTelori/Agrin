using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using Agrin.ViewModels.Helper.ComponentModel;

namespace Agrin.UI.ViewModels.Pages
{
    public class AddGroupViewModel : ANotifyPropertyChanged
    {
        public AddGroupViewModel()
        {
            AddGroupCommand = new RelayCommand(AddGroup, CanAddGroup);
            EditGroupCommand = new RelayCommand(EditGroup, CanEditGroup);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            This = this;
        }

        public static AddGroupViewModel This;

        public RelayCommand AddGroupCommand { get; set; }
        public RelayCommand EditGroupCommand { get; set; }
        public RelayCommand BrowseFileCommand { get; set; }

        bool _IsEditMode = false;
        public bool IsEditMode
        {
            get { return _IsEditMode; }
            set { _IsEditMode = value; }
        }

        string _GroupName;
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                _GroupName = value;
                OnPropertyChanged("GroupName");
            }
        }

        string _SaveToPath;
        public string SaveToPath
        {
            get { return _SaveToPath; }
            set { _SaveToPath = value; OnPropertyChanged("SaveToPath"); }
        }

        string _extentions = "";

        public string Extentions
        {
            get { return _extentions; }
            set { _extentions = value; OnPropertyChanged("Extentions"); }
        }

        GroupInfo _EditGroupInfo;
        public GroupInfo EditGroupInfo
        {
            get { return _EditGroupInfo; }
            set
            {
                _EditGroupInfo = value;
                OnPropertyChanged("EditGroupInfo");
                if (value != null)
                {
                    GroupName = value.Name;
                    SaveToPath = value.SavePath;
                    Extentions = "";
                    if (value.Extentions != null)
                        for (int i = 0; i < value.Extentions.Count; i++)
                        {
                            Extentions += value.Extentions[i] + (i == value.Extentions.Count - 1 ? "" : ",");
                        }
                }

            }
        }

        private void BrowseFile()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog() { SelectedPath = SaveToPath };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (MPath.IsValidDirectoryPath(dialog.SelectedPath))
                {
                    SaveToPath = dialog.SelectedPath;
                }
            }
        }

        public void Clear()
        {
            IsEditMode = false;
            EditGroupInfo = null;
            GroupName = "";
            SaveToPath = "";
            Extentions = "";
            PagesManagerViewModel.This.BackItem();
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

        void AddGroup()
        {
            ApplicationGroupManager.Current.AddGroupInfo(new GroupInfo() { Name = GroupName, SavePath = SaveToPath, Extentions = GetExtentions() });
            Clear();
        }

        bool CanAddGroup()
        {
            return !IsEditMode && MPath.IsValidDirectoryPath(SaveToPath) && !string.IsNullOrWhiteSpace(GroupName);
        }

        private bool CanEditGroup()
        {
            return IsEditMode && EditGroupInfo != null && MPath.IsValidDirectoryPath(SaveToPath) && !string.IsNullOrWhiteSpace(GroupName);
        }

        private void EditGroup()
        {
            EditGroupInfo.Name = GroupName;
            EditGroupInfo.SavePath = SaveToPath;
            EditGroupInfo.Extentions = GetExtentions();
            ApplicationGroupManager.Current.SaveEditGroup(EditGroupInfo);
            Clear();
        }
    }
}
