using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Helper
{
    public static class ExceptionExtensions
    {
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
    }
}
