using Agrin.Windows.UI.ViewModels.RapidBaz;
using Agrin.Windows.UI.Views.Toolbox;
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
    /// Interaction logic for QueueListRapidBaz.xaml
    /// </summary>
    public partial class QueueListRapidBaz : UserControl
    {
        public QueueListRapidBaz()
        {
            InitializeComponent();
            ((QueueListRapidBazViewModel)this.DataContext).MainQueueListRapidBaz = this;
        }

        public DataGrid MainDataGrid { get; set; }

        public LinksToolbar Toolbar { get; set; }
        private void LinksToolbar_Loaded(object sender, EventArgs e)
        {
            Grid grid = sender as Grid;
            Toolbar = grid.Children[0] as LinksToolbar;
        }
    }
}
