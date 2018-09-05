using Agrin.Windows.UI.ViewModels.Popups;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Agrin.Windows.UI.Views.Popups
{
    /// <summary>
    /// Interaction logic for CompleteLinksBalloon.xaml
    /// </summary>
    public partial class CompleteLinksBalloon : UserControl
    {
        public CompleteLinksBalloon()
        {
            InitializeComponent();
        }

        private void Control_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CompleteLinksBalloonViewModel.This.CurrentLinkInfo != null && CompleteLinksBalloonViewModel.This.CurrentLinkInfo.DownloadingProperty.State == Download.Web.ConnectionState.Complete && File.Exists(CompleteLinksBalloonViewModel.This.CurrentLinkInfo.PathInfo.FullAddressFileName))
                {
                    string[] paths = new string[] { CompleteLinksBalloonViewModel.This.CurrentLinkInfo.PathInfo.FullAddressFileName };
                    DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Move);
                }
            }
            catch
            {

            }
        }
    }
}
