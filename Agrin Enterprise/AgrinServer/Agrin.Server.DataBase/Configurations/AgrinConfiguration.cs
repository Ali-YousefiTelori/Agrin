#if (!PORTABLE)
using Agrin.Server.DataBase.Contexts;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Configurations
{
    public class AgrinConfiguration : DbMigrationsConfiguration<AgrinContext>
    {
        /// <summary>
        /// سازنده ی تنظیمات دیتابیس
        /// </summary>
        public AgrinConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        /// <summary>
        /// هنگام ساخت دیتابیس تنظیمات ان اینجا انجام میگیرد
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(AgrinContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
#endif