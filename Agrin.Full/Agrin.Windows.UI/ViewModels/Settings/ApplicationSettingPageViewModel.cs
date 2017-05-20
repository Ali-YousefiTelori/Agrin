using Agrin.BaseViewModels.Settings;
using Agrin.ViewModels.WindowLayouts.Asuda;
using Agrin.Windows.UI.Views.WindowLayouts.Asuda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Windows.UI.ViewModels.Settings
{
    public class ApplicationSettingPageViewModel : ApplicationSettingPageBaseViewModel
    {
        public override bool IsShowAsudaWindow
        {
            get
            {
                return base.IsShowAsudaWindow;
            }
            set
            {
                base.IsShowAsudaWindow = value;
                if (!value)
                {
                    if (BasketReceiverViewModel.This.IsAsudaOn)
                        BasketReceiverViewModel.This.IsAsudaOn = false;
                    BasketReceiverWindow.HideBasket();
                }
                else
                {
                    BasketReceiverWindow.ShowBasket();
                }
            }
        }
    }
}
