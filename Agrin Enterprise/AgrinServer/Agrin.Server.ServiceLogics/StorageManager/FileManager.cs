using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.DataBase.Models.Relations;
using Agrin.Server.Models;
using SignalGo.Server.DataTypes;
using SignalGo.Server.Models;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.StorageManager
{
    [ServiceContract("FileManager", ServiceType.ServerService)]
    [ServiceContract("FileManager", ServiceType.OneWayService)]
    public class FileManager
    {
        public MessageContract<long> CreateEmptyFile()
        {
            var userId = OperationContext<UserInfo>.CurrentSetting.Id;
            return CreateEmptyFile(userId);
        }

        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.144.179" })]
        public MessageContract<long> CreateEmptyFile(int userId)
        {
            using (var context = new AgrinContext())
            {
                var fileInfo = new DirectFileInfo()
                {
                    DirectFileToUserRelationInfoes = new List<DirectFileToUserRelationInfo>()
                    {
                         new DirectFileToUserRelationInfo()
                         {
                              AccessType = DirectFileFolderAccessType.Creator,
                              UserId = userId,
                         }
                    },
                    ServerId = context.ServerInfoes.FirstOrDefault().Id,
                    IsComplete = false,
                    CreatedDateTime = DateTime.Now
                };
                context.DirectFileInfoes.Add(fileInfo);
                context.SaveChanges();
                return fileInfo.Id.Success();
            }
        }

        [ConcurrentLock(Type = ConcurrentLockType.PerIpAddress)]
        public MessageContract RoamStorageFileComplete(int userId, long fileId, long fileSize)
        {
            using (var context = new AgrinContext())
            {
                int BistMB = 1024 * 1024 * 20;
                var user = context.UserInfoes.FirstOrDefault(x => x.Id == userId);
                if (fileSize > BistMB && user.RoamUploadSize < fileSize)
                {
                    return MessageType.StorageFull;
                }
                var file = context.DirectFileInfoes.FirstOrDefault(x => x.Id == fileId);
                file.IsComplete = true;
                if (fileSize > BistMB)
                {
                    user.RoamUploadSize -= fileSize;
                }
                context.SaveChanges();
                return MessageType.Success;
            }
        }
    }
}
