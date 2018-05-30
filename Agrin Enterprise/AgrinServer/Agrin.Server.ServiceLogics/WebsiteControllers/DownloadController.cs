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
        public ActionResult AgrinDownloadManagerProLastVersion()
        {
            try
            {
                var directory = "";// FileManager.GetAgrinApplicationDirectory("Android");
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
                var directory = "";// FileManager.GetAgrinApplicationDirectory("Windows");
                var filePath = Directory.GetFiles(directory).OrderByDescending(x => x).FirstOrDefault();
                return DownloadFile(filePath);
            }
            catch (Exception ex)
            {
                return InternalError("error to download file");
            }
        }
		
		public ActionResult DownloadFile(string file)
        {
            try
            {
                var directory = "";// FileManager.GetAgrinApplicationDirectory("CustomFiles");
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
