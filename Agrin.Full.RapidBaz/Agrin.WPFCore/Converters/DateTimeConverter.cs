using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.Helper.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        string[] StrNumber = new string[] { "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه", "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده", "بیست", "بیست و یک", "بیست و دو", "بیست و سه", "بیست و چهار", "بیست و پنج", "بیست و شش", "بیست و هفت", "بیست و هشت", "بیست و نه", "سی", "سی و یک" };
        string[] DayMode = new string[] { "روز", "ماه", "سال", "امروز", "دیروز" };
        static Calendar cal = new PersianCalendar();
        public static string GetDateTime(DateTime dateTime, bool isPersian, bool isString)
        {

            if (isPersian)
                return GetPersian(dateTime, isString);
            return GetPersian(dateTime, isString);
        }

        static string GetPersian(DateTime dateTime, bool isString)
        {
            if (isString)
                return ApplicationHelper.GetAppResource(cal.GetDayOfWeek(dateTime) + "_Language") + " " + cal.GetYear(dateTime) + "/" + cal.GetMonth(dateTime) + "/" + cal.GetDayOfMonth(dateTime) + " " + ApplicationHelper.GetAppResource("Hour_Language") + " " + dateTime.Hour + ":" + dateTime.Minute;
            return cal.GetYear(dateTime) + "/" + cal.GetMonth(dateTime) + "/" + cal.GetDayOfMonth(dateTime) + " " + dateTime.Hour + ":" + dateTime.Minute;
        }
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {

            if (value is DateTime && DateTime.Now >= (DateTime)value)
            {
                if (IsTextMode)
                {
                    try
                    {
                        List<string> items = new List<string>();
                        TimeSpan time = DateTime.Now - (DateTime)value;
                        DateTime dt = new DateTime(time.Ticks);
                        int year = dt.Year - 1;
                        int month = dt.Month - 1;
                        int day = dt.Day - 1;

                        if (year > 0)
                        {
                            items.Add(StrNumber[year - 1] + " " + DayMode[2]);
                        }
                        if (month > 0)
                        {
                            if (items.Count > 0)
                                items.Add(" " + ApplicationHelper.GetAppResource("And_Language") + " ");
                            items.Add(StrNumber[month - 1] + " " + DayMode[1]);
                        }
                        if (day > 0)
                        {
                            if (day == 1 && items.Count == 0)
                                items.Add(DayMode[4]);
                            else
                            {
                                if (items.Count > 0)
                                    items.Add(" " + ApplicationHelper.GetAppResource("And_Language") + " ");
                                items.Add(StrNumber[day - 1] + " " + DayMode[0]);
                                items.Add(" پیش");
                            }
                        }
                        else if (items.Count == 0)
                            items.Add(DayMode[3]);
                        else
                            items.Add(" پیش");
                        StringBuilder str = new StringBuilder();
                        foreach (var item in items)
                        {
                            str.Append(item);
                        }
                        return str.ToString();
                    }
                    catch
                    {
                        return "نامشخص";
                    }

                }
                return GetDateTime((DateTime)value, IsPersian, IsString);

            }
            return "";
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }

        bool _isPersian = true;
        public bool IsPersian
        {
            get { return _isPersian; }
            set { _isPersian = value; }
        }

        bool _IsString;
        public bool IsString
        {
            get { return _IsString; }
            set { _IsString = value; }
        }

        bool _IsTextMode;
        public bool IsTextMode
        {
            get { return _IsTextMode; }
            set { _IsTextMode = value; }
        }
    }
}
