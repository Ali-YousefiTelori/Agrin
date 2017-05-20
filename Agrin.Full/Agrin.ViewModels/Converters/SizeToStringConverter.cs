using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.ViewModels.Converters
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
        public static string GetSizeString(double Size, int Digits)
        {
            if (Size < 0)
                return ApplicationHelperBase.GetAppResource("Unknown_Language");
            int i = 0;
            for (i = 0; i < 6; i++)
            {
                if (Size >= 1024)
                    Size /= 1024;
                else
                    break;
            }
            return String.Format("{0:0." + new String('0', Digits) + "}", Size) + " " + ApplicationHelperBase.GetAppResource(((SizeEnum)i) + "_Language");
        }

        public static string GetEnglishSizeString(double Size, int Digits)
        {
            if (Size < 0)
                return ApplicationHelperBase.GetAppResource("Unknown_Language");
            int i = 0;
            for (i = 0; i < 6; i++)
            {
                if (Size >= 1024)
                    Size /= 1024;
                else
                    break;
            }
            return System.Math.Round(Size, Digits).ToString() + " " + ((SizeEnum)i).ToString();
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            double val = 0;
            if (value != null)
            {
                double.TryParse(value.ToString(), out val);
            }

            string size = IsEnglish ? GetEnglishSizeString(val, Digits) : GetSizeString(val, Digits);
            size = IsPerSecound ? size + "/s" : size;
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

        int _Digits = 2;

        public int Digits
        {
            get { return _Digits; }
            set { _Digits = value; }
        }
    }
}
