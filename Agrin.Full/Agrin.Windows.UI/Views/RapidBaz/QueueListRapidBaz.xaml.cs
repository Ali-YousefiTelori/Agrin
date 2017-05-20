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
        public static QueueListRapidBaz This { get; set; }
        public QueueListRapidBaz()
        {
            This = this;
            InitializeComponent();
        }
        public DataGrid MainDataGrid { get; set; }
    }
}
