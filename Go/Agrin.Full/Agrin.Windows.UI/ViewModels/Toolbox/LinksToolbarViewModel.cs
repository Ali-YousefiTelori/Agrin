using Agrin.Download.Web;
using Agrin.ViewModels.Managers;
using Agrin.Windows.UI.ViewModels.Pages;
using Agrin.Windows.UI.Views.Link;
using Agrin.Windows.UI.Views.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Windows.UI.ViewModels.Toolbox
{
    public class LinksToolbarViewModel : Agrin.ViewModels.Toolbox.LinksToolbarViewModel
    {
        public static LinksToolbarViewModel This { get; set; }
        public LinksToolbarViewModel()
        {
            This = this;
        }

        public override IEnumerable<LinkInfo> GetSelectedLinks()
        {
            if (Links.This == null || Links.This.MainDataGrid == null)
                return new List<LinkInfo>();

            return Links.This.MainDataGrid.SelectedItems.Cast<LinkInfo>();
        }

        public override void AddLink()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.linkPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<AddLinks>());
        }
    }
}
