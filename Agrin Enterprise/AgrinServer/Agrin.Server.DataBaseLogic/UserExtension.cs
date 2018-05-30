using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
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
                context.UserInfoes.Add(userInfo);
                context.SaveChanges();
            }
            return true;
        }
    }
}
