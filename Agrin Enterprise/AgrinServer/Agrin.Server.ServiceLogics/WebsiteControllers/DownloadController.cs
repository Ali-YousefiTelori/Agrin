using Agrin.Server.Models;
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
    public class DownloadController
    {
        public ActionResult AgrinDownloadManagerProLastVersion()
        {
            try
            {
                var directory = Path.Combine(AgrinConfigInformation.Current.FileStoragePath, "AgrinApplication", "Android");
                var filePath = Directory.GetFiles(directory).OrderByDescending(x => x).FirstOrDefault();
                return BaseHttpRequestController.DownloadFile(filePath);
            }
            catch (Exception ex)
            {
                return BaseHttpRequestController.InternalError("error to download file");
            }
        }

        public ActionResult AgrinDownloadManagerWindowsLastVersion()
        {
            try
            {
                var directory = Path.Combine(AgrinConfigInformation.Current.FileStoragePath, "AgrinApplication", "Windows");
                var filePath = Directory.GetFiles(directory).OrderByDescending(x => x).FirstOrDefault();
                return BaseHttpRequestController.DownloadFile(filePath);
            }
            catch (Exception ex)
            {
                return BaseHttpRequestController.InternalError("error to download file");
            }
        }

        public ActionResult DownloadCustomFile(string fileName)
        {
            try
            {
                var directory = Path.Combine(AgrinConfigInformation.Current.FileStoragePath, "AgrinApplication", "CustomFiles");
                var filePath = Path.Combine(directory, fileName);
                return BaseHttpRequestController.DownloadFile(filePath);
            }
            catch (Exception ex)
            {
                return BaseHttpRequestController.InternalError("error to download file");
            }
        }
    }
}
