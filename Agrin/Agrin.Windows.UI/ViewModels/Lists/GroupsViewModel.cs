using Agrin.Download.Web;
using Agrin.ViewModels.Managers;
using Agrin.Windows.UI.ViewModels.Pages;
using Agrin.Windows.UI.Views.Group;
using Agrin.Windows.UI.Views.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Windows.UI.ViewModels.Lists
{
    public class GroupsViewModel : Agrin.ViewModels.Lists.GroupsViewModel
    {
        //public static GroupsViewModel This { get; set; }
        public GroupsViewModel()
        {
            //This = this;
        }

        public override IEnumerable<GroupInfo> GetSelectedItems()
        {
            if (Groups.This == null || Groups.This.MainDataGrid == null)
                return new List<GroupInfo>();

            return Groups.This.MainDataGrid.SelectedItems.Cast<GroupInfo>();
        }

        public override void AddGroup()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.groupsPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<AddGroup>());
        }

        public override void EditGroup(GroupInfo group)
        {
            BaseViewModels.Group.AddGroupBaseViewModel.This.EditGroupInfo = group;
            BaseViewModels.Group.AddGroupBaseViewModel.This.IsEditMode = true;
            ((PagesManagerViewModel)PagesManagerViewModel.groupsPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<AddGroup>());
        }
    }
}
