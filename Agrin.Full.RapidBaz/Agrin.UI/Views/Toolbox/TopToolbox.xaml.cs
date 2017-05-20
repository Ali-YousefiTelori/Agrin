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

namespace Agrin.UI.Views.Toolbox
{
    /// <summary>
    /// Interaction logic for TopToolbox.xaml
    /// </summary>
    public partial class TopToolbox
    {
        public TopToolbox()
        {
            InitializeComponent();
        }

        public Action<SelectedToolboxGroupGridEnum> SelectionChanged;

        private void rdo_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                if (rdoGroup.IsChecked.Value)
                    SelectionChanged(SelectedToolboxGroupGridEnum.Groups);
                else if (rdoQueue.IsChecked.Value)
                    SelectionChanged(SelectedToolboxGroupGridEnum.Tasks);
                else if (rdoNone.IsChecked.Value)
                    SelectionChanged(SelectedToolboxGroupGridEnum.None);
            }
        }
    }

    public enum SelectedToolboxGroupGridEnum
    {
        Tasks,
        Groups,
        None
    }
}
