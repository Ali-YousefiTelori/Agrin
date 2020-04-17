using SignalGo.Shared.DataTypes;
using System;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// ساخت یک جی یو آی دی هنگامی که مدل ساخته میشود یا به عنوان پارامتر از سوی کاربر وارد میشود
    /// </summary>
    public class AutoGenerateGuidAttribute : ValidationRuleInfoAttribute
    {
        public AutoGenerateGuidAttribute()
        {
            TaskType = ValidationRuleInfoTaskType.ChangeValue;
        }

        public override bool CheckIsValidate()
        {
            return false;
        }

        public override object GetChangedValue()
        {
            return Guid.NewGuid();
        }

        public override object GetErrorValue()
        {
            throw new NotImplementedException();
        }
    }
}
