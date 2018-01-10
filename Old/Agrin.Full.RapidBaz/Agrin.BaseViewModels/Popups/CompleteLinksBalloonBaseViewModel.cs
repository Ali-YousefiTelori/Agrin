using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Popups
{
    public class CompleteLinksBalloonBaseViewModel : ANotifyPropertyChanged
    {
        public CompleteLinksBalloonBaseViewModel()
        {
            IgnoreStopChanged = true;
        }

        public LinkInfo CurrentLinkInfo
        {
            get
            {
                //if (ApplicationBalloonManager.Current.CurrentLinkInfo != null)
                //    IsComplete = ApplicationBalloonManager.Current.CurrentLinkInfo.DownloadingProperty.State == ConnectionState.Complete;
                return ApplicationBalloonManager.Current.CurrentLinkInfo;
            }
        }

        int _CurrentIndex;
        public int CurrentIndex
        {
            get { return _CurrentIndex; }
            set { _CurrentIndex = value; OnPropertyChanged("CurrentIndex"); }
        }

        int _Count = 0;

        public int Count
        {
            get { return _Count; }
            set { _Count = value; OnPropertyChanged("Count"); }
        }

        bool _ShowControlEnabled = false;
        public bool ShowControlEnabled
        {
            get { return _ShowControlEnabled; }
            set { _ShowControlEnabled = value; OnPropertyChanged("ShowControlEnabled"); }
        }

        bool _ShowSetting;
        public bool ShowSetting
        {
            get { return _ShowSetting; }
            set { _ShowSetting = value; OnPropertyChanged("ShowSetting"); }
        }

        bool _IsShowBalloonAppSetting;
        public bool IsShowBalloonAppSetting
        {
            get { return _IsShowBalloonAppSetting; }
            set { _IsShowBalloonAppSetting = value; OnPropertyChanged("IsShowBalloonAppSetting"); }
        }

        public bool CanOpenFile()
        {
            return CurrentLinkInfo != null && File.Exists(CurrentLinkInfo.PathInfo.FullAddressFileName);
        }

        public bool CanOpenFileLocation()
        {
            return CurrentLinkInfo != null && File.Exists(CurrentLinkInfo.PathInfo.FullAddressFileName);
        }

        public bool CanNextLink()
        {
            return ApplicationBalloonManager.Current.BalloonLinkInfoes.Count > 1;
        }

        public void Setting()
        {
            IsShowBalloonAppSetting = Agrin.Download.Data.Settings.ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon;
            ShowSetting = true;
        }

        public void CloseSetting()
        {
            ShowSetting = false;
        }

        public void SaveSetting()
        {
            Agrin.Download.Data.Settings.ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon = IsShowBalloonAppSetting;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            CloseSetting();
        }


        public void BackLink()
        {
            ApplicationBalloonManager.Current.Back();
            CurrentIndex = ApplicationBalloonManager.Current.BalloonLinkInfoes.IndexOf(CurrentLinkInfo) + 1;
            OnPropertyChanged("CurrentLinkInfo");
        }

        public void NextLink()
        {
            ApplicationBalloonManager.Current.Next();
            CurrentIndex = ApplicationBalloonManager.Current.BalloonLinkInfoes.IndexOf(CurrentLinkInfo) + 1;
            OnPropertyChanged("CurrentLinkInfo");
        }
        //private bool CanRefresh()
        //{
        //    return CurrentLinkInfo != null && CurrentLinkInfo.DownloadingProperty.State == ConnectionState.Error;
        //}

        //private void Refresh()
        //{
        //    CurrentLinkInfo.Play(true);
        //}
    }
}
