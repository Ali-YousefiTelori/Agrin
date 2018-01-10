using Agrin.BaseViewModels.WindowLayouts.Asuda;
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

namespace Agrin.Windows.UI.Views.WindowLayouts.Asuda
{
    /// <summary>
    /// Interaction logic for BasketReceiverDataControl.xaml
    /// </summary>
    public partial class BasketReceiverDataControl : UserControl
    {
        public BasketReceiverDataControl()
        {
            AsudaDataOptimizerBaseViewModel.Dispatcher = Dispatcher;
            InitializeComponent();
        }

        private void DataGrid_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
