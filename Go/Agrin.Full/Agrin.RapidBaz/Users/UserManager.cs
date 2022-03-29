using Agrin.RapidBaz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.RapidBaz.Users
{
    public static class UserManager
    {
        public static Action<RapidUserInfo> ChangedUserInfo { get; set; }
        public static UserInfo CurrentUser { get; set; }
        public static RapidUserInfo CurrentRapidUserInfo { get; set; }

        static IWSDLapiusersService CurrentService { get; set; }

        public static void Initialize(IWSDLapiusersService WSDLapiusersService)
        {
            CurrentService = WSDLapiusersService;
            CurrentUser = new UserInfo();
        }

        public static event Action LoginChanged;

        static bool _IsLogin = false;

        public static bool IsLogin
        {
            get { return _IsLogin; }
            set
            {
                _IsLogin = value;
                if (LoginChanged != null)
                    LoginChanged();
            }
        }
        public static string CurrentSession { get; set; }

        internal static string GetMD5String(string password)
        {
            // byte array representation of that string
            byte[] encodedPassword = new UTF8Encoding().GetBytes(password);

            // need MD5 to calculate the hash
            byte[] hash = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            // string representation (similar to UNIX format)
            string encoded = BitConverter.ToString(hash)
                // without dashes
               .Replace("-", string.Empty)
                // make lowercase
               .ToLower();
            return encoded;
        }

        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        public static bool Login(string userName, string password)
        {
            try
            {
                resetEvent.Reset();
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return false;
                }

                var session = CurrentService.Login(userName, GetMD5String(password), "12X3U3A", "Agrin WPF Client", "30");
                int i = 0;
                if (!int.TryParse(session, out i))
                {
                    CurrentSession = session;
                    CurrentRapidUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<RapidUserInfo>(UserInfo());
                    if (ChangedUserInfo != null)
                        ChangedUserInfo(CurrentRapidUserInfo);
                    IsLogin = true;
                    var task = new System.Threading.Tasks.Task(() =>
                    {
                        resetEvent.WaitOne(new TimeSpan(30, 0, 0));
                        IsLogin = false;
                        CurrentSession = "";
                    });
                    task.Start();
                }
                else
                {
                    IsLogin = false;
                    CurrentSession = "";
                }
                return IsLogin;
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "UserManager Login", true);
                IsLogin = false;
                CurrentSession = "";
                return false;
            }
        }

        public static void LogOut()
        {
            try
            {
                CurrentService.Logout(CurrentSession);
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "UserManager LogOut", true);
            }
            CurrentSession = "";
            IsLogin = false;
        }

        public static string UserInfo()
        {
            return CurrentService.UserInfo(CurrentSession);
        }
    }
}
