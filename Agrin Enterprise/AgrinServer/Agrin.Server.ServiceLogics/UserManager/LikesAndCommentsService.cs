using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Agrin.Server.Models.Filters;
using Framesoft.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agrin.Server.ServiceLogics.UserManager
{
    [ServiceContract("LikesAndComments", ServiceType.ServerService, InstanceType = InstanceType.SingleInstance)]
    public class LikesAndCommentsService
    {
        /// <summary>
        /// like or unlike a RequestIdeaInfo
        /// </summary>
        /// <param name="isLike"></param>
        /// <param name="requestIdeaId"></param>
        /// <returns></returns>
        [AgrinSecurityPermission(IsNormalUser = true)]
        public MessageContract LikeAndUnlikeRequestIdeaInfo(bool isLike, int requestIdeaId)
        {
            using (AgrinContext context = new AgrinContext())
            {
                if (isLike)
                {
                    if (!context.LikeInfoes.Any(x => x.UserId == CurrentUserInfo.UserId && x.RequestIdeaId == requestIdeaId))
                    {
                        context.LikeInfoes.Add(new LikeInfo()
                        {
                            RequestIdeaId = requestIdeaId,
                            UserId = CurrentUserInfo.UserId
                        });
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (context.LikeInfoes.Any(x => x.UserId == CurrentUserInfo.UserId && x.RequestIdeaId == requestIdeaId))
                    {
                        context.LikeInfoes.RemoveRange(from x in context.LikeInfoes where x.UserId == CurrentUserInfo.UserId && x.RequestIdeaId == requestIdeaId select x);
                        context.SaveChanges();
                    }
                }
                return MessageType.Success;
            }
        }

        /// <summary>
        /// add comment
        /// </summary>
        /// <param name="commentInfo"></param>
        /// <returns></returns>
        [AgrinSecurityPermission(IsNormalUser = true)]
        public MessageContract AddCommentInfo(CommentInfo commentInfo)
        {
            using (AgrinContext context = new AgrinContext())
            {
                commentInfo.UserId = CurrentUserInfo.UserId;
                commentInfo.CreatedDateTime = DateTime.Now;
                context.CommentInfoes.Add(commentInfo);
                context.SaveChanges();
                return MessageType.Success;
            }
        }

        /// <summary>
        /// filter comments of request idea info
        /// </summary>
        /// <param name="filterInfo"></param>
        /// <returns></returns>
        public MessageContract<List<CommentInfo>> FilterRequestIdeaCommentInfoes(FilterIdInfo filterInfo)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                IQueryable<CommentInfo> query = context.CommentInfoes.Where(x => x.RequestIdeaId == filterInfo.Id).AsNoTracking().AsQueryable();

                return query.SelectPage(filterInfo.Index, filterInfo.Length).Success();
            }
        }
    }
}
