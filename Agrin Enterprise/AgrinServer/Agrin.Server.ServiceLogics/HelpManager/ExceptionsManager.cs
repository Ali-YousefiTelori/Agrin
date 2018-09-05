using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Agrin.Server.Models.Filters;
using Microsoft.EntityFrameworkCore;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agrin.Server.ServiceLogics.HelpManager
{
    [ServiceContract("ExceptionsAndIdea", ServiceType.ServerService, InstanceType = InstanceType.SingleInstance)]
    public class ExceptionsManager
    {
        public MessageContract<ExceptionInfo> GetExceptionInformationByCode(int errorCode)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                IQueryable<ExceptionInfo> query = context.ExceptionInfoes.AsNoTracking().AsQueryable();
                ExceptionInfo find = query.FirstOrDefault(x => x.ErrorCode == errorCode);
                if (find == null)
                    return MessageType.NotFound;
                return find.Success();
            }
        }

        public MessageContract<ExceptionInfo> GetExceptionInformationByHttpCode(int errorCode)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                IQueryable<ExceptionInfo> query = context.ExceptionInfoes.AsNoTracking().AsQueryable();
                ExceptionInfo find = query.FirstOrDefault(x => x.HttpErrorCode == errorCode);
                if (find == null)
                    return MessageType.NotFound;
                return find.Success();
            }
        }

        [AgrinSecurityPermission(IsNormalUser = true)]
        public MessageContract AddRequestIdeaInfo(RequestIdeaInfo requestExceptionInfo)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                if (requestExceptionInfo.HttpErrorCode.HasValue && context.RequestIdeaInfoes.Any(x => x.HttpErrorCode == requestExceptionInfo.HttpErrorCode))
                    return MessageType.Duplicate;
                else if (context.RequestIdeaInfoes
                    .Any(x => x.ErrorMessage == requestExceptionInfo.ErrorMessage))
                    return MessageType.Duplicate;
                requestExceptionInfo.CreatedDateTime = DateTime.Now;
                requestExceptionInfo.UpdatedDateTime = DateTime.Now;
                requestExceptionInfo.Status = RequestIdeaStatus.Created;

                requestExceptionInfo.UserId = CurrentUserInfo.UserId;

                context.RequestIdeaInfoes.Add(requestExceptionInfo);
                context.SaveChanges();
                return MessageType.Success;
            }
        }

        [AgrinSecurityPermission(IsNormalUser = true)]
        public MessageContract<List<RequestIdeaInfo>> FilterRequestIdeaInfoes(FilterBaseInfo filterInfo)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                var query = context.RequestIdeaInfoes.Select(x => new
                {
                    RequestIdeaInfo = x,
                    LikesCount = x.LikeInfoes.Count,
                    CommentsCount = x.CommentInfoes.Count,
                    IsLiked = x.LikeInfoes.Any(y => y.UserId == CurrentUserInfo.UserId)
                }).AsNoTracking().AsQueryable();

                var result = query.ToList();
                result.ForEach(x =>
                {
                    x.RequestIdeaInfo.LikesCount = x.LikesCount;
                    x.RequestIdeaInfo.CommentsCount = x.CommentsCount;
                    x.RequestIdeaInfo.IsLiked = x.IsLiked;

                });
                return result.Select(x => x.RequestIdeaInfo).ToList().Success();
            }
        }
    }
}
