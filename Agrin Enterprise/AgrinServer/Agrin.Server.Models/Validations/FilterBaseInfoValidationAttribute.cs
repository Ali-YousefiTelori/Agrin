using Agrin.Server.Models.Filters;
using SignalGo.Shared.DataTypes;
using System;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// وضعیت نوع خطا
    /// </summary>
    public enum FilterBaseInfoValidationState : byte
    {
        /// <summary>
        /// فیلتر اوکی هست مشکلی نداره
        /// </summary>
        Success = 0,
        /// <summary>
        /// مقدار اومده خالی و نال هست
        /// </summary>
        Null = 1,
        /// <summary>
        /// داده ها اعداد بزرگ و زیادی هستند
        /// </summary>
        OverFlow = 2,
        /// <summary>
        /// تاریخ و زمان جابجا یا اشتباه است
        /// </summary>
        DateWrong = 3
    }
    /// <summary>
    /// اعتبار سنجی جلوگیری از دریافت بیگ دیتا توسط کلاینت
    /// </summary>
    public class FilterBaseInfoValidationAttribute : BaseValidationRuleInfoAttribute, IValidationRuleInfoAttribute<FilterBaseInfoValidationState>
    {
        public bool CheckStateIsSuccess(FilterBaseInfoValidationState state)
        {
            return state == FilterBaseInfoValidationState.Success;
        }

        public FilterBaseInfoValidationState CheckIsValidate()
        {
            FilterBaseInfo filter = (FilterBaseInfo)CurrentValue;
            if (CurrentValue == null || filter == null)
                return FilterBaseInfoValidationState.Null;
            if (filter.Length > 20 || filter.Index < 0)
                return FilterBaseInfoValidationState.OverFlow;
            else if (filter.StartDateTime.HasValue && filter.EndDateTime.HasValue && filter.EndDateTime <= filter.StartDateTime)
                return FilterBaseInfoValidationState.DateWrong;
            return FilterBaseInfoValidationState.Success;
        }

        public object GetChangedValue(FilterBaseInfoValidationState state)
        {
            throw new NotImplementedException();
        }

        public object GetErrorValue(FilterBaseInfoValidationState state)
        {
            ValidationResultInfo validation = new ValidationResultInfo();
            if (state == FilterBaseInfoValidationState.Null)
                validation.Message = "اطلاعات وارد شده صحیح نمی باشد، لطفا مقادیر فیلتر را پر کنید";
            else if (state == FilterBaseInfoValidationState.DateWrong)
                validation.Message = "مقادیر تاریخ و زمان اشتباه وارد شده است، تاریخ پایان نمی تواند کوچکتر از تاریخ شروع باشد";
            else if (state == FilterBaseInfoValidationState.OverFlow)
                validation.Message = "مقادیر وارد شده برای فیلتر صحیح نمی باشد، اندازه صفحه نباید بزرگتر از 20 یا کوچکتر از صفر باشد";

            if (PropertyInfo != null)
                validation.FieldName = PropertyInfo.Name;
            else
                validation.FieldName = ParameterInfo.Name;
            return validation;
        }
    }
}
