using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Agrin.ViewModels.Converters
{
    public class GetTabItemCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is TabItem)
            {
                var tab = value as TabItem;
                var tabCtrl = tab.Parent as TabControl;
                if (tabCtrl.Items.Cast<TabItem>().FirstOrDefault() == tab)
                {
                    return new CornerRadius(5,0,0,0); 
                }
            }
            return new CornerRadius();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
