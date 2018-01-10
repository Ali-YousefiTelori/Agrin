using Agrin.Server.ServiceModels;
using Agrin.Server.ServiceModels.StorageManager;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.StorageManager
{
    public class PostStorageManager : IPostStorageManager
    {
        public StreamInfo<DateTime> DownloadPostImage(int postUserId, int postId, string fileName)
        {
            var stream = new StreamInfo<DateTime>();
            string filePath = FileManager.GePostImageDirectory(postUserId, postId, fileName);
            if (!File.Exists(filePath))
            {
                stream.Status = System.Net.HttpStatusCode.NotFound;
                return stream;
            }
            FileInfo file = new FileInfo(filePath);
            stream.Stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            stream.Length = file.Length;
            stream.Data = file.LastWriteTime;
            return stream;
        }
    }
}
