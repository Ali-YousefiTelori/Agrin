using FrameSoft.Agrin.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Contexts
{
    public class AgrinContext : DbContext
    {
        public AgrinContext()
              : base("data source=(LOCAL)\\SQLEXPRESS;initial catalog=FrameSoftAmar;User ID=Ali;Password=.;MultipleActiveResultSets=True;App=EntityFramework")
        {
            Database.SetInitializer<AgrinContext>(null);
            Database.SetInitializer(new CreateDatabaseIfNotExists<AgrinContext>());
            Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<AgrinContext, Migrations.Configuration>());
        }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<ApplicationErrorReport> ApplicationErrorReports { get; set; }
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<UserFileInfo> LeechFiles { get; set; }
        /// <summary>
        /// خرید های کاربران
        /// </summary>
        public DbSet<UserPurchase> UserPurchases { get; set; }
        /// <summary>
        /// پیغام های کاربران
        /// </summary>
        public DbSet<UserMessageReceiveInfo> UserReceiveMessages { get; set; }
        /// <summary>
        /// پاسخ های مدیر به پیغام های خصوصی کاربران
        /// </summary>
        public DbSet<UserMessageReplayInfo> UserMessageReplays { get; set; }
        /// <summary>
        /// پیغام های عمومی به کاربران
        /// </summary>
        public DbSet<PublicMessageInfo> PublicMessages { get; set; }

    }
}
