using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.DataBaseLogic;
using Agrin.Server.Models;
using Framesoft.Helpers.Helpers;
using SignalGo.Server.DataTypes;
using SignalGo.Server.Models;
using SignalGo.Shared.DataTypes;
using System;
using System.Linq;
using System.Text;

namespace Agrin.Server.ServiceLogics.Authentication
{
    [ServiceContract("Authentication", ServiceType.ServerService, InstanceType = InstanceType.SingleInstance)]
    public class AuthenticationService
    {
        public MessageContract<UserInfo> Login(Guid firstKey, Guid secondKey)
        {
            using (AgrinContext context = new AgrinContext())
            {
                UserInfo user = context.UserSessionInfoes.Where(x => x.FirstKey == firstKey && x.SecondKey == secondKey && x.IsActive).Select(x => x.UserInfo).FirstOrDefault();
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
            using (AgrinContext context = new AgrinContext(false))
            {
                userInfo.UserName = userInfo.UserName.FixPhoneNumber();
                if (string.IsNullOrEmpty(userInfo.UserName) || userInfo.UserName.Length < 5)
                    return MessageType.PleaseFillAllData;
                else if (!userInfo.UserName.IsNumberValudate())
                    return MessageType.DataIsNotValid;

                UserInfo find = context.UserInfoes.FirstOrDefault(x => x.UserName == userInfo.UserName);
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
                    if (userInfo.UserName.Length >= 8)
                    {
                        var aStringBuilder = new StringBuilder(userInfo.UserName);
                        aStringBuilder.Remove(5, 3);
                        aStringBuilder.Insert(5, "***");
                        find.DisplayName = aStringBuilder.ToString();
                    }
                }

                context.UserInfoes.Add(find);
                context.SaveChanges();
                UserExtension.AddConfirmSMS(find.UserName, find.Id);
                return find.Id.Success();
            }
        }

        [ConcurrentLock(Type = ConcurrentLockType.PerIpAddress)]
        public MessageContract<UserSessionInfo> ConfirmUserWithSMS(int userId, int randomNumber, UserSessionInfo userSessionInfo)
        {
            using (AgrinContext context = new AgrinContext())
            {
                UserInfo find = context.UserInfoes.FirstOrDefault(x => x.Id == userId);
                if (find == null)
                    return MessageType.NotFound;
                else if (find.Status == UserStatus.Blocked)
                    return MessageType.AccessDenied;

                UserConfirmHashInfo confirm = context.UserConfirmHashInfoes.FirstOrDefault(x => x.UserId == userId && x.RandomNumber == randomNumber && !x.IsUsed);
                if (confirm == null)
                    return MessageType.CodeNotExist;
                confirm.IsUsed = true;
                find.Status = UserStatus.Confirm;
                UserSessionInfo finndSession = context.UserSessionInfoes.FirstOrDefault(x => x.UserId == userId &&
                x.OsName == userSessionInfo.OsName &&
                x.OsVersionName == userSessionInfo.OsVersionName &&
                x.OsVersionNumber == userSessionInfo.OsVersionNumber);
                if (finndSession == null)
                {
                    finndSession = new UserSessionInfo()
                    {
                        CreatedDateTime = DateTime.Now,
                        FirstKey = Guid.NewGuid(),
                        SecondKey = Guid.NewGuid(),
                        IsActive = true,
                        UserId = userId,
                        DeviceName = userSessionInfo.DeviceName,
                        OsName = userSessionInfo.OsName,
                        OsVersionName = userSessionInfo.OsVersionName,
                        OsVersionNumber = userSessionInfo.OsVersionNumber
                    };
                    context.UserSessionInfoes.Add(finndSession);
                    context.SaveChanges();
                }

                return finndSession.Success();
            }
        }

        [AgrinSecurityPermission(IsNormalUser = true)]
        public MessageContract EditUserSessionInfo(UserSessionInfo userSessionInfo)
        {
            using (AgrinContext context = new AgrinContext())
            {
                UserSessionInfo find = context.UserSessionInfoes.FirstOrDefault(x => x.Id == userSessionInfo.Id && x.UserId == CurrentUserInfo.UserId);
                if (find == null)
                    return MessageType.AccessDenied;
                find.OsName = userSessionInfo.OsName;
                find.OsVersionName = userSessionInfo.OsVersionName;
                find.OsVersionNumber = userSessionInfo.OsVersionNumber;
                find.DeviceName = userSessionInfo.DeviceName;
                context.SaveChanges();
                return MessageType.Success;
            }
        }
    }
}
