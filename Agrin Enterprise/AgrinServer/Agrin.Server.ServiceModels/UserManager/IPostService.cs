using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Agrin.Server.Models.Filters;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceModels.UserManager
{
    [ServiceContract("PostService", InstanceType = InstanceType.SingleInstance)]
    public interface IPostService
    {
        MessageContract<List<PostInfo>> GetListOfPost(int index, int length);

        MessageContract<List<PostCategoryInfo>> FilterPostCategories(FilterBaseInfo filterBaseInfo);

        MessageContract<List<PostCategoryInfo>> FilterVirtualPostCategories(FilterBaseInfo filterBaseInfo);

        MessageContract<List<PostInfo>> FilterPosts(FilterPostInfo filterPostInfo);
    }
}
