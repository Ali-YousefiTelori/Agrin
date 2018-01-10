using Agrin.BaseViewModels.Link;
using Agrin.RapidBaz.Models;
using Agrin.Windows.UI.ViewModels.Lists;
using Agrin.Windows.UI.ViewModels.Toolbox;
using Agrin.Windows.UI.Views.RapidBaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Agrin.Windows.UI.ViewModels.RapidBaz
{
    public class QueueListRapidBazViewModel : Agrin.ViewModels.RapidBaz.QueueListRapidBazViewModel
    {
        public QueueListRapidBaz MainQueueListRapidBaz { get; set; }
        public override IEnumerable<RapidItemInfo> GetSelectedItems()
        {
            if (MainQueueListRapidBaz == null || MainQueueListRapidBaz.MainDataGrid == null)
                return new List<RapidItemInfo>();

            return MainQueueListRapidBaz.MainDataGrid.SelectedItems.Cast<RapidItemInfo>();
        }

        public override void AddLinkSelectedItem()
        {
            base.AddLinkSelectedItem();
            var links = GetSelectedItems().Where(x => x.IsComplete);
            CompleteListRapidBazViewModel.AddLinkListItem(links.Select<RapidItemInfo, string>(x => x.Link), SelectedItem.Link, true);
            SelectedItem.Validate();
            //BindingOperations.GetBindingExpression(QueueListRapidBaz.This.MainDataGrid, DataGrid.ItemsSourceProperty).UpdateTarget();
        }
    }
}
