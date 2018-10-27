using Agrin.Server.Models;
using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.Streams
{
    [ServiceContract("FileManager", ServiceType.StreamService)]
    public class FileStreamManager
    {
        /// <summary>
        /// download image of file extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public StreamInfo<DateTime> DownloadIconByExtension(string extension)
        {
            string directory = Path.Combine(AgrinConfigInformation.Current.FileStoragePath, "AgrinApplication", "IconExtensions");
            extension = extension.TrimStart('.');
            string filePath = Path.Combine(directory, extension + ".png");
            var stream = new StreamInfo<DateTime>();
            if (!File.Exists(filePath))
            {
                string notfoundExtensions = Path.Combine(directory, "notfoundExtensions.txt");
                List<string> extentions = null;
                if (!File.Exists(notfoundExtensions))
                {
                    extentions = new List<string>();
                }
                else
                {
                    extentions = File.ReadAllLines(notfoundExtensions).ToList();
                }
                extension = extension.ToLower();

                if (!extentions.Contains(extension))
                {
                    extentions.Add(extension);
                    File.WriteAllLines(notfoundExtensions, extentions.ToArray());
                }
                stream.Status = System.Net.HttpStatusCode.NotFound;
                return stream;
            }
            var file = new System.IO.FileInfo(filePath);
            stream.Stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            //length is important
            stream.Length = file.Length;
            //your result data
            stream.Data = file.LastWriteTime;
            return stream;
        }
    }
}
