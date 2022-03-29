using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Agrin.Helper.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsInverse { get; set; }
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value == Visibility.Visible && !IsInverse ? true : false;
            else
            {
                if (IsInverse)
                    value = !(bool)value;
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }

              
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
