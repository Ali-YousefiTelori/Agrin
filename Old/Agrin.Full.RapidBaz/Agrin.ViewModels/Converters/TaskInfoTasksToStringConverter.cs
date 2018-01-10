using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.ViewModels.Converters
{
    public class TaskInfoTasksToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is TaskInfo))
                return "نا مشخص";
            var task = value as TaskInfo;
            StringBuilder text = new StringBuilder();
            foreach (var item in task.TaskUtilityActions)
            {
                if (text.Length != 0)
                    text.Append(" , ");
                if (item == TaskUtilityModeEnum.CloseApplication)
                    text.Append("بستن نرم افزار");
                else if (item == TaskUtilityModeEnum.DeactiveTasks)
                    text.Append("غیر فعال کردن چند وظیفه");
                else if (item == TaskUtilityModeEnum.ActiveTasks)
                    text.Append("فعال کردن چند وظیفه");
                else if (item == TaskUtilityModeEnum.InternetOf)
                    text.Append("خاموش کردن اینترنت");
                else if (item == TaskUtilityModeEnum.InternetOn)
                    text.Append("روشن کردن اینترنت");
                else if (item == TaskUtilityModeEnum.Sleep)
                    text.Append("حالت خواب بردن کامپیوتر");
                else if (item == TaskUtilityModeEnum.StartLink)
                    text.Append("دانلود چند لینک");
                else if (item == TaskUtilityModeEnum.StopLink)
                    text.Append("ایست چند لینک");
                else if (item == TaskUtilityModeEnum.StopTasks)
                    text.Append("ایست کردن چند وظیفه");
                else if (item == TaskUtilityModeEnum.TurrnOff)
                    text.Append("خاموش کردن کامپیوتر");
                else if (item == TaskUtilityModeEnum.WiFiOff)
                    text.Append("خاموش کردن وای فای");
                else if (item == TaskUtilityModeEnum.WiFiOn)
                    text.Append("روشن کردن وای فای");
                else
                {
                    text.Append("نا مشخص");
                }
            }
            return text.ToString();
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}