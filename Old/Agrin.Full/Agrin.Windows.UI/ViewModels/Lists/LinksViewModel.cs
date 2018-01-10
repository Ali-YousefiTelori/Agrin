using Agrin.BaseViewModels.Link;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.ViewModels.Managers;
using Agrin.Windows.UI.ViewModels.Pages;
using Agrin.Windows.UI.ViewModels.Toolbox;
using Agrin.Windows.UI.Views.Link;
using Agrin.Windows.UI.Views.Lists;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.Windows.UI.ViewModels.Lists
{
    public enum SelectedToolboxGroupGridEnum
    {
        Tasks,
        Groups,
        None
    }

    public class LinksViewModel : Agrin.ViewModels.Lists.LinksViewModel
    {
        public static LinksViewModel This { get; set; }
        public LinksViewModel()
        {
            This = this;
            if (Links.This == null)
                return;
            Links.This.Loaded += MainDataGrid_Loaded;

            ApplicationGroupManager.Current.ChangedGroups = (item) =>
            {
                RefreshGridGrouping(item);
            };
        }
        bool loadedOne = false;
        void MainDataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (loadedOne)
                return;
            loadedOne = true;
            ResetGridViewGroups(SelectedToolboxGroupGridEnum.None);
        }

        public override IEnumerable<LinkInfo> GetSelectedItems()
        {
            if (Links.This == null || Links.This.MainDataGrid == null)
                return new List<LinkInfo>();

            return Links.This.MainDataGrid.SelectedItems.Cast<LinkInfo>();
        }

        public override void AddLink()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.linkPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<AddLinks>());
        }

        public void ResetGridViewGroups(SelectedToolboxGroupGridEnum mode)
        {
            Links.This.MainDataGrid.Items.GroupDescriptions.Clear();
            if (mode == SelectedToolboxGroupGridEnum.Groups)
                Links.This.MainDataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription("PathInfo.CurrentGroupInfo"));
            else if (mode == SelectedToolboxGroupGridEnum.Tasks)
                Links.This.MainDataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription("PathInfo.CurrentQueueInfo"));
        }

        public void RefreshGridGrouping(object item)
        {
            IEditableCollectionView view = Links.This.MainDataGrid.Items as IEditableCollectionView;
            view.EditItem(item);
            view.CommitEdit();
        }

        public override void ShowGroupLinks(bool value)
        {
            if (value)
                ResetGridViewGroups(SelectedToolboxGroupGridEnum.Groups);
            else
                ResetGridViewGroups(SelectedToolboxGroupGridEnum.None);
        }

        public static void AddLinkListItem(IEnumerable<string> links, string firstItem, bool isClear = true)
        {
            if (isClear)
                AddLinksBaseViewModel.This.ClearItems();
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
    }
}
