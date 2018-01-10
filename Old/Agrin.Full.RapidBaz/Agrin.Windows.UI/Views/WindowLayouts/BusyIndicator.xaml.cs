using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.WindowLayouts
{
    /// <summary>
    /// Interaction logic for BusyIndicator.xaml
    /// </summary>
    [ContentProperty("ContentChild")]
    public partial class BusyIndicator : UserControl
    {
        public BusyIndicator()
        {
            InitializeComponent();

        }

        public static readonly DependencyProperty ContentChildProperty = DependencyProperty.Register(
  "ContentChild",
  typeof(object),
  typeof(BusyIndicator),
  new PropertyMetadata());

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
          "Message",
          typeof(object),
          typeof(BusyIndicator),
          new PropertyMetadata());

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
  "Icon",
  typeof(ControlTemplate),
  typeof(BusyIndicator),
  new PropertyMetadata(Application.Current.FindResource("Info_TemplateStyle")));

        public static readonly DependencyProperty MessageWidthProperty = DependencyProperty.Register(
  "MessageWidth",
  typeof(double),
  typeof(BusyIndicator),
  new PropertyMetadata(200.0));

        public static readonly DependencyProperty MessageHeightProperty = DependencyProperty.Register(
"MessageHeight",
typeof(double),
typeof(BusyIndicator),
new PropertyMetadata(80.0));

        public ControlTemplate Icon
        {
            get { return (ControlTemplate)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public object Message
        {
            get { return GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public bool IsBusy
        {
            get
            {
                return (bool)GetValue(IsBusyProperty);
            }
            set
            {
                SetValue(IsBusyProperty, value);
            }
        }

        public object ContentChild
        {
            get { return GetValue(ContentChildProperty); }
            set { SetValue(ContentChildProperty, value); }
        }

        public double MessageWidth
        {
            get { return (double)GetValue(MessageWidthProperty); }
            set { SetValue(MessageWidthProperty, value); }
        }

        public double MessageHeight
        {
            get { return (double)GetValue(MessageHeightProperty); }
            set { SetValue(MessageHeightProperty, value); }
        }
    }
}
