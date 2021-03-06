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
    public class TimeRemainingConverter : IValueConverter
    {
        StringBuilder str = new StringBuilder();
        List<string> items = new List<string>();

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                return ApplicationHelperBase.GetAppResource("Unknown_Language");
            str.Clear();
            items.Clear();
            TimeSpan time = (TimeSpan)value;

            items.Add(time.Seconds + " " + ApplicationHelperBase.GetAppResource("Secound_Language"));
            if (time.Minutes > 0)
            {
                if (!ShowSecounds)
                {
                    items.Clear();
                    items.Add(time.Minutes + " " + ApplicationHelperBase.GetAppResource("Minute_Language") + " ");
                }
                else
                    items.Add(time.Minutes + " " + ApplicationHelperBase.GetAppResource("Minute_Language") + " " + ApplicationHelperBase.GetAppResource("And_Language") + " ");

                if (time.Hours > 0)
                {
                    items.Add(time.Hours + " " + ApplicationHelperBase.GetAppResource("Hour_Language") + " " + ApplicationHelperBase.GetAppResource("And_Language") + " ");
                    if ((int)time.TotalDays > 0)
                        items.Add((int)time.TotalDays + " " + ApplicationHelperBase.GetAppResource("Day_Language") + " " + ApplicationHelperBase.GetAppResource("And_Language") + " ");
                }
            }
            items.Reverse();
            foreach (var item in items)
            {
                str.Append(item);
            }

            return str.ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }

        private bool _ShowSecounds = true;

        public bool ShowSecounds
        {
            get { return _ShowSecounds; }
            set { _ShowSecounds = value; }
        }
    }
}