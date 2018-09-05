using Agrin.Server.Models;
using SignalGo.Server.Models;
using SignalGo.Shared.Http;
using System;
using System.IO;

namespace Agrin.Server.ServiceLogics.Controllers
{
    /// <summary>
    /// کلاس بیس کنترل های توابع وبی
    /// </summary>
    public class BaseHttpRequestController
    {
        /// <summary>
        /// دریافت یک فایل جهت دانلود اطلاعات از کلاینت
        /// کلاینت فایلش را اپلود میکند
        /// </summary>
        /// <param name="fileInfo">فایل مورد نظر</param>
        /// <param name="errorMessage">خطای بازگشتی</param>
        /// <returns>عملیات با موفقیت انجام شده یا خیر</returns>
        public static bool TakeFile(out HttpPostedFileInfo fileInfo, out MessageType? errorMessage)
        {
            errorMessage = null;
            HttpClientInfo client = OperationContext.Current.HttpClient;
            fileInfo = client.TakeNextFile();
            if (fileInfo == null)
            {
                client.Status = System.Net.HttpStatusCode.Forbidden;
                errorMessage = MessageType.FileNotFound;
                return false;
            }
            else if (fileInfo.ContentLength > 1024 * 1024 * 10)
            {
                client.Status = System.Net.HttpStatusCode.Forbidden;
                errorMessage = MessageType.DataOverFlow;
                return false;
            }
            return true;
        }

        /// <summary>
        /// دریافت اکشن یک خطا و ارسال به کاربر
        /// </summary>
        /// <param name="msg">خطای مورد نظر</param>
        /// <returns>خطای عدم دسترسی</returns>
        public static ActionResult Error(MessageType msg)
        {
            HttpClientInfo client = OperationContext.Current.HttpClient;
            client.Status = System.Net.HttpStatusCode.Forbidden;
            return new ActionResult((MessageContract)msg);
        }

        /// <summary>
        /// دریافت خطای داخلی سرور به وسیله ی یک متن پیام
        /// </summary>
        /// <param name="message">متن خطا</param>
        /// <returns>خطای داخلی سرور</returns>
        public static ActionResult InternalError(string message)
        {
            HttpClientInfo client = OperationContext.Current.HttpClient;
            client.Status = System.Net.HttpStatusCode.InternalServerError;
            return new ActionResult("Internal Error: " + Environment.NewLine + message);
        }

        /// <summary>
        /// حذف فایل
        /// </summary>
        /// <param name="filePath">آدرس فایل</param>
        public static void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {

            }
        }

        /// <summary>
        /// دانلود فایل از کاربر که فایل هایش را اپلود میکند
        /// </summary>
        /// <param name="filePath">آدرس فایل که باید در ان قسمت ذخیره شود</param>
        /// <param name="file">فایلی که کاربر اپلود میکند</param>
        /// <returns>حجم فایل</returns>
        public static long DownloadFile(string filePath, HttpPostedFileInfo file)
        {
            using (FileStream streamWriter = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] bytes = new byte[1024 * 10];
                while (true)
                {
                    int readCount = file.InputStream.Read(bytes, 0, bytes.Length);
                    if (readCount <= 0)
                        break;
                    streamWriter.Write(bytes, 0, readCount);
                }
                return streamWriter.Length;
            }
        }
        /// <summary>
        /// دانلود یک فایل به وسیله ی کاربر
        /// </summary>
        /// <param name="filePath">آدرس فایل</param>
        /// <returns>اکشن فایل یا خطایی که به کاربر  فرستاده میشود</returns>
        public static ActionResult DownloadFile(string filePath)
        {
            HttpClientInfo client = OperationContext.Current.HttpClient;
            if (!System.IO.File.Exists(filePath))
            {
                client.Status = System.Net.HttpStatusCode.NotFound;
                return new ActionResult($"File {Path.GetFileName(filePath)} Not found");
            }
            FileInfo info = new FileInfo(filePath);
            client.ResponseHeaders.Add("content-disposition", new string[] { "attachment; filename=" + info.Name });
            client.ResponseHeaders.Add("Content-Length", new string[] { info.Length.ToString() });
            client.ResponseHeaders.Add("Content-Type", new string[] { MimeTypes.MimeTypeMap.GetMimeType(info.Extension) });
            client.ResponseHeaders.Add("Last-Modified", info.LastWriteTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'UTC'").Split(','));
            return new FileActionResult(filePath);
        }
    }
}
