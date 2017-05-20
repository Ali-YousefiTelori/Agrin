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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agrin.UI.Views.Downloads
{
    /// <summary>
    /// Interaction logic for LinkInfoDownload.xaml
    /// </summary>
    public partial class LinkInfoDownload : UserControl
    {
        public LinkInfoDownload()
        {
            InitializeComponent();
            DataContextChanged += LinkInfoDownload_DataContextChanged;
        }

        void LinkInfoDownload_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.SetBinding(LinkInfoDownload.IsSelectedProperty, new Binding("DownloadingProperty.IsSelected") { Source = this.DataContext });
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(LinkInfoDownload), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(LinkInfoDownload), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LinkInfoDownload), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }
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
        private void mainLayoutFocusButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
        }
    }
}
