using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Agrin.Server.ServiceLogics.Logics;
using SignalGo.Server.DataTypes;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.Authentication
{
    /// <summary>
    /// just calling from admin and valid server
    /// </summary>
    [ServiceContract("StorageAuthentication", ServiceType.OneWayService, InstanceType = InstanceType.SingleInstance)]
    public class StorageAuthenticationService
    {
        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public MessageContract CheckAccessUserToFileUpload(Guid firstKey, Guid secondKey, long directFileId)
        {
            using (var context = new AgrinContext(false))
            {
                if (context.UserSessions.Any(x => x.FirstKey == firstKey && x.SecondKey == secondKey && x.IsActive && x.UserInfo.DirectFileToUserRelationInfoes.Any(y => y.DirectFileId == directFileId && y.AccessType == DataBase.Models.Relations.DirectFileFolderAccessType.Creator)))
                    return MessageType.Success;
                return MessageType.SessionAccessDenied;
            }
        }

        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public MessageContract<UserInfo> GetUserByTelegramUserId(int telegramUserId)
        {
            using (var context = new AgrinContext(false))
            {
                var find = context.Users.Where(x => x.TelegramUserId == telegramUserId).FirstOrDefault();
                return find;
            }
        }

        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public MessageContract<UserInfo> GetUserByUserName(string userName)
        {
            using (var context = new AgrinContext(false))
            {
                var find = context.Users.Where(x => x.UserName == userName).FirstOrDefault();
                return find;
            }
        }

        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public MessageContract ChangeUserTelegramId(int userId, int telegramUserId)
        {
            using (var context = new AgrinContext())
            {
                var find = context.Users.FirstOrDefault(x => x.Id == userId);
                find.TelegramUserId = telegramUserId;
                context.SaveChanges();
                return MessageType.Success;
            }
        }

        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public MessageContract<UserInfo> AddUser(UserInfo userInfo)
        {
            using (var context = new AgrinContext())
            {
                userInfo.CreatedDateTime = DateTime.Now;
                context.Users.Add(userInfo);
                context.SaveChanges();
                return userInfo;
            }
        }

        public MessageContract GiftCradit(Guid key, Guid value, int amount, int toUserId)
        {
            var userName = Guid.Parse("5822a0eb-87e3-4995-beb8-599f0e5d5f5e");
            var password = Guid.Parse("954a20c8-d9d9-4ae0-8a96-70bdb24d19b4");
            if (key == userName && password == value)
            {
                return StorageLogic.AddUserCredit(new UserCreditInfo()
                {
                    Amount = amount,
                    FromUserId = 2,
                    ToUserId = toUserId,
                    Type = CreditType.Gift,
                    Key = Guid.NewGuid()
                });
            }
            return MessageType.WrongData;
        }
    }
}
