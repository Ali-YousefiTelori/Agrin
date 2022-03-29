using Agrin.BaseViewModels.Lists;
using Agrin.Download.Web;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Lists
{
    public class GroupsViewModel : GroupsBaseViewModel
    {
        public GroupsViewModel()
        {
            DeleteGroupsCommand = new RelayCommand(DeleteGroups, CanDeleteGroups);
            AddGroupCommand = new RelayCommand(AddGroup);
            EditGroupCommand = new RelayCommand<GroupInfo>(EditGroup);
            ChangeGroupSavePathCommand = new RelayCommand<GroupInfo>(ChangeGroupSavePath);
            OpenFolderLocationCommand = new RelayCommand<GroupInfo>(OpenFolderLocation, CanOpenFolderLocation);
        }

        public RelayCommand DeleteGroupsCommand { get; set; }
        public RelayCommand AddGroupCommand { get; set; }
        public RelayCommand<GroupInfo> EditGroupCommand { get; set; }
        public RelayCommand<GroupInfo> ChangeGroupSavePathCommand { get; set; }
        public RelayCommand<GroupInfo> OpenFolderLocationCommand { get; set; }

        RelayCommand _MessageCommand;
        public RelayCommand MessageCommand
        {
            get { return _MessageCommand; }
            set { _MessageCommand = value; OnPropertyChanged("MessageCommand"); }
        }

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; OnPropertyChanged("Message"); }
        }

        private string _MessageTitle;

        public string MessageTitle
        {
            get { return _MessageTitle; }
            set { _MessageTitle = value; OnPropertyChanged("MessageTitle"); }
        }

        bool _IsShowMessage;

        public bool IsShowMessage
        {
            get { return _IsShowMessage; }
            set { _IsShowMessage = value; OnPropertyChanged("IsShowMessage"); }
        }

        public override void DeleteGroups()
        {
            MessageCommand = new RelayCommand(() =>
            {
                base.DeleteGroups();
                IsShowMessage = false;
            });
            MessageTitle = "حذف گروه ها";
            Message = "به تعداد (" + GetSelectedItems().Count() + ") گروه انتخاب شده است.آیا میخواهید گروه های انتخاب شده را حذف کنید؟";
            IsShowMessage = true;
        }

        private void ChangeGroupSavePath(GroupInfo group)
        {
            var saveFileDialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = group.SavePath };
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                group.UserSavePath = saveFileDialog.SelectedPath;
                Agrin.Download.Data.SerializeData.SaveGroupInfoesToFile();
            }
        }

        public virtual void EditGroup(GroupInfo group)
        {

        }

        private bool CanOpenFolderLocation(GroupInfo group)
        {
            if (group == null)
                return false;
            return Directory.Exists(group.SavePath);
        }

        private void OpenFolderLocation(GroupInfo group)
        {
            if (Directory.Exists(group.SavePath))
            {
                try
                {
                    //System.Diagnostics.Process.Start("explorer.exe", "/select, " + linkInfo.PathInfo.FullAddressFileName);
                    System.Diagnostics.Process.Start("explorer.exe",group.SavePath + "\"");

                }
                catch
                {

                }
            }
        }
    }
}
