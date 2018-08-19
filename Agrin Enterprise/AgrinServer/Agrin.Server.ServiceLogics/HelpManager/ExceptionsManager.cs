using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Microsoft.EntityFrameworkCore;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.HelpManager
{
    [ServiceContract("ExceptionService", ServiceType.ServerService, InstanceType = InstanceType.SingleInstance)]
    public class ExceptionsManager
    {
        public MessageContract<ExceptionInfo> GetExceptionInformationByCode(int errorCode)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                var query = context.ExceptionInfoes.AsNoTracking().AsQueryable();
                var find = query.FirstOrDefault(x => x.ErrorCode == errorCode);
                if (find == null)
                    return MessageType.NotFound;
                return find.Success();
            }
        }

        public MessageContract<ExceptionInfo> GetExceptionInformationByHttpCode(int errorCode)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                var query = context.ExceptionInfoes.AsNoTracking().AsQueryable();
                var find = query.FirstOrDefault(x => x.HttpErrorCode == errorCode);
                if (find == null)
                    return MessageType.NotFound;
                return find.Success();
            }
        }

        [AgrinSecurityPermission(IsNormalUser = true)]
        public MessageContract AddRequestException(RequestIdeaInfo requestExceptionInfo)
        {
            using (AgrinContext context = new AgrinContext(false))
            {
                if (requestExceptionInfo.HttpErrorCode.HasValue && context.RequestExceptionInfoes.Any(x => x.HttpErrorCode == requestExceptionInfo.HttpErrorCode))
                    return MessageType.Duplicate;
                else if (context.RequestExceptionInfoes
                    .Any(x => x.ErrorMessage == requestExceptionInfo.ErrorMessage))
                    return MessageType.Duplicate;
                requestExceptionInfo.CreatedDateTime = DateTime.Now;
                requestExceptionInfo.UpdatedDateTime = DateTime.Now;
                requestExceptionInfo.Status = RequestIdeaStatus.Created;

                requestExceptionInfo.UserId = CurrentUserInfo.UserId;

                context.RequestExceptionInfoes.Add(requestExceptionInfo);
                context.SaveChanges();
                return MessageType.Success;
            }
        }
    }
}
