using Agrin.Download.Web;
using Agrin.Download.Web.Link;
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

namespace Agrin.UI.Views.Downloads
{
    /// <summary>
    /// Interaction logic for LinkInfoDownloadDetail.xaml
    /// </summary>
    public partial class LinkInfoDownloadDetail : UserControl
    {
        public LinkInfoDownloadDetail()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsShowListProperty = DependencyProperty.Register("IsShowList", typeof(bool), typeof(LinkInfoDownloadDetail), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback((sender, e) =>
        {
            if ((bool)e.NewValue)
                ((LinkInfoDownloadDetail)sender).DrawConnections();
            else
                ((LinkInfoDownloadDetail)sender).CleanUpConnections();

        })));
        public bool IsShowList
        {
            get { return (bool)this.GetValue(IsShowListProperty); }
            set
            {
                this.SetValue(IsShowListProperty, value);
            }
        }

        List<LinkWebRequest> connections = new List<LinkWebRequest>();
        object lockObj = new object();
        void ReDrawConnections()
        {
            lock (lockObj)
            {
                foreach (var item in info.Connections.ToList())
                {
                    if (!connections.Contains(item))
                    {
                        connections.Add(item);
                        if (wrapConnections.Children.Count < connections.Count)
                        {
                            ConnectionInfoDownload conn = new ConnectionInfoDownload() { Margin = new Thickness(5) };
                            wrapConnections.Children.Add(conn);
                        }
                    }
                }
                if (wrapConnections.Children.Count > connections.Count)
                {
                    wrapConnections.Children.RemoveRange(0, wrapConnections.Children.Count - connections.Count);
                }
                for (int i = 0; i < wrapConnections.Children.Count; i++)
                {
                    ((ConnectionInfoDownload)wrapConnections.Children[i]).DataContext = connections[i];
                }
            }
        }
        LinkInfo info = null;
        public void DrawConnections()
        {
            info = ((Agrin.UI.ViewModels.Downloads.LinkInfoDownloadDetailViewModel)this.DataContext).CurrentLinkInfo;
            info.Connections.ChangedCollection = () =>
            {
                ReDrawConnections();
            };
            ReDrawConnections();
        }

        public void CleanUpConnections()
        {
            info.Connections.ChangedCollection = null;
            connections.Clear();
        }

        WrapPanel wrapConnections;
        private void WrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            wrapConnections = (WrapPanel)sender;
        }
    }
}
