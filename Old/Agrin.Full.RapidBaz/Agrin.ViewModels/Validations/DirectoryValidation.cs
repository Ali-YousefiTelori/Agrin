using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Agrin.ViewModels.Validations
{
    public class DirectoryValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string path = value == null ? "" : value.ToString();
            if (Path.IsPathRooted(path))
            {
                if (!Directory.Exists(path))
                    return new ValidationResult(false, ApplicationHelper.GetAppResource("DirectoryNotExist_Language"));
            }
            else
            {
                return new ValidationResult(false, ApplicationHelper.GetAppResource("DirectoryRootError_Language"));

            }
            return new ValidationResult(true, null);

        }
    }
}
