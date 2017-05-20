using Agrin.BaseViewModels.Toolbox;
using Agrin.ViewModels.Helper.ComponentModel;
using Agrin.ViewModels.RapidBaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Toolbox
{
    public class RapidbazToolbarViewModel : RapidbazToolbarBaseViewModel
    {
        static RapidbazToolbarViewModel()
        {
            RapidbazToolbarBaseViewModel.InitilizeAction = () =>
            {
                Initilize();
            };
        }

        public static RapidbazToolbarViewModel This { get; set; }
        public RapidbazToolbarViewModel()
        {
            This = this;
            RefreshCommand = new RelayCommand(Refresh, CanRefresh);
            Initilize();
        }

        public static void Initilize()
        {
            if (This == null)
                return;
            if (!This.IsCompleteList)
            {
                if (QueueListRapidBazViewModel.This != null)
                {
                    This.AddLinksCommand = ((QueueListRapidBazViewModel)QueueListRapidBazViewModel.This).DownloadCommand;
                    This.DeleteLinksCommand = ((QueueListRapidBazViewModel)QueueListRapidBazViewModel.This).DeleteCommand;
                }
            }
            else
            {
                if (CompleteListRapidBazViewModel.This != null)
                {
                    This.AddLinksCommand = ((CompleteListRapidBazViewModel)CompleteListRapidBazViewModel.This).DownloadCommand;
                    This.DeleteLinksCommand = ((CompleteListRapidBazViewModel)CompleteListRapidBazViewModel.This).DeleteCommand;
                }
            }
        }

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand AddLinksCommand { get; set; }
        public RelayCommand DeleteLinksCommand { get; set; }
    }
}
