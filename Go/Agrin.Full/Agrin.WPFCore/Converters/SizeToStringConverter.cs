using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.Helper.Converters
{
    public enum SizeEnum
    {
        Byte = 0,
        KB = 1,
        MB = 2,
        GB = 3,
        TB = 4,
        EXB = 5
    }
    public class SizeToStringConverter : IValueConverter
    {
        public static string GetSizeString(double Size)
        {
            if (Size < 0)
                return ApplicationHelper.GetAppResource("Unknown_Language");
            int i = 0;
            for (i = 0; i < 6; i++)
            {
                if (Size >= 1024)
                    Size /= 1024;
                else
                    break;
            }
            return String.Format("{0:0.000}", Size) + " " + ApplicationHelper.GetAppResource(((SizeEnum)i) + "_Language");
        }

        public static string GetEnglishSizeString(double Size)
        {
            if (Size < 0)
                return ApplicationHelper.GetAppResource("Unknown_Language");
            int i = 0;
            for (i = 0; i < 6; i++)
            {
                if (Size >= 1024)
                    Size /= 1024;
                else
                    break;
            }
            return System.Math.Round(Size, 3).ToString() + " " + ((SizeEnum)i).ToString();
        }
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            double val = 0;
            if (value != null)
            {
                double.TryParse(value.ToString(), out val);
            }

            string size = IsEnglish ? GetEnglishSizeString(val) : GetSizeString(val);
            size = IsPerSecound ? size + " بر ثانیه" : size;
            return size;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }

        bool _IsEnglish = false;
        public bool IsEnglish
        {
            get { return _IsEnglish; }
            set { _IsEnglish = value; }
        }

        bool _IsPerSecound;
        public bool IsPerSecound
        {
            get { return _IsPerSecound; }
            set { _IsPerSecound = value; }
        }
    }
}
