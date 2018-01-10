using Agrin.Server.Models;
using SignalGo.Shared.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.Controllers
{
    /// <summary>
    /// کلاس بیس کنترل های توابع وبی
    /// </summary>
    public class BaseHttpRequestController : HttpRequestController
    {
        /// <summary>
        /// دریافت یک فایل جهت دانلود اطلاعات از کلاینت
        /// کلاینت فایلش را اپلود میکند
        /// </summary>
        /// <param name="fileInfo">فایل مورد نظر</param>
        /// <param name="errorMessage">خطای بازگشتی</param>
        /// <returns>عملیات با موفقیت انجام شده یا خیر</returns>
        public bool TakeFile(out HttpPostedFileInfo fileInfo, out ErrorMessage? errorMessage)
        {
            errorMessage = null;
            fileInfo = TakeNextFile();
            if (fileInfo == null)
            {
                Status = System.Net.HttpStatusCode.Forbidden;
                errorMessage = ErrorMessage.FileNotFound;
                return false;
            }
            else if (fileInfo.ContentLength > 1024 * 1024 * 10)
            {
                Status = System.Net.HttpStatusCode.Forbidden;
                errorMessage = ErrorMessage.DataOverFlow;
                return false;
            }
            return true;
        }

        /// <summary>
        /// دریافت اکشن یک خطا و ارسال به کاربر
        /// </summary>
        /// <param name="msg">خطای مورد نظر</param>
        /// <returns>خطای عدم دسترسی</returns>
        public ActionResult Error(ErrorMessage msg)
        {
            Status = System.Net.HttpStatusCode.Forbidden;
            return Content((MessageContract)msg);
        }

        /// <summary>
        /// دریافت خطای داخلی سرور به وسیله ی یک متن پیام
        /// </summary>
        /// <param name="message">متن خطا</param>
        /// <returns>خطای داخلی سرور</returns>
        public ActionResult InternalError(string message)
        {
            Status = System.Net.HttpStatusCode.InternalServerError;
            return Content("Internal Error: " + Environment.NewLine + message);
        }

        /// <summary>
        /// بررسی دسترسی توکن کاربر
        /// </summary>
        /// <param name="token">توکن مورد نظر</param>
        /// <param name="Ip">آی پی کاربر مورد نظر</param>
        /// <param name="userId">آی دی کاربر بر اساس توکن</param>
        /// <param name="result">خروجی برای نمایش به کاربر</param>
        /// <returns>دسترسی دارد یا خیر</returns>
        public bool CheckAccessToken(Guid token, string Ip, out int userId, out ActionResult result)
        {
            result = null;
            if (!Authentication.AuthenticationService.RegisteredWebTokensOfUser.TryGetValue(token, out (int UserId, string Ip) value))
            {
                userId = 0;
                result = Content((MessageContract)ErrorMessage.SessionAccessDenied);
                return false;
            }
            else if (value.Ip != Ip)
            {
                Console.WriteLine($"Last Ip : {value.Ip} Previos IP: {Ip}");
                userId = 0;
                result = Content((MessageContract)ErrorMessage.ClientIpIsNotValid);
                return false;
            }
            userId = value.UserId;
            return true;
        }

        /// <summary>
        /// حذف فایل
        /// </summary>
        /// <param name="filePath">آدرس فایل</param>
        public void DeleteFile(string filePath)
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
        public long DownloadFile(string filePath, HttpPostedFileInfo file)
        {
            using (var streamWriter = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var bytes = new byte[1024 * 10];
                while (true)
                {
                    var readCount = file.InputStream.Read(bytes, 0, bytes.Length);
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
        public ActionResult DownloadFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Status = System.Net.HttpStatusCode.NotFound;
                return Content($"File {Path.GetFileName(filePath)} Not found");
            }
            var info = new FileInfo(filePath);
            ResponseHeaders.Add("content-disposition", "attachment; filename=" + info.Name);
            ResponseHeaders.Add("Content-Length", info.Length.ToString());
            ResponseHeaders.Add("Content-Type", MimeTypes.MimeTypeMap.GetMimeType(info.Extension));
            ResponseHeaders.Add("Last-Modified", info.LastWriteTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'UTC'"));
            return new FileActionResult(filePath);
        }
    }
}
