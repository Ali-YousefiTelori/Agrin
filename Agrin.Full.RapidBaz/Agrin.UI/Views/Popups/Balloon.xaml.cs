using Agrin.UI.ViewModels.Popups;
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

namespace Agrin.UI.Views.Popups
{
    /// <summary>
    /// Interaction logic for Balloon.xaml
    /// </summary>
    public partial class Balloon : UserControl
    {
        public Balloon()
        {
            InitializeComponent();
            this.SetViewModelViewElement();
        }

        private void Control_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (BalloonViewModel.This.CurrentLinkInfo != null && BalloonViewModel.This.CurrentLinkInfo.DownloadingProperty.State == Download.Web.ConnectionState.Complete && File.Exists(BalloonViewModel.This.CurrentLinkInfo.PathInfo.FullAddressFileName))
                {
                    string[] paths = new string[] { BalloonViewModel.This.CurrentLinkInfo.PathInfo.FullAddressFileName };
                    DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, paths), DragDropEffects.Move);
                }
            }
            catch
            {

            }
        }

        //private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    Agrin.UI.ViewModels.Popups.BalloonViewModel.This.balloonWindow.DragMove();
        //}
    }
}
