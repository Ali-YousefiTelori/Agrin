using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.Pages.Settings
{
    public class SpeedSettingViewModel : ANotifyPropertyChanged
    {
        public SpeedSettingViewModel()
        {
            This = this;
        }

        public static SpeedSettingViewModel This;

        int _ConnectionCount;
        public int ConnectionCount
        {
            get { return _ConnectionCount; }
            set { _ConnectionCount = value; OnPropertyChanged("ConnectionCount"); }
        }

        int _BufferSize;
        public int BufferSize
        {
            get { return _BufferSize; }
            set { _BufferSize = value; OnPropertyChanged("BufferSize"); }
        }

        int _SpeedSize;
        public int SpeedSize
        {
            get { return _SpeedSize; }
            set { _SpeedSize = value; OnPropertyChanged("SpeedSize"); }
        }

        bool _IsLimit;
        public bool IsLimit
        {
            get { return _IsLimit; }
            set { _IsLimit = value; OnPropertyChanged("IsLimit"); }
        }
    }
}
