using SignalGo.Shared.DataTypes;
using System;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// بررسی میکند که مقدار وارد شده فقط عددی باشد
    /// </summary>
    public class IsNumberValidationRuleAttribute : ValidationRuleInfoAttribute
    {
        public override bool CheckIsValidate()
        {
            if (CurrentValue != null)
            {
                string text = CurrentValue.ToString();
                if (!string.IsNullOrEmpty(text))
                    return long.TryParse(text, out long result);
            }
            return true;
        }

        public override object GetChangedValue()
        {
            throw new NotImplementedException();
        }

        public override object GetErrorValue()
        {
            string displayName = ValidationResultInfo.GetDisplayName(PropertyInfo, ParameterInfo);
            string message = "";
            if (string.IsNullOrEmpty(displayName))
                message = "مقدار وارد شده باید عددی باشد";
            else
                message = $"{displayName} باید عددی باشد";

            if (PropertyInfo != null)
                return new ValidationResultInfo() { Message = message, FieldName = PropertyInfo.Name };
            return new ValidationResultInfo() { Message = message, FieldName = ParameterInfo.Name };
        }
    }
}