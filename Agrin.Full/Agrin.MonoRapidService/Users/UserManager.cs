using Agrin.MonoRapidService.RapidBazUsersService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.RapidService.Users
{
    public static class UserManager
    {
        public static UserInfo CurrentUser { get; set; }
        static WSDLapiusersService CurrentService { get; set; }

        static UserManager()
        {

            CurrentService = new WSDLapiusersService();
            CurrentUser = new UserInfo();
        }

        public static bool IsLogin { get; set; }
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
                    IsLogin = true;
                    var task = new System.Threading.Tasks.Task(() =>
                    {
                        resetEvent.WaitOne(new TimeSpan(30, 0, 0));
                        IsLogin = false;
                        CurrentSession = "";
                    });
                    task.Start();
                    CurrentSession = session;
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
    }
}
