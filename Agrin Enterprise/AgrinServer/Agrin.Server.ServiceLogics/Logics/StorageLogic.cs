using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.Logics
{
    public static class StorageLogic
    {
        static ConcurrentDictionary<int, object> LockChangesCredit { get; set; } = new ConcurrentDictionary<int, object>();

        public static MessageType AddUserCredit(UserCreditInfo userCreditInfo)
        {
            if (LockChangesCredit.TryGetValue(userCreditInfo.ToUserId, out object lockObject))
            {
                try
                {
                    lock (lockObject)
                    {
                        using (var context = new AgrinContext())
                        {
                            if (context.UserCreditInfoes.Any(x => x.Key == userCreditInfo.Key))
                                return MessageType.Duplicate;
                            var user = context.UserInfoes.FirstOrDefault(x => x.Id == userCreditInfo.ToUserId);
                            user.Credit += userCreditInfo.Amount;
                            context.UserCreditInfoes.Add(userCreditInfo);
                            context.SaveChanges();
                            return MessageType.Success;
                        }
                    }
                }
                finally
                {
                    LockChangesCredit.TryRemove(userCreditInfo.ToUserId, out lockObject);
                }
            }
            else
            {
                LockChangesCredit.TryAdd(userCreditInfo.ToUserId, new object());
                return AddUserCredit(userCreditInfo);
            }

        }
    }
}
