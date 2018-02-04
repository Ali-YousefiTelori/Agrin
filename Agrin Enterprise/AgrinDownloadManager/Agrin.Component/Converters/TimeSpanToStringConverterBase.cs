using Agrin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Converters
{
    public class TimeSpanToStringConverterBase
    {
        public string GetTimeString(TimeSpan? value)
        {
            if (value == null)
                return ApplicationResourceBase.Current.GetAppResource("Unknown");
            TimeSpan time = (TimeSpan)value;
            StringBuilder str = new StringBuilder();
            List<string> items = new List<string>();

            str.Clear();
            items.Clear();
            items.Add(time.Seconds + " " + ApplicationResourceBase.Current.GetAppResource("Secound"));
            if (time.Minutes > 0)
            {
                if (!ShowSecounds)
                {
                    items.Clear();
                    items.Add(time.Minutes + " " + ApplicationResourceBase.Current.GetAppResource("Minute") + " ");
                }
                else
                    items.Add(time.Minutes + " " + ApplicationResourceBase.Current.GetAppResource("Minute") + " " + ApplicationResourceBase.Current.GetAppResource("And") + " ");

                if (time.Hours > 0)
                {
                    items.Add(time.Hours + " " + ApplicationResourceBase.Current.GetAppResource("Hour") + " " + ApplicationResourceBase.Current.GetAppResource("And") + " ");
                    if ((int)time.TotalDays > 0)
                        items.Add((int)time.TotalDays + " " + ApplicationResourceBase.Current.GetAppResource("Day") + " " + ApplicationResourceBase.Current.GetAppResource("And") + " ");
                }
            }
            items.Reverse();
            foreach (var item in items)
            {
                str.Append(item);
            }

            return str.ToString();
        }


        private bool _ShowSecounds = true;

        public bool ShowSecounds
        {
            get { return _ShowSecounds; }
            set { _ShowSecounds = value; }
        }
    }
}
