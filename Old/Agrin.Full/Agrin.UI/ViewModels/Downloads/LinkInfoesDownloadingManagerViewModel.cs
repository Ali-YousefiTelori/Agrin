using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.UI.Views.Downloads;
using Agrin.ViewModels.Helper.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agrin.UI.ViewModels.Downloads
{
    public class LinkInfoesDownloadingManagerViewModel : ANotifyPropertyChanged<LinkInfoesDownloadingManager>
    {
        #region Constructors
        public LinkInfoesDownloadingManagerViewModel()
        {
            if (MainWindow.This == null)
                return;
            ClearSelectionCommand = new RelayCommand(ClearSelection);
            PlaySelectionCommand = new RelayCommand(PlaySelection, CanPlaySelection);
            PauseSelectionCommand = new RelayCommand(PauseSelection, CanPauseSelection);
            StopSelectionCommand = new RelayCommand(StopSelection, CanStopSelection);
            DisposeSelectionCommand = new RelayCommand(DisposeSelection, CanDisposeSelection);

            ClickLinkInfoCommand = new RelayCommand<LinkInfo>(ClickLinkInfo);
            CloseDetailedCommand = new RelayCommand(CloseDetailed);
            CloseDownloadingCommand = new RelayCommand(CloseDownloading);
            ShowDownloadingCommand = new RelayCommand(ShowDownloading);

            This = this;
            //Random rnd = new Random();
            //List<LinkInfo> ittt = new List<LinkInfo>();
            //for (int i = 0; i < 20; i++)
            //{
            //    var link = new Agrin.Download.Web.LinkInfo("http://kaz.dl.sourceforge.net/project/winsmtpserver/Version%200.90.01/" + System.IO.Path.GetRandomFileName() + ".rar");
            //    link.PathInfo.AppSavePath = "E:\\";
            //    link.DownloadingProperty.SelectionChanged = (linkInfo, sel) =>
            //    {
            //        if (sel)
            //            AddSelection(linkInfo);
            //        else
            //            RemoveSelection(linkInfo);
            //    };
            //    for (int b = 0; b < rnd.Next(1, 50); b++)
            //    {
            //        link.AddConnection(new Download.Web.Connections.NormalConnectionInfo(link.PathInfo.Address));
            //    }
            //    ittt.Add(link);
            //}
            //Items.AddRange(ittt);
            Initilize();
        }
        void Initilize()
        {
            ApplicationLinkInfoManager.Current.SelectionChanged = (info, sel) =>
            {
                if (sel)
                    AddSelection(info);
                else
                    RemoveSelection(info);
            };
        }
        #endregion

        #region Commands
        public RelayCommand<LinkInfo> ClickLinkInfoCommand { get; private set; }
        public RelayCommand ClearSelectionCommand { get; private set; }
        public RelayCommand PlaySelectionCommand { get; private set; }
        public RelayCommand StopSelectionCommand { get; private set; }
        public RelayCommand PauseSelectionCommand { get; private set; }
        public RelayCommand DisposeSelectionCommand { get; private set; }
        public RelayCommand CloseDetailedCommand { get; private set; }
        public RelayCommand ShowDownloadingCommand { get; private set; }
        public RelayCommand CloseDownloadingCommand { get; set; }
        #endregion

        #region Events

        #endregion

        #region Fields
        static LinkInfoesDownloadingManagerViewModel _This;
        public static LinkInfoesDownloadingManagerViewModel This
        {
            get { return LinkInfoesDownloadingManagerViewModel._This; }
            set { LinkInfoesDownloadingManagerViewModel._This = value; }
        }
        #endregion

        #region Properties
        public FastCollection<LinkInfo> Items
        {
            get
            {
                return ApplicationLinkInfoManager.Current.LinkInfoes;
            }
        }

        public FastCollection<LinkInfo> DownloadingItems
        {
            get
            {
                return ApplicationLinkInfoManager.Current.DownloadingLinkInfoes;
            }
        }

        List<LinkInfo> _selectedItems = new List<LinkInfo>();
        public List<LinkInfo> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set { _selectedItems = value; }
        }

        bool _isShowToolbar = false;
        public bool IsShowToolbar
        {
            get { return _isShowToolbar; }
            set
            {
                _isShowToolbar = value;
                OnPropertyChanged("IsShowToolbar");
            }
        }

        bool _IsShowListLinks = true;
        public bool IsShowListLinks
        {
            get { return _IsShowListLinks; }
            set
            {
                _IsShowListLinks = value;
                OnPropertyChanged("IsShowListLinks");
                if (value)
                    IsShowLinkInfoDownloadingTop = IsShowLinkInfoDetail = IsShowLinkInfoDownloading = !value;
            }
        }

        bool _isShowLinkInfoDetail = false;
        public bool IsShowLinkInfoDetail
        {
            get { return _isShowLinkInfoDetail; }
            set
            {
                _isShowLinkInfoDetail = value;
                OnPropertyChanged("IsShowLinkInfoDetail");
                if (value)
                    IsShowLinkInfoDownloadingTop = IsShowListLinks = IsShowLinkInfoDownloading = !value;
                else
                    ((LinkInfoDownloadDetailViewModel)MainWindow.This.downloadManager.downloadDetail.DataContext).CurrentLinkInfo = null;

            }
        }

        bool _isShowLinkInfoDownloading = false;
        public bool IsShowLinkInfoDownloading
        {
            get { return _isShowLinkInfoDownloading; }
            set
            {
                _isShowLinkInfoDownloading = value;
                OnPropertyChanged("IsShowLinkInfoDownloading");
                if (value)
                {
                    IsShowListLinks = IsShowLinkInfoDetail = !value;
                    //if (MainWindow.This.IsToolbarPin)
                    //    MainWindow.This.IsShowToolbar = false;
                }
                //else
                //{
                //    if (MainWindow.This.IsToolbarPin)
                //        MainWindow.This.IsShowToolbar = true;
                //}
            }
        }

        bool _isShowLinkInfoDownloadingTop = false;
        public bool IsShowLinkInfoDownloadingTop
        {
            get { return _isShowLinkInfoDownloadingTop; }
            set
            {
                _isShowLinkInfoDownloadingTop = value;
                OnPropertyChanged("IsShowLinkInfoDownloadingTop");
            }
        }

        System.Windows.Thickness _DownloadMargin = new System.Windows.Thickness(0, 800, 0, -800);
        public System.Windows.Thickness DownloadMargin
        {
            get
            {
                return _DownloadMargin;
            }
            set { _DownloadMargin = value; OnPropertyChanged("DownloadMargin"); }
        }
        #endregion

        #region Methods
        public void AddSelection(LinkInfo info)
        {
            SelectedItems.Add(info);
            if (!IsShowToolbar)
                IsShowToolbar = true;
        }
        public void RemoveSelection(LinkInfo info)
        {
            SelectedItems.Remove(info);
            if (SelectedItems.Count == 0)
                IsShowToolbar = false;
        }
        public void ClearAllSelectionItems()
        {
            foreach (var item in SelectedItems.ToList())
            {
                item.DownloadingProperty.IsSelected = false;
            }
        }
        public void SelectAll()
        {
            foreach (var item in Items)
            {
                if (!item.DownloadingProperty.IsSelected)
                    item.DownloadingProperty.IsSelected = true;
            }
        }
        #endregion

        #region CommandsMethods
        void ClearSelection()
        {
            if (SelectedItems.Count != Items.Count)
                SelectAll();
            else
                ClearAllSelectionItems();
        }

        LinkInfo _detailClickedLinkInfo;
        public LinkInfo DetailClickedLinkInfo
        {
            get { return _detailClickedLinkInfo; }
            set { _detailClickedLinkInfo = value; }
        }

        void ClickLinkInfo(LinkInfo linkInfo)
        {
            if (IsShowLinkInfoDetail)
                return;
            if (IsShowToolbar)
                linkInfo.DownloadingProperty.IsSelected = !linkInfo.DownloadingProperty.IsSelected;
            else
            {
                ((LinkInfoDownloadDetailViewModel)MainWindow.This.downloadManager.downloadDetail.DataContext).CurrentLinkInfo = linkInfo;
                DownloadMargin = new System.Windows.Thickness(0, -800, 0, 800);
                DetailClickedLinkInfo = linkInfo;
                IsShowLinkInfoDetail = true;
            }
        }

        public void AddLinkInfo(string uriAddress, GroupInfo groupInfo, string userSavePath, string userName, string password, bool isPlay, int shareingIndex)
        {
            LinkInfo linkInfo = new LinkInfo(uriAddress);
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(uriAddress) && shareingIndex != -1)
                linkInfo.Management.SharingSettings.Add(shareingIndex);
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                linkInfo.Management.NetworkUserPass = new Download.Web.Link.NetworkCredentialInfo() { UserName = userName, Password = password };
            if (!string.IsNullOrEmpty(userSavePath))
            {
                if (groupInfo != null)
                {
                    if (!Agrin.IO.Helper.MPath.EqualPath(groupInfo.SavePath, userSavePath))
                    {
                        linkInfo.PathInfo.UserSavePath = userSavePath;
                    }
                }
                else
                    linkInfo.PathInfo.UserSavePath = userSavePath;
            }

            ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, groupInfo, isPlay);
            linkInfo.DownloadingProperty.SelectionChanged = (link, sel) =>
            {
                if (sel)
                    AddSelection(link);
                else
                    RemoveSelection(link);
            };
            //linkInfo.CommandBindingChanged = () =>
            //{
            //    ApplicationHelper.EnterDispatcherThreadActionBegin(() =>
            //    {
            //        CommandManager.InvalidateRequerySuggested();
            //    });
            //};
        }

        private void CloseDetailed()
        {
            //IsShowLinkInfoDownloadingTop = true;
            ShowDownloading();

        }
        private void ShowDownloading()
        {
            IsShowLinkInfoDownloading = true;
        }
        private void CloseDownloading()
        {
            DownloadMargin = new System.Windows.Thickness(0, 800, 0, -800);
            IsShowListLinks = true;
        }

        private bool CanPlaySelection()
        {
            foreach (var item in SelectedItems)
            {
                if (item.CanPlay)
                    return true;
            }
            return false;
        }

        private void PlaySelection()
        {
            foreach (var item in SelectedItems)
            {
                if (item.CanPlay)
                    ApplicationLinkInfoManager.Current.PlayLinkInfo(item);
            }
        }

        private bool CanDisposeSelection()
        {
            return SelectedItems.Count > 0;
        }

        private void DisposeSelection()
        {
            foreach (var item in SelectedItems)
            {
                ApplicationLinkInfoManager.Current.DisposeLinkInfo(item);
            }
            ClearAllSelectionItems();
        }

        private bool CanStopSelection()
        {
            foreach (var item in SelectedItems)
            {
                if (item.CanStop)
                    return true;
            }
            return false;
        }

        private void StopSelection()
        {
            foreach (var item in SelectedItems)
            {
                if (item.CanStop)
                    ApplicationLinkInfoManager.Current.StopLinkInfo(item);
            }
        }

        private void PauseSelection()
        {
            foreach (var item in SelectedItems)
            {
                if (item.CanPause)
                    ApplicationLinkInfoManager.Current.PauseLinkInfo(item);
            }
        }

        private bool CanPauseSelection()
        {
            foreach (var item in SelectedItems)
            {
                if (item.CanPause)
                    return true;
            }
            return false;
        }

        #endregion

    }
}
