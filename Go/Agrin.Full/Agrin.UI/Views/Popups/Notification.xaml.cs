using Agrin.Download.Engine;
using Agrin.Download.Manager;
using Agrin.Download.Web;
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

namespace Agrin.UI.Views.Popups
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        public Notification()
        {
            InitializeComponent();
            this.SetViewModelViewElement();
        }

        void Item_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SearchEngine.IsNotify = true;
            ListBoxItem item = (ListBoxItem)sender;
            ApplicationNotificationManager.Current.ReadNotify(((NotificationInfo)item.DataContext));
            ApplicationNotificationManager.Current.CurrentNotification = (NotificationInfo)item.DataContext;
            SearchEngine.Show(((NotificationInfo)item.DataContext).Items.ToList());
            mainPopUp.IsOpen = false;
            gridNotify.Visibility = System.Windows.Visibility.Collapsed;
            gridShowAll.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
