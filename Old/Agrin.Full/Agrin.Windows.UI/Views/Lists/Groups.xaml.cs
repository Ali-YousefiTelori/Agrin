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

namespace Agrin.Windows.UI.Views.Lists
{
    /// <summary>
    /// Interaction logic for Groups.xaml
    /// </summary>
    public partial class Groups : UserControl
    {
        public static Groups This;
        public Groups()
        {
            This = this; 
            InitializeComponent();
        }
        public DataGrid MainDataGrid { get; set; }
    }
}
