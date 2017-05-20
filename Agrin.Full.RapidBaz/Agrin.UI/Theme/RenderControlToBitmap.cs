using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Agrin.UI.Theme
{
    public class RenderControlToBitmap
    {
        public static string GetStyleName(DependencyObject obj)
        {
            return (string)obj.GetValue(StyleNameProperty);
        }
        public static void SetStyleName(DependencyObject obj, string value)
        {
            FrameworkElement behavior = obj as FrameworkElement;
            //if (value == "value")
            //{

            //}
            behavior.Loaded += (s, e) =>
            {
                Control parent = behavior.TemplatedParent as Control;
                if (parent == null)
                    return;
                double max, max2;
                if (behavior.ActualHeight < behavior.ActualWidth)
                {
                    max = parent.ActualHeight;
                    max2 = behavior.ActualHeight;
                }
                else
                {
                    max = parent.ActualWidth;
                    max2 = behavior.ActualWidth;
                }

                double scale = 1 * (max / max2);
                behavior.CacheMode = new BitmapCache(scale) { SnapsToDevicePixels = false, EnableClearType = false };
            };
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyleNameProperty =
            DependencyProperty.RegisterAttached("StyleName", typeof(string), typeof(RenderControlToBitmap),
                new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SetStyleName(dependencyObject, (string)e.NewValue);
        }
    }
    //public class RenderControlToBitmap
    //{
    //    public static string GetStyleName(DependencyObject obj)
    //    {
    //        return (string)obj.GetValue(StyleNameProperty);
    //    }

    //    public static void SetStyleName(DependencyObject obj, string value)
    //    {
    //        Image behavior = obj as Image;
    //        behavior.Loaded += (s, e) =>
    //        {
    //            FrameworkElement parent = behavior.TemplatedParent as FrameworkElement;
    //            parent = parent.Parent as FrameworkElement;
    //            Brush foreground = Brushes.White;
    //            if (parent is UserControl)
    //                foreground = ((UserControl)parent).Foreground;
    //            Control control = new Control() { Foreground = foreground };
    //            control.Template = (ControlTemplate)Application.Current.Resources[value];
    //            if (double.IsNaN(parent.Width) || double.IsNaN(parent.Height))
    //            {
    //                if (parent.ActualWidth != 0 && parent.ActualHeight != 0)
    //                    behavior.Source = GetImage(control, parent.ActualWidth, parent.ActualHeight);
    //            }
    //            else
    //                behavior.Source = GetImage(control, parent.Width, parent.Height);
    //        };
    //        //FrameworkElement parent = behavior.TemplatedParent as FrameworkElement;

    //        //Func<FrameworkElement> CreateInstance = () =>
    //        //{
    //        //    Control control = new Control();
    //        //    control.Template = (ControlTemplate)Application.Current.Resources[value];
    //        //    return control;
    //        //};

    //        //Func<FrameworkElement, FrameworkElement> DrawControl = (s) =>
    //        //{
    //        //    if (double.IsNaN(parent.Width) || double.IsNaN(parent.Height))
    //        //    {
    //        //        if (parent.ActualWidth != 0 && parent.ActualHeight != 0)
    //        //        {
    //        //            behavior.Source = GetImage(CreateInstance(), parent.ActualWidth, parent.ActualHeight);
    //        //        }
    //        //        else
    //        //        {
    //        //            if ((s as FrameworkElement).TemplatedParent != null)
    //        //                parent = (s as FrameworkElement).TemplatedParent as FrameworkElement;
    //        //            else
    //        //                parent = (s as FrameworkElement).Parent as FrameworkElement;
    //        //            return parent;
    //        //        }
    //        //    }
    //        //    else
    //        //    {
    //        //        behavior.Source = GetImage(CreateInstance(), parent.Width, parent.Height);
    //        //    }
    //        //    return null;
    //        //};

    //        //Action<FrameworkElement> loaded = null;
    //        //loaded = (ctrl) =>
    //        //{
    //        //    ctrl.Loaded += (s, e) =>
    //        //    {
    //        //        try
    //        //        {
    //        //            FrameworkElement ct = DrawControl(s as FrameworkElement);
    //        //            if (ct != null)
    //        //            {
    //        //                parent = (ct as FrameworkElement).TemplatedParent as FrameworkElement;
    //        //                while (DrawControl(parent) != null)
    //        //                {
    //        //                }
    //        //            }

    //        //        }
    //        //        catch
    //        //        {

    //        //        }
    //        //    };
    //        //};
    //        //loaded(parent);
    //    }

    //    // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty StyleNameProperty =
    //        DependencyProperty.RegisterAttached("StyleName", typeof(string), typeof(RenderControlToBitmap),
    //            new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

    //    public static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    //    {
    //        SetStyleName(dependencyObject, (string)e.NewValue);
    //    }

    //    static BitmapFrame GetImage(FrameworkElement ctrl, double width, double height)
    //    {
    //        ctrl.Measure(new Size(width, height));
    //        ctrl.Arrange(new Rect(new Size(width, height)));
    //        RenderTargetBitmap bmp = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
    //        bmp.Render(ctrl);
    //        return BitmapFrame.Create(bmp);
    //    }
    //}
}
