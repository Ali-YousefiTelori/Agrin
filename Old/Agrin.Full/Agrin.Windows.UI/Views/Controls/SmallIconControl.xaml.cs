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

namespace Agrin.Windows.UI.Views.Controls
{
    /// <summary>
    /// Interaction logic for SmallIconControl.xaml
    /// </summary>
    public partial class SmallIconControl : UserControl
    {
        public SmallIconControl()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty ContentStyleProperty = DependencyProperty.Register("ContentStyle", typeof(object), typeof(SmallIconControl), new PropertyMetadata());

        public object ContentStyle { get { return GetValue(ContentStyleProperty); } set { SetValue(ContentStyleProperty, value); } }


    }
}
