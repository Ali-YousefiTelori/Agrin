using Agrin.BaseViewModels.Tasks;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Agrin.ViewModels.Converters
{
    public class TaskInfoItemsToListBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            TaskInfo task = value as TaskInfo;
            List<LinkInfoItem> items = new List<LinkInfoItem>();
            int index = 0;
            foreach (var item in task.TaskItemInfoes)
            {
                if (item.Mode != Download.Web.Tasks.TaskItemMode.LinkInfo)
                    continue;
                var info = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)item.Value);
                if (info != null)
                {
                    items.Add(new LinkInfoItem() { Link = info, Index = index, ParentTask = task });
                    index++;
                }
            }
            return items;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
