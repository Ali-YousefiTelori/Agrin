using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Framesoft.Helpers.Helpers;
using SignalGo.Shared.Log;
using SMSService.OneWayServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBaseLogic
{
    public static class UserExtension
    {
        public static MessageContract RegisterUser(UserInfo userInfo)
        {
            if (userInfo == null || string.IsNullOrEmpty(userInfo.UserName) || userInfo.UserName.Length < 10)
                return MessageType.PleaseFillAllData;

            using (AgrinContext context = new AgrinContext(false))
            {
                context.Users.Add(userInfo);
                context.SaveChanges();
            }
            return true;
        }

        public static async void AddConfirmSMS(string phone, int userId)
        {
            using (var context = new AgrinContext())
            {
                var randomNumber = RandomizationHelper.GetRandomNumber();
                context.UserConfirmHashes.Add(new UserConfirmHashInfo()
                {
                    IsUsed = false,
                    RandomGuid = Guid.NewGuid(),
                    RandomNumber = randomNumber,
                    UserId = userId,
                    CreatedDateTime = DateTime.Now
                });
                context.SaveChanges();
                try
                {
                    var result = await SMSSenderController.Current.SendSMSAsync(AgrinConfigInformation.Current.SMSServerUserName, AgrinConfigInformation.Current.SMSServerPassword, "کد ثبت نام در آگرین: " + randomNumber, phone);
                    if (!result.IsSuccess)
                    {
                        AutoLogger.Default.LogText("Send SMS not success " + result.Message);

                    }
                }
                catch (Exception ex)
                {
                    AutoLogger.Default.LogError(ex, "Send SMS Error");
                }

            }
        }
    }
}
