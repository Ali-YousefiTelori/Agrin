using Agrin.Download.Engine;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.RapidService.Models;
using Agrin.UI.ViewModels.Pages;
using Agrin.UI.ViewModels.Toolbox;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agrin.UI.ViewModels.Lists
{
    public class RapidBazGetListViewModel : ANotifyPropertyChanged
    {
        public static RapidBazGetListViewModel This { get; set; }
        public static bool MustRefresh { get; set; }

        public RapidBazGetListViewModel()
        {
            This = this;
            DownloadCommand = new RelayCommand<RapidItemInfo>(Download);
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            CompleteListCommand = new RelayCommand(CompleteList, CanRefresh);
            QueueListCommand = new RelayCommand(QueueList, CanRefresh);
            MustRefresh = true;
        }

        public RelayCommand<RapidItemInfo> DownloadCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand CompleteListCommand { get; set; }
        public RelayCommand QueueListCommand { get; set; }

        FastCollection<RapidItemInfo> _Items = null;
        public FastCollection<RapidItemInfo> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new FastCollection<RapidItemInfo>(ApplicationHelper.DispatcherThread);
                }
                return _Items;
            }
        }

        FastCollection<RapidItemInfo> _QueueItems = null;
        public FastCollection<RapidItemInfo> QueueItems
        {
            get
            {
                if (_QueueItems == null)
                {
                    _QueueItems = new FastCollection<RapidItemInfo>(ApplicationHelper.DispatcherThread);
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
                ApplicationHelper.EnterDispatcherThreadActionBegin(() =>
                {
                    OnPropertyChanged("IsBusy");
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                });
            }
        }

        bool _IsShowComplete = true;

        public bool IsShowComplete
        {
            get { return _IsShowComplete; }
            set { _IsShowComplete = value; OnPropertyChanged("IsShowComplete"); }
        }

        void Clear()
        {
            PagesManagerViewModel.This.BackItem();
        }

        private void Download(RapidItemInfo item)
        {
            ToolbarViewModel.This.AddLinkPageCommand.Execute();
            AddLinksViewModel.This.UriAddress = item.Link;
            AddLinksViewModel.This.UserName = RapidBazEngineHelper.UserName;
            AddLinksViewModel.This.Password = RapidBazEngineHelper.Password;
        }

        public bool CanRefresh()
        {
            return !IsBusy;
        }

        public void RefreshList()
        {
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                if (!RapidBazEngineHelper.IsLogin)
                {
                    RapidTextStatus = "در حال ورود...";
                    if (!RapidBazEngineHelper.Login())
                    {
                        RapidTextStatus = "خطا در ورود رخ داده است. لطفاً نام کاربری و رمز عبور یا پروکسی سیستم خود را بررسی کنید";
                        System.Threading.Thread.Sleep(2000);
                        IsBusy = false;
                        return;
                    }
                }

                RapidTextStatus = "در حال دریافت لیست...";
                if (IsShowComplete)
                {
                    var list = Agrin.Download.Engine.RapidBazEngineHelper.GetCompleteList();
                    Items.Clear();
                    Items.AddRange(list);
                }
                else
                {
                    var list = Agrin.Download.Engine.RapidBazEngineHelper.GetQueueList();
                    QueueItems.Clear();
                    QueueItems.AddRange(list);
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

        private void Refresh()
        {
            RefreshList();
        }

        private void QueueList()
        {
            IsShowComplete = false;
        }

        private void CompleteList()
        {
            IsShowComplete = true;
        }
    }
}