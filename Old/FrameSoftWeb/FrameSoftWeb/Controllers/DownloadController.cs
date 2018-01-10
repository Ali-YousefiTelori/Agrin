using Agrin.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FrameSoftWeb.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/
        public DownloadResult DownloadOneFile(string fileName)
        {
            return new DownloadResult(this, "~/Files/Agrin/" + fileName);
        }

        public DownloadResult AgrinDownloadManagerLastVersion()
        {
            return new DownloadResult(this, "~/Files/Agrin/Agrin Download Manager.apk");
        }

        public DownloadResult AgrinDownloadManagerProLastVersion()
        {
            return new DownloadResult(this, "~/Files/Agrin/Agrin Download Manager Android Pro.zip");
        }

        public DownloadResult AgrinDownloadManagerWindowsLastVersion()
        {
            return new DownloadResult(this, "~/Files/Agrin/Agrin Download Manager (WPF).zip");
        }


        public DownloadEndOfOneFileStreamResult DownloadEndOfOneFileStreamIfCannotRead()
        {
            //try
            //{
            //    string address = this.HttpContext.Request.Headers["address"];
            //    long range = long.Parse(this.HttpContext.Request.Headers["RNG"]);
            //    long size = long.Parse(this.HttpContext.Request.Headers["Size"]);

            //    var _request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(address);
            //    return Content(address);                
            //}
            //catch(Exception ex)
            //{
            //    return Content(ex.Message);
            //}
            return new DownloadEndOfOneFileStreamResult();
        }
        
    }

    public class DownloadResult : ActionResult
    {
        public int ErrorCode { get; set; }
        public DownloadResult(Controller httpContext, string fileName, int errorCode = 0)
        {
            ErrorCode = errorCode;
            if (File.Exists(fileName))
                FileDownloadName = fileName;
            else
                FileDownloadName = httpContext.HttpContext.Server.MapPath(fileName);
        }

        public string FileDownloadName
        {
            get;
            set;
        }

        static object lockOBJ = new object();
        static int ConnectionsCount = 0;
        static int DownloadingCount()
        {
            lock (lockOBJ)
            {
                return ConnectionsCount;
            }
        }

        static void SumConnectionCount()
        {
            lock (lockOBJ)
            {
                ConnectionsCount++;
            }
        }

        static void SubConnectionCount()
        {
            lock (lockOBJ)
            {
                ConnectionsCount--;
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            try
            {

                var downloadCount = DownloadingCount();
                var sleep = downloadCount;
                if (downloadCount < 50)
                    sleep = 20;
                else if (downloadCount < 100)
                    sleep = 40;
                else if (downloadCount < 150)
                    sleep = 60;
                else if (downloadCount < 300)
                    sleep = 80;
                else
                    sleep = 100;
                switch (context.HttpContext.Request.HttpMethod.ToUpper())
                { //support Get and head method
                    case "GET":
                    case "HEAD":
                        break;
                    default:
                        context.HttpContext.Response.StatusCode = 501;
                        return;
                }
                if (ErrorCode != 0)
                {
                    context.HttpContext.Response.StatusCode = ErrorCode;
                    return;
                }

                if (!System.IO.File.Exists(FileDownloadName))
                {
                    context.HttpContext.Response.StatusCode = 404;
                    return;
                }


                long startBytes = 0;
                int packSize = 1024 * 10; //read in block，every block 10K bytes
                string fileName = Path.GetFileName(FileDownloadName);
                FileStream myFile = new FileStream(FileDownloadName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                long fileLength = myFile.Length;


                //int sleep = (int)Math.Ceiling(1000.0 * packSize / speed);//the number of millisecond
                string lastUpdateTiemStr = System.IO.File.GetLastWriteTimeUtc(FileDownloadName).ToString("r");
                string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + lastUpdateTiemStr;

                //validate whether the file is too large
                if (myFile.Length > Int32.MaxValue)
                {
                    context.HttpContext.Response.StatusCode = 413;
                    return;
                }

                if (context.HttpContext.Request.Headers["If-Range"] != null)
                {

                    if (context.HttpContext.Request.Headers["If-Range"].Replace("\"", "") != eTag)
                    {
                        context.HttpContext.Response.StatusCode = 412;
                        return;
                    }
                }

                try
                {
                    SumConnectionCount();
                    context.HttpContext.Response.Clear();
                    context.HttpContext.Response.Buffer = true;
                    context.HttpContext.Response.AddHeader("Accept-Ranges", "bytes");
                    context.HttpContext.Response.AppendHeader("ETag", "\"" + eTag + "\"");
                    context.HttpContext.Response.AppendHeader("Last-Modified", lastUpdateTiemStr);
                    context.HttpContext.Response.ContentType = "application/octet-stream";
                    context.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" +

                    HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace("+", "%20"));
                    context.HttpContext.Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    context.HttpContext.Response.AddHeader("Connection", "Keep-Alive");
                    context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
                    if (context.HttpContext.Request.Headers["Range"] != null)
                    {
                        context.HttpContext.Response.StatusCode = 206;
                        string[] range = context.HttpContext.Request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                        if (startBytes < 0 || startBytes >= fileLength)
                        {
                            return;
                        }
                    }
                    if (startBytes > 0)
                    {
                        context.HttpContext.Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }

                    //send data
                    myFile.Seek(startBytes, SeekOrigin.Begin);
                    long downloadLenght = myFile.Length - startBytes;

                    long downloaded = 0;
                    //send data
                    //int maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize);//download in block
                    while (context.HttpContext.Response.IsClientConnected)
                    {
                        int count = 1024 * 1024 * 2;
                        if (count > fileLength)
                            count = (int)fileLength;
                        byte[] bytes = new byte[count];

                        int readCount = myFile.Read(bytes, 0, count);
                        if (readCount == 0)
                            break;
                        downloaded += readCount;
                        context.HttpContext.Response.BinaryWrite(bytes.ToList().GetRange(0, readCount).ToArray());
                        context.HttpContext.Response.Flush();
                        if (downloaded >= downloadLenght)
                            break;
                        if (sleep > 1)
                            Thread.Sleep(sleep);
                    }
                    //int maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize);//download in block
                    //for (int i = 0; i < maxCount && context.HttpContext.Response.IsClientConnected; i++)
                    //{
                    //    context.HttpContext.Response.BinaryWrite(br.ReadBytes(packSize));
                    //    context.HttpContext.Response.Flush();

                    //}
                }
                catch
                {
                }
                finally
                {
                    SubConnectionCount();
                    myFile.Close();
                }
            }
            catch
            {
            }
            return;
        }
    }

    public class DownloadEndOfOneFileStreamResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            try
            {
                switch (context.HttpContext.Request.HttpMethod.ToUpper())
                { //support Get and head method
                    case "GET":
                    case "HEAD":
                        break;
                    default:
                        context.HttpContext.Response.StatusCode = 501;
                        return;
                }


                string address = context.HttpContext.Request.Headers["address"];
                long range = long.Parse(context.HttpContext.Request.Headers["RNG"]);
                long size = long.Parse(context.HttpContext.Request.Headers["Size"]);


                var _request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(address);

                _request.Timeout = 60000;
                _request.AllowAutoRedirect = true;
                _request.ServicePoint.ConnectionLimit = int.MaxValue;

                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                context.HttpContext.Response.ContentType = "application/octet-stream";
                _request.AddRange(range);

                var _response = _request.GetResponse();

                Stream myFile = _response.GetResponseStream();

                //int sleep = (int)Math.Ceiling(1000.0 * packSize / speed);//the number of millisecond
                //string lastUpdateTiemStr = System.IO.File.GetLastWriteTimeUtc(FileDownloadName).ToString("r");
                //string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + lastUpdateTiemStr;

                //validate whether the file is too large
                if (_response.ContentLength <= 0)
                {
                    context.HttpContext.Response.StatusCode = 404;
                    return;
                }

                long fileLength = size - range;

                try
                {
                    context.HttpContext.Response.Clear();
                    context.HttpContext.Response.Buffer = false;
                    context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
                    long downloaded = 0;
                    //send data
                    //int maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize);//download in block
                    while (context.HttpContext.Response.IsClientConnected)
                    {
                        int count = 1024 * 1024 * 2;
                        if (count > fileLength)
                            count = (int)fileLength;
                        byte[] bytes = new byte[count];
                        int readCount = myFile.Read(bytes, 0, count);
                        if (readCount == 0)
                            break;
                        downloaded += readCount;

                        context.HttpContext.Response.BinaryWrite(bytes.ToList().GetRange(0, readCount).ToArray());
                        context.HttpContext.Response.Flush();

                        if (downloaded >= fileLength)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (_response.ContentLength <= 0)
                    {
                        context.HttpContext.Response.StatusCode = 503;
                        context.HttpContext.Response.AddHeader("ex1", ex.Message);
                        return;
                    }
                }
                finally
                {
                    //br.Close();
                    myFile.Close();
                }
            }
            catch (Exception ex)
            {
                context.HttpContext.Response.StatusCode = 503;
                context.HttpContext.Response.AddHeader("ex2", ex.Message);
                return;
            }
        }
    }
}
