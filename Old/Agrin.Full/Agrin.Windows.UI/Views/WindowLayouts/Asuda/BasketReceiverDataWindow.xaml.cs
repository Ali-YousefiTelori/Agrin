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
using System.Windows.Shapes;

namespace Agrin.Windows.UI.Views.WindowLayouts.Asuda
{
    /// <summary>
    /// Interaction logic for BasketReceiverDataWindow.xaml
    /// </summary>
    public partial class BasketReceiverDataWindow : Window
    {
        public BasketReceiverDataWindow()
        {
            InitializeComponent();
        }

        public HorizontalAlignment PositionGridHorizontalAlignment
        {
            get
            {
                return positionGrid.HorizontalAlignment;
            }
            set
            {
                positionGrid.HorizontalAlignment = value;
            }
        }

        public VerticalAlignment PositionGridVerticalAlignment
        {
            get
            {
                return positionGrid.VerticalAlignment;
            }
            set
            {
                positionGrid.VerticalAlignment = value;
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            BasketReceiverPinnedWindow.DragEnterData(e);
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            BasketReceiverPinnedWindow.DragOverData(e);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            BasketReceiverPinnedWindow.DropData(e.Data);
        }
    }
}
