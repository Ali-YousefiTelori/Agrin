using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase
{
    /// <summary>
    /// اکستنشن های دیتابسی
    /// </summary>
    public static class DataBaseExtensions
    {
        /// <summary>
        /// دریافت لاگ از طریق یک اکسپشن در صورتی که خطا از نوع دیتابیسی باشد
        /// </summary>
        /// <param name="ex">اکسپشن مورد نظر</param>
        /// <returns>متن لاگ</returns>
        public static string GetSqlExceptionLog(this Exception ex)
        {
            if (ex is System.Data.Entity.Validation.DbEntityValidationException)
            {
                return DbEntityValidationExceptionToString((System.Data.Entity.Validation.DbEntityValidationException)ex);
            }
            else
                return null;
        }
        /// <summary>
        ///     A DbEntityValidationException extension method that formates validation errors to string.
        /// </summary>
        static string DbEntityValidationExceptionToString(System.Data.Entity.Validation.DbEntityValidationException e)
        {
            var validationErrors = DbEntityValidationResultToString(e);
            var exceptionMessage = string.Format("{0}{1}Validation errors:{1}{2}", e, Environment.NewLine, validationErrors);
            return exceptionMessage;
        }

        /// <summary>
        ///     A DbEntityValidationException extension method that aggregate database entity validation results to string.
        /// </summary>
        static string DbEntityValidationResultToString(System.Data.Entity.Validation.DbEntityValidationException e)
        {
            return e.EntityValidationErrors
                    .Select(dbEntityValidationResult => DbValidationErrorsToString(dbEntityValidationResult, dbEntityValidationResult.ValidationErrors))
                    .Aggregate(string.Empty, (current, next) => string.Format("{0}{1}{2}", current, Environment.NewLine, next));
        }

        /// <summary>
        ///     A DbEntityValidationResult extension method that to strings database validation errors.
        /// </summary>
        static string DbValidationErrorsToString(System.Data.Entity.Validation.DbEntityValidationResult dbEntityValidationResult, IEnumerable<System.Data.Entity.Validation.DbValidationError> dbValidationErrors)
        {
            var entityName = string.Format("[{0}]", dbEntityValidationResult.Entry.Entity.GetType().Name);
            const string indentation = "\t - ";
            var aggregatedValidationErrorMessages = dbValidationErrors.Select(error => string.Format("[{0} - {1}]", error.PropertyName, error.ErrorMessage))
                                                   .Aggregate(string.Empty, (current, validationErrorMessage) => current + (Environment.NewLine + indentation + validationErrorMessage));
            return string.Format("{0}{1}", entityName, aggregatedValidationErrorMessages);
        }

        /// <summary>
        /// سلکت کردن یک صفحه
        /// </summary>
        /// <typeparam name="T">نوع جدول</typeparam>
        /// <typeparam name="T2">نوع پروپرتی مرتب سازی</typeparam>
        /// <param name="list">لیست دیتابیسی</param>
        /// <param name="sortFunc">فانکشن مرتب سازی</param>
        /// <param name="isDescending">ایا نزولی است؟</param>
        /// <param name="index">ایندکس شروع از ایتم برای صفحه بندی</param>
        /// <param name="length">تعداد آیتم هایی که از شروع باید دریافت کند</param>
        /// <returns></returns>
        public static IEnumerable<T> SelectPage<T, T2>(this IEnumerable<T> list, Func<T, T2> sortFunc, bool isDescending, int index, int length)
        {
            List<T> result = null;
            if (index < 0 || length <= 0)
                result = list.ToList();
            else
            {
                if (isDescending)
                    result = list.OrderByDescending(sortFunc).Skip(index).Take(length).ToList();
                else
                    result = list.OrderBy(sortFunc).Skip(index).Take(length).ToList();
            }

            return result;
        }

        /// <summary>
        /// سلکت کردن یک صفحه
        /// </summary>
        /// <typeparam name="T">نوع جدول</typeparam>
        /// <param name="list">لیست دیتابیسی</param>
        /// <param name="index">ایندکس شروع از ایتم برای صفحه بندی</param>
        /// <param name="length">تعداد آیتم هایی که از شروع باید دریافت کند</param>
        /// <returns></returns>
        public static IEnumerable<T> SelectPage<T>(this IEnumerable<T> list, int index, int length)
        {
            List<T> result = null;
            if (index < 0 || length <= 0)
                result = list.ToList();
            else
            {
                result = list.Skip(index).Take(length).ToList();
            }
            return result;
        }
    }
}
