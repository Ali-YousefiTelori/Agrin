using Agrin.BaseViewModels.Models;
using Agrin.BaseViewModels.WindowLayouts.Asuda;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.WindowLayouts.Asuda
{
    public class BasketReceiverViewModel : BasketReceiverBaseViewModel
    {
        public static BasketReceiverViewModel This { get; set; }
        public BasketReceiverViewModel()
        {
            IgnoreStopChanged = true;
            This = this;
            ShowListCommand = new RelayCommand(ShowList);
            DownloadCommand = new RelayCommand<AsudaWebResponseData>(DownloadResponse, CanDownloadResponse);
            AddCommand = new RelayCommand<AsudaWebResponseData>(AddResponse, CanAddResponse);
            RefreshCommand = new RelayCommand<AsudaWebResponseData>(RefreshResponse, CanRefreshResponse);
            RemoveCommand = new RelayCommand<AsudaWebResponseData>(RemoveResponse, CanRemoveResponse);
            RemoveAllCommand = new RelayCommand(RemoveAll, CanRemoveAll);
            AddAllCommand = new RelayCommand(AddAllResponses, CanAddAllResponses);
            ShowSettingPageCommand = new RelayCommand(ShowSettingPage, CanShowSettingPage);
            ShowListPageCommand = new RelayCommand(ShowListPage, CanShowListPage);
        }


        public RelayCommand ShowListCommand { get; set; }
        public RelayCommand<AsudaWebResponseData> DownloadCommand { get; set; }
        public RelayCommand<AsudaWebResponseData> AddCommand { get; set; }
        public RelayCommand<AsudaWebResponseData> RefreshCommand { get; set; }
        public RelayCommand<AsudaWebResponseData> RemoveCommand { get; set; }
        public RelayCommand RemoveAllCommand { get; set; }
        public RelayCommand AddAllCommand { get; set; }
        public RelayCommand ShowSettingPageCommand { get; set; }
        public RelayCommand ShowListPageCommand { get; set; }
    }
}
