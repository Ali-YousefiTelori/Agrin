using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Agrin.ViewModels.ControlHelpers
{
    public class TabControlExpander
    {
        public static bool GetIsTabControlExpander(DependencyObject d)
        {
            return (bool)d.GetValue(TabControlExpander.IsTabControlExpanderProperty);
        }

        public static void SetIsTabControlExpander(DependencyObject d, bool val)
        {
            d.SetValue(TabControlExpander.IsTabControlExpanderProperty, val);
        }

        public static readonly DependencyProperty IsTabControlExpanderProperty =
            DependencyProperty.RegisterAttached("IsTabControlExpander",
                typeof(bool),
                typeof(TabControlExpander),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) =>
                    {
                        if ((bool)e.NewValue)
                        {
                            if (d is TabControl)
                            {
                                Initialize((TabControl)d);
                            }
                        }
                    }));

        static bool inited = false;
        static void Initialize(TabControl tabControl)
        {
            if (inited)
                return;
            _tabControl = tabControl;
            tabControl.Loaded += tabControl_Loaded;
        }

        static void tabControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (inited || sender == null )
                return;
            inited = true;
            foreach (TabItem item in ((TabControl)sender).Items)
            {
                item.ApplyTemplate();
                Grid header = (Grid)item.Template.FindName("Root", item);
                if (header == null)
                {
                    item.Loaded -= item_Loaded;
                    item.Loaded += item_Loaded;
                    return;
                }

                header.MouseDown += item_MouseDown;
                header.MouseUp += item_MouseUp;
            }
        }

        static void item_Loaded(object sender, RoutedEventArgs e)
        {
            TabItem item = sender as TabItem;
            item.ApplyTemplate();
            Grid header = (Grid)item.Template.FindName("Root", item);
            if (header != null)
            {
                header.MouseDown += item_MouseDown;
                header.MouseUp += item_MouseUp;
            }
        }

        static TabControl _tabControl = null;
        static void item_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender == capture && isSelected)
            {
                if (_tabControl.Height == 115)
                {
                    DoubleAnimation anim = new DoubleAnimation(115, 25, TimeSpan.FromMilliseconds(300));
                    _tabControl.BeginAnimation(FrameworkElement.HeightProperty, anim);
                }
                else if (_tabControl.Height == 25)
                {
                    DoubleAnimation anim = new DoubleAnimation(25, 115, TimeSpan.FromMilliseconds(300));
                    _tabControl.BeginAnimation(FrameworkElement.HeightProperty, anim);
                }
            }
        }

        static object capture = null;
        static bool isSelected = false;
        static void item_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            capture = sender;
            Grid item = (Grid)sender;
            TabItem tab = (TabItem)item.TemplatedParent;
            isSelected = tab.IsSelected;
        }
    }
}
