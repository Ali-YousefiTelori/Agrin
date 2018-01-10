using Agrin.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Agrin.Windows.UI.Helpers
{
    public class UserControlHelper
    {
        public static string GetName(DependencyObject d)
        {
            return (string)d.GetValue(UserControlHelper.NameProperty);
        }

        public static void SetName(DependencyObject d, string val)
        {
            d.SetValue(UserControlHelper.NameProperty, val);
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name",
                typeof(string),
                typeof(UserControlHelper),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if (!string.IsNullOrEmpty((string)e.NewValue))
                        {
                            string[] names = e.NewValue.ToString().Split(new char[] { ',' });
                            if (d is FrameworkElement)
                            {
                                Type t = Type.GetType(names[1]);
                                if (t == null)
                                    return;
                                var parent = ViewsHelper.FindVisualParent(d, t);
                                if (parent == null)
                                    return;
                                var p = parent.GetType().GetProperty(names[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
                                p.SetValue(parent, d, null);
                            }
                        }
                    }));

        public static object GetDataContextToSource(DependencyObject d)
        {
            return d.GetValue(UserControlHelper.DataContextToSourceProperty);
        }

        public static void SetDataContextToSource(DependencyObject d, object val)
        {
            d.SetValue(UserControlHelper.DataContextToSourceProperty, val);
        }

        public static readonly DependencyProperty DataContextToSourceProperty =
            DependencyProperty.RegisterAttached("DataContextToSource",
                typeof(string),
                typeof(UserControlHelper),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if (e.NewValue != null)
                        {
                            string name = e.NewValue.ToString();
                            if (d is FrameworkElement)
                            {
                                var parent = ViewsHelper.FindVisualParent(d, name);
                                if (parent == null)
                                    return;
                                ((FrameworkElement)parent).DataContext = ((FrameworkElement)d).DataContext;
                            }
                        }
                    }));

        public static object GetDataContextToSourceChild(DependencyObject d)
        {
            return d.GetValue(UserControlHelper.DataContextToSourceChildProperty);
        }

        public static void SetDataContextToSourceChild(DependencyObject d, object val)
        {
            d.SetValue(UserControlHelper.DataContextToSourceChildProperty, val);
        }
        public static readonly DependencyProperty DataContextToSourceChildProperty =
            DependencyProperty.RegisterAttached("DataContextToSourceChild",
                typeof(string),
                typeof(UserControlHelper),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if (e.NewValue != null)
                        {
                            string name = e.NewValue.ToString();
                            if (d is FrameworkElement)
                            {
                                var parent = ViewsHelper.FindLastParent(d, name);
                                parent = ViewsHelper.FindChildUid(parent, name);
                                if (parent == null)
                                    return;
                                ((FrameworkElement)parent).DataContext = ((FrameworkElement)d).DataContext;
                            }
                        }
                    }));
    
    }
}
