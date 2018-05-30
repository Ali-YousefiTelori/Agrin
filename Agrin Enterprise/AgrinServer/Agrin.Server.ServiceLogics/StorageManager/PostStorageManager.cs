using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.StorageManager
{
    [ServiceContract("PostStorageManager", ServiceType.StreamService)]
    public class PostStorageManager
    {
        public StreamInfo<DateTime> DownloadPostImage(int postUserId, int postId, string fileName)
        {
            var stream = new StreamInfo<DateTime>();
            string filePath = "";// FileManager.GetPostImageDirectory(postUserId, postId, fileName);
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
