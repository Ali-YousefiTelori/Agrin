using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.Models
{
    /// <summary>
    /// کلاس قوانین ارسال مقادر به ای پی آی سرور
    /// </summary>
    public class ValidationResultInfo
    {
        /// <summary>
        /// نام فیلد مورد نظر که نیاز به تغییرات دارد
        /// در صورتی که پارامتر های یک متد باشد نام پارامتر جای آن قرار میگیرد
        /// در صورتی که یک پروپتری باشد نام پروپرتی در آن قرار میگیرد
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// راهنمایی فرستنده جهت رفع مشکل
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// دریافت نام نمایشی روی پروپتری ها
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        public static string GetDisplayName(PropertyInfo propertyInfo, ParameterInfo parameterInfo)
        {
            if (propertyInfo != null)
            {
                var attribute = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
                if (attribute != null)
                    return attribute.DisplayName;
            }
            if (parameterInfo != null)
            {
                var attribute = parameterInfo.GetCustomAttribute<SignalGo.Shared.DataTypes.ParameterDisplayNameAttribute>();
                if (attribute != null)
                    return attribute.Name;
            }
            return "";
        }
    }
}
