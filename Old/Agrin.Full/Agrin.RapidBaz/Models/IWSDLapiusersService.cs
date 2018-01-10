using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Models
{
    public interface IWSDLapiusersService
    {
        string Login(string userName, string passWord, string code, string userAgent, string time);
        string Logout(string session);
        string UserInfo(string session);
    }
}
