using Agrin.BaseViewModels.WindowLayouts.Asuda;
using Agrin.Network.Models;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.WindowLayouts.Asuda
{
    public class BasketReceiverSettingPageViewModel : BasketReceiverSettingPageBaseViewModel
    {
        public BasketReceiverSettingPageViewModel()
        {
            IgnoreStopChanged = true;
            AddExtensionCommand = new RelayCommand(AddExtension, CanAddExtension);
            AddApplicationCommand = new RelayCommand(AddApp, CanAddApp);
            RemoveExtensionCommand = new RelayCommand<ExtensionInfo>(RemoveExtension);
            RemoveAppCommand = new RelayCommand<ApplicationInfo>(RemoveApp);
        }

        public RelayCommand AddExtensionCommand { get; set; }
        public RelayCommand AddApplicationCommand { get; set; }
        public RelayCommand<ExtensionInfo> RemoveExtensionCommand { get; set; }
        public RelayCommand<ApplicationInfo> RemoveAppCommand { get; set; }
    }
}
