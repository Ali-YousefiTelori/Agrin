//using Agrin.BaseViewModels.Lists;
//using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Agrin.Views.Lists
{
    public partial class LinksList : ContentView
    {
        //LinksBaseViewModel viewModel = null;
        public LinksList()
        {
            //InitializeComponent();
            //viewModel = this.BindingContext as LinksBaseViewModel;
            //viewModel.Items.CollectionChanged += Items_CollectionChanged;
            //foreach (var item in viewModel.Items)
            //{
            //    listStack.Children.Add(new Grid() { Children = { new LinkInfoItemTemplate() { BindingContext = item } }, MinimumHeightRequest = 70, HeightRequest = 70 });
            //}
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //ListView list = sender as ListView;
            //var linkInfo = list.SelectedItem as LinkInfo;
            //if (linkInfo != null)
            //    linkInfo.DownloadingProperty.IsSelected = !linkInfo.DownloadingProperty.IsSelected;
            //list.SelectedItem = null;
        }
    }
}
