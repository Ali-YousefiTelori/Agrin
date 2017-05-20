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
    public class GetControlMousePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            MenuItem element = (MenuItem)((FrameworkElement)value).TemplatedParent;
            element.Tag = value;
            element.Loaded += element_Loaded;
            //System.Windows.Controls.Menu menu = (System.Windows.Controls.Menu)element.Parent;
            return new Thickness(0);
           // return new Thickness(Math.Abs(p.X), 0, 0, 0);
        }

        void element_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            FrameworkElement element = (FrameworkElement)menu.Tag;
            Point p;
            p = ((Menu)menu.Parent).TranslatePoint(new Point(),element);
            FrameworkElement findel= (FrameworkElement)element.FindName("lineGrid");
            findel.Margin = new Thickness(Math.Abs(p.X), 0, 0, 0);
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
