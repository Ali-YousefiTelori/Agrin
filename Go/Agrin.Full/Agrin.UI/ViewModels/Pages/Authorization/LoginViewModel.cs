using Agrin.Download.Data.Settings;
using Agrin.Download.Engine;
using Agrin.Helper.ComponentModel;
using Agrin.UI.Views.Pages.Authorization;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.Pages.Authorization
{
    public class LoginViewModel : ANotifyPropertyChanged
    {
        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            UserName = ApplicationSetting.Current.RapidBazSetting.UserName;
            Password = ApplicationSetting.Current.RapidBazSetting.Password;

            LoginCommand = new RelayCommand(() =>
            {
                IsEnabled = false;
                ErrorMessage = "در حال ورود...";
                AsyncActions.Action(() =>
                {
                    if (RapidBazEngineHelper.Login(UserName, Password))
                    {
                        ErrorMessage = "ورود با موفقیت انجام شد.";
                        Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                        System.Threading.Thread.Sleep(1000);
                        ErrorMessage = "";
                        ApplicationHelper.EnterDispatcherThreadAction(PagesManagerViewModel.This.BackItem);
                    }
                    else
                    {
                        ErrorMessage = "خطا در ورود رخ داده است.";
                    }
                    IsEnabled = true;
                });
            });

        }

        string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; OnPropertyChanged("UserName"); }
        }

        string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password = value; OnPropertyChanged("Password"); }
        }

        string _ErrorMessage;

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; OnPropertyChanged("ErrorMessage"); }
        }

        bool _IsEnabled = true;

        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

       
    }
}
