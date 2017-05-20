using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.UI.Models.Converters
{
    public class FileNameToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            string fileName = value.ToString();
            return converter.GetImage(fileName, 32);
        }
        Agrin.Drawing.IO.FileToIconConverter converter = new Drawing.IO.FileToIconConverter();
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
