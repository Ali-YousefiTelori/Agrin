using Agrin.Server.DataBase.Models;
using Agrin.Server.DataBase.Models.Relations;
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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string current = AppDomain.CurrentDomain.BaseDirectory;
            System.AppDomain.CurrentDomain.SetData("DataDirectory", current);
            //local
            //optionsBuilder.UseSqlServer($"Server=.\\SQLEXPRESS;initial catalog=AgrinServer;User ID=Agrinuser;Password=QRNVMLzXSaqpm5dvsd^%$#@@%8er6unmkcS54DVherewd&^*156");
            //server
            // optionsBuilder.UseSqlServer("Server=agrin.info,4565\\SQLEXPRESS;initial catalog=AgrinServer;User ID=Agrinuser;Password=QRNVMLzXSaqpm5dvsd^%$#@@%8er6unmkcS54DVherewd&^*156");
            optionsBuilder.UseSqlServer("Data Source=agrin.info,4565\\SQLEXPRESS;initial catalog=AgrinServer;Integrated Security=False;User ID=agrinuser;Password=QRNVMLzXSaqpm5dvsd^%$#@@%8er6unmkcS54DVherewd&^*156");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>()
                .HasMany(u => u.FromUserCreditInfoes)
                .WithOne(t => t.FromUserInfo);

            modelBuilder.Entity<UserInfo>()
                .HasMany(u => u.ToUserCreditInfoes)
                .WithOne(t => t.ToUserInfo);


            modelBuilder.Entity<DirectFolderInfo>().HasIndex(e => e.Name).IsUnique();
            modelBuilder.Entity<UserInfo>().HasIndex(e => e.UserName).IsUnique();
            modelBuilder.Entity<DirectFileToUserRelationInfo>().HasKey(x => new { x.UserId, x.DirectFileId });
            modelBuilder.Entity<PostCategoryTagRelationInfo>().HasKey(x => new { x.TagId, x.PostCategoryId });
            modelBuilder.Entity<PostTagRelationInfo>().HasKey(x => new { x.TagId, x.PostId });
            modelBuilder.Entity<DirectFolderToUserRelationInfo>().HasKey(x => new { x.UserId, x.DirectFolderId });

            modelBuilder.Entity<UserInfo>().HasIndex(u => u.TelegramUserId).IsUnique().HasFilter(null); 
            modelBuilder.Entity<UserInfo>().HasIndex(u => u.UserName).IsUnique().HasFilter(null); 

            modelBuilder.Entity<UserCreditInfo>().HasIndex(u => u.Key).IsUnique().HasFilter(null); 

            //modelBuilder.Entity<PostCategoryTagRelationInfo>()
            //    .HasOne(bc => bc.PostCategoryInfo)
            //    .WithMany(b => b.PostCategoryTagRelationInfoes)
            //    .HasForeignKey(bc => bc.PostCategoryId);

            //modelBuilder.Entity<PostCategoryTagRelationInfo>()
            //    .HasOne(bc => bc.TagInfo)
            //    .WithMany(c => c.PostCategoryTagRelationInfoes)
            //    .HasForeignKey(bc => bc.TagId);
            //modelBuilder.Entity<PostCategoryInfo>()
            //    .HasMany(e => e.PostCategoryTagInfoes)
            //    .we()
            //    .HasForeignKey(e => e.FromProfileId).WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
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
        public DbSet<TagInfo> TagInfoes { get; set; }
        /// <summary>
        /// relation of tag and post
        /// </summary>
        public DbSet<PostCategoryTagRelationInfo> PostCategoryTagRelationInfoes { get; set; }
        /// <summary>
        /// relation of post tags
        /// </summary>
        public DbSet<PostTagRelationInfo> PostTagRelationInfoes { get; set; }
        /// <summary>
        /// user sessions for login or access
        /// </summary>
        public DbSet<UserSessionInfo> UserSessionInfoes { get; set; }
        /// <summary>
        /// direct file
        /// </summary>
        public DbSet<DirectFileInfo> DirectFileInfoes { get; set; }
        /// <summary>
        /// direct folder
        /// </summary>
        public DbSet<DirectFolderInfo> DirectFolderInfoes { get; set; }
        /// <summary>
        /// user credit
        /// </summary>
        public DbSet<UserCreditInfo> UserCreditInfoes { get; set; }
        /// <summary>
        /// user folders
        /// </summary>
        public DbSet<DirectFolderToUserRelationInfo> DirectFolderToUserRelationInfoes { get; set; }
        /// <summary>
        /// user files
        /// </summary>
        public DbSet<DirectFileToUserRelationInfo> DirectFileToUserRelationInfoes { get; set; }
        /// <summary>
        /// agrin server infoes
        /// </summary>
        public DbSet<ServerInfo> ServerInfoes { get; set; }

    }
}
