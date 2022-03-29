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

namespace Agrin.UI.Views.Pages
{
    /// <summary>
    /// Interaction logic for LinkInfoSetting.xaml
    /// </summary>
    public partial class LinkInfoSetting : UserControl
    {
        public LinkInfoSetting()
        {
            InitializeComponent();
            this.SetViewModelViewElement();
        }
    }
}
