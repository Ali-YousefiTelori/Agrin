using Agrin.Server.Models;
using SignalGo.Server.DataTypes;
using SignalGo.Server.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Agrin.Server.ServiceLogics
{
    public class AgrinSecurityPermission : SecurityContractAttribute
    {
        public bool IsAdmin { get; set; }
        public bool IsNormalUser { get; set; }
        
        public override bool CheckPermission(ClientInfo client, object service, MethodInfo method, List<object> parameters)
        {
            var data = OperationContext<CurrentUserInfo>.CurrentSetting;

            if (data == null)
                return false;
            else if (data.IsAdmin && IsAdmin)
                return true;
            else if (data.IsNormalUser && IsNormalUser)
                return true;
            return false;
        }

        public override object GetValueWhenDenyPermission(ClientInfo client, object service, MethodInfo method, List<object> parameters)
        {
            var msg = (MessageContract)MessageType.SessionAccessDenied;
            var data = OperationContext<CurrentUserInfo>.CurrentSetting;
            var userIsAdmin = data == null ? false : data.IsAdmin;
            var isNormalUser = data == null ? false : data.IsNormalUser;
            string roles = "";
            if (data != null)
            {
                roles = $"Your Roles {{IsAdmin: {userIsAdmin} IsNormalUser: {isNormalUser}}}";
            }
            else
                msg.Message += "Access Denied";
            msg.Message += $" Need Roles {{IsAdmin: {IsAdmin} IsNormalUser: {IsNormalUser} {Environment.NewLine}}} IsNullUserCookie: {data == null} {roles}  {Environment.NewLine}";
            return msg;
        }
    }
}
