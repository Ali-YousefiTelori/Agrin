using Agrin.Download.Data.Settings;
using Agrin.Download.Engine;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using Agrin.LinkExtractor;
using Agrin.UI.ViewModels.Downloads;
using Agrin.ViewModels.Helper.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.Pages
{
    public class AddLinksViewModel : ANotifyPropertyChanged
    {
        public static AddLinksViewModel This { get; set; }
        public AddLinksViewModel()
        {
            This = this;
            AddLinkAndPlayCommand = new RelayCommand(AddLinkAndPlay, CanAddLink);
            AddLinkCommand = new RelayCommand(AddLink, CanAddLink);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            LoadYoutubeLinkCommand = new RelayCommand(LoadYoutubeLink);
        }

        public RelayCommand AddLinkCommand { get; set; }
        public RelayCommand AddLinkAndPlayCommand { get; set; }
        public RelayCommand BrowseFileCommand { get; set; }
        public RelayCommand LoadYoutubeLinkCommand { get; set; }

        bool setedUserPass = false;
        string _UriAddress;
        public string UriAddress
        {
            get { return _UriAddress; }
            set
            {
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
                if (DownloadUrlResolver.IsYoutubeLink(value))
                {
                    IsYoutubeVisiility = System.Windows.Visibility.Visible;
                }
                else
                {
                    IsYoutubeVisiility = System.Windows.Visibility.Collapsed;
                }
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
                    IsYoutubeVisiility = System.Windows.Visibility.Collapsed;
                else
                {
                    UriAddress = UriAddress;
                }
            }
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

        bool _IsBussy = false;

        public bool IsBussy
        {
            get { return _IsBussy; }
            set { _IsBussy = value; OnPropertyChanged("IsBussy"); }
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

        FastCollection<string> _YoutubeLinks = null;

        public FastCollection<string> YoutubeLinks
        {
            get
            {
                if (_YoutubeLinks == null)
                {
                    _YoutubeLinks = new FastCollection<string>(ApplicationHelper.DispatcherThread);
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

        System.Windows.Visibility _IsYoutubeVisiility = System.Windows.Visibility.Collapsed;

        public System.Windows.Visibility IsYoutubeVisiility
        {
            get { return _IsYoutubeVisiility; }
            set { _IsYoutubeVisiility = value; OnPropertyChanged("IsYoutubeVisiility"); }
        }

        List<VideoInfo> _YoutubeFoundLinks = new List<VideoInfo>();

        string _SaveToPath;
        public string SaveToPath
        {
            get { return _SaveToPath; }
            set { _SaveToPath = value; OnPropertyChanged("SaveToPath"); }
        }

        private void BrowseFile()
        {
            //SaveFileDialog dialog = new SaveFileDialog() { Filter = "*.*|All Files", InitialDirectory = SaveToPath, FileName = CanAddLink() ? System.IO.Path.GetFileName(UriAddress) : "" };
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = SaveToPath, ShowNewFolderButton = true };
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!MPath.EqualPath(folderDialog.SelectedPath, SaveToPath))
                {
                    SaveToPath = folderDialog.SelectedPath;
                }
            }
        }

        public void ClearItems()
        {
            IsBussy = false;
            UriAddress = "";
            SaveToPath = "";
            RapidTextStatus = "";
            UserName = "";
            Password = "";
        }

        void BackItem()
        {
            ClearItems();
            PagesManagerViewModel.This.BackItem();
        }

        void RunYoutubeLink()
        {
            _YoutubeFoundLinks.Clear();
            YoutubeLinks.Clear();
            AsyncActions.Action(() =>
            {
                Action<string> showMessageException = (message) =>
                {
                    RapidTextStatus = "خطا در دریافت لینک ها رخ داده است: " + message;
                    System.Threading.Thread.Sleep(1500);
                    IsBussy = false;
                };

                try
                {
                    RapidTextStatus = "در حال یافتن لینک ها...";
                    IsBussy = true;
                    List<string> items = new List<string>();

                    _YoutubeFoundLinks.AddRange(Agrin.LinkExtractor.DownloadUrlResolver.GetDownloadUrls(UriAddress));
                    foreach (var item in _YoutubeFoundLinks)
                    {
                        items.Add("نوع : " + item.VideoType + " کیفیت " + item.Resolution);
                    }
                    if (items.Count > 0)
                    {
                        IsYoutubeVisiility = System.Windows.Visibility.Collapsed;
                    }
                    YoutubeLinks.AddRange(items);
                    IsBussy = false;
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

        void AddLink()
        {
            LinkInfoesDownloadingManagerViewModel.This.AddLinkInfo(UriAddress, SelectedGroup, SaveToPath, UserName, Password, false, _YoutubeFoundLinks[YoutubeLinksSelectedIndex].FormatCode);
            BackItem();
        }

        void AddLinkAndPlay()
        {
            if (IsRapidBazLink)
            {
                IsBussy = true;
                AsyncActions.Action(() =>
                {
                    if (!RapidBazEngineHelper.IsLogin)
                    {
                        RapidTextStatus = "در حال ورود...";
                        if (!RapidBazEngineHelper.Login())
                        {
                            RapidTextStatus = "خطا در ورود رخ داده است.";
                            System.Threading.Thread.Sleep(1500);
                            IsBussy = false;
                            return;
                        }
                        else
                            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    }
                    RapidTextStatus = "در حال ارسال...";
                    var id = RapidBazEngineHelper.SendFile(UriAddress);

                    //var list = Agrin.Download.Engine.RapidBazEngineHelper.GetList();
                    var status = Agrin.Download.Engine.RapidBazEngineHelper.FileStatus(id);
                    RapidTextStatus = Agrin.RapidService.Web.WebManager.GetStatusString(status.Status);

                    if (id != "0" && (status.Status == "0" || status.Status == "1" || status.Status == "7" || status.Status == "8"))
                    {
                        Agrin.UI.ViewModels.Lists.RapidBazGetListViewModel.MustRefresh = true;
                        System.Threading.Thread.Sleep(1500);
                        ApplicationHelper.EnterDispatcherThreadAction(() =>
                        {
                            BackItem();
                        });
                    }

                    IsBussy = false;
                }, (error) =>
                {
                    RapidTextStatus = "خطا در ارتباط نرم افزار رخ داده است.";
                    IsBussy = false;
                });
            }
            else
            {
                LinkInfoesDownloadingManagerViewModel.This.AddLinkInfo(UriAddress, SelectedGroup, SaveToPath, UserName, Password, true, _YoutubeFoundLinks[YoutubeLinksSelectedIndex].FormatCode);
                BackItem();
            }

        }

        private void LoadYoutubeLink()
        {
            RunYoutubeLink();
        }

        bool CanAddLink()
        {
            if (IsBussy)
                return false;
            Uri uri = null;
            return Uri.TryCreate(UriAddress, UriKind.Absolute, out uri);
        }
    }
}
