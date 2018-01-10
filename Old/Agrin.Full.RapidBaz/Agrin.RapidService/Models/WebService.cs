using Agrin.RapidBaz.Models;
using Agrin.RapidService.RapidBazWebService;
using System.Collections.Generic;

namespace Agrin.RapidService.Models
{
    public class WebService : IWSDLapiwebService
    {
        WSDLapiwebService service = null;
        public WebService()
        {
            service = new WSDLapiwebService();
        }

        public string Queue(string session, string url)
        {
            return service.Queue(session, url);
        }

        public string MultiQueue(string session, string url)
        {
            return service.MultiQueue(session, url);
        }

        public string Status(string session, string queueID)
        {
            return service.Status(session, queueID);
        }

        public string Get(string session, string max, string length)
        {
            return service.Get(session, max, length, "");
        }

        public string QueueGet(string session, string max, string length)
        {
            return service.QueueGet(session, max, length);
        }

        public string Free(string session, string id)
        {
            return service.Free(session, id);
        }

        public string QueueRemove(string session, string queueID)
        {
            return service.QueueRemove(session, queueID);
        }

        public string FolderList(string session)
        {
            return service.FolderList(session, int.MaxValue.ToString(), "0");
        }

        public string FolderGet(string session, string folder)
        {
            return service.FolderGet(session, folder, int.MaxValue.ToString(), "0");
        }

        public string FolderMake(string session, string name)
        {
            return service.FolderMake(session, name, int.MaxValue.ToString(), "0");
        }

        public string FolderRemove(string session, string folderId)
        {
            return service.FolderRemove(session, folderId);
        }

        public string FolderReset(string session, string folderId)
        {
            return service.FolderReset(session, folderId);
        }

        public string Repair(string session, string id)
        {
            return service.Repair(session, id);
        }

        public string SetFolder(string session, string folderID, string fileID)
        {
            return service.SetFolder(session, folderID, fileID);
        }
    }
}
