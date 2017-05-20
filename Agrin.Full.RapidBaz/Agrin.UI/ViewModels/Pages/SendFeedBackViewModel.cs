using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agrin.UI.ViewModels.Pages
{
    public class SendFeedBackViewModel : ANotifyPropertyChanged
    {
        public SendFeedBackViewModel()
        {
            SendCommand = new RelayCommand(Send, CanSend);
        }

        private bool CanSend()
        {
            return !string.IsNullOrWhiteSpace(Content) && !string.IsNullOrWhiteSpace(Content);
        }

        public RelayCommand SendCommand { get; set; }

        private void Send()
        {
            IsEnabled = false;
            SendingMessage = "در حال ارسال...";
            AsyncActions.Action(() =>
            {
                bool isSend = false;
                try
                {
                    Assembly assembly = Assembly.Load(Agrin.UI.AgrinRsourceFiles.Agrin_About);
                    var msg = Activator.CreateInstance(assembly.GetType("Agrin.About.SendMessage"));
                    MethodInfo method = msg.GetType().GetMethod("SendMessages", BindingFlags.Public | BindingFlags.Instance);
                    isSend = (bool)method.Invoke(msg, new object[] { Name, Mail, Title, Content, null });
                    //Agrin.About.SendMessage a = new About.SendMessage();
                    //a.SendMessages(Name, Mail, Title, Content);
                }
                catch
                {
                }
                finally
                {

                    IsEnabled = true;
                    if (isSend)
                    {
                        SendingMessage = "با تشکر از شما پیغام شما با موفقیت ارسال شد در صورت لزوم به ایمیل شما پاسخ داده خواهد شد.";
                        Name = Mail = Content = Title = "";
                    }
                    else
                    {
                        SendingMessage = "خطا در ارسال پیغام شما رخ داده است.";
                    }
                }
            });
        }

        bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        string _SendingMessage = "نظرات و پیشنهادات شما برای ما ارزشمند هستند ما به پیشنهادات شما پاسخ می دهیم.";
        public string SendingMessage
        {
            get { return _SendingMessage; }
            set { _SendingMessage = value; OnPropertyChanged("SendingMessage"); }
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        string _mail;
        public string Mail
        {
            get { return _mail; }
            set { _mail = value; OnPropertyChanged("Mail"); }
        }

        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; OnPropertyChanged("Content"); }
        }
    }
}
