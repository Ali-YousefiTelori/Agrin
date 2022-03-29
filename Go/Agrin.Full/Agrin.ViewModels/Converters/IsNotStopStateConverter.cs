using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.ViewModels.Converters
{
    public class IsNotStopStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            else
            {
                var state = (Agrin.Download.Web.ConnectionState)value;
                if (state == Download.Web.ConnectionState.Stoped || state == Download.Web.ConnectionState.Complete)
                    return false;
                else
                    return true;
            }
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
