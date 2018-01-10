using Agrin.UI.ViewModels.UserControls;
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

namespace Agrin.UI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for WindowMessageBox.xaml
    /// </summary>
    public partial class WindowMessegeBox : UserControl
    {
        public WindowMessegeBox()
        {
            InitializeComponent();
            This = this;
        }

        static WindowMessegeBox This;
        public static void ShowMessage(Action<bool> action, string title, string message)
        {
            MainWindow.This.IsShowMessege = true;
            ((WindowMessegeBoxViewModel)This.DataContext).ShowMessage(action, title, message);
        }
    }
}
