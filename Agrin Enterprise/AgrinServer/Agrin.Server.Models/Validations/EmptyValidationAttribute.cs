using SignalGo.Shared.DataTypes;
using SignalGo.Shared.Models;
using System;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// اعتبار سنجی جلوگیری از خالی بودن متن
    /// </summary>
    public class EmptyValidationAttribute : ValidationRuleInfoAttribute
    {
        public override bool CheckIsValidate()
        {
            if (CurrentValue == null || (CurrentValue is string text && string.IsNullOrEmpty(text.Trim())))
                return false;
            return true;
        }

        public override object GetChangedValue()
        {
            throw new NotImplementedException();
        }

        public override object GetErrorValue()
        {
            var displayName = ValidationResultInfo.GetDisplayName(PropertyInfo, ParameterInfo);

            string message = "";
            if (string.IsNullOrEmpty(displayName))
                message = "لطفا مقدار وارد شده را پر کنید";
            else
                message = $"لطفا مقدار {displayName} را پر کنید";

            if (PropertyInfo != null)
                return new ValidationResultInfo() { Message = message, FieldName = PropertyInfo.Name };
            return new ValidationResultInfo() { Message = message, FieldName = ParameterInfo.Name };
        }
    }
}
