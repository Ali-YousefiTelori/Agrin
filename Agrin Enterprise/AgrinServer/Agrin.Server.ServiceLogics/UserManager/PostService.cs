using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Agrin.Server.DataBaseLogic;
using Agrin.Server.Models.Filters;
using SignalGo.Shared.DataTypes;

namespace Agrin.Server.ServiceLogics
{
    [ServiceContract("PostService", ServiceType.ServerService, InstanceType = InstanceType.SingleInstance)]
    public class PostService
    {
        public MessageContract<List<PostInfo>> GetListOfPost(int index, int length)
        {
            return PostExtension.GetListOfPost(index, length);
        }

        public MessageContract<List<PostCategoryInfo>> FilterPostCategories(FilterBaseInfo filterBaseInfo)
        {
            return PostExtension.FilterPostCategories(filterBaseInfo);
        }

        public MessageContract<List<PostCategoryInfo>> FilterVirtualPostCategories(FilterBaseInfo filterBaseInfo)
        {
            return PostExtension.FilterVirtualPostCategories(filterBaseInfo);
        }

        public MessageContract<List<PostInfo>> FilterPosts(FilterPostInfo filterPostInfo)
        {
            return PostExtension.FilterPosts(filterPostInfo);
        }
    }
}
