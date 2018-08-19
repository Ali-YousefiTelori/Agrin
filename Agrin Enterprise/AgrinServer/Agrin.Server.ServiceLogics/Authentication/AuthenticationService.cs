using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.DataBaseLogic;
using Agrin.Server.Models;
using Framesoft.Helpers.Helpers;
using SignalGo.Server.DataTypes;
using SignalGo.Server.Models;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.Authentication
{
    [ServiceContract("AuthenticationService", ServiceType.ServerService, InstanceType = InstanceType.SingleInstance)]
    public class AuthenticationService
    {
        public MessageContract<UserInfo> Login(Guid firstKey, Guid secondKey)
        {
            using (var context = new AgrinContext())
            {
                var user = context.UserSessionInfoes.Where(x => x.FirstKey == firstKey && x.SecondKey == secondKey && x.IsActive).Select(x => x.UserInfo).FirstOrDefault();
                if (user == null)
                    return MessageType.UsernameOrPasswordIncorrect;
                else if (user.Status != UserStatus.Confirm)
                    return MessageType.IsNotConfirm;
                OperationContext<CurrentUserInfo>.CurrentSetting = new CurrentUserInfo() { UserInfo = user, IsNormalUser = true, IsAdmin = false };
                return user.Success();
            }
        }

        [ConcurrentLock(Type = ConcurrentLockType.PerIpAddress)]
        public MessageContract<int> RegisterUser(UserInfo userInfo)
        {
            using (var context = new AgrinContext(false))
            {
                userInfo.UserName = userInfo.UserName.CleanPhoneNumber();
                if (string.IsNullOrEmpty(userInfo.UserName) || userInfo.UserName.Length < 5)
                    return MessageType.PleaseFillAllData;
                var find = context.UserInfoes.FirstOrDefault(x => x.UserName == userInfo.UserName);
                if (find != null)
                {
                    UserExtension.AddConfirmSMS(find.UserName, find.Id);
                    return find.Id.Success();
                }
                else
                {
                    find = new UserInfo();
                    find.Credit = 0;
                    find.Status = UserStatus.JustRegistered;
                    find.CreatedDateTime = DateTime.Now;
                    find.UserName = userInfo.UserName;
                }

                context.UserInfoes.Add(find);
                context.SaveChanges();
                UserExtension.AddConfirmSMS(find.UserName, find.Id);
                return find.Id.Success();
            }
        }

        [ConcurrentLock(Type = ConcurrentLockType.PerIpAddress)]
        public MessageContract<UserSessionInfo> ConfirmUserWithSMS(int userId, int randomNumber)
        {
            using (var context = new AgrinContext())
            {
                var find = context.UserInfoes.FirstOrDefault(x => x.Id == userId);
                if (find == null)
                    return MessageType.NotFound;
                else if (find.Status == UserStatus.Blocked)
                    return MessageType.AccessDenied;

                var confirm = context.UserConfirmHashInfoes.FirstOrDefault(x => x.UserId == userId && x.RandomNumber == randomNumber && !x.IsUsed);
                if (confirm == null)
                    return MessageType.CodeNotExist;
                confirm.IsUsed = true;
                find.Status = UserStatus.Confirm;
                UserSessionInfo userSessionInfo = new UserSessionInfo()
                {
                    CreatedDateTime = DateTime.Now,
                    FirstKey = Guid.NewGuid(),
                    SecondKey = Guid.NewGuid(),
                    IsActive = true,
                    UserId = find.Id,
                };
                context.UserSessionInfoes.Add(userSessionInfo);
                context.SaveChanges();

                return userSessionInfo.Success();
            }
        }

        [AgrinSecurityPermission(IsNormalUser = true)]
        public MessageContract EditUserSessionInfo(UserSessionInfo userSessionInfo)
        {
            using (var context = new AgrinContext())
            {
                var find = context.UserSessionInfoes.FirstOrDefault(x => x.Id == userSessionInfo.Id && x.UserId == CurrentUserInfo.UserId);
                if (find == null)
                    return MessageType.AccessDenied;
                find.OsName = userSessionInfo.OsName;
                find.OsVersionName = userSessionInfo.OsVersionName;
                find.OsVersionNumber = userSessionInfo.OsVersionNumber;
                context.SaveChanges();
                return MessageType.Success;
            }
        }
    }
}
