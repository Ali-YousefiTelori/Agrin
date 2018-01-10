using Agrin.Download.Engine;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.UI.Views.Popups;
using Agrin.ViewModels.Helper.ComponentModel;

namespace Agrin.UI.ViewModels.Popups
{
    public class NotificationViewModel : ANotifyPropertyChanged<Notification>
    {
        public NotificationViewModel()
        {
            if (ApplicationLinkInfoManager.Current == null)
                return;
            ApplicationNotificationManager.Current.NotifyCountChanged = () =>
                {
                    OnPropertyChanged("NotifyCount");
                };
            ShowAllCommand = new RelayCommand(ShowAll);
            //ApplicationNotificationManager.Current.ReadNotify(ApplicationNotificationManager.Current.Items[0]);
        }

        public RelayCommand ShowAllCommand { get; set; }

        public int NotifyCount
        {
            get
            {
                if (ApplicationNotificationManager.Current == null)
                    return 0;
                return ApplicationNotificationManager.Current.NotifyCount;
            }
        }

        public FastCollection<NotificationInfo> Items
        {
            get
            {
                if (ApplicationNotificationManager.Current == null)
                    return null;
                return ApplicationNotificationManager.Current.NotificationInfoes;
            }
        }

        private void ShowAll()
        {
            ViewElement.gridNotify.Visibility = System.Windows.Visibility.Visible;
            ViewElement.gridShowAll.Visibility = System.Windows.Visibility.Collapsed;
            SearchEngine.IsNotify = false;
            ApplicationNotificationManager.Current.CurrentNotification = null;
            SearchEngine.Search();
        }
    }
}
