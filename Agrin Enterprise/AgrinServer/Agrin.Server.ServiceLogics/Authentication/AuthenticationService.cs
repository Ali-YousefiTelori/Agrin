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
        /// <summary>
        /// توکن های رجیستر شده ی کاربران برای صدا زدن توابع وب
        /// </summary>
        public static ConcurrentDictionary<Guid, (int UserId, string Ip)> RegisteredWebTokensOfUser { get; set; } = new ConcurrentDictionary<Guid, (int UserId, string Ip)>();

        public void Login()
        {

        }
    }
}
