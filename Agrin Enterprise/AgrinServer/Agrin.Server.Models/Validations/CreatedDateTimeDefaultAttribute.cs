using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.Models.Validations
{
    /// <summary>
    /// تغییر تاریخ و زمان یک پروپتری به زمان حال
    /// </summary>
    public class CreatedDateTimeDefaultAttribute : ValidationRuleInfoAttribute
    {
        public CreatedDateTimeDefaultAttribute()
        {
            TaskType = ValidationRuleInfoTaskType.ChangeValue;
        }

        public override bool CheckIsValidate()
        {
            return false;
        }

        public override object GetChangedValue()
        {
            return DateTime.Now;
        }

        public override object GetErrorValue()
        {
            throw new NotImplementedException();
        }
    }
}
