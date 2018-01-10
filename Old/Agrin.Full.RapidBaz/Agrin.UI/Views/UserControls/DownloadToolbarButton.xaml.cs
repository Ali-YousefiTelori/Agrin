using Agrin.Helper.ComponentModel;
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

namespace Agrin.UI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DownloadToolbarButton.xaml
    /// </summary>
    public partial class DownloadToolbarButton : UserControl
    {
        public DownloadToolbarButton()
        {
            InitializeComponent(); 
            this.OnApplyTemplate();
            //if (this.Background == null)
            //{
            //    this.Background = (SolidColorBrush)Application.Current.FindResource("ToolbarButtonBackground_ApplicationColors");
            //}
        }

        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register("IconTemplate", typeof(ControlTemplate), typeof(DownloadToolbarButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DownloadToolbarButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DownloadToolbarButton), new UIPropertyMetadata(null));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(DownloadToolbarButton), new FrameworkPropertyMetadata(null));

        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }
        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }
            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }  
        public ControlTemplate IconTemplate
        {
            get
            {
                return (ControlTemplate)this.GetValue(IconTemplateProperty);
            }
            set
            {
                this.SetValue(IconTemplateProperty, value);
            }
        }
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }
    }
}
