using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Agrin.ViewModels.Validations
{
    public class IntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double result;
            //int test;
            if (value != null && double.TryParse(value.ToString(), out result) && double.TryParse(value.ToString(), out result))
            {
                //result = Agrin_Engine.Models.Math.Convertor.ConvertToByte(result, (SizeEnum)(ValidationMode + VisibleCount), SizeEnum.Byte);
                if (result > MaxValue || result > MaxValue)
                {
                    if (MaxValue != int.MaxValue)
                        return new ValidationResult(false, ApplicationHelperBase.GetAppResource("MinValidation_Language") + " " + MaxValue + " " + ApplicationHelperBase.GetAppResource("IsValidation_Language"));
                    else
                        return new ValidationResult(false, ApplicationHelperBase.GetAppResource("IsMaxValidation_Language"));
                }
                else if (result < MinValue || result < MinValue)
                    return new ValidationResult(false, ApplicationHelperBase.GetAppResource("MaxValidation_Language") + " " + MinValue + " " + ApplicationHelperBase.GetAppResource("IsValidation_Language"));
                return new ValidationResult(true, null);
            }
            else
                return new ValidationResult(false, ApplicationHelperBase.GetAppResource("PleaseSetTrue_Language"));
        }

        int _maxValue = int.MaxValue;

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

        int _validationMode;

        public int ValidationMode
        {
            get { return _validationMode; }
            set { _validationMode = value; }
        }

        int _visibleCount;

        public int VisibleCount
        {
            get { return _visibleCount; }
            set { _visibleCount = value; }
        }
    }
}
