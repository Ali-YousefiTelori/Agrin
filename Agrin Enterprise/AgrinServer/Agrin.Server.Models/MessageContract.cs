using System.Collections.Generic;

namespace Agrin.Server.Models
{

    public class MessageContract
    {
        public string Message { get; set; }
        public MessageType Error { get; set; }
        public bool IsSuccess { get; set; }
        /// <summary>
        /// وقتی مقادیر ارسال شده توسط کاربر صحیح نباشد
        /// </summary>
        public List<ValidationResultInfo> ValidationErrors { get; set; }

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

    public class MessageContract<T> : MessageContract
    {
        public T Data { get; set; }

        public static implicit operator MessageContract<T>(MessageType errorMessage)
        {
            if (errorMessage == MessageType.Success)
                return new MessageContract<T>() { IsSuccess = true };
            return new MessageContract<T>() { Error = errorMessage, IsSuccess = false };
        }

        public static implicit operator MessageContract<T>(bool value)
        {
            return new MessageContract<T>() { IsSuccess = value };
        }

        public static implicit operator MessageContract<T>(T data)
        {
            return new MessageContract<T>() { IsSuccess = true, Data = data };
        }
    }
}
