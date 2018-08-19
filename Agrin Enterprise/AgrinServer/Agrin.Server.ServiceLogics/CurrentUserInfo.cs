using Agrin.Server.DataBase.Models;
using SignalGo.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics
{
    public class CurrentUserInfo
    {
        public static CurrentUserInfo Current
        {
            get
            {
                return OperationContext<CurrentUserInfo>.CurrentSetting;
            }
        }

        public static int UserId
        {
            get
            {
                return Current.UserInfo.Id;
            }
        }

        public UserInfo UserInfo { get; set; }

        public string Session { get; set; }

        public DateTime ExpireDateTime { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsNormalUser { get; set; }
    }
}
