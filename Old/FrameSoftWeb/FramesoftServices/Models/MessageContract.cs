using Framesoft.Services.Heplers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using FrameSoft.Agrin.DataBase.Helper;

namespace Framesoft.Services.Models
{
    public static class MessageContractExtension
    {
        static MessageContractExtension()
        {
            ExceptionsHelper.GetSqlExceptionLog = (ex) =>
            {
                return ex.GetSqlExceptionLog();
            };
        }
        public static MessageContract GetMessageContractFromException(this Exception ex)
        {
            return new MessageContract() { IsSuccess = false, Message = "Service Exception: " + ex.Message, StackTraceMessage = ex.GetFullString() };
        }

        public static MessageContract<T> GetMessageContractFromException<T>(this Exception ex)
        {
            return new MessageContract<T>() { IsSuccess = false, Message = "Service Exception: " + ex.Message, StackTraceMessage = ex.GetFullString() };
        }
    }

    [DataContract(Name = "MessageContract{0}")]
    public class MessageContract<T>
    {
        [DataMember]
        public T Data { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string StackTraceMessage { get; set; }

        public static MessageContract<T> Suscess(T data, string message = null)
        {
            return new MessageContract<T>() { IsSuccess = true, Data = data, Message = message };
        }

        public static MessageContract<T> NoSuscess(string message)
        {
            return new MessageContract<T>() { IsSuccess = false, Message = message };
        }
    }

    [DataContract]
    public class MessageContract
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string StackTraceMessage { get; set; }

        public static MessageContract Suscess(string message = null)
        {
            return new MessageContract() { IsSuccess = true, Message = message };
        }

        public static MessageContract NoSuscess(string message)
        {
            return new MessageContract() { IsSuccess = false, Message = message };
        }
    }
}