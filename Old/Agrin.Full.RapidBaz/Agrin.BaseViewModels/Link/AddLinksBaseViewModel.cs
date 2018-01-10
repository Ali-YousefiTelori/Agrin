using Agrin.BaseViewModels.Lists;
using Agrin.BaseViewModels.RapidBaz;
using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Engine;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using Agrin.IO.Strings;
using Agrin.LinkExtractor;
using Agrin.LinkExtractor.RapidBaz;
using Agrin.Log;
using Agrin.RapidBaz.Models;
using Agrin.RapidBaz.Web;
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
        public static Action BackItemClipboardClick { get; set; }

        public static AddLinksBaseViewModel This { get; set; }
        public bool IsBackAction { get; set; }

        public static object CurrentDispatcherMustSet { get; set; }
        object DispatcherThread { get; set; }
        public AddLinksBaseViewModel()
        {
            if (CurrentDispatcherMustSet == null)
                CurrentDispatcherMustSet = ApplicationHelperMono.DispatcherThread;

            DispatcherThread = CurrentDispatcherMustSet;
            CurrentDispatcherMustSet = null;
            IsBackAction = true;
            if (This == null)
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
                    _GroupLinks = new FastCollection<string>(DispatcherThread);
                return _GroupLinks;
            }
            set { _GroupLinks = value; }
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
                    //IsRapidBazLink = RapidBazFindDownloadLink.SupportThisLink(groupLinksMustAdd.FirstOrDefault());
                    return;
                }

                _UriAddress = value;
                //IsRapidBazLink = RapidBazFindDownloadLink.SupportThisLink(value);
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
                IsYoutube = DownloadUrlResolver.IsYoutubeLink(value);
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

        bool _IsDirectLink = false;

        public bool IsDirectLink
        {
            get { return _IsDirectLink; }
            set { _IsDirectLink = value; OnPropertyChanged("IsDirectLink"); }
        }

        bool _IsRapidBazLink = false;

        public bool IsRapidBazLink
        {
            get { return _IsRapidBazLink; }
            set
            {
                _IsRapidBazLink = value;
                if (value)
                    IsExpandSetting = false;
                OnPropertyChanged("IsRapidBazLink");
                if (value)
                    IsYoutube = false;
                else
                    IsYoutube = DownloadUrlResolver.IsYoutubeLink(UriAddress);
                if (!IsYoutube)
                    IsDirectLink = RapidBazFindDownloadLink.SupportThisLink(IsGroupList ? GroupLinks.FirstOrDefault() : UriAddress);
            }
        }

        bool _SendRapidBazToQueue = false;

        public bool SendRapidBazToQueue
        {
            get { return _SendRapidBazToQueue; }
            set { _SendRapidBazToQueue = value; OnPropertyChanged("SendRapidBazToQueue"); }
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
                if (_SelectedGroup != null && value == null)
                    return;
                var group = _SelectedGroup == null ? ApplicationGroupManager.Current.NoGroup : _SelectedGroup;
                bool changedAddress = MPath.EqualPath(group.SavePath, SaveToPath) || !System.IO.Path.IsPathRooted(SaveToPath);
                _SelectedGroup = value;
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

        FastCollection<FolderInfo> _RapidBazFolderList = null;
        public FastCollection<FolderInfo> RapidBazFolderList
        {
            get
            {
                if (_RapidBazFolderList == null)
                    _RapidBazFolderList = new FastCollection<FolderInfo>(DispatcherThread);
                return _RapidBazFolderList;
            }
        }

        FolderInfo _SelectedRapidBazFolder = null;

        public FolderInfo SelectedRapidBazFolder
        {
            get { return _SelectedRapidBazFolder; }
            set
            {
                _SelectedRapidBazFolder = value;
                OnPropertyChanged("SelectedRapidBazFolder");
            }
        }

        FastCollection<string> _YoutubeLinks = null;

        public FastCollection<string> YoutubeLinks
        {
            get
            {
                if (_YoutubeLinks == null)
                {
                    _YoutubeLinks = new FastCollection<string>(DispatcherThread);
                }
                return _YoutubeLinks;
            }
        }

        private int _YoutubeLinksSelectedIndex;

        public int YoutubeLinksSelectedIndex
        {
            get { return _YoutubeLinksSelectedIndex; }
            set { _YoutubeLinksSelectedIndex = value; OnPropertyChanged("YoutubeLinksSelectedIndex"); }
        }

        bool _IsYoutube = false;

        public bool IsYoutube
        {
            get { return _IsYoutube; }
            set
            {
                _IsYoutube = value;
                OnPropertyChanged("IsYoutube");
                IsDirectLink = value;
            }
        }

        List<VideoInfo> _YoutubeFoundLinks = new List<VideoInfo>();

        string _SaveToPath;
        public string SaveToPath
        {
            get { return _SaveToPath; }
            set { _SaveToPath = value; OnPropertyChanged("SaveToPath"); }
        }

        string _RapidBazUserName = "";

        public string RapidBazUserName
        {
            get { return _RapidBazUserName; }
            set { _RapidBazUserName = value; OnPropertyChanged("RapidBazUserName"); }
        }

        string _RapidBazPassword;

        public string RapidBazPassword
        {
            get { return _RapidBazPassword; }
            set { _RapidBazPassword = value; OnPropertyChanged("RapidBazPassword"); }
        }

        bool _IsSaveRapidBazSetting = false;

        public bool IsSaveRapidBazSetting
        {
            get { return _IsSaveRapidBazSetting; }
            set { _IsSaveRapidBazSetting = value; OnPropertyChanged("IsSaveRapidBazSetting"); }
        }

        bool _IsShowLogin = false;

        public bool IsShowLogin
        {
            get { return _IsShowLogin; }
            set { _IsShowLogin = value; OnPropertyChanged("IsShowLogin"); }
        }

        bool _IsEnableRapidBazAddLinks = true;

        public bool IsEnableRapidBazAddLinks
        {
            get { return _IsEnableRapidBazAddLinks; }
            set { _IsEnableRapidBazAddLinks = value; OnPropertyChanged("IsEnableRapidBazAddLinks"); }
        }

        bool _IsAddFolderMessage = false;

        public bool IsAddFolderMessage
        {
            get { return _IsAddFolderMessage; }
            set { _IsAddFolderMessage = value; OnPropertyChanged("IsAddFolderMessage"); }
        }

        string _AddFolderName = "";

        public string AddFolderName
        {
            get { return _AddFolderName; }
            set { _AddFolderName = value; OnPropertyChanged("AddFolderName"); }
        }

        public bool CanAddFolder()
        {
            return !string.IsNullOrEmpty(AddFolderName);
        }

        public void AddFolder()
        {
            AddFolderName = "";
            IsAddFolderMessage = true;
        }

        public void AddFolderOK()
        {
            IsAddFolderMessage = false;
            RapidTextStatus = "در حال بررسی...";
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                if (!RapidBazEngineHelper.IsLogin)
                {
                    LoginRapidBazBaseViewModel.Login((msg) => RapidTextStatus = msg, null, ApplicationSetting.Current.RapidBazSetting.UserName, ApplicationSetting.Current.RapidBazSetting.Password, true);
                    if (!RapidBazEngineHelper.IsLogin)
                    {
                        IsBusy = false;
                        return;
                    }
                }
                RapidTextStatus = "در حال درج پوشه...";

                var del = WebManager.FolderMake(AddFolderName);
                IsBusy = false;
                RefreshFolderList();
            }, (er) =>
            {
                RapidTextStatus = "خطا در درج پوشه رخ داده است...";
                Thread.Sleep(1500);
                IsBusy = false;
                IsAddFolderMessage = true;
                AutoLogger.LogError(er, "FolderFilesExplorerBaseViewModel");
            });
        }

        public virtual void BrowseFile()
        {

        }

        public void ClearItems()
        {
            IsBusy = false;
            UriAddress = "";
            SaveToPath = "";
            RapidTextStatus = "";
            UserName = "";
            Password = "";
            GroupLinks.Clear();
            IsGroupList = false;
            GroupLinkIsBusy = false;
            IsShowLogin = false;
            IsEnableRapidBazAddLinks = true;
            IsRapidBazLink = false;
            groupLinksMustAdd.Clear();
        }

        public void BackItem()
        {
            ClearItems();
            if (BackItemClick != null && IsBackAction)
                BackItemClick();
            if (BackItemClipboardClick != null && !IsBackAction)
                BackItemClipboardClick();
            //PagesManagerViewModel.This.BackItem();
        }

        public void AddGroupLinks()
        {
            groupLinksMustAdd = GroupLinks.ToList();
            GroupLinkIsBusy = false;
            IsGroupList = groupLinksMustAdd.Count > 0;
        }

        public virtual void FoundYoutubeLinks()
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

        public void RefreshFolderList()
        {
            idRefreshFolder = true;
            RapidTextStatus = "در حال بررسی...";
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                if (!RapidBazEngineHelper.IsLogin)
                {
                    LoginRapidBazBaseViewModel.Login((msg) => RapidTextStatus = msg, null, ApplicationSetting.Current.RapidBazSetting.UserName, ApplicationSetting.Current.RapidBazSetting.Password, true);
                    if (!RapidBazEngineHelper.IsLogin)
                    {
                        RapidBazUserName = ApplicationSetting.Current.RapidBazSetting.UserName;
                        RapidBazPassword = ApplicationSetting.Current.RapidBazSetting.Password;
                        IsSaveRapidBazSetting = ApplicationSetting.Current.RapidBazSetting.IsSaveSetting;
                        IsShowLogin = true;
                        IsBusy = false;
                        return;
                    }
                }
                IsShowLogin = false;
                RapidTextStatus = "در حال بارگزاری لیست...";
                var list = WebManager.FolderList();
                RapidBazFolderList.Clear();
                RapidBazFolderList.AddRange(list);
                IsBusy = false;
            }, (er) =>
            {
                RapidTextStatus = "خطا در بارگزاری رخ داده است...";
                Thread.Sleep(1500);
                IsBusy = false;
                AutoLogger.LogError(er, "RefreshFolderList AddLinksBaseViewModel");
            });
        }

        void RunYoutubeLink()
        {
            _YoutubeFoundLinks.Clear();
            YoutubeLinks.Clear();
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
                    List<string> items = new List<string>();

                    _YoutubeFoundLinks.AddRange(Agrin.LinkExtractor.DownloadUrlResolver.GetDownloadUrls(UriAddress));
                    foreach (var item in _YoutubeFoundLinks)
                    {
                        items.Add("نوع : " + item.VideoType + " کیفیت " + item.Resolution);
                    }

                    YoutubeLinks.AddRange(items);
                    IsBusy = false;
                    if (items.Count > 0)
                    {
                        FoundYoutubeLinks();
                    }
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

        bool idRefreshFolder = false;
        public void LoginRapidBaz()
        {
            ApplicationSetting.Current.RapidBazSetting.UserName = RapidBazUserName;
            ApplicationSetting.Current.RapidBazSetting.Password = RapidBazPassword;
            ApplicationSetting.Current.RapidBazSetting.IsSaveSetting = IsSaveRapidBazSetting;
            LoginRapidBazBaseViewModel.This.UserName = RapidBazUserName;
            LoginRapidBazBaseViewModel.This.Password = RapidBazPassword;
            LoginRapidBazBaseViewModel.This.IsSaveSetting = IsSaveRapidBazSetting;
            SerializeData.SaveApplicationSettingToFile();
            if (idRefreshFolder)
                RefreshFolderList();
            else
                AddToRapidBaz(_isPlay);
        }

        public void AddToRapidBaz(bool isPlayAfterComplete)
        {
            idRefreshFolder = false;
            RapidTextStatus = "در حال بررسی...";
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                if (!RapidBazEngineHelper.IsLogin)
                {
                    LoginRapidBazBaseViewModel.Login((msg) => RapidTextStatus = msg, null, ApplicationSetting.Current.RapidBazSetting.UserName, ApplicationSetting.Current.RapidBazSetting.Password, true);
                    if (!RapidBazEngineHelper.IsLogin)
                    {
                        RapidBazUserName = ApplicationSetting.Current.RapidBazSetting.UserName;
                        RapidBazPassword = ApplicationSetting.Current.RapidBazSetting.Password;
                        IsSaveRapidBazSetting = ApplicationSetting.Current.RapidBazSetting.IsSaveSetting;
                        IsShowLogin = true;
                        IsBusy = false;
                        return;
                    }
                }
                IsShowLogin = false;
                RapidTextStatus = "در حال ارسال...";
                string folderID = SelectedRapidBazFolder == null ? null : SelectedRapidBazFolder.ID;
                List<string> links = null;
                if (IsGroupList)
                    links = groupLinksMustAdd;
                else
                    links = new List<string>() { UriAddress };
                for (int i = 0; i < links.Count; i++)
                {
                    if (!RapidBazFindDownloadLink.SupportThisLink(links[i]) || IsDirectLink)
                    {
                        links[i] = links[i] + "**";
                    }
                }
                List<string> ids = new List<string>();
                if (IsGroupList)
                    ids = RapidBazEngineHelper.SendMultipeFile(links);
                else
                    ids = new List<string>() { RapidBazEngineHelper.SendFile(links.FirstOrDefault()) };

                //var list = Agrin.Download.Engine.RapidBazEngineHelper.GetList();
                int sendCount = 0;
                foreach (var id in ids)
                {
                    var fff = RapidBazEngineHelper.SetFolder(folderID, id);
                    var status = Agrin.Download.Engine.RapidBazEngineHelper.FileStatus(id);
                    RapidTextStatus = Agrin.RapidBaz.Web.WebManager.GetStatusString(status.Status);

                    if (id != "0" && (status.Status == "0" || status.Status == "1" || status.Status == "7" || status.Status == "8"))
                    {
                        if (isPlayAfterComplete)
                            ApplicationSetting.Current.RapidBazSetting.DownloadAfterComplete(id, SendRapidBazToQueue);
                        //RapidBazGetListBaseViewModel.MustRefresh = true;
                        sendCount++;
                    }
                }
                if (sendCount > 0)
                {
                    RapidTextStatus = sendCount + " فایل ارسال شد.";
                    System.Threading.Thread.Sleep(2000);
                    ApplicationHelperMono.EnterDispatcherThreadByDispatcherAction(() =>
                    {
                        BackItem();
                    }, DispatcherThread);
                }
                else
                {
                    RapidTextStatus = "هیچ فایلی ارسال نشد.";
                    System.Threading.Thread.Sleep(2000);
                }

                IsBusy = false;
            }, (error) =>
            {
                RapidTextStatus = "خطا در ارتباط نرم افزار رخ داده است.";
                IsBusy = false;
            });
        }
        bool _isPlay = false;
        public void AddLink()
        {
            _isPlay = false;
            if (IsRapidBazLink)
            {
                AddToRapidBaz(false);
            }
            else
            {
                if (IsGroupList)
                    LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(groupLinksMustAdd, SelectedGroup, SaveToPath, UserName, Password, false, _YoutubeFoundLinks.Count == 0 ? -1 : _YoutubeFoundLinks[YoutubeLinksSelectedIndex].FormatCode);
                else
                    LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(UriAddress, SelectedGroup, SaveToPath, UserName, Password, false, _YoutubeFoundLinks.Count == 0 ? -1 : _YoutubeFoundLinks[YoutubeLinksSelectedIndex].FormatCode);
                BackItem();
            }
        }

        public void AddLinkAndPlay()
        {
            _isPlay = true;
            if (IsRapidBazLink)
            {
                AddToRapidBaz(true);
            }
            else
            {
                if (IsGroupList)
                    LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(groupLinksMustAdd, SelectedGroup, SaveToPath, UserName, Password, true, _YoutubeFoundLinks.Count == 0 ? -1 : _YoutubeFoundLinks[YoutubeLinksSelectedIndex].FormatCode);
                else
                    LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(UriAddress, SelectedGroup, SaveToPath, UserName, Password, true, _YoutubeFoundLinks.Count == 0 ? -1 : _YoutubeFoundLinks[YoutubeLinksSelectedIndex].FormatCode);
                BackItem();
            }

        }

        public void LoadYoutubeLink()
        {
            RunYoutubeLink();
        }

        public bool CanAddLink()
        {
            if (IsBusy)
                return false;
            if (IsGroupList)
                return true;
            Uri uri = null;
            return Uri.TryCreate(UriAddress, UriKind.Absolute, out uri);
        }
    }
}
