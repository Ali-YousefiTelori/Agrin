using Agrin.BaseViewModels.Lists;
using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Engine;
using Agrin.Download.Helper;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using Agrin.IO.Strings;
using Agrin.LinkExtractor;
using Agrin.LinkExtractor.Helpers;
using Agrin.LinkExtractor.Models;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.BaseViewModels.Link
{
    public class AddLinksBaseViewModel : ANotifyPropertyChanged
    {
        public static Action BackItemClick { get; set; }
        public static AddLinksBaseViewModel This { get; set; }
        public AddLinksBaseViewModel()
        {
            This = this;
            //AddLinkAndPlayCommand = new RelayCommand(AddLinkAndPlay, CanAddLink);
            //AddLinkCommand = new RelayCommand(AddLink, CanAddLink);
            //BrowseFileCommand = new RelayCommand(BrowseFile);
            //LoadYoutubeLinkCommand = new RelayCommand(LoadYoutubeLink);
        }

        //public RelayCommand AddLinkCommand { get; set; }
        //public RelayCommand AddLinkAndPlayCommand { get; set; }
        //public RelayCommand BrowseFileCommand { get; set; }
        //public RelayCommand LoadYoutubeLinkCommand { get; set; }

        bool _IsGroupList;

        public bool IsGroupList
        {
            get { return _IsGroupList; }
            set
            {
                _IsGroupList = value;
                OnPropertyChanged("IsGroupList");
                if (value)
                {
                    UriAddress = "(درج گروهی)";
                }
                else
                {
                    UriAddress = "";
                }
            }
        }

        bool _GroupLinkIsBusy = false;

        public bool GroupLinkIsBusy
        {
            get { return _GroupLinkIsBusy; }
            set { _GroupLinkIsBusy = value; OnPropertyChanged("GroupLinkIsBusy"); }
        }

        string _GroupLinkTitle = "درج گروهی";

        public string GroupLinkTitle
        {
            get { return _GroupLinkTitle; }
            set { _GroupLinkTitle = value; OnPropertyChanged("GroupLinkTitle"); }
        }

        List<string> groupLinksMustAdd = new List<string>();
        FastCollection<string> _GroupLinks;

        public FastCollection<string> GroupLinks
        {
            get
            {
                if (_GroupLinks == null)
                    _GroupLinks = new FastCollection<string>(ApplicationHelperBase.DispatcherThread);
                return _GroupLinks;
            }
            set { _GroupLinks = value; }
        }

        public string NormalizeUriAddress
        {
            get
            {
                if (string.IsNullOrEmpty(UriAddress))
                    return UriAddress;
                string address = UriAddress.ToLower().StartsWith("http://") || UriAddress.ToLower().StartsWith("https://") || UriAddress.ToLower().StartsWith("ftp://") ? UriAddress : "http://" + UriAddress;
                return address;
            }
        }

        bool setedUserPass = false;
        string _UriAddress;
        public string UriAddress
        {
            get { return _UriAddress; }
            set
            {
                if (IsGroupList)
                {
                    _UriAddress = value;
                    OnPropertyChanged("UriAddress");
                    SharingLinks.Clear();
                    return;
                }

                _UriAddress = value;
                OnPropertyChanged("UriAddress");
                if (CanAddLink())
                    SelectedGroup = ApplicationGroupManager.Current.FindGroupByFileName(UriAddress);
                var findedAccount = AppUserAccountsSetting.FindFromAddress(_UriAddress);
                if (findedAccount != null)
                {
                    UserName = findedAccount.UserName;
                    Password = findedAccount.Password;
                    setedUserPass = true;
                }
                else if (setedUserPass)
                {
                    UserName = "";
                    Password = "";
                    setedUserPass = false;
                }
                IsVideoSharing = SharingHelper.IsVideoSharing(value, true);

                if (!IsVideoSharing)
                    SharingLinks.Clear();

            }
        }

        string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; OnPropertyChanged("UserName"); }
        }

        string _Password;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; OnPropertyChanged("Password"); }
        }

        bool _IsExpandSetting = false;
        public bool IsExpandSetting
        {
            get { return _IsExpandSetting; }
            set
            {
                _IsExpandSetting = value;
                OnPropertyChanged("IsExpandSetting");
            }
        }

        string _RapidTextStatus = "";

        public string RapidTextStatus
        {
            get { return _RapidTextStatus; }
            set { _RapidTextStatus = value; OnPropertyChanged("RapidTextStatus"); }
        }

        bool _IsBusy = false;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged("IsBusy"); }
        }

        GroupInfo _SelectedGroup;
        public GroupInfo SelectedGroup
        {
            get { return _SelectedGroup; }
            set
            {
                //if (_SelectedGroup != null && value == null)
                //    return;
                _SelectedGroup = value;
                var group = _SelectedGroup == null ? ApplicationGroupManager.Current.NoGroup : _SelectedGroup;
                //var gg = System.IO.Path.IsPathRooted(SaveToPath);
                //bool changedAddress = MPath.EqualPath(group.SavePath, SaveToPath) || !System.IO.Path.IsPathRooted(SaveToPath);
                bool changedAddress = !MPath.EqualPath(group.SavePath, SaveToPath);
                OnPropertyChanged("SelectedGroup");
                if (changedAddress)
                    SaveToPath = _SelectedGroup == null ? group.SavePath : _SelectedGroup.SavePath;
            }
        }

        public FastCollection<GroupInfo> Groups
        {
            get
            {
                return ApplicationGroupManager.Current.GroupInfoes;
            }
        }


        FastCollection<PublicSharingInfo> _SharingLinks = null;

        public FastCollection<PublicSharingInfo> SharingLinks
        {
            get
            {
                if (_SharingLinks == null)
                {
                    _SharingLinks = new FastCollection<PublicSharingInfo>(ApplicationHelperBase.DispatcherThread);
                }
                return _SharingLinks;
            }
        }

        private int _SharingLinksSelectedIndex = -1;

        public int SharingLinksSelectedIndex
        {
            get { return _SharingLinksSelectedIndex; }
            set
            {
                if (value == 1)
                {
                    var iii = SharingLinks.Count;
                }

                _SharingLinksSelectedIndex = value;
                OnPropertyChanged("SharingLinksSelectedIndex");

            }
        }

        bool _IsVideoSharing = false;

        public bool IsVideoSharing
        {
            get { return _IsVideoSharing; }
            set { _IsVideoSharing = value; OnPropertyChanged("IsVideoSharing"); }
        }

        string _SaveToPath;
        public string SaveToPath
        {
            get { return _SaveToPath; }
            set { _SaveToPath = value; OnPropertyChanged("SaveToPath"); }
        }

        string _SecurityPath;

        public string SecurityPath
        {
            get { return _SecurityPath; }
            set { _SecurityPath = value; OnPropertyChanged("SecurityPath"); }
        }

        bool _IsShowLogin = false;

        public bool IsShowLogin
        {
            get { return _IsShowLogin; }
            set { _IsShowLogin = value; OnPropertyChanged("IsShowLogin"); }
        }

        bool _IsAddFolderMessage = false;

        public bool IsAddFolderMessage
        {
            get { return _IsAddFolderMessage; }
            set { _IsAddFolderMessage = value; OnPropertyChanged("IsAddFolderMessage"); }
        }

        public List<LinkInfo> AddedLinks { get; set; }

        public virtual void BrowseFile()
        {

        }

        public void ClearItems()
        {
            IsBusy = false;
            UriAddress = "";
            SaveToPath = "";
            SecurityPath = "";
            RapidTextStatus = "";
            UserName = "";
            Password = "";
            GroupLinks.Clear();
            IsGroupList = false;
            GroupLinkIsBusy = false;
            IsShowLogin = false;
            IsVideoSharing = false;
            groupLinksMustAdd.Clear();
            SharingLinks.Clear();
        }

        public void BackItem()
        {
            ClearItems();
            if (BackItemClick != null)
                BackItemClick();
            //PagesManagerViewModel.This.BackItem();
        }

        public void AddGroupLinks()
        {
            groupLinksMustAdd = GroupLinks.ToList();
            GroupLinkIsBusy = false;
            IsGroupList = groupLinksMustAdd.Count > 0;
        }

        public virtual void FoundSharingLinks()
        {

        }

        public void ShowGroupList()
        {
            GroupLinks.Clear();
            GroupLinks.AddRange(groupLinksMustAdd);
            GroupLinkIsBusy = true;
        }

        public void RemoveLinkFromGroupList(string item)
        {
            GroupLinks.Remove(item);
        }

        public void AddFromList(string html)
        {
            var links = HtmlPage.ExtractLinksFromHtmlTwo(html);
            foreach (var item in links.ToArray())
            {
                if (GroupLinks.Contains(item))
                {
                    links.Remove(item);
                }
            }
            if (links.Count > 0)
            {
                GroupLinks.AddRange(links);
            }
        }

        void RunSharingLink()
        {
            SharingLinks.Clear();
            RapidTextStatus = "در حال بررسی...";
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                Action<string> showMessageException = (message) =>
                {
                    RapidTextStatus = "خطا در دریافت لینک ها رخ داده است: " + message;
                    System.Threading.Thread.Sleep(1500);
                    IsBusy = false;
                };

                try
                {
                    RapidTextStatus = "در حال یافتن لینک ها...";
                    IsBusy = true;
                    List<PublicSharingInfo> items = SharingHelper.GetSharingInfo(NormalizeUriAddress);

                    SharingLinks.AddRange(items);
                    SharingLinksSelectedIndex = 0;
                    IsBusy = false;
                    if (items.Count > 0)
                    {
                        FoundSharingLinks();
                    }
                    ApplicationHelperBase.RefreshCommandAction?.Invoke();
                }
                catch (AggregateException c)
                {
                    if (c.InnerExceptions != null && c.InnerExceptions.Count > 0)
                    {
                        if (c.InnerExceptions[0].InnerException != null)
                        {
                            showMessageException(c.InnerExceptions[0].InnerException.Message);
                        }
                        else
                            showMessageException(c.InnerExceptions[0].Message);
                    }
                    else
                        showMessageException(c.Message);
                }
                catch (Exception ex)
                {
                    showMessageException(ex.Message);
                }
            });
        }


        public virtual void AddLink()
        {
            if (IsGroupList)
                AddedLinks = LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(groupLinksMustAdd, SelectedGroup, SaveToPath, SecurityPath, UserName, Password, false, SharingHelper.GetSharingIndex(NormalizeUriAddress, SharingLinks, SharingLinksSelectedIndex));
            else
                AddedLinks = new List<LinkInfo>() { LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(NormalizeUriAddress, SelectedGroup, SaveToPath, SecurityPath, UserName, Password, false, SharingHelper.GetSharingIndex(NormalizeUriAddress, SharingLinks, SharingLinksSelectedIndex)) };
            BackItem();
        }

        public virtual void AddLinkAndPlay()
        {
            if (IsGroupList)
                AddedLinks = LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(groupLinksMustAdd, SelectedGroup, SaveToPath, SecurityPath, UserName, Password, true, SharingHelper.GetSharingIndex(NormalizeUriAddress, SharingLinks, SharingLinksSelectedIndex));
            else
                AddedLinks = new List<LinkInfo>() { LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(NormalizeUriAddress, SelectedGroup, SaveToPath, SecurityPath, UserName, Password, true, SharingHelper.GetSharingIndex(NormalizeUriAddress, SharingLinks, SharingLinksSelectedIndex)) };
            BackItem();
        }

        public void SelectQuality()
        {
            IsVideoSharing = false;
        }

        public bool CanSelectQuality()
        {
            return SharingLinksSelectedIndex >= 0 && SharingLinks.Count > 0;
        }

        public void RefreshSharingLinks()
        {
            RunSharingLink();
        }

        public bool CanAddLink()
        {
            if (IsBusy || string.IsNullOrEmpty(NormalizeUriAddress))
                return false;
            if (IsGroupList)
                return true;
            Uri uri = null;
            return Uri.TryCreate(NormalizeUriAddress, UriKind.Absolute, out uri);
        }
    }
}
