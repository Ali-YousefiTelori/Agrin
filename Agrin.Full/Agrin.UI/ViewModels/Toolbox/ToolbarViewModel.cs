using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.UI.ViewModels.Downloads;
using Agrin.UI.ViewModels.Pages;
using Agrin.UI.ViewModels.Pages.Settings;
using Agrin.UI.Views.Toolbox;
using Agrin.UI.Views.UserControls;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Agrin.UI.ViewModels.Toolbox
{
    public class ToolbarViewModel : ANotifyPropertyChanged
    {
        public ToolbarViewModel()
        {
            if (MainWindow.This == null)
                return;
            AddLinkPageCommand = new RelayCommand(AddLinkPage);
            PlayLinksCommand = new RelayCommand(PlayLinks, CanPlayLinks);
            PauseLinksCommand = new RelayCommand(PauseLinks, CanPauseLinks);
            StopLinksCommand = new RelayCommand(StopLinks, CanStopLinks);
            DeleteLinksCommand = new RelayCommand(DeleteLinks, CanDeleteLinks);
            PinCommand = new RelayCommand(Pin);
            SettingCommand = new RelayCommand(Setting);
            MainWindow.This.IsToolbarPin = true;
            Pin();
            This = this;
        }

        public static ToolbarViewModel This;
        public RelayCommand AddLinkPageCommand { get; set; }
        public RelayCommand PlayLinksCommand { get; set; }
        public RelayCommand PauseLinksCommand { get; set; }
        public RelayCommand StopLinksCommand { get; set; }
        public RelayCommand PinCommand { get; set; }
        public RelayCommand DeleteLinksCommand { get; set; }
        public RelayCommand SettingCommand { get; set; }
        ControlTemplate _PinStyle;
        public ControlTemplate PinStyle
        {
            get { return _PinStyle; }
            set { _PinStyle = value; OnPropertyChanged("PinStyle"); }
        }

        void AddLinkPage()
        {
            AddLinksViewModel.This.ClearItems();
            PagesManagerViewModel.SetIndex(0);
            MainWindow.This.IsShowPage = true;
        }

        void Pin()
        {
            MainWindow.This.IsToolbarPin = !MainWindow.This.IsToolbarPin;
            PinStyle = MainWindow.This.IsToolbarPin ? ApplicationHelper.GetAppResource<ControlTemplate>("UnPin_TemplateStyle") : ApplicationHelper.GetAppResource<ControlTemplate>("Pin_TemplateStyle");
        }

        public static List<LinkInfo> GetListLinks()
        {
            List<LinkInfo> links = new List<LinkInfo>();
            if (LinkInfoesDownloadingManagerViewModel.This.IsShowLinkInfoDetail)
            {
                links.Add(LinkInfoesDownloadingManagerViewModel.This.DetailClickedLinkInfo);
            }
            else if (LinkInfoesDownloadingManagerViewModel.This.IsShowListLinks)
            {
                //if (LinkInfoesDownloadingManagerViewModel.This.ViewElement == null)
                //    return false;
                if (LinkInfoesDownloadingManagerViewModel.This.ViewElement == null)
                    return links;
                foreach (var item in LinkInfoesDownloadingManagerViewModel.This.ViewElement.linksListData.mainDataGrid.SelectedItems.Cast<LinkInfo>())
                {
                    links.Add(item);
                }
            }
            return links;
        }

        private bool CanPlayLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                if (LinkInfoesDownloadingManagerViewModel.This.IsShowLinkInfoDownloading)
                {
                    return false;
                }
                foreach (var item in GetListLinks())
                {
                    if (item.CanPlay)
                        return true;
                }
            }
            return false;
        }
        private void PlayLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                foreach (var item in GetListLinks())
                {
                    if (item.CanPlay)
                        ApplicationLinkInfoManager.Current.PlayLinkInfo(item);
                }
            }
        }

        private bool CanPauseLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                if (LinkInfoesDownloadingManagerViewModel.This.IsShowLinkInfoDownloading)
                {
                    return false;
                }
                foreach (var item in GetListLinks())
                {
                    if (item.CanPause)
                        return true;
                }
            }
            return false;
        }

        private void PauseLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                foreach (var item in GetListLinks())
                {
                    if (item.CanPause)
                        ApplicationLinkInfoManager.Current.PauseLinkInfo(item);
                }
            }
        }

        private bool CanStopLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                if (LinkInfoesDownloadingManagerViewModel.This.IsShowLinkInfoDownloading)
                {
                    return false;
                }
                foreach (var item in GetListLinks())
                {
                    if (item.CanStop)
                        return true;
                }
            }
            return false;
        }

        void BackToMain()
        {
            LinkInfoesDownloadingManagerViewModel.This.CloseDetailedCommand.Execute();
            AsyncActions.Action(() =>
            {
                System.Threading.Thread.Sleep(500);
                ApplicationHelper.EnterDispatcherThreadActionBegin(() => LinkInfoesDownloadingManagerViewModel.This.CloseDownloadingCommand.Execute());
            });
        }

        private void StopLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                foreach (var item in GetListLinks())
                {
                    if (item.CanStop)
                        ApplicationLinkInfoManager.Current.StopLinkInfo(item);
                }

                if (LinkInfoesDownloadingManagerViewModel.This.DownloadingItems.Count == 0)
                {
                    BackToMain();
                }
            }
        }

        public static void DeleteLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                List<LinkInfo> deleteItems = new List<LinkInfo>();
                foreach (var item in GetListLinks())
                {
                    if (item.CanDelete)
                        deleteItems.Add(item);
                }
                if (deleteItems.Count > 0)
                    WindowMessegeBox.ShowMessage((val) =>
                    {
                        if (val)
                        {
                            ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(deleteItems);
                            if (LinkInfoesDownloadingManagerViewModel.This.IsShowLinkInfoDetail)
                                This.BackToMain();
                        }
                    }, "حذف", "کاربر محترم شما " + deleteItems.Count + " لینک انتخاب کرده اید.برای حذف روی قبول کلیک کنید" + Environment.NewLine + "(در صورت حذف، کلیه اطلاعات لینک پاک خواهند شد و قابل بازگردانی نیستند).");
            }
        }

        public static bool CanDeleteLinks()
        {
            if (!MainWindow.This.IsShowPage)
            {
                if (LinkInfoesDownloadingManagerViewModel.This.IsShowLinkInfoDownloading)
                {
                    return false;
                }
            }
            return GetListLinks().Count > 0;
        }

        public static void ShowSetting(LinkInfo linkInfo = null)
        {
            if (linkInfo == null)
            {
                UserAccountsSettingViewModel.This.IsApplicationSetting = true;
                LinkInfoSettingViewModel.This.ViewElement.radioSpeed.IsChecked = true;
                LinkInfoSettingViewModel.This.ViewElement.radioGeneral.Visibility = LinkInfoSettingViewModel.This.ViewElement.radioLinks.Visibility = Visibility.Collapsed;
                LinkInfoSettingViewModel.This.ViewElement.rectAppSetting.Visibility = LinkInfoSettingViewModel.This.ViewElement.radioAppSetting.Visibility = Visibility.Visible;
                LinkInfoSettingViewModel.This.AppSettingToUI();
            }
            else
            {
                UserAccountsSettingViewModel.This.IsApplicationSetting = false;
                LinkInfoSettingViewModel.This.ViewElement.radioGeneral.IsChecked = true;
                LinkInfoSettingViewModel.This.ViewElement.radioGeneral.Visibility = LinkInfoSettingViewModel.This.ViewElement.radioLinks.Visibility = Visibility.Visible;
                LinkInfoSettingViewModel.This.ViewElement.rectAppSetting.Visibility = LinkInfoSettingViewModel.This.ViewElement.radioAppSetting.Visibility = Visibility.Collapsed;
                LinkInfoSettingViewModel.This.LinkInfoSettingToUI(linkInfo);
            }
            PagesManagerViewModel.SetIndex(4);
            MainWindow.This.IsShowPage = true;
        }

        private void Setting()
        {
            ShowSetting();
        }


    }
}
