using Agrin.Server.ServiceModels.Authentication;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.Authentication
{
    public class AuthenticationService : IAuthentication
    {
        /// <summary>
        /// توکن های رجیستر شده ی کاربران برای صدا زدن توابع وب
        /// </summary>
        public static ConcurrentDictionary<Guid, (int UserId, string Ip)> RegisteredWebTokensOfUser { get; set; } = new ConcurrentDictionary<Guid, (int UserId, string Ip)>();
    }
}
