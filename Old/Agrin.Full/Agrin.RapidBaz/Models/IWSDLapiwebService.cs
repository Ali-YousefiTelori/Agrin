using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Models
{
    public interface IWSDLapiwebService
    {
        string Queue(string session, string url);
        string MultiQueue(string session, string url);
        string Status(string session, string queueID);
        string Get(string session, string max, string length);
        string QueueGet(string session, string max, string length);
        string Free(string session, string id);
        string QueueRemove(string session, string queueID);
        string FolderList(string session);
        string FolderGet(string session, string folder);
        string FolderMake(string session, string name);
        string FolderRemove(string session, string folderId);
        string FolderReset(string session, string folderId);
        string Repair(string session, string id);
        string SetFolder(string session, string folderID, string fileID);
    }
}
