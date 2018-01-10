using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
#if (!MobileApp && !XamarinApp)
using Agrin.Network.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.WindowLayouts.Asuda
{
    public class BasketReceiverSettingPageBaseViewModel : ANotifyPropertyChanged
    {
        public BasketReceiverSettingPageBaseViewModel()
        {
            NetworkProxySettings.ChangedAction = () =>
            {
                Download.Data.SerializeData.SaveNetworkProxySettingsFile();
            };
        }

        string _newExtensionName;
        string _newAppProcessName;
        FastCollection<ExtensionInfo> _Extensions;
        FastCollection<ApplicationInfo> _Apps;

        public string NewExtensionName
        {
            get
            {
                return _newExtensionName;
            }
            set
            {
                _newExtensionName = value;
                OnPropertyChanged("NewExtensionName");
            }
        }

        public FastCollection<ExtensionInfo> Extensions
        {
            get
            {
                if (_Extensions == null)
                {
                    _Extensions = new FastCollection<ExtensionInfo>(AsudaDataOptimizerBaseViewModel.Dispatcher);
                    _Extensions.AddRange(NetworkProxySettings.Current.SupportFileExtensions);
                }
                return _Extensions;
            }
            set
            {
                _Extensions = value;
            }
        }

        public FastCollection<ApplicationInfo> Apps
        {
            get
            {
                if (_Apps == null)
                {
                    _Apps = new FastCollection<ApplicationInfo>(AsudaDataOptimizerBaseViewModel.Dispatcher);
                    _Apps.AddRange(NetworkProxySettings.Current.SupportApps);
                }
                return _Apps;
            }
            set
            {
                _Apps = value;
            }
        }

        public string NewAppProcessName
        {
            get
            {
                return _newAppProcessName;
            }
            set
            {
                _newAppProcessName = value;
                OnPropertyChanged("NewAppProcessName");
            }
        }

        public void RemoveExtension(ExtensionInfo extensionInfo)
        {
            Extensions.Remove(extensionInfo);
            NetworkProxySettings.Current.SupportFileExtensions.Remove(extensionInfo);
            Download.Data.SerializeData.SaveNetworkProxySettingsFile();
        }

        public void AddExtension()
        {
            var ext = new ExtensionInfo() { Extension = "." + NewExtensionName.ToLower() };
            Extensions.Add(ext);
            NetworkProxySettings.Current.SupportFileExtensions.Add(ext);
            NewExtensionName = null;
            Download.Data.SerializeData.SaveNetworkProxySettingsFile();
        }

        public bool CanAddExtension()
        {
            return !string.IsNullOrEmpty(NewExtensionName) && !NewExtensionName.Contains(".");
        }

        public void RemoveApp(ApplicationInfo applicationInfo)
        {
            Apps.Remove(applicationInfo);
            NetworkProxySettings.Current.SupportApps.Remove(applicationInfo);
            Download.Data.SerializeData.SaveNetworkProxySettingsFile();
        }

        public void AddApp()
        {
            var app = new ApplicationInfo() { ProcessName = NewAppProcessName };
            Apps.Add(app);
            NetworkProxySettings.Current.SupportApps.Add(app);
            NewAppProcessName = null;
            Download.Data.SerializeData.SaveNetworkProxySettingsFile();
        }

        public bool CanAddApp()
        {
            return !string.IsNullOrEmpty(NewAppProcessName);
        }
    }
}
#endif
