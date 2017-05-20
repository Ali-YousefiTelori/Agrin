using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase
{
    /// <summary>
    /// اکستنشن های مورد نظر
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// در صورتی که مد های دیتابیس خطا داشته باشد این اکستنشن استفاده میشود
        /// </summary>
        public static Func<Exception, string> GetSqlExceptionLogFunc;
    }
}
