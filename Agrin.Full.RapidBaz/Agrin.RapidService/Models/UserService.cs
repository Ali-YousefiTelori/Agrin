using Agrin.RapidBaz.Models;
using Agrin.RapidService.RapidBazUsersService;

namespace Agrin.RapidService.Models
{
    public class UserService : IWSDLapiusersService
    { 
        WSDLapiusersService service = null;
        public UserService()
        {
            service = new WSDLapiusersService();
        }

        public string Login(string userName, string passWord, string code, string userAgent, string time)
        {
            return service.Login(userName, passWord, code, userAgent, time);
        }

        public string Logout(string session)
        {
            return service.Logout(session);
        }

        public string UserInfo(string session)
        {
            return service.UserInfo(session);
        }
    }
}
