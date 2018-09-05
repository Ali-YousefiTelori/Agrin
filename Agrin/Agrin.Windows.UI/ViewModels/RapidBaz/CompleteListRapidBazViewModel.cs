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
    public class CompleteListRapidBazViewModel : Agrin.ViewModels.RapidBaz.CompleteListRapidBazViewModel
    {
        public CompleteListRapidBazViewModel()
        {
            This = this;
        }

        public override IEnumerable<RapidItemInfo> GetSelectedItems()
        {
            if (CompleteListRapidBaz.This == null || CompleteListRapidBaz.This.MainDataGrid == null)
                return new List<RapidItemInfo>();

            return CompleteListRapidBaz.This.MainDataGrid.SelectedItems.Cast<RapidItemInfo>();
        }

        public override void AddLinkSelectedItem()
        {
            base.AddLinkSelectedItem();
            var links = GetSelectedItems().Where(x => x.IsComplete);
            AddLinkListItem(links.Select<RapidItemInfo, string>(x => x.Link), SelectedItem.Link, true);
            SelectedItem.Validate();
        }

        public static void AddLinkListItem(IEnumerable<string> links, string firstItem, bool isFromRapidBaz = false, bool isClear = true)
        {
            if (isClear)
                AddLinksBaseViewModel.This.ClearItems();
            if (isFromRapidBaz)
                AddLinksBaseViewModel.This.IsEnableRapidBazAddLinks = false;
            bool lowCount = links.Count() <= 1;
            if ((lowCount && isClear) || (lowCount && string.IsNullOrEmpty(AddLinksBaseViewModel.This.UriAddress) && AddLinksBaseViewModel.This.GroupLinks.Count == 0))
                AddLinksBaseViewModel.This.UriAddress = firstItem;
            else
            {
                if (AddLinksBaseViewModel.This.GroupLinks.Count == 0 && !string.IsNullOrEmpty(AddLinksBaseViewModel.This.UriAddress))
                    AddLinksBaseViewModel.This.GroupLinks.Add(AddLinksBaseViewModel.This.UriAddress);
                AddLinksBaseViewModel.This.GroupLinks.AddRange(links);
                AddLinksBaseViewModel.This.AddGroupLinks();
            }
            TabMenuControlViewModel.This.SelectedIndex = 0;
            LinksViewModel.This.AddLink();
        }

        public override void AddRapidBazLink()
        {
            AddLinksBaseViewModel.This.ClearItems();
            AddLinksBaseViewModel.This.IsRapidBazLink = true;
            TabMenuControlViewModel.This.SelectedIndex = 0;
            LinksViewModel.This.AddLink();
        }
    }
}
