using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.UI.Views.Popups;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Agrin.UI.ViewModels.Popups
{
    public class BalloonViewModel : ANotifyPropertyChanged<Balloon>
    {
        public static BalloonViewModel This;
        public BalloonViewModel()
        {
            This = this;
            bool loaded = false;
            ViewElementInited = () =>
                {
                    if (!loaded)
                        Initialize();
                    loaded = true;
                };
        }
        public Window balloonWindow;
        public void Initialize()
        {
            balloonWindow = new Window() { DataContext = this, Background = System.Windows.Media.Brushes.Transparent, Topmost = true, SizeToContent = SizeToContent.WidthAndHeight, Content = ViewElement, MaxWidth = ViewElement.Width, MaxHeight = ViewElement.Height, Width = ViewElement.Width, Height = ViewElement.Height, ShowInTaskbar = false, WindowStyle = WindowStyle.None, ResizeMode = ResizeMode.NoResize, AllowsTransparency = true };
            balloonWindow.Closing += (s, e) =>
            {
                e.Cancel = true;
                ApplicationBalloonManager.Current.Clear();
                ShowControlEnabled = false;
            };
            ApplicationBalloonManager.Current.ShowBalloonAction = (isShow) =>
            {
                if (isShow == BalloonMode.Changed || isShow == BalloonMode.Show)
                {
                    CurrentIndex = ApplicationBalloonManager.Current.BalloonLinkInfoes.IndexOf(CurrentLinkInfo);
                    OnPropertyChanged("CurrentLinkInfo");
                    if (isShow == BalloonMode.Show)
                    {
                        Show();
                    }
                }
                if (isShow != BalloonMode.Changed)
                    ShowControlEnabled = isShow == BalloonMode.Show;
                CommandManager.InvalidateRequerySuggested();
            };
            ViewElement.mainGrid.Margin = new Thickness(380, 0, -380, 0);
            ViewElement.mainGrid.Visibility = Visibility.Collapsed;
            CloseDataCommand = new RelayCommand(CloseData);
            NextLinkCommand = new RelayCommand(NextLink, CanNextLink);
            BackLinkCommand = new RelayCommand(BackLink, CanNextLink);
            OpenFileLocationCommand = new RelayCommand(OpenFileLocation, CanOpenFileLocation);
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            SettingCommand = new RelayCommand(Setting);
            CloseSettingCommand = new RelayCommand(CloseSetting);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            SaveSettingCommand = new RelayCommand(SaveSetting);
            OnPropertyChanged("CloseDataCommand");
            OnPropertyChanged("NextLinkCommand");
            OnPropertyChanged("BackLinkCommand");
            OnPropertyChanged("OpenFileLocationCommand");
            OnPropertyChanged("OpenFileCommand");
            OnPropertyChanged("SettingCommand");
            OnPropertyChanged("RefreshCommand");
            OnPropertyChanged("CloseSettingCommand");
            OnPropertyChanged("SaveSettingCommand");
        }

        public void Show()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            balloonWindow.Left = desktopWorkingArea.Right - balloonWindow.Width;
            balloonWindow.Top = desktopWorkingArea.Bottom - balloonWindow.Height;

            balloonWindow.Show();
            ShowControlEnabled = true;
        }

        public RelayCommand CloseDataCommand { get; set; }
        public RelayCommand NextLinkCommand { get; set; }
        public RelayCommand BackLinkCommand { get; set; }
        public RelayCommand OpenFileLocationCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }
        public RelayCommand CloseSettingCommand { get; set; }
        public RelayCommand SaveSettingCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }

        public LinkInfo CurrentLinkInfo
        {
            get
            {
                if (ApplicationBalloonManager.Current.CurrentLinkInfo != null)
                    IsComplete = ApplicationBalloonManager.Current.CurrentLinkInfo.DownloadingProperty.State == ConnectionState.Complete;
                return ApplicationBalloonManager.Current.CurrentLinkInfo;
            }
        }

        int _CurrentIndex;
        public int CurrentIndex
        {
            get { return _CurrentIndex; }
            set { _CurrentIndex = value; OnPropertyChanged("CurrentIndex"); }
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

        bool _IsComplete;
        public bool IsComplete
        {
            get { return _IsComplete; }
            set { _IsComplete = value; OnPropertyChanged("IsComplete"); }
        }
        public static void CreateBalloon()
        {
            if (BalloonViewModel.This == null)
            {
                Balloon blo = new Balloon();
            }
        }
        private void CloseData()
        {
            balloonWindow.Close();
        }
        private void BackLink()
        {
            ApplicationBalloonManager.Current.Back();
            CurrentIndex = ApplicationBalloonManager.Current.BalloonLinkInfoes.IndexOf(CurrentLinkInfo);
            OnPropertyChanged("CurrentLinkInfo");
        }
        private void NextLink()
        {
            ApplicationBalloonManager.Current.Next();
            CurrentIndex = ApplicationBalloonManager.Current.BalloonLinkInfoes.IndexOf(CurrentLinkInfo);
            OnPropertyChanged("CurrentLinkInfo");
        }
        private bool CanOpenFile()
        {
            return CurrentLinkInfo != null && IsComplete && File.Exists(CurrentLinkInfo.PathInfo.FullAddressFileName);
        }
        private bool CanOpenFileLocation()
        {
            return CurrentLinkInfo != null && IsComplete && File.Exists(CurrentLinkInfo.PathInfo.FullAddressFileName);
        }
        private bool CanNextLink()
        {
            return ApplicationBalloonManager.Current.BalloonLinkInfoes.Count > 1;
        }
        private void Setting()
        {
            IsShowBalloonAppSetting = Agrin.Download.Data.Settings.ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon;
            ShowSetting = true;
        }
        private void CloseSetting()
        {
            ShowSetting = false;
        }
        private void SaveSetting()
        {
            Agrin.Download.Data.Settings.ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon = IsShowBalloonAppSetting;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            CloseSetting();
        }
        private bool CanRefresh()
        {
            return CurrentLinkInfo != null && CurrentLinkInfo.DownloadingProperty.State == ConnectionState.Error;
        }

        private void Refresh()
        {
            CurrentLinkInfo.Play(true);
        }

        private void OpenFile()
        {
            try
            {
                Process.Start(CurrentLinkInfo.PathInfo.FullAddressFileName);
            }
            catch
            {

            }
        }
        private void OpenFileLocation()
        {
            try
            {
                Process.Start("explorer.exe", "/select, \"" + CurrentLinkInfo.PathInfo.FullAddressFileName + "\"");
            }
            catch
            {

            }
        }
    }
}
