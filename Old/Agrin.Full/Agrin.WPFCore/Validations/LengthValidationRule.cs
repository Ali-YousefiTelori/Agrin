using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Agrin.WPFCore.Validations
{
    public class LengthValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (String.IsNullOrEmpty(value as String))
                return new ValidationResult(false, ApplicationHelper.GetAppResource("MustSetValue_Language"));
            int lenght = value.ToString().Length;
            if (lenght > MaxValue && MinValue == 0)
                return new ValidationResult(false, ApplicationHelper.GetAppResource("MinCharValidation_Language") + " " + MaxValue + " " + ApplicationHelper.GetAppResource("IsValidation_Language"));
            else if (lenght < MinValue && MaxValue == 0)
                return new ValidationResult(false, ApplicationHelper.GetAppResource("MaxCharValidation_Language") + " " + MinValue + " " + ApplicationHelper.GetAppResource("IsValidation_Language"));
            else if (lenght < MinValue || lenght > MaxValue)
                return new ValidationResult(false, ApplicationHelper.GetAppResource("MustCharValidation_Language") + " " + MinValue + " " + ApplicationHelper.GetAppResource("IsRaghamValidation_Language"));

            return new ValidationResult(true, null);
        }

        int _maxValue = 0;

        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        int _minValue = 0;

        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        bool _IsNullable = false;

        public bool IsNullable
        {
            get { return _IsNullable; }
            set
            {
                _IsNullable = value;

            }
        }
    }
}
