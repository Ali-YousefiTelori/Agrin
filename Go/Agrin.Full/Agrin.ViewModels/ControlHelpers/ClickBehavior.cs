using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Agrin.ViewModels.ControlHelpers
{
    public static class ShowRowDetailsWhenClick
    {
        public static bool GetIsShowRowDetailsWhenClick(DependencyObject d)
        {
            return (bool)d.GetValue(ShowRowDetailsWhenClick.IsShowRowDetailsWhenClickProperty);
        }

        public static void SetIsShowRowDetailsWhenClick(DependencyObject d, bool val)
        {
            d.SetValue(ShowRowDetailsWhenClick.IsShowRowDetailsWhenClickProperty, val);
        }

        public static readonly DependencyProperty IsShowRowDetailsWhenClickProperty =
            DependencyProperty.RegisterAttached("IsShowRowDetailsWhenClick",
                typeof(bool),
                typeof(ShowRowDetailsWhenClick),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if ((bool)e.NewValue)
                        {
                            if (d is DataGridRow)
                            {
                                var element = d as DataGridRow;
                                element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(element_MouseLeftButtonDown);
                                element.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(element_MouseLeftButtonUp);
                            }
                        }
                    }));
        static DataGridRow lastDataGridRow = null;
        static void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastDataGridRow = sender as DataGridRow;
            //if (uie != null)
            //{
            //    uie.CaptureMouse();
            //}
        }
        static void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGridRow element = sender as DataGridRow;
            if (element != null && lastDataGridRow != null && lastDataGridRow == element && !(e.OriginalSource is Button) && !(e.Source is  DataGridDetailsPresenter))
            {
                //uie.ReleaseMouseCapture();
                //DataGridRow element = e.OriginalSource as DataGridRow;
                if (element != null && element.InputHitTest(e.GetPosition(element)) != null)
                {
                    element.DetailsVisibility = element.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }
    }
}
