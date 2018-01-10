#if(!MobileApp || Debug)
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
    public class FolderFilesExplorerBaseViewModel : ANotifyPropertyChanged
    {
        public static FolderFilesExplorerBaseViewModel This { get; set; }
        public static bool MustRefresh { get; set; }
        public FolderFilesExplorerBaseViewModel()
        {
            MustRefresh = true;
            This = this;
        }

        FastCollection<DocumantInfo> _Items;
        public FastCollection<DocumantInfo> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new FastCollection<DocumantInfo>(ApplicationHelperMono.DispatcherThread);
                return _Items;
            }
            set { _Items = value; }
        }

        string _RapidTextStatus = "";

        public string RapidTextStatus
        {
            get { return _RapidTextStatus; }
            set
            {
                _RapidTextStatus = value;
                OnPropertyChanged("RapidTextStatus");
            }
        }

        bool _IsAddFolderMessage;

        public bool IsAddFolderMessage
        {
            get { return _IsAddFolderMessage; }
            set { _IsAddFolderMessage = value; OnPropertyChanged("IsAddFolderMessage"); }
        }

        string _AddFolderName = "";

        public string AddFolderName
        {
            get { return _AddFolderName; }
            set { _AddFolderName = value; OnPropertyChanged("AddFolderName"); }
        }

        bool _IsBusy;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged("IsBusy"); }
        }

        bool _IsFolderList = true;

        public bool IsFolderList
        {
            get { return _IsFolderList; }
            set { _IsFolderList = value; OnPropertyChanged("IsFolderList"); }
        }

        bool _IsShowMessage = false;
        public bool IsShowMessage
        {
            get { return _IsShowMessage; }
            set { _IsShowMessage = value; OnPropertyChanged("IsShowMessage"); }
        }

        string _Message = "";
        public string Message
        {
            get { return _Message; }
            set { _Message = value; OnPropertyChanged("Message"); }
        }

        public bool CanDeleteFolderMessage()
        {
            return GetSelectedLinks().Count() > 0;
        }

        public void DeleteFolderMessage()
        {
            Message = "آیا می خواهید پوشه های انتخاب شده را حذف کنید؟";
            IsShowMessage = true;
        }

        public void RefreshFolderListManual()
        {
            RefreshFolderList(true);
        }

        public void AddFolder()
        {
            AddFolderName = "";
            IsAddFolderMessage = true;
        }

        public bool CanAddFolder()
        {
            return !string.IsNullOrEmpty(AddFolderName);
        }

        public void AddFolderOK()
        {
            IsAddFolderMessage = false;
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
                RapidTextStatus = "در حال درج پوشه...";

                var del = WebManager.FolderMake(AddFolderName);
                IsBusy = false;
                RefreshFolderListManual();
            }, (er) =>
            {
                RapidTextStatus = "خطا در درج پوشه رخ داده است...";
                Thread.Sleep(1500);
                IsBusy = false;
                IsAddFolderMessage = true;
                AutoLogger.LogError(er, "FolderFilesExplorerBaseViewModel");
            });
        }

        public void DeleteSelectionFolder()
        {
            IsShowMessage = false;
            RapidTextStatus = "در حال بررسی...";
            IsBusy = true;
            var list = GetSelectedLinks();
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
                    var del = WebManager.FolderRemove(((FolderInfo)item).ID);
                    MustRefresh = true;
                }

                IsBusy = false;
                if (MustRefresh)
                    RefreshFolderListManual();
            }, (er) =>
            {
                RapidTextStatus = "خطا در حذف رخ داده است...";
                Thread.Sleep(1500);
                IsBusy = false;
                if (MustRefresh)
                    RefreshFolderListManual();
                AutoLogger.LogError(er, "FolderFilesExplorerBaseViewModel");
            });
        }

        public virtual IEnumerable<DocumantInfo> GetSelectedLinks()
        {
            return null;
        }

        public void RefreshFolderList(bool isManual = false)
        {
            if (!MustRefresh && !isManual)
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
                var list = WebManager.FolderList();
                MustRefresh = false;
                Items.Clear();
                Items.AddRange(list);
                IsBusy = false;
            }, (er) =>
            {
                RapidTextStatus = "خطا در بارگزاری رخ داده است...";
                Thread.Sleep(1500);
                IsBusy = false;
                AutoLogger.LogError(er, "FolderFilesExplorerBaseViewModel");
            });
        }

        public void GetFolderFiles(FolderInfo folder)
        {
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
                var list = WebManager.FolderGet(folder.ID);
                Items.Clear();
                Items.AddRange(list);
                IsFolderList = false;
                IsBusy = false;
            }, (er) =>
            {
                RapidTextStatus = "خطا در بارگزاری رخ داده است...";
                Thread.Sleep(1500);
                IsBusy = false;
                AutoLogger.LogError(er, "FolderFilesExplorerBaseViewModel");
            });
        }

        public static void CheckRefresh()
        {
            if (MustRefresh)
                This.RefreshFolderListManual();
        }
    }
}

#endif