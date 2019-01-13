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
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.StorageManager
{
    [ServiceContract("FileManager", ServiceType.ServerService)]
    [ServiceContract("FileManager", ServiceType.OneWayService)]
    public class FileManager
    {
        /// <summary>
        /// create an empty direct file
        /// </summary>
        /// <returns></returns>
        public MessageContract<long> CreateEmptyDirectFile()
        {
            int userId = OperationContext<UserInfo>.CurrentSetting.Id;
            return CreateEmptyDirectFile(userId);
        }

        /// <summary>
        /// create an empty file
        /// </summary>
        /// <returns></returns>
        public Task<MessageContract<long>> CreateEmptyFile()
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
        public MessageContract<long> CreateEmptyDirectFile(int userId)
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
                    Password = Guid.NewGuid(),
                    ServerId = context.Servers.FirstOrDefault().Id,
                    IsComplete = false,
                    CreatedDateTime = DateTime.Now
                };
                context.DirectFiles.Add(fileInfo);
                context.SaveChanges();
                return fileInfo.Id.Success();
            }
        }

        [ClientLimitation(AllowAccessList = new string[] { "", "::1", "localhost", "127.0.0.1", "94.130.214.125" })]
        public async Task<MessageContract<long>> CreateEmptyFile(int userId)
        {
            using (AgrinContext context = new AgrinContext())
            {
                FileInfo fileInfo = new FileInfo()
                {
                    ServerId = context.Servers.FirstOrDefault().Id,
                    CreatedDateTime = DateTime.Now,
                    Password = Guid.NewGuid(),
                    Type = FileType.Data,
                };
                await context.Files.AddAsync(fileInfo);
                await context.SaveChangesAsync();
                return fileInfo.Id;
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
                UserInfo user = context.Users.FirstOrDefault(x => x.Id == userId);
                if (fileSize > BistMB && user.RoamUploadSize < fileSize)
                {
                    return MessageType.StorageFull;
                }
                DirectFileInfo file = context.DirectFiles.FirstOrDefault(x => x.Id == fileId);
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
