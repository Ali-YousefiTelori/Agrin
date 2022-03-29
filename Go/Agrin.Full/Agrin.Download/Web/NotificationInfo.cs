using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web
{
    public enum NotificationMode
    {
        Complete,
        Error
    }
    public class NotificationInfo : ANotifyPropertyChanged
    {
        public string Text
        {
            get
            {
                if (Mode == NotificationMode.Complete)
                    return ApplicationHelperBase.GetAppResource("Complete_Language");
                else
                    return ApplicationHelperBase.GetAppResource("ErrorNotify_Language");
            }
        }

        public string DateText
        {
            get
            {
                return "";
            }
        }

        DateTime _NotifyDateTime;
        public DateTime NotifyDateTime
        {
            get { return _NotifyDateTime; }
            set { _NotifyDateTime = value; OnPropertyChanged("NotifyDateTime"); OnPropertyChanged("Text"); }
        }

        bool _IsRead;
        public bool IsRead
        {
            get { return _IsRead; }
            set { _IsRead = value; OnPropertyChanged("IsRead"); }
        }

        FastCollection<LinkInfo> _Items;
        public FastCollection<LinkInfo> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new FastCollection<LinkInfo>(ApplicationHelperBase.DispatcherThread);
                return _Items;
            }
            set { _Items = value; }
        }

        NotificationMode _Mode;
        public NotificationMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; OnPropertyChanged("Mode"); }
        }
    }
}