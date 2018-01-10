using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Settings
{
    public class ApplicationSettingPageBaseViewModel : ANotifyPropertyChanged
    {
        bool _IsShowAsudaWindow = true;

        public virtual bool IsShowAsudaWindow
        {
            get
            {
                return _IsShowAsudaWindow;
            }
            set
            {
                _IsShowAsudaWindow = value;
                OnPropertyChanged("IsShowAsudaWindow");
            }
        }
    }
}
