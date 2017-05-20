using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.Pages.Settings
{
    public class LinkInfoDownloadSettingViewModel : ANotifyPropertyChanged
    {
        public LinkInfoDownloadSettingViewModel()
        {
            This = this;
            EndDownloadSelectedIndex = 0;
        }

        public static LinkInfoDownloadSettingViewModel This;

        bool _IsExtreme;
        public bool IsExtreme
        {
            get { return _IsExtreme; }
            set { _IsExtreme = value; OnPropertyChanged("IsExtreme"); }
        }

        bool _isEndDownloaded;
        public bool IsEndDownloaded
        {
            get { return _isEndDownloaded; }
            set { _isEndDownloaded = value; OnPropertyChanged("IsEndDownloaded"); }
        }

        int _endDownloadSelectedIndex;

        public int EndDownloadSelectedIndex
        {
            get { return _endDownloadSelectedIndex; }
            set
            {
                _endDownloadSelectedIndex = value;
                OnPropertyChanged("EndDownloadSelectedIndex");
            }
        }

        bool _IsShowBallon;
        public bool IsShowBallon
        {
            get { return _IsShowBallon; }
            set { _IsShowBallon = value; OnPropertyChanged("IsShowBallon"); }
        }

        int _TryException;

        public int TryException
        {
            get { return _TryException; }
            set { _TryException = value; OnPropertyChanged("TryException"); }
        }
    }
}
