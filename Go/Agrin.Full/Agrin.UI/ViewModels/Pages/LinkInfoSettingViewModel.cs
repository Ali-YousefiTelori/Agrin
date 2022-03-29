using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.UI.ViewModels.Pages.Settings;
using Agrin.UI.Views.Pages;
using Agrin.UI.Views.Pages.Settings;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;

namespace Agrin.UI.ViewModels.Pages
{
    public class LinkInfoSettingViewModel : ANotifyPropertyChanged<LinkInfoSetting>
    {
        public LinkInfoSettingViewModel()
        {
            This = this;
            BackCommand = new RelayCommand(BackItem);
            SaveSettingCommand = new RelayCommand(SaveSetting, CanSaveSetting);
            AddItem(new GeneralSetting(), new RelayCommand(BackToList), true);
            AddItem(new SpeedSetting(), new RelayCommand(BackToList), false);
            AddItem(new ProxySetting(), new RelayCommand(BackToList), false);
            AddItem(new UserAccountsSetting(), new RelayCommand(BackToList), false);
            AddItem(new LinkAddressesSetting(), new RelayCommand(BackToList), false);
            AddItem(new LinkInfoDownloadSetting(), new RelayCommand(BackToList), false);
            AddItem(new ApplicationViewSetting(), new RelayCommand(BackToList), false);
            ViewElementInited = () =>
                {
                    ViewElement.radioGeneral.Checked += radio_Checked;
                    ViewElement.radioSpeed.Checked += radio_Checked;
                    ViewElement.radioProxy.Checked += radio_Checked;
                    ViewElement.radioAccount.Checked += radio_Checked;
                    ViewElement.radioLinks.Checked += radio_Checked;
                    ViewElement.radioDownloadEnd.Checked += radio_Checked;
                    ViewElement.radioAppSetting.Checked += radio_Checked;
                };

        }

        public static LinkInfoSettingViewModel This;
        void radio_Checked(object sender, RoutedEventArgs e)
        {
            SetIndex((int)((FrameworkElement)sender).Tag);
        }

        public RelayCommand SaveSettingCommand { get; set; }
        public RelayCommand BackCommand { get; set; }

        FrameworkElement _CurrentControl;

        public FrameworkElement CurrentControl
        {
            get { return _CurrentControl; }
            set { _CurrentControl = value; OnPropertyChanged("CurrentControl"); }
        }
        FrameworkElement _CurrentControlCollapsed;

        public FrameworkElement CurrentControlCollapsed
        {
            get { return _CurrentControlCollapsed; }
            set { _CurrentControlCollapsed = value; OnPropertyChanged("CurrentControlCollapsed"); }
        }

        private bool _IsSettingForAllLinks;

        public bool IsSettingForAllLinks
        {
            get { return _IsSettingForAllLinks; }
            set { _IsSettingForAllLinks = value; OnPropertyChanged("IsSettingForAllLinks"); }
        }

        bool _IsSettingForNewLinks;

        public bool IsSettingForNewLinks
        {
            get { return _IsSettingForNewLinks; }
            set { _IsSettingForNewLinks = value; OnPropertyChanged("IsSettingForNewLinks"); }
        }

        bool _canSave;
        public bool CanSave
        {
            get { return _canSave; }
            set { _canSave = value; OnPropertyChanged("CanSave"); }
        }

        Dictionary<FrameworkElement, RelayCommand> _Items;

