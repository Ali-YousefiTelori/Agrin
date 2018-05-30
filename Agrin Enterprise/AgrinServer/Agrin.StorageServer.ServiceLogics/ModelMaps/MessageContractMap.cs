using Agrin.Server.Models;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.StorageServer.ServiceLogics.ModelMaps
{
    /*
    [ModelMapp(typeof(MessageContract))]
    public class MessageContractMap : MessageContract
    {
        public static implicit operator MessageContract(MessageType errorMessage)
        {
            if (errorMessage == MessageType.Success)
                return new MessageContract() { IsSuccess = true };
            return new MessageContract() { Error = errorMessage, IsSuccess = false };
        }

        public static implicit operator MessageContract(bool value)
        {
            return new MessageContract() { IsSuccess = value };
        }
    }
    */
}
