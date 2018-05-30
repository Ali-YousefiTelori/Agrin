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
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string current = AppDomain.CurrentDomain.BaseDirectory;
            System.AppDomain.CurrentDomain.SetData("DataDirectory", current);
            optionsBuilder.UseSqlServer($"Server=localhost;initial catalog=AgrinServer;User ID=sasfasf;Password=asfasff.asfaf");
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
