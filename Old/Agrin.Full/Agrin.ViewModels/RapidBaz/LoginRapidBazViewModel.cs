using Agrin.BaseViewModels.RapidBaz;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.RapidBaz
{
    public class LoginRapidBazViewModel : LoginRapidBazBaseViewModel
    {
        public LoginRapidBazViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);
            LogOutCommand = new RelayCommand(LogOut);
        }

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand LogOutCommand { get; set; }

    }
}
