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
            optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;initial catalog=AgrinServer;Integrated Security=False;User ID=agrinuser;Password=.");
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
            modelBuilder.Entity<PostCategorySubCategoryRelationInfo>(entity =>
            {
                entity.HasKey(x => new { x.PostSubCategoryId, x.PostCategoryId });
                entity.HasOne(d => d.PostCategoryInfo)
                   .WithMany(p => p.PostCategorySubCategoryRelationInfoes)
                   .HasForeignKey(d => d.PostCategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.PostSubCategoryInfo)
                 .WithMany(p => p.PostCategorySubCategoryRelationInfoes)
                 .HasForeignKey(d => d.PostSubCategoryId).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<LanguageKeyInfo>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(d => d.LanguageInfo)
                   .WithMany(p => p.LanguageKeyInfoes)
                   .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<PostCategoryInfo>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(d => d.LanguageKeyInfo)
                   .WithMany(p => p.PostCategoryInfoes)
                   .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<PostTagRelationInfo>().HasKey(x => new { x.TagId, x.PostId });
            modelBuilder.Entity<DirectFolderToUserRelationInfo>().HasKey(x => new { x.UserId, x.DirectFolderId });

            modelBuilder.Entity<UserInfo>().HasIndex(u => u.TelegramUserId).HasFilter(null); 
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
        public DbSet<UserInfo> Users { get; set; }
        /// <summary>
        /// جدول پست ها
        /// </summary>
        public DbSet<PostInfo> Posts { get; set; }
        /// <summary>
        /// table of post categories
        /// </summary>
        public DbSet<PostCategoryInfo> PostCategories { get; set; }
        /// <summary>
        /// پست های ویدئویی
        /// </summary>
        public DbSet<PostVideoInfo> PostVideoes { get; set; }
        /// <summary>
        /// پست های موسیقی
        /// </summary>
        public DbSet<PostSoundInfo> PostSounds { get; set; }
        /// <summary>
        /// جدول تگ ها و زیر مجموعه های یک موضوع
        /// هر موضوع میتواند چندین زیر مجموعه داشته باشد و هر زیر مجموعه میتواند در چندین موضوع باشد
        /// </summary>
        public DbSet<TagInfo> Tags { get; set; }
        /// <summary>
        /// relation of post tags
        /// </summary>
        public DbSet<PostTagRelationInfo> PostTagRelations { get; set; }
        /// <summary>
        /// user sessions for login or access
        /// </summary>
        public DbSet<UserSessionInfo> UserSessions { get; set; }
        /// <summary>
        /// direct file
        /// </summary>
        public DbSet<DirectFileInfo> DirectFiles { get; set; }
        /// <summary>
        /// direct folder
        /// </summary>
        public DbSet<DirectFolderInfo> DirectFolders { get; set; }
        /// <summary>
        /// user credit
        /// </summary>
        public DbSet<UserCreditInfo> UserCredits { get; set; }
        /// <summary>
        /// user folders
        /// </summary>
        public DbSet<DirectFolderToUserRelationInfo> DirectFolderToUserRelations { get; set; }
        /// <summary>
        /// user files
        /// </summary>
        public DbSet<DirectFileToUserRelationInfo> DirectFileToUserRelations { get; set; }
        /// <summary>
        /// agrin server infoes
        /// </summary>
        public DbSet<ServerInfo> Servers { get; set; }
        /// <summary>
        /// exception info is helpfull for users to know how to fix problems
        /// </summary>
        public DbSet<ExceptionInfo> Exceptions { get; set; }
        /// <summary>
        /// users request to add this exception to new verison of app
        /// </summary>
        public DbSet<RequestIdeaInfo> RequestIdeas { get; set; }
        /// <summary>
        /// hash for confirm
        /// </summary>
        public DbSet<UserConfirmHashInfo> UserConfirmHashes { get; set; }
        /// <summary>
        /// likes of user
        /// </summary>
        public DbSet<LikeInfo> Likes { get; set; }
        /// <summary>
        /// comment infoes
        /// </summary>
        public DbSet<CommentInfo> Comments { get; set; }
        /// <summary>
        /// language info
        /// </summary>
        public DbSet<LanguageInfo> Languages { get; set; }
        /// <summary>
        /// language keys
        /// </summary>
        public DbSet<LanguageKeyInfo> LanguageKeys { get; set; }
        /// <summary>
        /// sub category of post categories
        /// </summary>
        public DbSet<PostSubCategoryInfo> PostSubCategories { get; set; }
        /// <summary>
        /// role accesses of users
        /// </summary>
        public DbSet<UserRoleInfo> UserRoles { get; set; }
        /// <summary>
        /// visit card infoes
        /// </summary>
        public DbSet<VisitCardInfo> VisitCards { get; set; }
        /// <summary>
        /// file infoes
        /// </summary>
        public DbSet<FileInfo> Files { get; set; }
    }
}
