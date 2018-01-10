using Agrin.BaseViewModels.Help;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Help
{
    public class FeedBackViewModel : FeedBackBaseViewModel
    {
        public FeedBackViewModel()
        {
            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            ShowLastMessageCommand = new RelayCommand(ShowLastMessage);
        }

        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand ShowLastMessageCommand { get; set; }
    }
}
