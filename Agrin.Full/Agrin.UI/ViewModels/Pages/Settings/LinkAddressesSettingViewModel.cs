using Agrin.Download.Web;
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
    public class LinkAddressesSettingViewModel : ANotifyPropertyChanged
    {
        public LinkAddressesSettingViewModel()
        {
            This = this;
            AddLinkInfoCommand = new RelayCommand(AddLinkInfo, CanAddLinkInfo);
            DeleteItemCommand = new RelayCommand<AMultiLinkAddress>(DeleteItem, CanDeleteItem);
            CheckLinkInfoCommand = new RelayCommand(CheckLinkInfo, CanCheckLinkInfo);
            CopyToClipboardCommand = new RelayCommand<AMultiLinkAddress>(CopyToClipboard);
        }

        private bool CanCheckLinkInfo()
        {
            int count = 0;
            foreach (var item in Items)
            {
                if (item.IsSelected)
                    count++;
            }
            return IsEnabled && count >= 2;
        }

        public static LinkAddressesSettingViewModel This;
        public RelayCommand AddLinkInfoCommand { get; set; }
        public RelayCommand CheckLinkInfoCommand { get; set; }
        public RelayCommand<AMultiLinkAddress> DeleteItemCommand { get; set; }
        public RelayCommand<AMultiLinkAddress> CopyToClipboardCommand { get; set; }


        FastCollection<ProxyInfo> _Proxies;

        public FastCollection<ProxyInfo> Proxies
        {
            get
            {
                if (_Proxies == null)
                    _Proxies = new FastCollection<ProxyInfo>(ApplicationHelper.DispatcherThread);
                return _Proxies;
            }
            set { _Proxies = value; }
        }
        string _uriAddress;

        public string UriAddress
        {
            get { return _uriAddress; }
            set { _uriAddress = value; OnPropertyChanged("UriAddress"); }
        }

        bool _isEnabled = false;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        string _Messege;
        public string Messege
        {
            get { return _Messege; }
            set { _Messege = value; OnPropertyChanged("Messege"); }
        }

        FastCollection<AMultiLinkAddress> _items;

        public FastCollection<AMultiLinkAddress> Items
        {
            get
            {
                if (_items == null)
                    _items = new FastCollection<AMultiLinkAddress>(ApplicationHelper.DispatcherThread);
                return _items;
            }
            set { _items = value; }
        }

        LinkCheckerHelper _currentChecker;
        public LinkCheckerHelper CurrentChecker
        {
            get { return _currentChecker; }
            set { _currentChecker = value; }
        }

        private bool CanAddLinkInfo()
        {
            //foreach (var item in Items)
            //{
            //    if (item.Address == UriAddress)
            //        return false;
            //}
            Uri uri = null;
            return Uri.TryCreate(UriAddress, UriKind.Absolute, out uri);
        }

        private void AddLinkInfo()
        {
            Items.Add(new AMultiLinkAddress() { Address = UriAddress, IsSelected = true });
            ResetProxyList();
            UriAddress = "";
        }

        private bool CanDeleteItem(AMultiLinkAddress item)
        {
            return Items.Count > 1;
        }

        private void DeleteItem(AMultiLinkAddress item)
        {
            Items.Remove(item);
        }

        public void ProxyToIds()
        {
            foreach (var item in Items)
            {
                item.ProxyID = Proxies[item.ProxySelectedIndex].Id;
            }
        }

        public void ResetProxyList(List<AMultiLinkAddress> setSelectedIndexes = null)
        {
            if (setSelectedIndexes == null)
                setSelectedIndexes = Items.ToList();
            Proxies.Clear();
            Proxies.Add(new ProxyInfo() { ServerAddress = "اتوماتیک", Port = -1 });
            Proxies.AddRange(ProxySettingViewModel.This.Items.ToList());
            foreach (var item in setSelectedIndexes)
            {
                int i = 0;
                foreach (var proxy in Proxies)
                {
                    if (proxy.Id == item.ProxyID)
                    {
                        item.ProxySelectedIndex = i;
                        item.SelectedProxy = Proxies[i];
                        i = -1;
                        break;
                    }
                    i++;
                }
                if (i != -1)
                {
                    item.ProxySelectedIndex = 0;
                    item.SelectedProxy = Proxies.First();
                }
            }
        }

        private void CheckLinkInfo()
        {
            IsEnabled = false;
            Messege = "در حال بازنگری لینک ها...";
            CurrentChecker.CompleteCheck = (mode, checker) =>
                {
                    if (checker == CurrentChecker)
                    {
                        IsEnabled = true;
                        ApplicationHelper.EnterDispatcherThreadActionBegin(() =>
                        {
                            switch (mode)
                            {
                                case LinkaddressCheckMode.Exception:
                                    {
                                        Messege = "خطا در بازنگری فایل ها رخ داده است.";
                                        break;
                                    }
                                case LinkaddressCheckMode.True:
                                    {
                                        Messege = "حجم فایل ها برابر است. شما میتوانید فایل را دانلود کنید";
                                        break;
                                    }
                                case LinkaddressCheckMode.False:
                                    {
                                        Messege = "فایل ها هماهنگ نیستند";
                                        break;
                                    }
                                case LinkaddressCheckMode.UnknownFileSize:
                                    {
                                        Messege = "حجم فایل نامشخص است.";
                                        break;
                                    }
                            }
                        });
                    }
                };
            List<MultiLinkAddress> items = new List<MultiLinkAddress>();
            foreach (var item in Items)
            {
                items.Add(item.ToMuliLink());
            }
            CurrentChecker.Check(items,Proxies.ToList());
        }


        private void CopyToClipboard(AMultiLinkAddress item)
        {
            System.Windows.Clipboard.SetText(item.Address);
        }

        public List<AMultiLinkAddress> ToAMuliLinkAddress(List<MultiLinkAddress> items)
        {
            List<AMultiLinkAddress> retItems = new List<AMultiLinkAddress>();
            foreach (var item in items)
            {
                retItems.Add(new AMultiLinkAddress() { IsSelected = item.IsSelected, Address = item.Address, ProxyID = item.ProxyID });
            }
            return retItems;
        }
        public List<MultiLinkAddress> ToMuliLinkAddress()
        {
            List<MultiLinkAddress> retItems = new List<MultiLinkAddress>();
            foreach (var item in Items)
            {
                retItems.Add(item.ToMuliLink());
            }
            return retItems;
        }
    }

    public class LinkCheckerHelper
    {
        public Action<LinkaddressCheckMode, LinkCheckerHelper> CompleteCheck;
        public void Check(List<MultiLinkAddress> items,List<ProxyInfo> proxies)
        {
            AsyncActions.Action(() =>
            {
                var mode = LinkChecker.CheckMultiLinkInfo(items, proxies);
                if (CompleteCheck != null)
                    CompleteCheck(mode, this);
            });
        }
    }

    public class AMultiLinkAddress : ANotifyPropertyChanged
    {
        int _ProxySelectedIndex;
        public int ProxySelectedIndex
        {
            get { return _ProxySelectedIndex; }
            set { _ProxySelectedIndex = value; OnPropertyChanged("ProxySelectedIndex"); OnPropertyChanged("FullProxyName"); }
        }
        ProxyInfo _SelectedProxy;
        public ProxyInfo SelectedProxy
        {
            get { return _SelectedProxy; }
            set
            {
                if (value != null)
                {
                    _SelectedProxy = value;
                    if (SelectedProxy.Port == -1)
                        FullProxyName = SelectedProxy.ServerAddress;
                    else
                        FullProxyName = SelectedProxy.ServerAddress + ":" + SelectedProxy.Port + (string.IsNullOrEmpty(SelectedProxy.UserName) ? "" : ":" + SelectedProxy.UserName);
                    ProxyID = value.Id;
                    OnPropertyChanged("SelectedProxy");
                }
            }
        }

        string _FullProxyName;

        public string FullProxyName
        {
            get { return _FullProxyName; }
            set { _FullProxyName = value; OnPropertyChanged("FullProxyName"); }
        }
   
        public int ProxyID { get; set; }
        public string Address { get; set; }
        public bool IsSelected { get; set; }

        public MultiLinkAddress ToMuliLink()
        {
            return new MultiLinkAddress() { ProxyID = ProxyID, Address = Address, IsSelected = IsSelected };
        }
    }
}
