using HeyRed.Mime;
using SignalGo.Server.Models;
using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.StorageServer.ServiceLogics.StorageManager
{
    [ServiceContract("Download", ServiceType.HttpService)]
    public class LinkDownloadManager
    {
        public ActionResult DownloadFile(long fileId, string password)
        {
            var info = UltraStreamGo.StreamIdentifier.GetFileInfo(fileId, password);
            if (info == null)
            {
                OperationContext.Current.HttpClient.Status = System.Net.HttpStatusCode.NotFound;
                return "File Not found!";
            }

            OperationContext.Current.HttpClient.ResponseHeaders.Add("content-disposition", "attachment; filename=" + info.FileName);
            OperationContext.Current.HttpClient.ResponseHeaders.Add("Accept-Ranges", "bytes");

            OperationContext.Current.HttpClient.ResponseHeaders.Add("Content-Type", MimeTypesMap.GetMimeType(info.Extension));
            OperationContext.Current.HttpClient.ResponseHeaders.Add("Last-Modified", info.LastUpdateDateTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'UTC'"));
            long range = 0;
            if (OperationContext.Current.HttpClient.RequestHeaders.ExistHeader("Range"))
            {
                string[] split = split = OperationContext.Current.HttpClient.RequestHeaders["Range"].Split(new string[] { "bytes" }, StringSplitOptions.RemoveEmptyEntries);



                var data = split.Where(x => !string.IsNullOrWhiteSpace(x)).FirstOrDefault().Split('=').Where(x => !string.IsNullOrWhiteSpace(x)).FirstOrDefault();
                var splitDash = data.Split('-').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                if (splitDash.Length == 2 || splitDash.Length == 1)
                {
                    //ResponseHeaders.Add("Accept-Ranges", "bytes");
                    var fromSize = long.Parse(splitDash[0]);
                    var toSize = splitDash.Length == 1 ? -1 : long.Parse(splitDash[1]);
                    if (toSize == -1)
                        toSize = info.FileSize - 1;
                    OperationContext.Current.HttpClient.ResponseHeaders.Add("Content-Range", "bytes " + fromSize + "-" + toSize + "/" + (toSize + 1));
                    var len = toSize - fromSize;
                    if (len < 0)
                        len = info.FileSize;
                    OperationContext.Current.HttpClient.ResponseHeaders.Add("Content-Length", (len + 1).ToString());
                    range = fromSize;
                    Console.WriteLine("start from range:" + range);
                }
                else
                {
                    Console.WriteLine("range wronge:" + data);
                    throw new Exception("range wronge");
                }

            }
            else
                OperationContext.Current.HttpClient.ResponseHeaders.Add("Content-Length", info.FileSize.ToString());
            var fileStream = UltraStreamGo.StreamIdentifier.GetFileStream(fileId, range);
            return new FileActionResult(fileStream);
        }
    }
}
