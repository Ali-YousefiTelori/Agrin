using Agrin.Download.Web.Link;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.Pages.Settings
{
    public class ProxySettingViewModel : ANotifyPropertyChanged
    {
        public ProxySettingViewModel()
        {
            This = this;
            AddProxyCommand = new RelayCommand(AddProxy, CanAddProxy);
            GetSystemProxyCommand = new RelayCommand(GetSystemProxy);
            DeleteItemCommand = new RelayCommand<ProxyInfo>(DeleteItem);
        }

        public static ProxySettingViewModel This;

        public RelayCommand AddProxyCommand { get; set; }
        public RelayCommand GetSystemProxyCommand { get; set; }
        public RelayCommand<ProxyInfo> DeleteItemCommand { get; set; }

        FastCollection<ProxyInfo> _Items;
        public FastCollection<ProxyInfo> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new FastCollection<ProxyInfo>(ApplicationHelper.DispatcherThread);
                return _Items;
            }
            set { _Items = value; }
        }
        string _ServerAddress;
        public string ServerAddress
        {
            get { return _ServerAddress; }
            set { _ServerAddress = value; OnPropertyChanged("ServerAddress"); }
        }

        int _port;
        public int Port
        {
            get { return _port; }
            set { _port = value; OnPropertyChanged("Port"); }
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

        bool _IsUserPass;

        public bool IsUserPass
        {
            get { return _IsUserPass; }
            set { _IsUserPass = value; OnPropertyChanged("IsUserPass"); }
        }

        bool _IsNotNullProxy = true;
        public bool IsNotNullProxy
        {
            get { return _IsNotNullProxy; }
            set { _IsNotNullProxy = value; OnPropertyChanged("IsNotNullProxy"); }
        }

        private void GetSystemProxy()
        {
            Items.Add(new ProxyInfo() { Id = GetNewId(), ServerAddress = "پروکسی سیستم", IsSystemProxy = true, IsSelected = true });
            LinkAddressesSettingViewModel.This.ResetProxyList();
            //var proxy = System.Net.WebProxy.GetDefaultProxy();
            //if (proxy.Address == null)
            //    return;

            //ServerAddress = proxy.Address.ToString().Replace("http:", "").Trim(new char[] { '/', '\\' });
            //ServerAddress = ServerAddress.Replace(":" + proxy.Address.Port, "");
            //Port = proxy.Address.Port;
            //IsUserPass = proxy.UseDefaultCredentials;
            //if (IsUserPass)
            //{
            //    UserName = ((System.Net.NetworkCredential)proxy.Credentials).UserName;
            //    Password = ((System.Net.NetworkCredential)proxy.Credentials).Password;
            //}
        }

        private bool CanAddProxy()
        {
            if (!IsNotNullProxy)
                return true;
            bool can = String.IsNullOrWhiteSpace(ServerAddress) || Port < 0;
            if (can)
                return false;
            if (IsUserPass)
                return !String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password);
            return true;
        }

        private void AddProxy()
        {
            if (!IsNotNullProxy)
            {
                Items.Add(new ProxyInfo() { Id = 1, ServerAddress = "بدون پروکسی", Port = -1, IsUserPass = false, UserName = "", Password = "", IsSelected = true });
            }
            else
            {
                Items.Add(new ProxyInfo() { Id = GetNewId(), ServerAddress = ServerAddress, Port = Port, IsUserPass = IsUserPass, UserName = UserName, Password = Password, IsSelected = true });
                UserName = Password = ServerAddress = "";
            }
            LinkAddressesSettingViewModel.This.ResetProxyList();
        }

        int GetNewId()
        {
            int i = 0;
            foreach (var item in Items)
            {
                if (item.Id > i)
                    i = item.Id;
            }
            i++;
            return i <= 1 ? 2 : i;
        }

        private void DeleteItem(ProxyInfo item)
        {
            Items.Remove(item);
            LinkAddressesSettingViewModel.This.ResetProxyList();
        }
    }
}