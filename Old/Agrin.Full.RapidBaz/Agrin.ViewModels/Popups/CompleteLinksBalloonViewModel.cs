using Agrin.BaseViewModels.Popups;
using Agrin.Download.Manager;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Agrin.ViewModels.Popups
{
    public class CompleteLinksBalloonViewModel : CompleteLinksBalloonBaseViewModel
    {
        public Window balloonWindow;
        double width = 380.0, height = 126.0;
        public FrameworkElement CurrentContentElement { get; set; }
        public void Initialize()
        {
            balloonWindow = new Window() { DataContext = this, Background = System.Windows.Media.Brushes.Transparent, Topmost = true, SizeToContent = SizeToContent.WidthAndHeight, Content = CurrentContentElement, MaxWidth = width, MaxHeight = height, Width = width, Height = height, ShowInTaskbar = false, WindowStyle = WindowStyle.None, ResizeMode = ResizeMode.NoResize, AllowsTransparency = true, Title = "RapidbazPlus Balloon" };
            balloonWindow.Closing += (s, e) =>
            {
                e.Cancel = true;
                ApplicationBalloonManager.Current.Clear();
                ShowControlEnabled = false;
            };

            ApplicationBalloonManager.Current.ShowBalloonAction = (isShow) =>
            {
                if (CurrentLinkInfo != null && !CurrentLinkInfo.IsComplete)
                    return;
                if (isShow == BalloonMode.Changed || isShow == BalloonMode.Show)
                {
                    CurrentIndex = ApplicationBalloonManager.Current.BalloonLinkInfoes.IndexOf(CurrentLinkInfo) + 1;
                    Count = ApplicationBalloonManager.Current.BalloonLinkInfoes.Where((info) => info.IsComplete).Count();
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

            //ViewElemet.mainGrid.Margin = new Thickness(380, 0, -380, 0);
            //ViewElement.mainGrid.Visibility = Visibility.Collapsed;
            CloseDataCommand = new RelayCommand(CloseData);
            NextLinkCommand = new RelayCommand(NextLink, CanNextLink);
            BackLinkCommand = new RelayCommand(BackLink, CanNextLink);
            OpenFileLocationCommand = new RelayCommand(OpenFileLocation, CanOpenFileLocation);
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
            SettingCommand = new RelayCommand(Setting);
            CloseSettingCommand = new RelayCommand(CloseSetting);
            //RefreshCommand = new RelayCommand(Refresh, CanRefresh);
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
        //public RelayCommand RefreshCommand { get; set; }

        //public static void CreateBalloon()
        //{
        //    if (BalloonViewModel.This == null)
        //    {
        //        Balloon blo = new Balloon();
        //    }
        //}

        private void CloseData()
        {
            balloonWindow.Close();
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
