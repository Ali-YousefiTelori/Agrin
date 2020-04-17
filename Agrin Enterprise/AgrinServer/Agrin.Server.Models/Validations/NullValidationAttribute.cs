using SignalGo.Shared.DataTypes;
using System;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// اعتبار سنجی عدم خالی فرستادن مقادر توسط کلاینت
    /// </summary>
    public class NullValidationAttribute : ValidationRuleInfoAttribute
    {
        public override bool CheckIsValidate()
        {
            return CurrentValue != null;
        }

        public override object GetChangedValue()
        {
            throw new NotImplementedException();
        }

        public override object GetErrorValue()
        {
            var validation = new ValidationResultInfo() { Message = "مقدار پارامتر ورودی نباید خالی باشد، ممکن است این مقدار به دلیل معتبر نبودن Json ورودی خالی شده باشد" };
            if (PropertyInfo != null)
                validation.FieldName = PropertyInfo.Name;
            else
                validation.FieldName = ParameterInfo.Name;
            return validation;
        }
    }
}