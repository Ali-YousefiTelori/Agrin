using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceModels.StorageManager
{
    [ServiceContract("PostStorageManager")]
    public interface IPostStorageManager
    {
        StreamInfo<DateTime> DownloadPostImage(int postUserId, int postId, string fileName);
    }
}
