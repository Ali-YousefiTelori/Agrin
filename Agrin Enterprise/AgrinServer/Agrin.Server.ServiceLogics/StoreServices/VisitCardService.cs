using Agrin.Server.DataBase.Contexts;
using Agrin.Server.DataBase.Models;
using Agrin.Server.Models;
using Agrin.Server.Models.Filters;
using Microsoft.EntityFrameworkCore;
using SignalGo.Shared.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agrin.Server.ServiceLogics.StoreServices
{
    [ServiceContract("VisitCard", ServiceType.ServerService)]
    public class VisitCardService
    {
        [AgrinSecurityPermission(IsNormalUser = true)]
        public async Task<MessageContract<List<VisitCardInfo>>> FilterVisitCards(FilterBaseInfo filterInfo)
        {
            int userId = CurrentUserInfo.UserId;
            using (AgrinContext context = new AgrinContext())
            {
                return await context.VisitCards.Where(x => x.UserId == userId).ToListAsync();
            }
        }

        [AgrinSecurityPermission(IsNormalUser = true)]
        public async Task<MessageContract<int>> AddVisitCardInfo(VisitCardInfo visitCardInfo)
        {
            int userId = CurrentUserInfo.UserId;
            using (AgrinContext context = new AgrinContext())
            {
                visitCardInfo.UserId = userId;
                await context.VisitCards.AddAsync(visitCardInfo);
                await context.SaveChangesAsync();
                return visitCardInfo.Id;
            }
        }


    }
}
