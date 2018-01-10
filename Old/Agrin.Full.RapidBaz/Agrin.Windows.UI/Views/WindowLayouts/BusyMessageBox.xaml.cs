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
    /// Interaction logic for BusyMessageBox.xaml
    /// </summary>
    [ContentProperty("ContentChild")]
    public partial class BusyMessageBox : UserControl
    {
        public BusyMessageBox()
        {
            InitializeComponent();
        }

        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);

        //    Border border = new Border();
        //    border.VerticalAlignment = VerticalAlignment.Stretch;
        //    border.HorizontalAlignment = HorizontalAlignment.Stretch;

        //    Grid grid = new Grid();

        //    ContentPresenter content = new ContentPresenter();
        //    content.Content = Content;

        //    grid.Children.Add(content);
        //    border.Child = grid;

        //    Content = border;
        //}
        public static readonly DependencyProperty CommandButton1Property = DependencyProperty.Register("CommandButton1", typeof(ICommand), typeof(BusyMessageBox), new PropertyMetadata());
        public static readonly DependencyProperty CommandButton1ParameterProperty = DependencyProperty.Register("CommandButton1Parameter", typeof(object), typeof(BusyMessageBox), new PropertyMetadata());
        public static readonly DependencyProperty ContentButton1Property = DependencyProperty.Register("ContentButton1", typeof(object), typeof(BusyMessageBox), new PropertyMetadata());
        public static readonly DependencyProperty IsButton1Property = DependencyProperty.Register("IsButton1", typeof(bool), typeof(BusyMessageBox), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(BusyMessageBox), new PropertyMetadata());
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(BusyMessageBox), new PropertyMetadata());
        public static readonly DependencyProperty ContentOKButtonProperty = DependencyProperty.Register("ContentOKButton", typeof(object), typeof(BusyMessageBox), new PropertyMetadata("تایید"));

        public static readonly DependencyProperty ContentChildProperty = DependencyProperty.Register("ContentChild", typeof(object), typeof(BusyMessageBox), new PropertyMetadata());

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyMessageBox), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsCancelButtonProperty = DependencyProperty.Register("IsCancelButton", typeof(bool), typeof(BusyMessageBox), new FrameworkPropertyMetadata(true));

        //public static readonly DependencyPropertyKey IsBusyProperty = DependencyProperty.RegisterReadOnly(
        //  "IsBusy",
        //  typeof(bool),
        //  typeof(BusyMessageBox),
        //  new PropertyMetadata(false));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
          "Message",
          typeof(object),
          typeof(BusyMessageBox),
          new PropertyMetadata(new PropertyChangedCallback((sender, e) =>
              {
                  var ctrl = sender as BusyMessageBox;
                  if (e.NewValue is string)
                  {
                      ctrl.txtContent.Visibility = Visibility.Visible;
                      ctrl.ctrlContent.Visibility = Visibility.Collapsed;
                  }
                  else
                  {
                      ctrl.txtContent.Visibility = Visibility.Collapsed;
                      ctrl.ctrlContent.Visibility = Visibility.Visible;
                  }
              })));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
  "Title",
  typeof(object),
  typeof(BusyMessageBox),
  new PropertyMetadata());

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
  "Icon",
  typeof(ControlTemplate),
  typeof(BusyMessageBox),
  new PropertyMetadata(Application.Current.FindResource("Info_TemplateStyle")));

        public static readonly DependencyProperty MessageWidthProperty = DependencyProperty.Register(
  "MessageWidth",
  typeof(double),
  typeof(BusyMessageBox),
  new PropertyMetadata(400.0));

        public static readonly DependencyProperty MessageHeightProperty = DependencyProperty.Register(
"MessageHeight",
typeof(double),
typeof(BusyMessageBox),
new PropertyMetadata(180.0));

        public ICommand CommandButton1
        {
            get { return (ICommand)GetValue(CommandButton1Property); }
            set { SetValue(CommandButton1Property, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object ContentButton1
        {
            get { return GetValue(ContentButton1Property); }
            set { SetValue(ContentButton1Property, value); }
        }
        public object ContentOKButton
        {
            get { return GetValue(ContentOKButtonProperty); }
            set { SetValue(ContentOKButtonProperty, value); }
        }

        public object CommandButton1Parameter
        {
            get { return GetValue(CommandButton1ParameterProperty); }
            set { SetValue(CommandButton1ParameterProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ControlTemplate Icon
        {
            get { return (ControlTemplate)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public object Message
        {
            get
            {
                return GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
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

        public bool IsCancelButton
        {
            get
            {
                return (bool)GetValue(IsCancelButtonProperty);
            }
            set
            {
                SetValue(IsCancelButtonProperty, value);
            }
        }

        public bool IsButton1
        {
            get
            {
                return (bool)GetValue(IsButton1Property);
            }
            set
            {
                SetValue(IsButton1Property, value);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(this, BusyMessageBox.IsBusyProperty);
            if (bindingExpression != null)
            {
                PropertyInfo property = bindingExpression.DataItem.GetType().GetProperty(bindingExpression.ParentBinding.Path.Path);
                if (property != null)
                    property.SetValue(bindingExpression.DataItem, false, null);
            }
            else
            {
                IsBusy = false;
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button_Click(null, null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Command == null)
                Button_Click(null, null);
        }
    }
}
