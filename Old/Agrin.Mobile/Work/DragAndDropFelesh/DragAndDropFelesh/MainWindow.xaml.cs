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

namespace DragAndDropFelesh
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool isMouseMove = false;
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(gridMain);
            mainLine.X1 = pos.X;
            mainLine.Y1 = pos.Y;
            mainLine.X2 = pos.X;
            mainLine.Y2 = pos.Y;
            fleshControl.Margin = new Thickness(pos.X - 5, pos.Y - 7, 0, 0);
            isMouseMove = true;
            gridMain.CaptureMouse();
            e.Handled = true;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseMove)
            {
                var pos = e.GetPosition(gridMain);
                Point center = new Point(fleshControl.Width / 2, fleshControl.Height / 2);
                Point relPoint = new Point(pos.X - center.X, pos.Y - center.Y);
                mainLine.X2 = pos.X;
                mainLine.Y2 = pos.Y;
                //var sss = Math.Atan2(mainLine.Y2 - mainLine.Y1, mainLine.X2 - mainLine.X1);
                fleshControl.Margin = new Thickness(relPoint.X, relPoint.Y, 0, 0);
                var group = (TransformGroup)fleshControl.RenderTransform;
                var rotate = group.Children[2] as RotateTransform;
                rotate.Angle = (180 / Math.PI) * Math.Atan2(mainLine.Y2 - mainLine.Y1, mainLine.X2 - mainLine.X1);
            }

        }

        private void gridMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseMove = false;
            gridMain.ReleaseMouseCapture();
            e.Handled = true;
        }
    }
}
