namespace Agrin.Server.Models
{

    public class MessageContract
    {
        public object Data { get; set; }
        public string Message { get; set; }
        public MessageType Error { get; set; }
        public bool IsSuccess { get; set; }

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

    public class MessageContract<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public MessageType Error { get; set; }
        public bool IsSuccess { get; set; }

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

    /// <summary>
    /// اکستنشن های پیام
    /// </summary>
    public static class MessageContractExtension
    {
        /// <summary>
        /// تبدیل یک کلاس به پیام تایید شده
        /// </summary>
        /// <typeparam name="T">نوع دیتای مورد نظر</typeparam>
        /// <param name="data">دیتای مورد نظر</param>
        /// <returns></returns>
        public static MessageContract<T> Success<T>(this T data)
        {
            return new MessageContract<T>() { IsSuccess = true, Data = data };
        }

        /// <summary>
        /// تبدیل یک آبجکت به یک پیام تایید شده
        /// </summary>
        /// <param name="data">دیتای مورد نظر</param>
        /// <returns></returns>
        public static MessageContract Success(this object data)
        {
            return new MessageContract() { IsSuccess = true, Data = data };
        }

    }
}
