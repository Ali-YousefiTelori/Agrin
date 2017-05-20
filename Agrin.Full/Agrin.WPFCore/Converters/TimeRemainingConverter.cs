using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.Helper.Converters
{
    public class TimeRemainingConverter : IValueConverter
    {
        StringBuilder str = new StringBuilder();
        List<string> items = new List<string>();

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                return ApplicationHelper.GetAppResource("Unknown_Language");
            str.Clear();
            items.Clear();
            TimeSpan time = (TimeSpan)value;

            items.Add(time.Seconds + " " + ApplicationHelper.GetAppResource("Secound_Language"));
            if (time.Minutes > 0)
            {
                items.Add(time.Minutes + " " + ApplicationHelper.GetAppResource("Minute_Language") + " " + ApplicationHelper.GetAppResource("And_Language") + " ");
                if (time.Hours > 0)
                {
                    items.Add(time.Hours + " " + ApplicationHelper.GetAppResource("Hour_Language") + " " + ApplicationHelper.GetAppResource("And_Language") + " ");
                    if ((int)time.TotalDays > 0)
                        items.Add((int)time.TotalDays + " " + ApplicationHelper.GetAppResource("Day_Language") + " " + ApplicationHelper.GetAppResource("And_Language") + " ");
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
    }
}