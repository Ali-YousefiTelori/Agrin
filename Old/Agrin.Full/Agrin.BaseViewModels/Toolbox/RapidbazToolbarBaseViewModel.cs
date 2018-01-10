using Agrin.BaseViewModels.RapidBaz;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Toolbox
{
    public class RapidbazToolbarBaseViewModel : ANotifyPropertyChanged
    {
        public static Action InitilizeAction { get; set; }

        public static void InitializeData()
        {
            if (InitilizeAction != null)
                InitilizeAction();
        }

        public RapidbazToolbarBaseViewModel()
        {

        }

        bool _IsCompleteList = false;

        public bool IsCompleteList
        {
            get { return _IsCompleteList; }
            set { _IsCompleteList = value; }
        }

        public bool CanRefresh()
        {
            if (IsCompleteList)
            {
                if (CompleteListRapidBazBaseViewModel.This != null)
                    return CompleteListRapidBazBaseViewModel.This.CanRefresh();
            }
            else
            {
                if (QueueListRapidBazBaseViewModel.This != null)
                    return QueueListRapidBazBaseViewModel.This.CanRefresh();
            }
            return false;
        }

        public void Refresh()
        {
            if (IsCompleteList)
            {
                if (CompleteListRapidBazBaseViewModel.This != null)
                    CompleteListRapidBazBaseViewModel.This.Refresh();
            }
            else
            {
                if (QueueListRapidBazBaseViewModel.This != null)
                    QueueListRapidBazBaseViewModel.This.Refresh();
            }
        }
    }
}
