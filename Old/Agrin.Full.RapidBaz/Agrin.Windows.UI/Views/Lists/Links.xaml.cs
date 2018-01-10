using Agrin.BaseViewModels.Lists;
using Agrin.Download.Web;
using Agrin.ViewModels.Lists;
using Agrin.Windows.UI.Views.Toolbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.Lists
{
    /// <summary>
    /// Interaction logic for Links.xaml
    /// </summary>
    public partial class Links : UserControl
    {
        public static Links This;
        public Links()
        {
            This = this;
            InitializeComponent();
        }

        void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var linkInfo = row.DataContext as LinkInfo;
            if (LinksBaseViewModel.CanOpenFile(linkInfo))
                LinksViewModel.OpenFileStatic(linkInfo);
        }

        public DataGrid MainDataGrid { get; set; }
        ToolTip backTooltip = null;
        private void ToolTipButton_Click(object sender, RoutedEventArgs e)
        {
            if (backTooltip != null)
                backTooltip.IsOpen = false;
            FrameworkElement element = ((FrameworkElement)sender);
            backTooltip = ((ToolTip)element.ToolTip);
            var info = element.DataContext as LinkInfo;
            backTooltip.DataContext = info;
            element.LostFocus += backTooltip_LostFocus;
            backTooltip.IsOpen = true;
            backTooltip.Focus();
        }

        void backTooltip_LostFocus(object sender, RoutedEventArgs e)
        {
            if (backTooltip != null)
                backTooltip.IsOpen = false;
        }

        //private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if ((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return)))
        //    {
        //        var link = ((FrameworkElement)sender).DataContext as LinksViewModel;
        //        link.OpenFileCommand.Execute();
        //        e.Handled = true;
        //    }
        //}

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in MainDataGrid.Items.Cast<LinkInfo>())
            {
                item.ShowSelectedNumberVisibility = false;
            }

            if (MainDataGrid.SelectedItems.Count > 1)
            {
                int i = 1;
                foreach (var item in MainDataGrid.SelectedItems.Cast<LinkInfo>())
                {
                    item.SelectedIndexNumber = i;
                    item.ShowSelectedNumberVisibility = true;
                    i++;
                }
            }
        }
    }
}
