#if(!MobileApp || Debug)
using Agrin.Download.Data.Settings;
using Agrin.Download.Engine;
using Agrin.Helper.ComponentModel;
using Agrin.RapidBaz.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.RapidBaz
{
    public class LoginRapidBazBaseViewModel : ANotifyPropertyChanged
    {
        public static LoginRapidBazBaseViewModel This { get; set; }
        public LoginRapidBazBaseViewModel()
        {
            This = this;
            if (ApplicationSetting.Current == null)
                return;
            UserManager.LoginChanged += () =>
                {
                    OnPropertyChanged("IsLogin");
                    OnPropertyChanged("RapidUserInfo");
                };
            UserName = ApplicationSetting.Current.RapidBazSetting.UserName;
            Password = ApplicationSetting.Current.RapidBazSetting.Password;
            IsSaveSetting = ApplicationSetting.Current.RapidBazSetting.IsSaveSetting;
        }

        public RapidUserInfo RapidUserInfo
        {
            get
            {
                return UserManager.CurrentRapidUserInfo;
            }
        }

        string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        string _Password;

        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }

        string _ErrorMessage;

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; OnPropertyChanged("ErrorMessage"); }
        }

        bool _IsBusy = false;

        public bool IsBusy
        {
            get { return _IsBusy; }
            set { _IsBusy = value; OnPropertyChanged("IsBusy"); }
        }

        public bool IsLogin
        {
            get { return Agrin.RapidBaz.Users.UserManager.IsLogin; }
            set { Agrin.RapidBaz.Users.UserManager.IsLogin = value; OnPropertyChanged("IsLogin"); }
        }

        bool _IsSaveSetting = false;

        public bool IsSaveSetting
        {
            get { return _IsSaveSetting; }
            set { _IsSaveSetting = value; OnPropertyChanged("IsSaveSetting"); }
        }

        public void Login()
        {
            ErrorMessage = "در حال ورود...";
            IsBusy = true;
            AsyncActions.Action(() =>
            {
                Login((msg) => ErrorMessage = msg, (busy) => IsBusy = busy, UserName, Password, false);
                ChangeIsLogin();
            }, (e) =>
            {
                ErrorMessage = "خطا در بارگزاری اطلاعات";
                System.Threading.Thread.Sleep(2000);
                IsBusy = false;
            });

            //ErrorMessage = "در حال ورود...";
            //IsBusy = true;
            //AsyncActions.Action(() =>
            //{
            //    if (RapidBazEngineHelper.Login(UserName, Password))
            //    {
            //        ErrorMessage = "ورود با موفقیت انجام شد.";
            //        if (IsSaveSetting)
            //        {
            //            ApplicationSetting.Current.RapidBazSetting.UserName = UserName;
            //            ApplicationSetting.Current.RapidBazSetting.Password = Password;
            //            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            //        }
            //        IsLogin = true;
            //        QueueListRapidBazBaseViewModel.MustRefresh = true;
            //        CompleteListRapidBazBaseViewModel.MustRefresh = true;
            //    }
            //    else
            //    {
            //        ErrorMessage = "خطا در ورود رخ داده است.";
            //    }
            //    System.Threading.Thread.Sleep(1500);
            //    IsBusy = false;
            //});
        }

        static object lockLogin = new object();
        public static void Login(Action<string> chanedMessage, Action<bool> changedBusy, string userName, string password, bool setThis = false)
        {
            string msg = "در حال ورود...";
            chanedMessage(msg);
            if (changedBusy != null)
                changedBusy(true);
            lock (lockLogin)
            {
                if (RapidBazEngineHelper.IsLogin)
                {
                    if (changedBusy != null)
                        changedBusy(false);
                    return;
                }
                bool isSaveSetting = false;
                if (LoginRapidBazBaseViewModel.This != null)
                    isSaveSetting = LoginRapidBazBaseViewModel.This.IsSaveSetting;
                if (setThis && LoginRapidBazBaseViewModel.This != null)
                {
                    LoginRapidBazBaseViewModel.This.ErrorMessage = msg;
                    LoginRapidBazBaseViewModel.This.IsBusy = true;
                }

                if (RapidBazEngineHelper.Login(userName, password))
                {
                    msg = "ورود با موفقیت انجام شد.";
                    chanedMessage(msg);

                    if (setThis && LoginRapidBazBaseViewModel.This != null)
                        LoginRapidBazBaseViewModel.This.ErrorMessage = msg;


                    if (isSaveSetting)
                    {
                        ApplicationSetting.Current.RapidBazSetting.UserName = userName;
                        ApplicationSetting.Current.RapidBazSetting.Password = password;
                    }
                    else
                    {
                        ApplicationSetting.Current.RapidBazSetting.UserName = "";
                        ApplicationSetting.Current.RapidBazSetting.Password = "";
                    }

                    ApplicationSetting.Current.RapidBazSetting.IsSaveSetting = isSaveSetting;
                    Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
                    Agrin.RapidBaz.Users.UserManager.IsLogin = true;

                    if (setThis && LoginRapidBazBaseViewModel.This != null)
                        LoginRapidBazBaseViewModel.This.IsLogin = true;

                    QueueListRapidBazBaseViewModel.MustRefresh = true;
                    CompleteListRapidBazBaseViewModel.MustRefresh = true;
                }
                else
                {
                    msg = "خطا در ورود رخ داده است. لطفاً نام کاربری و رمز عبور یا پروکسی سیستم خود را بررسی کنید";
                    chanedMessage(msg);
                    if (setThis && LoginRapidBazBaseViewModel.This != null)
                        LoginRapidBazBaseViewModel.This.ErrorMessage = msg;
                }
                System.Threading.Thread.Sleep(1500);
                if (changedBusy != null)
                    changedBusy(false);
                if (setThis && LoginRapidBazBaseViewModel.This != null)
                    LoginRapidBazBaseViewModel.This.IsBusy = false;
            }
        }

        public bool CanLogin()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }

        public void LogOut()
        {
            IsLogin = false;
        }

        public static void ChangeIsLogin()
        {
            if (This != null)
                This.OnPropertyChanged("IsLogin");
        }
    }
}
#endif