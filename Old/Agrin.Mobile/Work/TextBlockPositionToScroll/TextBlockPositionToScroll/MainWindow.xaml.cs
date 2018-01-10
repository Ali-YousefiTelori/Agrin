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

namespace TextBlockPositionToScroll
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
        double left = 100;
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            txtText.Text = "Offest = " + e.HorizontalOffset;
            if (e.HorizontalOffset > left && txtText.Margin.Left < e.HorizontalOffset - left)
            {
                txtText.Margin = new Thickness(e.HorizontalOffset - left, 0, 0, 0);
            }
            else
                txtText.Margin = new Thickness(0, 0, 0, 0);
        }
    }
}
