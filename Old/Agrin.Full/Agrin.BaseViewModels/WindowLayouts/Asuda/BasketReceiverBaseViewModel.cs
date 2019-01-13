using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if (!MobileApp && !XamarinApp && !__ANDROID__)

namespace Agrin.BaseViewModels.WindowLayouts.Asuda
{
    public enum AlignMode
    {
        TopRight = 0,
        TopLeft = 1,
        BottomRight = 2,
        BottomLeft = 3
    }

    public class BasketReceiverBaseViewModel : AsudaDataOptimizerBaseViewModel
    {
        public BasketReceiverBaseViewModel()
        {
            base.IgnoreStopChanged = true;
        }

        public Action<bool> IsShowListChanged { get; set; }

        bool _IsShowList = false;
        AlignMode _ArcAlignMode = AlignMode.BottomLeft;
        AlignMode _OldArcAlignMode = AlignMode.BottomLeft;
        bool _IsShowSettingPage;
        bool _IsShowListPage = true;

        public bool IsShowList
        {
            get
            {
                return _IsShowList;
            }
            set
            {
                _IsShowList = value;
                IsShowListChanged?.Invoke(value);
                OnPropertyChanged("IsShowList");
            }
        }



        public AlignMode ArcAlignMode
        {
            get
            {
                return _ArcAlignMode;
            }
            set
            {
                if (_ArcAlignMode == value)
                    return;
                OldArcAlignMode = _ArcAlignMode;
                _ArcAlignMode = value;
                OnPropertyChanged("ArcAlignMode");
                OnPropertyChanged("OldArcAlignMode");
                //OnPropertyChanged("OldArcAlignMode");
            }
        }

        public AlignMode OldArcAlignMode
        {
            get
            {
                return _OldArcAlignMode;
            }
            set
            {
                _OldArcAlignMode = value;
            }
        }

        public bool IsShowSettingPage
        {
            get
            {
                return _IsShowSettingPage;
            }
            set
            {
                _IsShowSettingPage = value;
                OnPropertyChanged("IsShowSettingPage");
            }
        }

        public bool IsShowListPage
        {
            get
            {
                return _IsShowListPage;
            }
            set
            {
                _IsShowListPage = value;
                OnPropertyChanged("IsShowListPage");
            }
        }

        public void ShowList()
        {
            IsShowList = !IsShowList;
        }

        public bool CanShowSettingPage()
        {
            return !IsShowSettingPage;
        }

        void ShowPage(bool isShowSettingPage = false, bool isShowListPage = false)
        {
            IsShowSettingPage = isShowSettingPage;
            IsShowListPage = isShowListPage;
        }

        public void ShowSettingPage()
        {
            ShowPage(isShowSettingPage: true);
        }


        public bool CanShowListPage()
        {
            return !IsShowListPage;
        }

        public void ShowListPage()
        {
            ShowPage(isShowListPage: true);
        }
    }
}
#endif