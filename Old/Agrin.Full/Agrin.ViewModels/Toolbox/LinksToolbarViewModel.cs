using Agrin.BaseViewModels.Toolbox;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Toolbox
{
    public class LinksToolbarViewModel : LinksToolbarBaseViewModel
    {
        public LinksToolbarViewModel()
        {
            PlayLinksCommand = new RelayCommand(PlayLinks, CanPlayLinks);
            StopLinksCommand = new RelayCommand(StopLinks, CanStopLinks);
            DeleteLinksCommand = new RelayCommand(DeleteLinks, CanDeleteLinks);
            SettingCommand = new RelayCommand(SettingLinks);
            AddLinkCommand = new RelayCommand(AddLink);
        }

        public RelayCommand AddLinkCommand { get; set; }
        public RelayCommand PlayLinksCommand { get; set; }
        public RelayCommand StopLinksCommand { get; set; }
        public RelayCommand DeleteLinksCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }

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

        public override void DeleteLinks()
        {
            MessageCommand = new RelayCommand(() =>
            {
                base.DeleteLinks();
                IsShowMessage = false;
            });
            MessageTitle = "حذف لینک ها";
            Message = "به تعداد (" + GetSelectedLinks().Count() + ") لینک انتخاب شده است.آیا میخواهید لینک های انتخاب شده را حذف کنید؟";
            IsShowMessage = true;
        }

        public virtual void AddLink()
        {

        }
    }
}
