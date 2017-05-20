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

namespace Agrin.Windows.UI.Views.RapidBaz
{
    /// <summary>
    /// Interaction logic for LoginPageDesignRapidBaz.xaml
    /// </summary>
    public partial class LoginPageDesignRapidBaz : UserControl
    {
        public LoginPageDesignRapidBaz()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://rapidbaz.com");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://telegram.me/rapidbaz_official");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://help.rapidbaz.com/%D8%AB%D8%A8%D8%AA-%D9%86%D8%A7%D9%85/%D8%A7%D8%B4%D8%AA%D8%B1%D8%A7%DA%A9-%D8%AC%D8%AF%DB%8C%D8%AF-%DA%A9%D8%AF-%D8%AF%D8%B9%D9%88%D8%AA%D9%86%D8%A7%D9%85%D9%87/%DA%86%DA%AF%D9%88%D9%86%D9%87-%D9%85%DB%8C%D8%AA%D9%88%D8%A7%D9%86%DB%8C%D9%85-%D8%A7%D8%B2-%D8%B3%D8%A7%DB%8C%D8%AA-%D8%B1%D9%BE%DB%8C%D8%AF%D8%A8%D8%A7%D8%B2-%D8%A7%D8%B4%D8%AA%D8%B1%D8%A7%DA%A9/");
        }
    }
}
