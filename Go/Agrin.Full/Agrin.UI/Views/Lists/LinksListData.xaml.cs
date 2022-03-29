using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.UI.ViewModels.Lists;
using Agrin.UI.Views.Toolbox;
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

namespace Agrin.UI.Views.Lists
{
    /// <summary>
    /// Interaction logic for LinksListData.xaml
    /// </summary>
    public partial class LinksListData : UserControl
    {
        public LinksListData()
        {
            InitializeComponent();
            this.SetViewModelViewElement();
        }


        public TopToolbox CurrentToolbox
        {
            set
            {
                value.SelectionChanged = (selectedItem) =>
                {
                    var data = (LinksListDataViewModel)this.DataContext;
                    data.ResetGridViewGroups(selectedItem);
                };
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            string text = Clipboard.GetText();
            Uri uri = null;
            if (Uri.TryCreate(text, UriKind.Absolute, out uri))
                txt.Text = text;
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox_GotFocus(sender, null);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LinksListDataViewModel.This.SearchCommand.Execute();
            else if (e.Key == Key.Escape)
            {
                LinksListDataViewModel.This.SeachAddress = "";
                LinksListDataViewModel.This.SearchCommand.Execute();
            }
        }
    }
}
