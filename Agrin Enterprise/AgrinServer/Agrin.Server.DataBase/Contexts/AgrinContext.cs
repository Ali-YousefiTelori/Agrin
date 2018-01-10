#if (!PORTABLE)
using Agrin.Server.DataBase.Configurations;
using Agrin.Server.DataBase.Models;
using Framesoft.Helpers.DataTypes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Contexts
{
    public class AgrinContext : DbContext, IDisposable
    {
        static AgrinContext()
        {
            string current = AppDomain.CurrentDomain.BaseDirectory;
            System.AppDomain.CurrentDomain.SetData("DataDirectory", current);
        }

        /// <summary>
        /// حذف کلاس از حافظه
        /// </summary>
        public new void Dispose()
        {
            Configuration.AutoDetectChangesEnabled = true;
            base.Dispose();
        }

        /// <summary>
        /// کانستراکتور پیفرض برای تایین کانکشن استرینگ و ساخت دیتابیس و اپدیت
        /// </summary>
        public AgrinContext(bool autoDetectChangesEnabled)
             : base("data source=82.102.13.102;initial catalog=AgrinServer;persist security info=True;user id=Ali;password=AVS?3773284HAMISHEBAHAR?2354;MultipleActiveResultSets=True;App=EntityFramework")
             //: base("data source=(LOCAL)\\SQLEXPRESS;initial catalog=AgrinServer;persist security info=True;user id=Ali;password=AVS?3773284HAMISHEBAHAR?2354;MultipleActiveResultSets=True;App=EntityFramework")
        //    : base("Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\HealthFamily.mdf;User Instance=false;Integrated Security=True;MultipleActiveResultSets=True")
        {
            Initialize(autoDetectChangesEnabled);
        }
        /// <summary>
        /// کانستراکتور پیفرض برای تایین کانکشن استرینگ و ساخت دیتابیس و اپدیت
        /// </summary>
        public AgrinContext()
             : base("data source=82.102.13.102;initial catalog=AgrinServer;persist security info=True;user id=Ali;password=AVS?3773284HAMISHEBAHAR?2354;MultipleActiveResultSets=True;App=EntityFramework")
             //: base("data source=(LOCAL)\\SQLEXPRESS;initial catalog=AgrinServer;persist security info=True;user id=Ali;password=AVS?3773284HAMISHEBAHAR?2354;MultipleActiveResultSets=True;App=EntityFramework")
        //     : base("Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\HealthFamily.mdf;User Instance=false;Integrated Security=True;MultipleActiveResultSets=True")
        {
            Initialize(true);
        }

        /// <summary>
        /// سازنده ی دیتابیس و تنظیمات پیشفرض
        /// </summary>
        /// <param name="autoDetectChangesEnabled"></param>
        public void Initialize(bool autoDetectChangesEnabled)
        {
            Database.SetInitializer<AgrinContext>(null);
            Database.SetInitializer(new CreateDatabaseIfNotExists<AgrinContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AgrinContext, AgrinConfiguration>());
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += (sender, e) => DateTimeKindAttribute.Apply(e.Entity);
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;

        }

        /// <summary>
        /// ساخت مدل دیتابیسی
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            modelBuilder.Conventions.Add<CascadeDeleteAttributeConvention>();
            var conv = new AttributeToTableAnnotationConvention<SoftDeleteAttribute, string>(
                "SoftDeleteColumnName",
                (type, attributes) => attributes.Single().ColumnName);
            modelBuilder.Conventions.Add(conv);
            //modelBuilder.Entity<ProfileInfo>()
            //    .HasMany(e => e.FamilyRelationSenders)
            //    .WithRequired()
            //    .HasForeignKey(e => e.FromProfileId).WillCascadeOnDelete(false);

            //modelBuilder.Entity<ProfileInfo>()
            //    .HasMany(e => e.FamilyRelationReceivers)
            //    .WithRequired()
            //    .HasForeignKey(e => e.ToProfileId).WillCascadeOnDelete(false);
        }

        /// <summary>
        /// جدول کاربران
        /// </summary>
        public DbSet<UserInfo> UserInfoes { get; set; }
        /// <summary>
        /// جدول پست ها
        /// </summary>
        public DbSet<PostInfo> PostInfoes { get; set; }
        /// <summary>
        /// table of post categories
        /// </summary>
        public DbSet<PostCategoryInfo> PostCategoryInfoes { get; set; }
        /// <summary>
        /// پست های ویدئویی
        /// </summary>
        public DbSet<PostVideoInfo> PostVideoInfoes { get; set; }
        /// <summary>
        /// پست های موسیقی
        /// </summary>
        public DbSet<PostSoundInfo> PostSoundInfoes { get; set; }
        /// <summary>
        /// جدول تگ ها و زیر مجموعه های یک موضوع
        /// هر موضوع میتواند چندین زیر مجموعه داشته باشد و هر زیر مجموعه میتواند در چندین موضوع باشد
        /// </summary>
        public DbSet<PostCategoryTagInfo> PostCategoryTagInfoes { get; set; }
    }
}
#endif