using Agrin.Helper.ComponentModel;
using Agrin.OldViews.Link;
using Agrin.OldViews.Lists;
using Agrin.ViewModels.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Agrin.ViewModels
{
    public class MainMenuViewModel : ANotifyPropertyChanged
    {
        public MainMenuViewModel()
        {
            AddLinkCommand = new RelayCommand(AddLinkPage);
            LinksListCommand = new RelayCommand(LinksListPage);
        }

        private void LinksListPage()
        {
            App.NavigateTo(new LinksList());
        }

        private void AddLinkPage(object obj)
        {
            App.NavigateTo(new AddLink());
        }

        public RelayCommand AddLinkCommand { get; set; }
        public RelayCommand LinksListCommand { get; set; }
    }
}
