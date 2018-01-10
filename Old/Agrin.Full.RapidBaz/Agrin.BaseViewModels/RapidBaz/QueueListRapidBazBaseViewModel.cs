using Agrin.BaseViewModels.Lists;
using Agrin.BaseViewModels.Toolbox;
using Agrin.Download.Data.Settings;
using Agrin.Download.Engine;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.Log;
using Agrin.RapidBaz.Models;
using Agrin.RapidBaz.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.BaseViewModels.RapidBaz
{
    public class QueueListRapidBazBaseViewModel : ListBaseViewModel<RapidItemInfo>
    {
        public static bool MustRefresh { get; set; }
        public static event Action<QueueListRapidBazBaseViewModel> CheckRefreshEvent;
        public QueueListRapidBazBaseViewModel()
        {
            MustRefresh = true;
            CheckRefreshEvent += (vm) =>
            {
                if (MustRefresh && IsRefreshFromPage)
                    Refresh();
            };
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

        /// <summary>
        /// پوشه ی پیشفرض برای رفرش
        /// </summary>
        public string CurrentFolderID { get; set; }

        /// <summary>
        /// اتوماتیک رفرش کردن وقتی که از سوی نرم افزار درخواست میشه این باید برای صفحه ی اصلی باشه نه فایل هایی که در مدیریت پوشه ها نمایش داده میشن
        /// </summary>
        bool _IsRefreshFromPage = true;

        public bool IsRefreshFromPage
        {
            get { return _IsRefreshFromPage; }
            set { _IsRefreshFromPage = value; }
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

        int _CurrentPage = 1;

        public int CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
            set
            {
                _CurrentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }


        public bool CanRefresh()
        {
            return !IsBusy;
        }

        public void RefreshFolderFiles()
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
                RapidTextStatus = "در حال بارگزاری لیست...";
                var list = WebManager.FolderGet(CurrentFolderID);
                QueueItems.Clear();
                QueueItems.AddRange(list);
                foreach (var item in QueueItems)
                {
                    if (item.IsComplete)
                        item.Url = item.Link;
                }
                MustRefresh = false;
                IsBusy = false;
            }, (er) =>
            {
                RapidTextStatus = "خطا در بارگزاری رخ داده است...";
                Thread.Sleep(1500);
                IsBusy = false;
                AutoLogger.LogError(er, "QueueListRapidBazBaseViewModel");
            });
        }

        void RefreshList()
        {
            if (IsBusy)
                return;
            if (!string.IsNullOrEmpty(CurrentFolderID))
            {
                RefreshFolderFiles();
                return;
            }
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

                var list = Agrin.Download.Engine.RapidBazEngineHelper.GetQueueList((CurrentPage - 1) * 50, 50);
                QueueItems.Clear();
                QueueItems.AddRange(list);
                foreach (var item in QueueItems)
                {
                    if (item.IsComplete)
                        item.Url = item.Link;
                }
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

        public static void CheckRefresh(QueueListRapidBazBaseViewModel vm)
        {
            if (CheckRefreshEvent != null)
                CheckRefreshEvent(vm);
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

        public bool CanAddToListAndDownloadOne(RapidItemInfo obj)
        {
            return obj != null && obj.IsComplete;
        }

        public void AddToListAndDownloadOne(RapidItemInfo obj)
        {
            CompleteListRapidBazBaseViewModel.AddToListSelectedItem(new List<RapidItemInfo>() { obj }, true);
        }

        public bool CanRetryOne(RapidItemInfo obj)
        {
            return obj != null && obj.Status == "5";
        }

        public void RetryOne(RapidItemInfo obj)
        {
            if (IsBusy)
                return;
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

                RapidTextStatus = "در حال سعی مجدد...";
                var retVal = Agrin.Download.Engine.RapidBazEngineHelper.Retry(obj.ID);
                IsBusy = false;
                RefreshList();
            }, (error) =>
            {
                RapidTextStatus = "خطا در سعی مجدد رخ داده است";
                System.Threading.Thread.Sleep(2000);
                IsBusy = false;
            });
        }

        public bool CanDeleteOne(RapidItemInfo obj)
        {
            return obj != null && (obj.Status == "9" || obj.Status == "3");
        }

        public void DeleteOne(RapidItemInfo obj)
        {
            dItems = new List<RapidItemInfo>() { obj };
            DeleteSelectedItem();
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

        IEnumerable<RapidItemInfo> dItems = null;
        public void DeleteSelectedItem()
        {
            if (IsBusy)
                return;
            var list = dItems == null ? GetSelectedItems() : dItems;
            dItems = null;
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
                    var retVal = Agrin.Download.Engine.RapidBazEngineHelper.QueueRemove(item.ID);
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
        
        public bool CanPreviousPage()
        {
            return CurrentPage > 1 && !IsBusy;
        }

        public void PreviousPage()
        {
            CurrentPage--;
            RefreshList();
        }

        public bool CanNextPage()
        {
            return QueueItems.Count >= 50 && !IsBusy;
        }

        public void NextPage()
        {
            CurrentPage++;
            RefreshList();
        }
    }
}
