#if(!MobileApp || Debug)
using Agrin.BaseViewModels.Lists;
using Agrin.BaseViewModels.Toolbox;
using Agrin.Download.Data.Settings;
using Agrin.Download.Engine;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.RapidBaz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.RapidBaz
{
    public class QueueListRapidBazBaseViewModel : ListBaseViewModel<RapidItemInfo>
    {
        public static bool MustRefresh { get; set; }
        public static QueueListRapidBazBaseViewModel This { get; set; }
        public QueueListRapidBazBaseViewModel()
        {
            This = this;
            MustRefresh = true;
            //RapidbazToolbarBaseViewModel.InitializeData();
        }

        FastCollection<RapidItemInfo> _QueueItems = null;
        public FastCollection<RapidItemInfo> QueueItems
        {
            get
            {
                if (_QueueItems == null)
                {
                    _QueueItems = new FastCollection<RapidItemInfo>(ApplicationHelperMono.DispatcherThread);
                    RapidBazSetting.QueueItems = _QueueItems;
                }
                return _QueueItems;
            }
        }


        RapidItemInfo _SelectedItem = null;

        public RapidItemInfo SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged("SelectedItem"); }
        }

        string _RapidTextStatus;

        public string RapidTextStatus
        {
            get { return _RapidTextStatus; }
            set { _RapidTextStatus = value; OnPropertyChanged("RapidTextStatus"); }
        }

        bool _IsBusy = false;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                if (!value)
                    RapidTextStatus = "";
                OnPropertyChanged("IsBusy");
            }
        }

        bool _IsMessageBoxBusy = false;

        public bool IsMessageBoxBusy
        {
            get { return _IsMessageBoxBusy; }
            set
            {
                _IsMessageBoxBusy = value;
                OnPropertyChanged("IsMessageBoxBusy");
            }
        }

        string _MessageBoxTitle = "حذف لینک های انتخاب شده";

        public string MessageBoxTitle
        {
            get { return _MessageBoxTitle; }
            set
            {
                _MessageBoxTitle = value;
                OnPropertyChanged("MessageBoxTitle");
            }
        }

        string _Message = "آیا میخواهید لینک های انتخاب شده را حذف کنید؟";

        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                OnPropertyChanged("Message");
            }
        }

        public bool CanRefresh()
        {
            return !IsBusy;
        }

        void RefreshList()
        {
            if (IsBusy)
                return;
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

                RapidTextStatus = "در حال دریافت لیست...";

                var list = Agrin.Download.Engine.RapidBazEngineHelper.GetQueueList();
                QueueItems.Clear();
                QueueItems.AddRange(list);

                MustRefresh = false;
                IsBusy = false;

            }, (error) =>
            {
                RapidTextStatus = "خطا در بارگزاری رخ داده است";
                System.Threading.Thread.Sleep(2000);
                IsBusy = false;
            });
        }

        public void Refresh()
        {
            RefreshList();
        }

        public static void CheckRefresh()
        {
            if (MustRefresh)
                This.Refresh();
        }

        public virtual void AddRapidBazLink()
        {
            CompleteListRapidBazBaseViewModel.This.AddRapidBazLink();
        }

        public virtual void AddLinkSelectedItem()
        {
            CompleteListRapidBazBaseViewModel.AddLinkSelectedItem(GetSelectedItems().Where(x => x.IsComplete));
        }

        public void AddToList()
        {
            CompleteListRapidBazBaseViewModel.AddToListSelectedItem(GetSelectedItems().Where(x => x.IsComplete));
        }

        public void AddToListAndDownload()
        {
            CompleteListRapidBazBaseViewModel.AddToListSelectedItem(GetSelectedItems().Where(x => x.IsComplete), true);
        }

        public bool CanAddLinkSelectedItem()
        {
            return GetSelectedItems().Where(x => x.IsComplete).FirstOrDefault() != null;
        }

        public void ShowDeleteMessageBox()
        {
            IsMessageBoxBusy = true;
        }

        public void DeleteSelectedItem()
        {
            if (IsBusy)
                return;
            var list = GetSelectedItems();
            IsMessageBoxBusy = false;
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

                RapidTextStatus = "در حال حذف...";
                foreach (var item in list)
                {
                    var retVal = Agrin.Download.Engine.RapidBazEngineHelper.QueueRemove(SelectedItem.ID);
                }
                IsBusy = false;
                RefreshList();
            }, (error) =>
            {
                RapidTextStatus = "خطا در حذف رخ داده است";
                System.Threading.Thread.Sleep(2000);
                IsBusy = false;
            });
        }

        public bool CanDeleteSelectedItem()
        {
            return GetSelectedItems().Count() > 0;
        }

        public bool CanRetrySelectedItems()
        {
            if (IsBusy)
                return false;
            var list = GetSelectedItems();
            foreach (var item in list)
            {
                if (item.Status == "5")
                    return true;
            }
            return false;
        }

        public void RetrySelectedItems()
        {
            if (IsBusy)
                return;
            var list = GetSelectedItems();
            IsMessageBoxBusy = false;
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

                RapidTextStatus = "در حال اعمال سعی مجدد...";
                foreach (var item in list)
                {
                    if (item.Status == "5")
                    {
                        var retVal = Agrin.Download.Engine.RapidBazEngineHelper.Retry(SelectedItem.ID);
                    }
                }
                IsBusy = false;
                RefreshList();
            }, (error) =>
            {
                RapidTextStatus = "خطا در حذف رخ داده است";
                System.Threading.Thread.Sleep(2000);
                IsBusy = false;
            });
        }

        public void PopupSettingSave()
        {

        }

        public void PopupShowSetting()
        {

        }

        public virtual string CopyLinkLocationBase()
        {
            StringBuilder links = new StringBuilder();
            foreach (var item in GetSelectedItems())
            {
                links.AppendLine(item.Url);
            }
            return links.ToString();
        }
    }
}

#endif