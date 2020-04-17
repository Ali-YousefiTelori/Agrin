using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Agrin.Server.Models.Filters;
using Framesoft.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBaseLogic
{
    public static class PostExtension
    {
        public static MessageContract<List<PostInfo>> GetListOfPost(int index, int length)
        {
            if (index < 0 || length <= 0)
                return MessageType.WrongData;

            using (AgrinContext context = new AgrinContext(false))
            {
                return context.Posts.AsNoTracking().SelectPage(x => x.CreatedDateTime, false, index, length).ToList();
            }
        }

        public static MessageContract<List<PostInfo>> FilterPosts(FilterPostInfo filterPostInfo)
        {
            if (filterPostInfo.Index < 0 || filterPostInfo.Length <= 0)
                return MessageType.WrongData;

            using (AgrinContext context = new AgrinContext(false))
            {
                var query = context.Posts.AsNoTracking().Include(x => x.FileInfoes).Include(x => x.PostMusicInfo)
                    .Include(x => x.PostVideoInfo).Include(x => x.PostTagRelationInfoes).AsQueryable();
                if (filterPostInfo.CategoryId != null)
                {
                    if (filterPostInfo.CategoryId == -1)
                    {
                        query = query.OrderByDescending(p => p.CreatedDateTime);
                    }
                    else if (filterPostInfo.CategoryId == -2)
                    {
                        query = query.OrderByDescending(p => p.LastUpdateFileVersionDateTime);
                    }
                    else if (filterPostInfo.CategoryId == -3)
                    {
                        query = query.OrderByDescending(p => p.ViewCount);
                    }
                    else if (filterPostInfo.CategoryId == -4)
                    {
                        query = query.OrderByDescending(p => p.ViewCount);//sort by likes
                    }
                    else
                    {
                        query = query.Where(p => p.CategoryId == filterPostInfo.CategoryId);//sort by likes
                    }

                }
                if (filterPostInfo.StartDateTime.HasValue && filterPostInfo.EndDateTime.HasValue)
                    query = query.Where(x => x.CreatedDateTime <= filterPostInfo.EndDateTime.Value && x.CreatedDateTime >= filterPostInfo.StartDateTime.Value);
                if (query.IsOrdered())
                    return query.SelectPage(filterPostInfo.Index, filterPostInfo.Length).ToList();
                else
                    return query.SelectPage(x => x.CreatedDateTime, false, filterPostInfo.Index, filterPostInfo.Length).ToList();
            }
        }

        public static MessageContract<List<PostCategoryInfo>> FilterPostCategories(FilterBaseInfo filterBaseInfo)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                return context.PostCategories.AsNoTracking().SelectPage(filterBaseInfo.Index, filterBaseInfo.Length).ToList();
            }
        }

        public static MessageContract<List<PostCategoryInfo>> FilterVirtualPostCategories(FilterBaseInfo filterBaseInfo)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                List<PostCategoryInfo> result = new List<PostCategoryInfo>();
                //کاربران همیشه باید تازه ها رو اول ببینند نه چیزهای تکراری
                result.Add(new PostCategoryInfo()
                {
                    Id = -1,
                    //Title = "تازه ها",
                    Posts = context.Posts.Include(x => x.FileInfoes).Include(x => x.PostMusicInfo).Include(x => x.PostVideoInfo).Include(x => x.PostTagRelationInfoes).OrderByDescending(p => p.CreatedDateTime).Take(5).ToList()
                });
                result.Add(new PostCategoryInfo()
                {
                    Id = -2,
                    //Title = "جدیدترین بروزرسانی ها",
                    Posts = context.Posts.Include(x => x.FileInfoes).Include(x => x.PostMusicInfo).Include(x => x.PostVideoInfo).Include(x => x.PostTagRelationInfoes).OrderByDescending(p => p.LastUpdateFileVersionDateTime).Take(5).ToList()
                });
                result.Add(new PostCategoryInfo()
                {
                    Id = -3,
                    //Title = "پربازدیدترین ها",
                    Posts = context.Posts.Include(x => x.FileInfoes).Include(x => x.PostMusicInfo).Include(x => x.PostVideoInfo).Include(x => x.PostTagRelationInfoes).OrderByDescending(p => p.ViewCount).Take(5).ToList()
                });
                result.Add(new PostCategoryInfo()
                {
                    Id = -4,
                    //Title = "برترین ها",
                    Posts = context.Posts.Include(x => x.FileInfoes).Include(x => x.PostMusicInfo).Include(x => x.PostVideoInfo).Include(x => x.PostTagRelationInfoes).OrderByDescending(p => p.ViewCount).Take(5).ToList()//sort by likes
                });
                return result;
            }
        }
    }
}
