using Agrin.Download.Data.Settings;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Help
{
    public class FeedBackBaseViewModel : ANotifyPropertyChanged
    {
        string _Message = "";

        public string Message
        {
            get { return _Message; }
            set { _Message = value; OnPropertyChanged("Message"); }
        }

        bool _IsBusy = false;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged("IsBusy"); }
        }

        string _BusyMessage = "";

        public string BusyMessage
        {
            get { return _BusyMessage; }
            set { _BusyMessage = value; OnPropertyChanged("BusyMessage"); }
        }

        string _MessageBoxMessage = "";

        public string MessageBoxMessage
        {
            get { return _MessageBoxMessage; }
            set { _MessageBoxMessage = value; OnPropertyChanged("MessageBoxMessage"); }
        }

        bool _IsMessageBoxBusy = false;

        public bool IsMessageBoxBusy
        {
            get { return _IsMessageBoxBusy; }
            set { _IsMessageBoxBusy = value; OnPropertyChanged("IsMessageBoxBusy"); }
        }

        public bool CanSendMessage()
        {
            return !string.IsNullOrEmpty(Message);
        }

        public void Clear()
        {
            Message = "";
        }

        public void SendMessage()
        {
            BusyMessage = "در حال ارسال پیام...";
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                var send = Agrin.About.SendMessage.SendFeedBack(new About.UserMessage() { GUID = ApplicationSetting.Current.ApplicationOSSetting.ApplicationGuid, message = Message, LastUserMessageID = ApplicationSetting.Current.LastUserMessageID });
                if (send)
                {
                    MessageBoxMessage = "پیام شما با موفقیت ارسال شد.";
                    Clear();
                }
                else
                {
                    MessageBoxMessage = "خطا در ارسال پیام رخ داده است لطفاً بعداً مجدداً سعی کنید.";
                }

                IsMessageBoxBusy = true;
                IsBusy = false;
            }, (ex) =>
            {
                MessageBoxMessage = "خطا در ارسال پیام رخ داده است لطفاً بعداً مجدداً سعی کنید.";
                IsMessageBoxBusy = true;
                IsBusy = false;
            });
        }

        public void ShowLastMessage()
        {
            BusyMessage = "در حال دریافت پیام...";
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                var send = Agrin.Download.Engine.TimeDownloadEngine.GetLastUserMessage();
                if (send == "Not Found")
                {
                    MessageBoxMessage = "هیچ پیامی برای شما پیدا نشد";
                }
                else
                {
                    MessageBoxMessage = send;
                }
                IsMessageBoxBusy = true;
                IsBusy = false;
            }, (ex) =>
            {
                MessageBoxMessage = "خطا در دریافت پیام رخ داده است لطفاً بعداً مجدداً سعی کنید.";
                IsMessageBoxBusy = true;
                IsBusy = false;
            });
        }
    }
}
