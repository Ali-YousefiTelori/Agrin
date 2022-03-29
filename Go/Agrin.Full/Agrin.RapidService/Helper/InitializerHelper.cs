using Agrin.RapidService.Models;
using Agrin.RapidService.RapidBazUsersService;
using Agrin.RapidService.RapidBazWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidService.Helper
{
    public static  class InitializerHelper
    {
        public static void Initialize()
        {
            Agrin.RapidBaz.Users.UserManager.Initialize(new UserService());
            Agrin.RapidBaz.Web.WebManager.Initialize(new WebService());
        }
    }
}
