using SignalGo.Shared.DataTypes;
using System;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// اعتبار سنجی درست بودن ایمیل ورودی توسط کاربر
    /// </summary>
    public class EmailValidationAttribute : ValidationRuleInfoAttribute
    {
        public override bool CheckIsValidate()
        {
            EmailAddressValidator validator = new EmailAddressValidator();
            if (CurrentValue == null || !(CurrentValue is string text))
                return false;
            return validator.IsEmailValid(text);
        }

        public override object GetChangedValue()
        {
            throw new NotImplementedException();
        }

        public override object GetErrorValue()
        {
            if (PropertyInfo != null)
                return new ValidationResultInfo() { Message = "لطفا ایمیل را صحیح وارد کنید", FieldName = PropertyInfo.Name };
            return new ValidationResultInfo() { Message = "لطفا ایمیل را صحیح وارد کنید", FieldName = ParameterInfo.Name};
        }
    }
}
