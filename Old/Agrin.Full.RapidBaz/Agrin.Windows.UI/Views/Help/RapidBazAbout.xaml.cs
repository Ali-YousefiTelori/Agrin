using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Agrin.Windows.UI.Views.Help
{
    /// <summary>
    /// Interaction logic for RapidBazAbout.xaml
    /// </summary>
    public partial class RapidBazAbout : UserControl
    {
        public RapidBazAbout()
        {
            InitializeComponent();
        }

        private void btnFaceBook_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.facebook.com/Rapidbazcom");
        }

        private void btnInstagram_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://instagram.com/_rapidbaz");
        }

        private void btnTwitte_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitter.com/rapidbaz");
        }

        private void btnRapidBaz_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://weblog.rapidbaz.com/");
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://help.rapidbaz.com/");
        }
    }
}
