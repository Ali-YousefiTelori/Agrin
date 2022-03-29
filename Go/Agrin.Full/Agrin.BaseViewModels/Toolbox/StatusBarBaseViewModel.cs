using Agrin.Download.Data;
using Agrin.Download.Data.Serializition;
using Agrin.Download.Data.Settings;
using Agrin.Download.Manager;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.BaseViewModels.Toolbox
{
    public class StatusBarItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
    }

    public class StatusBarBaseViewModel : ANotifyPropertyChanged
    {
        public StatusBarBaseViewModel()
        {
            if (ApplicationLinkInfoManager.Current == null)
                return;
            ApplicationLinkInfoManager.Current.ChangedDownloadedSizeAction = (value) =>
            {
                DownloadedSize = value;
            };

            ApplicationLinkInfoManager.Current.ChangedDownloadedSize();

            SpeedLimiterItems.Add(new StatusBarItem() { Text = "نامحدود" });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "5 کیلوبایت", Value = 5 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "10 کیلوبایت", Value = 10 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "20 کیلوبایت", Value = 20 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "30 کیلوبایت", Value = 30 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "40 کیلوبایت", Value = 40 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "60 کیلوبایت", Value = 60 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "100 کیلوبایت", Value = 100 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "150 کیلوبایت", Value = 150 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "200 کیلوبایت", Value = 200 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "300 کیلوبایت", Value = 300 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "400 کیلوبایت", Value = 400 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "500 کیلوبایت", Value = 500 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "700 کیلوبایت", Value = 700 });
            SpeedLimiterItems.Add(new StatusBarItem() { Text = "900 کیلوبایت", Value = 900 });

            ConnectionCountItems.Add(new StatusBarItem() { Text = "1", Value = 1 });
            ConnectionCountItems.Add(new StatusBarItem() { Text = "4", Value = 4 });
            ConnectionCountItems.Add(new StatusBarItem() { Text = "8", Value = 8 });
            ConnectionCountItems.Add(new StatusBarItem() { Text = "16", Value = 16 });
            ConnectionCountItems.Add(new StatusBarItem() { Text = "24", Value = 24 });
            ConnectionCountItems.Add(new StatusBarItem() { Text = "32", Value = 32 });

            foreach (var item in ConnectionCountItems)
            {
                if ((int)item.Value == ApplicationSetting.Current.SpeedSetting.ConnectionCount)
                {
                    ConnectionSelectedItem = item;
                    break;
                }
            }

            if (ConnectionSelectedItem == null)
            {
                ConnectionCountItems.Add(new StatusBarItem() { Text = ApplicationSetting.Current.SpeedSetting.ConnectionCount.ToString(), Value = ApplicationSetting.Current.SpeedSetting.ConnectionCount });
                ConnectionSelectedItem = ConnectionCountItems.Last();
            }

            if (ApplicationSetting.Current.SpeedSetting.IsLimit)
            {
                var speed = ApplicationSetting.Current.SpeedSetting.SpeedSize / 1024;

                foreach (var item in SpeedLimiterItems)
                {
                    if (item.Value != null && (int)item.Value == speed)
                    {
                        SpeedLimiterSelectedItem = item;
                        break;
                    }
                }

                if (SpeedLimiterSelectedItem == null)
                {
                    SpeedLimiterItems.Add(new StatusBarItem() { Text = speed.ToString() + " کیلوبایت", Value = speed });
                    SpeedLimiterSelectedItem = SpeedLimiterItems.Last();
                }
            }
            else
            {
                SpeedLimiterSelectedItem = SpeedLimiterItems.First();
            }

            var proxy = ApplicationSetting.Current.ProxySetting.GetFirstActiveProxy();
            if (proxy != null)
            {
                if (proxy.IsSystemProxy)
                    SelectedProxyIndex = 1;
            }

            IsUserSelected = true;
        }

        private FastCollection<StatusBarItem> _SpeedLimiterItems = null;

        public FastCollection<StatusBarItem> SpeedLimiterItems
        {
            get
            {
                if (_SpeedLimiterItems == null)
                    _SpeedLimiterItems = new FastCollection<StatusBarItem>(ApplicationHelperBase.DispatcherThread);
                return _SpeedLimiterItems;
            }
            set { _SpeedLimiterItems = value; }
        }


        private FastCollection<StatusBarItem> _ConnectionCountItems = null;

        public FastCollection<StatusBarItem> ConnectionCountItems
        {
            get
            {
                if (_ConnectionCountItems == null)
                    _ConnectionCountItems = new FastCollection<StatusBarItem>(ApplicationHelperBase.DispatcherThread);
                return _ConnectionCountItems;
            }
            set { _ConnectionCountItems = value; }
        }

        public bool IsUserSelected = false;

        StatusBarItem _SpeedLimiterSelectedItem;

        public StatusBarItem SpeedLimiterSelectedItem
        {
            get { return _SpeedLimiterSelectedItem; }
            set
            {
                _SpeedLimiterSelectedItem = value;
                OnPropertyChanged("SpeedLimiterSelectedItem");
                if (IsUserSelected)
                {
                    CanUserChanged = false;
                    var old = ApplicationSetting.Current.IsSettingForAllLinks;
                    ApplicationSetting.Current.IsSettingForAllLinks = true;

                    ApplicationSetting.Current.SpeedSetting.IsLimit = value.Value != null;
                    if (ApplicationSetting.Current.SpeedSetting.IsLimit)
                    {
                        ApplicationSetting.Current.SpeedSetting.SpeedSize = (int)value.Value * 1024;
                    }

                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    ApplicationSetting.Current.IsSettingForAllLinks = old;

                    AsyncActions.Action(() =>
                    {
                        System.Threading.Thread.Sleep(1000);
                        CanUserChanged = true;
                    });
                }
            }
        }

        StatusBarItem _ConnectionSelectedItem;

        public StatusBarItem ConnectionSelectedItem
        {
            get { return _ConnectionSelectedItem; }
            set
            {
                _ConnectionSelectedItem = value;
                OnPropertyChanged("ConnectionSelectedItem");
                if (IsUserSelected)
                {
                    CanUserChanged = false;
                    ApplicationSetting.Current.SpeedSetting.ConnectionCount = (int)value.Value;
                    var old = ApplicationSetting.Current.IsSettingForAllLinks;
                    ApplicationSetting.Current.IsSettingForAllLinks = true;
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    ApplicationSetting.Current.IsSettingForAllLinks = old;

                    AsyncActions.Action(() =>
                        {
                            System.Threading.Thread.Sleep(1000);
                            CanUserChanged = true;
                        });
                }
            }
        }

        int _SelectedProxyIndex = 0;

        public int SelectedProxyIndex
        {
            get
            {
                return _SelectedProxyIndex;
            }
            set
            {
                _SelectedProxyIndex = value;
                OnPropertyChanged("SelectedProxyIndex");
                if (IsUserSelected)
                {
                    CanUserChanged = false;
                    ApplicationSetting.Current.ProxySetting.Items.Clear();
                    ApplicationSetting.Current.ProxySetting.Items.Add(new Download.Web.Link.ProxyInfo() { Id = 1, IsSystemProxy = false, IsSelected = value == 0, Port = -1 });
                    ApplicationSetting.Current.ProxySetting.Items.Add(new Download.Web.Link.ProxyInfo() { Id = 2, IsSystemProxy = true, IsSelected = value == 1 });

                    var old = ApplicationSetting.Current.IsSettingForAllLinks;
                    ApplicationSetting.Current.IsSettingForAllLinks = true;
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    ApplicationSetting.Current.IsSettingForAllLinks = old;

                    AsyncActions.Action(() =>
                    {
                        System.Threading.Thread.Sleep(1000);
                        CanUserChanged = true;
                    });
                }
            }
        }

        bool _CanUserChanged = true;

        public bool CanUserChanged
        {
            get { return _CanUserChanged; }
            set
            {
                _CanUserChanged = value;
                OnPropertyChanged("CanUserChanged");
            }
        }

        long _DownloadedSize = 0;

        public long DownloadedSize
        {
            get { return _DownloadedSize; }
            set
            {
                _DownloadedSize = value;
                OnPropertyChanged("DownloadedSize");
            }
        }

        double _TotalApplicationSpeed;

        public double TotalApplicationSpeed
        {
            get { return _TotalApplicationSpeed; }
            set
            {
                _TotalApplicationSpeed = value;
                OnPropertyChanged("TotalApplicationSpeed");
            }
        }

        bool _isGettingHostInfoProperties = false;
        IPPropertiesSerialize _HostInfoProperties = null;
        public IPPropertiesSerialize HostInfoProperties
        {
            get
            {
                try
                {
                    if (_HostInfoProperties != null)
                        return _HostInfoProperties;
                    if (_isGettingHostInfoProperties)
                        return _HostInfoProperties;
                    _isGettingHostInfoProperties = true;
                    AsyncActions.Action(() =>
                    {

                        while (true)
                        {
                            try
                            {
                                var ip = ApplicationIPsData.AddNewHostIP("");

                                if (ip != null)
                                {
                                    _HostInfoProperties = ip;
                                    _HostFlag = ApplicationIPsData.GetFlagByCountryCode(ip.CountryCode);
                                    ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                                    {
                                        OnPropertyChanged("HostInfoProperties");
                                        OnPropertyChanged("HostFlag");
                                    });
                                    break;
                                }
                                Thread.Sleep(5000);
                            }
                            catch (Exception e)
                            {
                                Log.AutoLogger.LogError(e, "HostInfoProperties");
                                Thread.Sleep(5000);
                            }
                        }
                        _isGettingHostInfoProperties = false;
                    });
                    return _HostInfoProperties;
                }
                catch
                {
                    return _HostInfoProperties;
                }
            }
        }

        byte[] _HostFlag = null;
        public byte[] HostFlag
        {
            get
            {
                if (_HostFlag == null)
                {
                    if (HostInfoProperties != null)
                    {
                        _HostFlag = ApplicationIPsData.GetFlagByCountryCode(HostInfoProperties.CountryCode);
                    }
                }
                return _HostFlag;
            }
        }

        /// <summary>
        /// کلید تبادل برای تبدیل به تصویر و نگهداری فقط تصویر در حافظه و جلوگیری از کپی شدن ان
        /// BytesToImageConverter استفاده شده نمونه در رابط کاربری با استفاده از کانورتور
        /// </summary>
        public string CountryCodeKey
        {
            get
            {
                if (HostInfoProperties == null)
                    return null;
                return "IpProperties" + HostInfoProperties.CountryCode;
            }
        }


        public void RetryGetFlag()
        {
            _HostFlag = null;
            _HostInfoProperties = null;
            OnPropertyChanged("HostFlag");
            OnPropertyChanged("HostInfoProperties");
        }
    }
}
