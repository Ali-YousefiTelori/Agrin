using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.Pages.Settings
{
    public class ApplicationViewSettingViewModel : ANotifyPropertyChanged
    {
        public static ApplicationViewSettingViewModel This;

        public ApplicationViewSettingViewModel()
        {
            This = this;
        }

        bool _IsShowNotification;
        public bool IsShowNotification
        {
            get { return _IsShowNotification; }
            set { _IsShowNotification = value; OnPropertyChanged("IsShowNotification"); }
        }
    }
}
