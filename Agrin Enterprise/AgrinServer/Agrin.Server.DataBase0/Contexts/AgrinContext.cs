using Agrin.Server.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public AgrinContext(bool autoDetectChangesEnabled)
        {
            Initialize(autoDetectChangesEnabled);
        }

        public AgrinContext()
        {
            Initialize(true);
        }

        public void Initialize(bool autoDetectChangesEnabled)
        {
            if (!autoDetectChangesEnabled)
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// حذف کلاس از حافظه
        /// </summary>
        //public new void Dispose()
        //{
        //    Configuration.AutoDetectChangesEnabled = true;
        //    base.Dispose();
        //}

        /// <summary>
        /// سازنده ی دیتابیس و تنظیمات پیشفرض
        /// </summary>
        /// <param name="autoDetectChangesEnabled"></param>
        //public void Initialize(bool autoDetectChangesEnabled)
        //{
        //    //Database.SetInitializer<AgrinContext>(null);
        //    //Database.SetInitializer(new CreateDatabaseIfNotExists<AgrinContext>());
        //    //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AgrinContext, AgrinConfiguration>());
        //    ////((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += (sender, e) => DateTimeKindAttribute.Apply(e.Entity);
        //    //Configuration.LazyLoadingEnabled = false;
        //    //Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;

        //    string current = AppDomain.CurrentDomain.BaseDirectory;
        //    System.AppDomain.CurrentDomain.SetData("DataDirectory", current);
        //    optionsBuilder.UseSqlServer($"Server=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|AzadeganDB.mdf;Initial Catalog=AzadeganDB.mdf;User Instance=false;Trusted_Connection=True;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework;", x => x.MigrationsAssembly(this.GetType().Assembly.FullName));
        //    //optionsBuilder.UseSqlServer($"Server=.\\SQLEXPRESS;Initial Catalog=AzadeganDB.mdf;User Instance=false;Trusted_Connection=True;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework;", x => x.MigrationsAssembly(this.GetType().Assembly.FullName));

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string current = AppDomain.CurrentDomain.BaseDirectory;
            System.AppDomain.CurrentDomain.SetData("DataDirectory", current);
            optionsBuilder.UseSqlServer($"Server=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|AgrinServer.mdf;Initial Catalog=AgrinServer.mdf;User Instance=false;Trusted_Connection=True;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework;", x => x.MigrationsAssembly(this.GetType().Assembly.FullName));
        }
        /// <summary>
        /// ساخت مدل دیتابیسی
        /// </summary>
        /// <param name="modelBuilder"></param>
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        //    //modelBuilder.Conventions.Add<CascadeDeleteAttributeConvention>();
        //    //var conv = new AttributeToTableAnnotationConvention<SoftDeleteAttribute, string>(
        //    //    "SoftDeleteColumnName",
        //    //    (type, attributes) => attributes.Single().ColumnName);
        //    //modelBuilder.Conventions.Add(conv);
        //    //modelBuilder.Entity<ProfileInfo>()
        //    //    .HasMany(e => e.FamilyRelationSenders)
        //    //    .WithRequired()
        //    //    .HasForeignKey(e => e.FromProfileId).WillCascadeOnDelete(false);

        //    //modelBuilder.Entity<ProfileInfo>()
        //    //    .HasMany(e => e.FamilyRelationReceivers)
        //    //    .WithRequired()
        //    //    .HasForeignKey(e => e.ToProfileId).WillCascadeOnDelete(false);
        //}

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
