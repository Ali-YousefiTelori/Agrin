using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.UserControls
{
    public class WindowMessegeBoxViewModel : ANotifyPropertyChanged
    {
        public WindowMessegeBoxViewModel()
        {
            OKCommand = new RelayCommand(OKAction);
            CancelCommand = new RelayCommand(CancelAction);
        }

        public RelayCommand OKCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public Action<bool> ActionMessage;

        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        private void CancelAction()
        {
            ActionMessage(false);
            MainWindow.This.IsShowMessege = false;
        }

        private void OKAction()
        {
            ActionMessage(true);
            MainWindow.This.IsShowMessege = false;
        }

        public void ShowMessage(Action<bool> action, string title, string message)
        {
            Title = title;
            Message = message;
            ActionMessage = action;
        }
    }
}
