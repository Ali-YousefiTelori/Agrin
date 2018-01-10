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
    public class CompleteListRapidBazBaseViewModel : ListBaseViewModel<RapidItemInfo>
    {
        public static bool MustRefresh { get; set; }
        public static CompleteListRapidBazBaseViewModel This { get; set; }
        public CompleteListRapidBazBaseViewModel()
        {
            This = this;
            MustRefresh = true;
            //RapidbazToolbarBaseViewModel.InitializeData();
        }

        FastCollection<RapidItemInfo> _Items = null;
        public FastCollection<RapidItemInfo> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new FastCollection<RapidItemInfo>(ApplicationHelperMono.DispatcherThread);
                }
                return _Items;
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

        public void Refresh()
        {
            RefreshList();
        }

        public virtual void AddRapidBazLink()
        {

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
                var list = Agrin.Download.Engine.RapidBazEngineHelper.GetCompleteList();
                foreach (var item in list)
                {
                    item.Progress = "100";
                }
                Items.Clear();
                Items.AddRange(list);

                MustRefresh = false;
                IsBusy = false;

            }, (error) =>
            {
                RapidTextStatus = "خطا در بارگزاری رخ داده است";
                System.Threading.Thread.Sleep(2000);
                IsBusy = false;
            });
        }

        public static void CheckRefresh()
        {
            if (MustRefresh)
                This.Refresh();
        }

        public virtual void AddLinkSelectedItem()
        {
            AddLinkSelectedItem(GetSelectedItems().Where(x => x.IsComplete));
        }

        public bool CanAddLinkSelectedItem()
        {
            return GetSelectedItems().Where(x => x.IsComplete).FirstOrDefault() != null;
        }

        public void AddToList()
        {
            AddToListSelectedItem(GetSelectedItems().Where(x => x.IsComplete));
        }

        public void AddToListAndDownload()
        {
            AddToListSelectedItem(GetSelectedItems().Where(x => x.IsComplete), true);
        }

        public bool CanAddToListAndDownloadOne(RapidItemInfo obj)
        {
            return obj != null && obj.IsComplete;
        }

        public void AddToListAndDownloadOne(RapidItemInfo obj)
        {
            AddToListSelectedItem(new List<RapidItemInfo>() { obj }, true);
        }

        public static void AddLinkSelectedItem(IEnumerable<RapidItemInfo> items)
        {
            if (ApplicationSetting.Current == null || items == null || items.Count() == 0)
                return;
            bool save = false;
            foreach (var item in items)
            {
                if (!ApplicationSetting.Current.RapidBazSetting.DownloadedRapidBazItems.Contains(item.ID))
                {
                    ApplicationSetting.Current.RapidBazSetting.DownloadedRapidBazItems.Add(item.ID);
                    item.Validate();
                    save = true;
                }
            }
            if (save)
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
        }

        public static void AddToListSelectedItem(IEnumerable<RapidItemInfo> items, bool nowStartDownload = false)
        {
            if (ApplicationSetting.Current == null || items == null || items.Count() == 0)
                return;
            bool save = false;
            foreach (var item in items)
            {
                if (!ApplicationSetting.Current.RapidBazSetting.DownloadedRapidBazItems.Contains(item.ID))
                {
                    ApplicationSetting.Current.RapidBazSetting.DownloadedRapidBazItems.Add(item.ID);
                    item.Validate();
                    save = true;
                }
            }
            LinkInfoesDownloadingManagerBaseViewModel.AddLinkInfo(items.Select<RapidItemInfo, string>(x => x.Link).ToList(), null, null, null, null, nowStartDownload, -1);
            if (save)
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
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
                    var retVal = Agrin.Download.Engine.RapidBazEngineHelper.Free(SelectedItem.ID);
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

        public virtual string CopyLinkLocation()
        {
            StringBuilder links = new StringBuilder();
            foreach (var item in GetSelectedItems())
            {
                links.AppendLine(item.Link);
            }
            return links.ToString();
        }
    }
}
