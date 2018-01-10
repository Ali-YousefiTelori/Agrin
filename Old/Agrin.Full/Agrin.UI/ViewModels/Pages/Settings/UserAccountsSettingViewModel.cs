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
    public class UserAccountsSettingViewModel : ANotifyPropertyChanged
    {
        public UserAccountsSettingViewModel()
        {
            This = this;
            AddUserAccountCommand = new RelayCommand(AddUserAccount, CanAddUserAccount);
            RemoveUserAccountCommand = new RelayCommand<NetworkCredentialInfo>(RemoveUserAccount);
        }
        public static UserAccountsSettingViewModel This;

        public RelayCommand AddUserAccountCommand { get; set; }
        public RelayCommand<NetworkCredentialInfo> RemoveUserAccountCommand { get; set; }

        FastCollection<NetworkCredentialInfo> _items;

        public FastCollection<NetworkCredentialInfo> Items
        {
            get
            {
                if (_items == null)
                    _items = new FastCollection<NetworkCredentialInfo>(ApplicationHelper.DispatcherThread);
                return _items;
            }
            set { _items = value; }
        }

        bool _IsApplicationSetting = true;

        public bool IsApplicationSetting
        {
            get { return _IsApplicationSetting; }
            set { _IsApplicationSetting = value; OnPropertyChanged("IsApplicationSetting"); }
        }

        string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged("Address"); }
        }

        string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; OnPropertyChanged("UserName"); }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); }
        }

        private void RemoveUserAccount(NetworkCredentialInfo item)
        {
            Items.Remove(item);
        }

        private bool CanAddUserAccount()
        {
            return !(String.IsNullOrEmpty(Address) || String.IsNullOrEmpty(UserName) || String.IsNullOrEmpty(Password));
        }

        private void AddUserAccount()
        {
            Items.Add(new NetworkCredentialInfo() { IsUsed = true, Password = Password, ServerAddress = Address, UserName = UserName });
            Password = UserName = Address = "";
        }

    }
}
