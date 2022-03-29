using System.Windows.Controls;
using System;
using Agrin.Download.Web;
using System.Windows;
using System.Collections.Generic;

namespace Agrin.UI.Views.Downloads
{
    /// <summary>
    /// Interaction logic for LinkInfoesDownloadingManager.xaml
    /// </summary>
    public partial class LinkInfoesDownloadingManager : UserControl
    {
        public LinkInfoesDownloadingManager()
        {
            InitializeComponent();
            this.SetViewModelViewElement();
        }
    }
}
