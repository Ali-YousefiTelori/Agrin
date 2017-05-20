using Agrin.BaseViewModels.RapidBaz;
using Agrin.RapidBaz.Models;
using Agrin.ViewModels.Helper.ComponentModel;
using Agrin.Windows.UI.ViewModels.RapidBaz;
using System;
using System.Collections.Generic;
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

namespace Agrin.Windows.UI.Views.RapidBaz
{
    /// <summary>
    /// Interaction logic for FolderFilesExplorer.xaml
    /// </summary>
    public partial class FolderFilesExplorer : UserControl
    {
        public FolderFilesExplorer()
        {
            This = this;
            InitializeComponent();
        }

        public static FolderFilesExplorer This { get; set; }
        public ListBox MainList { get; set; }
        private void QueueListRapidBaz_Initialized(object sender, EventArgs e)
        {
            Grid grid = sender as Grid;
            QueueListRapidBaz ctrl = ((Grid)grid.Children[1]).Children[0] as QueueListRapidBaz;
            var Qvm = ctrl.DataContext as QueueListRapidBazViewModel;
            Qvm.IsRefreshFromPage = false;
            ctrl.Toolbar.BackCommand = new RelayCommand(() =>
            {
                ((FolderFilesExplorerBaseViewModel)this.DataContext).IsFolderList = true;
                Qvm.QueueItems.Clear();
            });
            ((FolderFilesExplorerBaseViewModel)this.DataContext).CurrentQueueListRapidBazBaseViewModel = Qvm;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var folder = ((ListBoxItem)sender).DataContext as FolderInfo;
            var vm = ((FolderFilesExplorerBaseViewModel)this.DataContext);
            vm.IsFolderList = false;
            vm.CurrentQueueListRapidBazBaseViewModel.CurrentFolderID = folder.ID;
            vm.CurrentQueueListRapidBazBaseViewModel.RefreshFolderFiles();
        }
    }
}
