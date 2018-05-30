using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
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
                return user.Success();
            }
        }
    }
}
