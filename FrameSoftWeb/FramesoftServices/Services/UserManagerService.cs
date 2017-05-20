using Framesoft.Services.IServices;
using Framesoft.Services.Models;
using Framesoft.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Framesoft.Services.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class UserManagerService : ILoginService
    {
        static Dictionary<string, ClientInfo> ClientSessions = new Dictionary<string, ClientInfo>();
        /// <summary>
        /// با استفاده از لیست شسن ها کلید را بدست می اورد
        /// </summary>
        static Dictionary<string, Guid> ClientSessionKeyValue = new Dictionary<string, Guid>();
        /// <summary>
        /// با استفاده از لیست کلید ها سشن ها را بر میگرداند
        /// هر کلید ممکن است چند سشن داشته باشد برای سرویس های مختلف
        /// </summary>
        static Dictionary<Guid, List<string>> ClientSessionKeys = new Dictionary<Guid, List<string>>();
        //static Dictionary<Guid, List<string>> ClientSessionKeys = new Dictionary<Guid, List<string>>();
        public UserManagerService()
        {
            //var newClient = ;//.GetCallbackChannel<ICalculatorDuplexCallback>();
            //IClientChannel proxy =;
            ClientSessions.Add(OperationContext.Current.SessionId, null);
            OperationContext.Current.Channel.Faulted += new EventHandler(Channel_Faulted);
            OperationContext.Current.Channel.Closed += new EventHandler(Channel_Faulted);
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            var context = (IContextChannel)sender;
            if (context != null)
                DisposeUser(context.SessionId);
        }

        public MessageContract Logout(UserInfo userInfo)
        {
            if (userInfo == null)
                return new MessageContract() { IsSuccess = false, Message = "User not found!" };
            bool find = false;
            foreach (var item in ClientSessions)
            {
                if (item.Value != null && item.Value.User.Password == userInfo.Password && item.Value.User.UserName == userInfo.UserName)
                {
                    DisposeUser(item.Key);
                    find = true;
                }
            }
            if (!find)
                return MessageContract.NoSuscess("user not found!");
            return new MessageContract() { IsSuccess = true };
        }

        void DisposeUser(string sessionId)
        {
            if (ClientSessions.ContainsKey(sessionId))
            {
                if (ClientSessions[sessionId] != null)
                {
                    bool error = true;
                    try
                    {
                        ClientSessions[sessionId].ClientChannel.Close();
                        error = false;
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (error)
                        {
                            if (ClientSessions.ContainsKey(sessionId))
                                ClientSessions[sessionId].ClientChannel.Abort();
                        }
                    }
                }
                ClientSessions.Remove(sessionId);
                if (ClientSessionKeyValue.ContainsKey(sessionId))
                {
                    var guid = ClientSessionKeyValue[sessionId];
                    ClientSessionKeyValue.Remove(sessionId);
                    if (ClientSessionKeys[guid] != null)
                        ClientSessionKeys[guid].Clear();
                    ClientSessionKeys.Remove(guid);
                }
            }
        }

        public static bool GenerateAccessKey(Guid accessKey)
        {
            if (ClientSessionKeys.ContainsKey(accessKey) && ClientSessionKeys[accessKey] != null)
            {
                foreach (var item in ClientSessionKeys[accessKey])
                {
                    if (ClientSessions.ContainsKey(item) && ClientSessions[item] == null)
                    {
                        return false;
                    }
                }
                if (!ClientSessionKeys[accessKey].Contains(OperationContext.Current.SessionId))
                    ClientSessionKeys[accessKey].Add(OperationContext.Current.SessionId);
                if (!ClientSessionKeyValue.ContainsKey(OperationContext.Current.SessionId))
                    ClientSessionKeyValue.Add(OperationContext.Current.SessionId, accessKey);
                return true;
            }
            else
                return false;
        }

        public MessageContract<Guid> GetAccessKey()
        {
            try
            {
                if (ClientSessionKeyValue.ContainsKey(OperationContext.Current.SessionId))
                    return MessageContract<Guid>.Suscess(ClientSessionKeyValue[OperationContext.Current.SessionId]);
                var guid = Guid.NewGuid();
                ClientSessionKeyValue.Add(OperationContext.Current.SessionId, guid);
                if (ClientSessionKeys.ContainsKey(guid))
                {
                    if (!ClientSessionKeys[guid].Contains(OperationContext.Current.SessionId))
                    {
                        ClientSessionKeys[guid].Add(OperationContext.Current.SessionId);
                    }
                }
                else
                {
                    ClientSessionKeys.Add(guid, new List<string>() { OperationContext.Current.SessionId });
                }
                return MessageContract<Guid>.Suscess(ClientSessionKeyValue[OperationContext.Current.SessionId]);
            }
            catch (Exception ex)
            {
                return ex.GetMessageContractFromException<Guid>();
            }
        }

        public MessageContract Login(string encryptedLoginData)
        {
            try
            {
                if (!ClientSessionKeyValue.ContainsKey(OperationContext.Current.SessionId))
                    return MessageContract.NoSuscess("You don't have premission !");
                var getAccessKey = ClientSessionKeyValue[OperationContext.Current.SessionId];
                var userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(StringCipher.DecryptStringAES(encryptedLoginData, ClientSessionKeyValue[OperationContext.Current.SessionId].ToString()));
                var userID = 0;// UserHelper.Login(userInfo.UserName, userInfo.Password);

                if (userID != -1)
                {
                    userInfo.UserId = userID;
                    if (ClientSessions[OperationContext.Current.SessionId] == null)
                        ClientSessions[OperationContext.Current.SessionId] = new ClientInfo() { SessionIds = new List<string>() { OperationContext.Current.SessionId }, User = userInfo, ClientChannel = (IClientChannel)OperationContext.Current.GetCallbackChannel<Callbacks.IUserManagerCallback>() };
                    return MessageContract.Suscess();
                }
                else
                    return MessageContract.NoSuscess("Error Login, check User-Name and Password");
            }
            catch (Exception ex)
            {
                return ex.GetMessageContractFromException();
            }
        }

        static bool IsValidSessionKey()
        {
            if (ClientSessionKeyValue.ContainsKey(OperationContext.Current.SessionId))
            {
                var guid = ClientSessionKeyValue[OperationContext.Current.SessionId];
                if (ClientSessionKeys.ContainsKey(guid) && ClientSessionKeys[guid] != null)
                {
                    foreach (var item in ClientSessionKeys[guid])
                    {
                        if (ClientSessions.ContainsKey(item) && ClientSessions[item] != null && ClientSessions[item].User != null)
                            return true;
                    }
                }
            }
            return false;
        }

        public static MessageContract IsValidSession()
        {
#if (TestDebug)
            return null;
#endif
            if (IsValidSessionKey())
                return null;
            return MessageContract.NoSuscess("Sorry, You cannot access !");
        }

        public static MessageContract<T> IsValidSession<T>()
        {
#if (TestDebug)
            return null;
#endif
            if (IsValidSessionKey())
                return null;
            return MessageContract<T>.NoSuscess("Sorry, You cannot access !");
        }

        public static UserInfo GetCurrentUser()
        {
#if(TestDebug)
            return new UserInfo() { UserId = 1 };
#endif
            if (ClientSessionKeyValue.ContainsKey(OperationContext.Current.SessionId))
            {
                var guid = ClientSessionKeyValue[OperationContext.Current.SessionId];
                if (ClientSessionKeys[guid] != null)
                {
                    foreach (var item in ClientSessionKeys[guid])
                    {
                        if (ClientSessions.ContainsKey(item) && ClientSessions[item] != null && ClientSessions[item].User != null)
                            return ClientSessions[item].User;
                    }
                }
            }
            return null;
        }
    }
}