        public Dictionary<FrameworkElement, RelayCommand> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new Dictionary<FrameworkElement, RelayCommand>();
                return _Items;
            }
            set { _Items = value; }
        }

        LinkInfo _CurrentLinkInfo;
        public LinkInfo CurrentLinkInfo
        {
            get { return _CurrentLinkInfo; }
            set { _CurrentLinkInfo = value; }
        }

        static bool nobati = true;
        public void SetIndex(int index)
        {
            var newControl = Items.Keys.ToArray()[index];
            if ((nobati && CurrentControl == newControl) || (!nobati && CurrentControlCollapsed == newControl))
                return;
            else
            {
                Storyboard story = new Storyboard();
                AnimationTimeline animation = new ThicknessAnimation(new Thickness(800, 0, -800, 0), new Duration(new TimeSpan(0, 0, 0, 0, 500))) { AccelerationRatio = 1 };

                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Margin") { Path = "Margin" });
                animation = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity") { Path = "Opacity" });

                ObjectAnimationUsingKeyFrames objKey = new ObjectAnimationUsingKeyFrames() { Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500)) };
                DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame(Visibility.Collapsed);
                objKey.KeyFrames.Add(discreteObjectKeyFrame);
                story.Children.Add(objKey);
                Storyboard.SetTargetProperty(objKey, new PropertyPath("Visibility") { Path = "Visibility" });

                if (nobati)
                {
                    CurrentControlCollapsed = Items.Keys.ToArray()[index];
                    ViewElement.currentControl.BeginStoryboard(story);
                }
                else
                {
                    CurrentControl = Items.Keys.ToArray()[index];
                    ViewElement.currentControlCollapsed.BeginStoryboard(story);
                }
                story = new Storyboard();
                animation = new ThicknessAnimation(new Thickness(-800, 0, 800, 0), new Thickness(0), new Duration(new TimeSpan(0, 0, 0, 0, 500))) { DecelerationRatio = 1 };
                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Margin") { Path = "Margin" });
                animation = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 500)));
                story.Children.Add(animation);
                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity") { Path = "Opacity" });
                objKey = new ObjectAnimationUsingKeyFrames() { Duration = new Duration(new TimeSpan(0)) };
                discreteObjectKeyFrame = new DiscreteObjectKeyFrame(Visibility.Visible);
                objKey.KeyFrames.Add(discreteObjectKeyFrame);
                story.Children.Add(objKey);
                Storyboard.SetTargetProperty(objKey, new PropertyPath("Visibility") { Path = "Visibility" });

                if (!nobati)
                    ViewElement.currentControl.BeginStoryboard(story);
                else
                    ViewElement.currentControlCollapsed.BeginStoryboard(story);

                nobati = !nobati;
            }
        }

        public void BackItem()
        {
            Items[CurrentControl].Execute();
        }

        public void AddItem(FrameworkElement element, RelayCommand command, bool current = false)
        {
            Items.Add(element, command);
            if (current)
                CurrentControl = element;
        }

        void BackToList()
        {
            MainWindow.This.IsShowPage = false;
        }

        private bool CanSaveSetting()
        {
            return CanSave;
        }

        private void SaveSetting()
        {
            CanSave = false;
            if (CurrentLinkInfo == null)
                UIToAppSetting();
            else
                UIToLinkInfoSetting(CurrentLinkInfo);
            BackCommand.Execute();
        }

        public void AppSettingToUI()
        {
            CurrentLinkInfo = null;
            ViewElement.stackAllSetting.Visibility = Visibility.Visible;
            CanSave = true;
            LinkInfoDownloadSettingViewModel.This.EndDownloadSelectedIndex = ApplicationSetting.Current.LinkInfoDownloadSetting.EndDownloadSelectedIndex;
            LinkInfoDownloadSettingViewModel.This.IsEndDownloaded = ApplicationSetting.Current.LinkInfoDownloadSetting.IsEndDownloaded;
            LinkInfoDownloadSettingViewModel.This.IsExtreme = ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme;
            LinkInfoDownloadSettingViewModel.This.TryException = ApplicationSetting.Current.LinkInfoDownloadSetting.TryException;
            LinkInfoDownloadSettingViewModel.This.IsShowBallon = ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon;

            ProxySettingViewModel.This.Items.Clear();
            ProxySettingViewModel.This.Items.AddRange(ApplicationSetting.Current.ProxySetting.Items);
            ProxySettingViewModel.This.IsNotNullProxy = true;

            SpeedSettingViewModel.This.BufferSize = ApplicationSetting.Current.SpeedSetting.BufferSize / 1024;
            SpeedSettingViewModel.This.ConnectionCount = ApplicationSetting.Current.SpeedSetting.ConnectionCount;
            SpeedSettingViewModel.This.SpeedSize = ApplicationSetting.Current.SpeedSetting.SpeedSize / 1024;
            SpeedSettingViewModel.This.IsLimit = ApplicationSetting.Current.SpeedSetting.IsLimit;

            UserAccountsSettingViewModel.This.Items.Clear();
            UserAccountsSettingViewModel.This.Items.AddRange(ApplicationSetting.Current.UserAccountsSetting.Items);

            IsSettingForAllLinks = ApplicationSetting.Current.IsSettingForAllLinks;
            IsSettingForNewLinks = ApplicationSetting.Current.IsSettingForNewLinks;

            
            ApplicationViewSettingViewModel.This.IsShowNotification = ApplicationSetting.Current.IsShowNotification;

        }
        public void UIToAppSetting()
        {
            ApplicationSetting.Current.LinkInfoDownloadSetting.EndDownloadSelectedIndex = LinkInfoDownloadSettingViewModel.This.EndDownloadSelectedIndex;
            ApplicationSetting.Current.LinkInfoDownloadSetting.IsEndDownloaded = LinkInfoDownloadSettingViewModel.This.IsEndDownloaded;
            ApplicationSetting.Current.LinkInfoDownloadSetting.IsExtreme = LinkInfoDownloadSettingViewModel.This.IsExtreme;
            ApplicationSetting.Current.LinkInfoDownloadSetting.TryException = LinkInfoDownloadSettingViewModel.This.TryException;
            ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon = LinkInfoDownloadSettingViewModel.This.IsShowBallon;

            ApplicationSetting.Current.ProxySetting.Items = ProxySettingViewModel.This.Items.ToList();

            ApplicationSetting.Current.SpeedSetting.BufferSize = SpeedSettingViewModel.This.BufferSize * 1024;
            ApplicationSetting.Current.SpeedSetting.ConnectionCount = SpeedSettingViewModel.This.ConnectionCount;
            ApplicationSetting.Current.SpeedSetting.SpeedSize = SpeedSettingViewModel.This.SpeedSize * 1024;
            ApplicationSetting.Current.SpeedSetting.IsLimit = SpeedSettingViewModel.This.IsLimit;

            ApplicationSetting.Current.UserAccountsSetting.Items = UserAccountsSettingViewModel.This.Items.ToList();

            ApplicationSetting.Current.IsSettingForAllLinks = IsSettingForAllLinks;
            ApplicationSetting.Current.IsSettingForNewLinks = IsSettingForNewLinks;

            ApplicationSetting.Current.IsShowNotification = ApplicationViewSettingViewModel.This.IsShowNotification;

            MainWindow.This.mainNotify.Visibility = ApplicationSetting.Current.IsShowNotification ? Visibility.Visible : Visibility.Collapsed;
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
        }
        public void LinkInfoSettingToUI(LinkInfo linkInfo)
        {
            CurrentLinkInfo = linkInfo;
            ViewElement.stackAllSetting.Visibility = Visibility.Collapsed;
            CanSave = true;
            LinkInfoDownloadSettingViewModel.This.EndDownloadSelectedIndex = (int)linkInfo.Management.EndDownloadSystemMode;
            LinkInfoDownloadSettingViewModel.This.IsEndDownloaded = linkInfo.Management.IsEndDownload;
            LinkInfoDownloadSettingViewModel.This.IsExtreme = linkInfo.Management.IsTryExtreme;
            LinkInfoDownloadSettingViewModel.This.TryException = linkInfo.Management.TryAginCount;
            LinkInfoDownloadSettingViewModel.This.IsShowBallon = linkInfo.Management.IsShowBalloon;

            ProxySettingViewModel.This.UserName = "";
            ProxySettingViewModel.This.Password = "";
            ProxySettingViewModel.This.IsUserPass = false;
            ProxySettingViewModel.This.ServerAddress = "";
            ProxySettingViewModel.This.Items.Clear();
            ProxySettingViewModel.This.Items.AddRange(linkInfo.Management.MultiProxy);
            ProxySettingViewModel.This.IsNotNullProxy = true;

            SpeedSettingViewModel.This.BufferSize = linkInfo.Management.ReadBuffer / 1024;
            SpeedSettingViewModel.This.ConnectionCount = linkInfo.DownloadingProperty.ConnectionCount;
            SpeedSettingViewModel.This.SpeedSize = linkInfo.Management.LimitPerSecound / 1024;
            SpeedSettingViewModel.This.IsLimit = linkInfo.Management.IsLimit;

            UserAccountsSettingViewModel.This.Items.Clear();
            if (linkInfo.Management.NetworkUserPass != null)
            {
                UserAccountsSettingViewModel.This.UserName = linkInfo.Management.NetworkUserPass.UserName;
                UserAccountsSettingViewModel.This.Password = linkInfo.Management.NetworkUserPass.Password;
            }
            else
            {
                UserAccountsSettingViewModel.This.UserName = "";
                UserAccountsSettingViewModel.This.Password = "";
            }


            GeneralSettingViewModel.This.FileName = linkInfo.PathInfo.FileName;
            GeneralSettingViewModel.This.FileType = linkInfo.PathInfo.FileType;
            GeneralSettingViewModel.This.Description = linkInfo.Management.Description;
            GeneralSettingViewModel.This.SaveAddress = linkInfo.PathInfo.SavePath;
            GeneralSettingViewModel.This.Size = linkInfo.DownloadingProperty.Size;

            LinkAddressesSettingViewModel.This.UriAddress = "";
            LinkAddressesSettingViewModel.This.IsEnabled = true;
            LinkAddressesSettingViewModel.This.CurrentChecker = new LinkCheckerHelper();
            LinkAddressesSettingViewModel.This.Messege = ApplicationHelper.GetAppResource("CheckLinksHelp_Language");
            LinkAddressesSettingViewModel.This.Items.Clear();
            List<AMultiLinkAddress> items = LinkAddressesSettingViewModel.This.ToAMuliLinkAddress(linkInfo.Management.MultiLinks);
            LinkAddressesSettingViewModel.This.ResetProxyList(items);
            LinkAddressesSettingViewModel.This.Items.AddRange(items);
        }
        public void UIToLinkInfoSetting(LinkInfo linkInfo)
        {
            ViewElement.stackAllSetting.Visibility = Visibility.Collapsed;
            CanSave = true;
            linkInfo.Management.EndDownloadSystemMode = (Download.Web.Link.CompleteDownloadSystemMode)LinkInfoDownloadSettingViewModel.This.EndDownloadSelectedIndex;
            linkInfo.Management.IsEndDownload = LinkInfoDownloadSettingViewModel.This.IsEndDownloaded;
            linkInfo.Management.IsTryExtreme = LinkInfoDownloadSettingViewModel.This.IsExtreme;
            linkInfo.Management.TryAginCount = LinkInfoDownloadSettingViewModel.This.TryException;
            linkInfo.Management.IsApplicationSetting = false;
            linkInfo.Management.IsShowBalloon = LinkInfoDownloadSettingViewModel.This.IsShowBallon;

            linkInfo.Management.MultiProxy = ProxySettingViewModel.This.Items.ToList();

            linkInfo.Management.ReadBuffer = SpeedSettingViewModel.This.BufferSize * 1024;
            linkInfo.DownloadingProperty.ConnectionCount = SpeedSettingViewModel.This.ConnectionCount;
            linkInfo.Management.LimitPerSecound = SpeedSettingViewModel.This.SpeedSize * 1024;
            linkInfo.Management.IsLimit = SpeedSettingViewModel.This.IsLimit;

            if (!String.IsNullOrEmpty(UserAccountsSettingViewModel.This.UserName) && !String.IsNullOrEmpty(UserAccountsSettingViewModel.This.Password))
            {
                if (linkInfo.Management.NetworkUserPass == null)
                    linkInfo.Management.NetworkUserPass = new Download.Web.Link.NetworkCredentialInfo();
                linkInfo.Management.NetworkUserPass.UserName = UserAccountsSettingViewModel.This.UserName;
                linkInfo.Management.NetworkUserPass.Password = UserAccountsSettingViewModel.This.Password;
            }
            else if (String.IsNullOrEmpty(UserAccountsSettingViewModel.This.UserName) && String.IsNullOrEmpty(UserAccountsSettingViewModel.This.Password))
            {
                linkInfo.Management.NetworkUserPass = null;
            }


            if (linkInfo.PathInfo.AddressFileName != GeneralSettingViewModel.This.FileName)
                linkInfo.PathInfo.UserFileName = GeneralSettingViewModel.This.FileName;
            linkInfo.Management.Description = GeneralSettingViewModel.This.Description;
            if (linkInfo.PathInfo.AppSavePath != GeneralSettingViewModel.This.SaveAddress)
                linkInfo.PathInfo.UserSavePath = GeneralSettingViewModel.This.SaveAddress;

            LinkAddressesSettingViewModel.This.ProxyToIds();
            linkInfo.Management.MultiLinks = LinkAddressesSettingViewModel.This.ToMuliLinkAddress();
            linkInfo.SaveThisLink();
        }
    }
}
