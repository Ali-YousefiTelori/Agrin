using Agrin.BaseViewModels.Lists;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
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
        }

        public RelayCommand DeleteGroupsCommand { get; set; }
        public RelayCommand AddGroupCommand { get; set; }

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
    }
}
