using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FramwsoftSignalGoServices
{

    /// <summary>
    /// کلاس خروجی های پیام که سرور به کلاینت ارسال میکند
    /// </summary>
    /// <typeparam name="T">نوع دیتا</typeparam>
    public class MessageContract<T>
    {
        /// <summary>
        /// دیتایی که کلاینت برای دریافت نیاز دارد
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// آیا تابع بع درستی اجرا شده است؟
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// پیام خطا
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// شماره ی خطا
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// پیام خطای سروری
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// ثبت خطا برای مسج کانترکت
        /// </summary>
        /// <param name="message">پیام خطا</param>
        /// <returns></returns>
        public static MessageContract<T> Fail(string message)
        {
            return new MessageContract<T>() { IsSuccess = false, Message = message };
        }
        /// <summary>
        /// ثبت پیام تایید شده
        /// </summary>
        /// <param name="data">دیتای مورد نظر</param>
        /// <returns></returns>
        public static MessageContract<T> Success(T data)
        {
            return new MessageContract<T>() { IsSuccess = true, Data = data };
        }

        /// <summary>
        /// ساخت پیام تایید
        /// </summary>
        /// <returns></returns>
        public static MessageContract<T> Success()
        {
            return new MessageContract<T>() { IsSuccess = true };
        }
        /// <summary>
        /// تدبیل کلاس بیس به کلاس نوع داده ای
        /// </summary>
        /// <param name="messageContract"></param>
        public static implicit operator MessageContract<T>(MessageContract messageContract)
        {
            return new MessageContract<T>() { IsSuccess = messageContract.IsSuccess, Message = messageContract.Message, StackTrace = messageContract.StackTrace };
        }
        
        /// <summary>
        /// تبدیل به کلاس بیس
        /// </summary>
        /// <returns></returns>
        public MessageContract ToMessageContract()
        {
            return new MessageContract() { IsSuccess = IsSuccess, Data = Data, Message = Message, StackTrace = StackTrace };
        }
    }

    /// <summary>
    /// کلاس بیس پیام های خروجی سرور به کلاینت
    /// </summary>
    public class MessageContract
    {
        /// <summary>
        /// دیتایی که کلاینت باید دریافت کند
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// در صورتی که همه چیز درست انجام شود سرور به کلاینت مقدار درست بر میگرداند
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// پیام سرور به کلاینت
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// جزئیات خطای ثبت شده در سرور
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// شماره ی خطا
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// فیل کردن یک مسج
        /// </summary>
        /// <param name="message">مسج مورد نظر</param>
        /// <returns></returns>
        public static MessageContract Fail(string message)
        {
            return new MessageContract() { IsSuccess = false, Message = message };
        }

        //public static MessageContract Success(object data)
        //{
        //    return new MessageContract() { IsSuccess = true, Data = data };
        //}
        /// <summary>
        /// تایید کردن یک پیام
        /// </summary>
        /// <returns></returns>
        public static MessageContract Success()
        {
            return new MessageContract() { IsSuccess = true };
        }
        /// <summary>
        /// دیتا فرستادن به عنوان تایید مسیج
        /// </summary>
        /// <param name="data">دیتا مورد نظر</param>
        /// <returns></returns>
        public static MessageContract Success(object data)
        {
            return new MessageContract() { IsSuccess = true, Data = data };
        }
        /// <summary>
        /// تایید پیام با متن
        /// </summary>
        /// <param name="message">پیغام مورد نظر</param>
        /// <returns></returns>
        public static MessageContract SuccessWithMessage(string message)
        {
            return new MessageContract() { IsSuccess = true, Message = message };
        }

        /// <summary>
        /// تبدیل متن به پیام
        /// </summary>
        /// <param name="message">پیام خطای مورد نظر</param>
        public static implicit operator MessageContract(string message)
        {
            return new MessageContract() { IsSuccess = false, Message = message };
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
        /// تبدیل یک کلاس به پیام تایید نشده
        /// </summary>
        /// <typeparam name="T">نوع کلاس مورد نظر</typeparam>
        /// <param name="message">پیام خطا</param>
        /// <param name="stackTrace">جزئیات خطا</param>
        /// <returns></returns>
        public static MessageContract<T> Fail<T>(this string message, string stackTrace)
        {
            return new MessageContract<T>() { IsSuccess = false, Message = message, StackTrace = stackTrace };
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
        /// <summary>
        /// تبدیل یک متن و جزئیات خطا به پیام تایید نشده
        /// </summary>
        /// <param name="message">متن خطا</param>
        /// <param name="stackTrace">جزئیات خطا</param>
        /// <returns></returns>
        public static MessageContract Fail(this string message, string stackTrace)
        {
            return new MessageContract() { IsSuccess = false, Message = message, StackTrace = stackTrace };
        }
        
    }
}
