using Framesoft.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Framesoft.Services.IServices
{
    [ServiceContract(Namespace = "CPM.WCFServices.Callbacks", SessionMode = SessionMode.Required,
                   CallbackContract = typeof(Callbacks.IUserManagerCallback))]
    public interface ILoginService
    {
        [OperationContract()]
        MessageContract<Guid> GetAccessKey();
        [OperationContract()]
        MessageContract Login(string encryptedLoginData);
        [OperationContract()]
        MessageContract Logout(UserInfo userInfo);
    }
}
