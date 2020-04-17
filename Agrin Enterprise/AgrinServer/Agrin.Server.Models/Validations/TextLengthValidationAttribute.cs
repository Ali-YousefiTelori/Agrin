using SignalGo.Shared.DataTypes;
using System;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// چک میکند که تعداد کاراکتر های وارد شده اشتباه نباشد
    /// </summary>
    public class TextLengthValidationAttribute : ValidationRuleInfoAttribute
    {
        /// <summary>
        /// میتواند خالی باشد
        /// </summary>
        public bool CanEmpty { get; set; }
        public TextLengthValidationAttribute()
        {

        }

        public TextLengthValidationAttribute(int max, bool canEmpty = false)
        {
            MaxValue = max;
            CanEmpty = canEmpty;
        }

        public TextLengthValidationAttribute(int min, int max, bool canEmpty = false)
        {
            MinValue = min;
            MaxValue = max;
            CanEmpty = canEmpty;
        }

        /// <summary>
        /// حداقل تعداد کاراکتر ها
        /// </summary>
        public int MinValue { get; set; } = int.MinValue;
        /// <summary>
        /// حداکثر تعداد کاراکتر ها
        /// </summary>
        public int MaxValue { get; set; } = int.MaxValue;

        public override bool CheckIsValidate()
        {
            EmailAddressValidator validator = new EmailAddressValidator();
            string text = "";
            if (CurrentValue != null)
                text = CurrentValue.ToString();
            if (!CanEmpty && string.IsNullOrEmpty(text))
                return false;
            if (text.Length < MinValue || text.Length > MaxValue)
                return false;
            return true;
        }

        public override object GetChangedValue()
        {
            throw new NotImplementedException();
        }

        public override object GetErrorValue()
        {
            string text = "";
            if (CurrentValue != null)
                text = CurrentValue.ToString();
            string displayName = ValidationResultInfo.GetDisplayName(PropertyInfo, ParameterInfo);
            string message = "";
            if (string.IsNullOrEmpty(displayName))
            {
                if (text.Length < MinValue)
                    message = $"مقدار وارد شده باید حداقل {MinValue} حرف داشته باشد";
                else
                    message = $"مقدار وارد شده باید حداکثر {MaxValue} حرف داشته باشد";
            }
            else
            {
                if (text.Length < MinValue)
                    message = $"{displayName} باید حداقل {MinValue} حرف داشته باشد";
                else
                    message = $"{displayName} باید حداکثر {MaxValue} حرف داشته باشد";
            }

            if (PropertyInfo != null)
                return new ValidationResultInfo() { Message = message, FieldName = PropertyInfo.Name };
            return new ValidationResultInfo() { Message = message, FieldName = ParameterInfo.Name };
        }
    }
}
