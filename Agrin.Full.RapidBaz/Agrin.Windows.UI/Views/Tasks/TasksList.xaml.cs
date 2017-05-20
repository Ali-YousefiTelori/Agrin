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

namespace Agrin.Windows.UI.Views.Tasks
{
    /// <summary>
    /// Interaction logic for TasksList.xaml
    /// </summary>
    public partial class TasksList : UserControl
    {
        public TasksList()
        {
            This = this;
            InitializeComponent();
        }

        public static TasksList This { get; set; }

        public DataGrid MainDataGrid { get; set; }

        private void DataGridRow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (changed)
            {
                changed = false;
                return;
            }
            DataGrid dg = MainDataGrid;
            if (e.Source is System.Windows.Controls.Primitives.DataGridDetailsPresenter)
                return;
            if (dg == null)
                return;
            if (dg.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            else
                dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        bool changed = false;
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changed = true;
        }
    }
}
