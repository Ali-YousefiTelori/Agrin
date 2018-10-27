using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.DataBase.Models.Relations;
using Agrin.Server.Models;
using Agrin.Server.ServiceLogics.Controllers;
using SignalGo.Server.DataTypes;
using SignalGo.Server.Models;
using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Agrin.Server.ServiceLogics.StorageManager
{
    [ServiceContract("FileManager", ServiceType.ServerService)]
    [ServiceContract("FileManager", ServiceType.OneWayService)]
    public class FileManager
    {
        public MessageContract<long> CreateEmptyFile()
        {
            int userId = OperationContext<UserInfo>.CurrentSetting.Id;
            return CreateEmptyFile(userId);
        }

        /// <summary>
        /// create an empty file to upload later
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public MessageContract<long> CreateEmptyFile(int userId)
        {
            using (AgrinContext context = new AgrinContext())
            {
                DirectFileInfo fileInfo = new DirectFileInfo()
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

        /// <summary>
        /// file complete uplodaed
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        [ConcurrentLock(Type = ConcurrentLockType.PerIpAddress)]
        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public MessageContract RoamStorageFileComplete(int userId, long fileId, long fileSize)
        {
            using (AgrinContext context = new AgrinContext())
            {
                //20 mb
                int BistMB = 1024 * 1024 * 20;
                UserInfo user = context.UserInfoes.FirstOrDefault(x => x.Id == userId);
                if (fileSize > BistMB && user.RoamUploadSize < fileSize)
                {
                    return MessageType.StorageFull;
                }
                DirectFileInfo file = context.DirectFileInfoes.FirstOrDefault(x => x.Id == fileId);
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
