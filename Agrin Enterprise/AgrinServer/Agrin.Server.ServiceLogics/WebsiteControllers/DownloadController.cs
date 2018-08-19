using Agrin.Server.ServiceLogics.Controllers;
using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.WebsiteControllers
{
    [ServiceContract("Download", ServiceType.HttpService, InstanceType.SingleInstance)]
    public class DownloadController : BaseHttpRequestController
    {
        static string defaultPath = "E:\\AgrinDataBaseFiles";
        public ActionResult AgrinDownloadManagerProLastVersion()
        {
            try
            {
                var directory = Path.Combine(defaultPath, "AgrinApplication", "Android");
                var filePath = Directory.GetFiles(directory).OrderByDescending(x => x).FirstOrDefault();
                return DownloadFile(filePath);
            }
            catch (Exception ex)
            {
                return InternalError("error to download file");
            }
        }

        public ActionResult AgrinDownloadManagerWindowsLastVersion()
        {
            try
            {
                var directory = Path.Combine(defaultPath, "AgrinApplication", "Windows");
                var filePath = Directory.GetFiles(directory).OrderByDescending(x => x).FirstOrDefault();
                return DownloadFile(filePath);
            }
            catch (Exception ex)
            {
                return InternalError("error to download file");
            }
        }

        public ActionResult DownloadCustomFile(string file)
        {
            try
            {
                var directory = Path.Combine(defaultPath, "AgrinApplication", "CustomFiles");
                var filePath = Directory.GetFiles(directory).OrderByDescending(x => x).FirstOrDefault();
                return DownloadFile(filePath);
            }
            catch (Exception ex)
            {
                return InternalError("error to download file");
            }
        }
    }
}
