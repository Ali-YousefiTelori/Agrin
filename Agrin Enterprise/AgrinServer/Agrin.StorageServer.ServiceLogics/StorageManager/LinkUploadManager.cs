using Agrin.Server.Models;
using AgrinMainServer.OneWayServices;
using HeyRed.Mime;
using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Log;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UltraStreamGo;

namespace Agrin.StorageServer.ServiceLogics.StorageManager
{
    [ServiceContract("LinkUploadManager", ServiceType.StreamService)]
    public class LinkUploadManager
    {
        public async Task<MessageContract> UploadFile(Guid firstKey, Guid secondKey, StreamInfo<UltraStreamGo.FileInfo> streamInfo)
        {
            MessageContract checkAccessToFile = StorageAuthenticationService.Current.CheckAccessUserToFileUpload(firstKey, secondKey, streamInfo.Data.Id);
            if (checkAccessToFile.IsSuccess)
            {
                using (StreamIdentifier streamIdentifier = new StreamIdentifier())
                {
                    Stream stream = streamInfo.Stream;
                    StreamIdentifierFileUploadResult result = await streamIdentifier.StartUpload(streamInfo.Data, stream, 0, streamInfo.Length, async (position) =>
                    {
                        await streamInfo.SetPositionFlush(position);
                    });
                    if (result != StreamIdentifierFileUploadResult.Success)
                    {
                        return MessageType.ServerException;
                    }
                }

                return MessageType.Success;
            }
            return checkAccessToFile;
        }

        public static List<int> UserFileUploading = new List<int>();
        private static readonly object listLock = new object();
        public static bool TryAddUserFileDownloading(int userId)
        {
            lock (listLock)
            {
                if (UserFileUploading.Contains(userId))
                    return false;
                UserFileUploading.Add(userId);
                return true;
            }
        }

        public static void TryRemoveUserFileDownloading(int userId)
        {
            lock (listLock)
            {
                UserFileUploading.Remove(userId);
            }
        }

        public static async Task<MessageContract> UploadStream(int userId, Stream stream, string uri, string filePath, long fileSize, string filePassword, Action<byte> inProgressAction, Action<bool, UltraStreamGo.FileInfo> completeAction)
        {
            try
            {
                MessageContract<long> createdFile = FileManager.Current.CreateEmptyFile(userId);
                if (!createdFile.IsSuccess)
                {
                    completeAction(false, null);
                    return MessageType.ServerException;
                }
                using (StreamIdentifier streamIdentifier = new StreamIdentifier())
                {
                    UltraStreamGo.FileInfo fileInfo = new UltraStreamGo.FileInfo()
                    {
                        CreatedDateTime = DateTime.Now,
                        DataType = MimeTypesMap.GetMimeType(Path.GetExtension(filePath)),
                        FileName = Path.GetFileName(filePath),
                        FileSize = fileSize,
                        Id = createdFile.Data,
                        Password = filePassword

                    };
                    StreamIdentifierFileUploadResult result = await streamIdentifier.StartUpload(fileInfo, stream, 0, fileSize, (position) =>
                    {
                        inProgressAction.Invoke((byte)(100 / ((double)fileSize / position)));
                    }, true);
                    if (result == StreamIdentifierFileUploadResult.Success)
                    {
                        FileManager.Current.RoamStorageFileComplete(userId, fileInfo.Id, fileSize);

                        completeAction(true, fileInfo);
                    }
                    else
                    {
                        AutoLogger.Default.LogText("upload false " + result.ToString());
                        completeAction(false, fileInfo);
                    }
                }
            }
            finally
            {
                lock (listLock)
                {
                    UserFileUploading.Remove(userId);
                }
            }
            return MessageType.None;
        }

    }
}
