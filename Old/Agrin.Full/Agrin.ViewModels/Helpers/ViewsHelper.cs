using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Agrin.ViewModels.Helpers
{
    public static class ViewsHelper
    {
        public static DependencyObject FindVisualParent(DependencyObject child, Type t)
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null)
            {
                var p = ((FrameworkElement)child).Parent;
                if (p == null)
                    return null;
                parentObject = p;
            }

            // check if the parent matches the type we’re looking for
            DependencyObject parent = parentObject.GetType() == t ? parentObject : null;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent(parentObject, t);
            }
        }

        public static UIElement FindChildUid(DependencyObject parent, string uid)
        {
            var lp = parent;
            while (parent is ContentControl)
            {
                parent = ((ContentControl)parent).Content as DependencyObject;
            }
            if (parent == null)
            {
                return null;
            }
            var count = VisualTreeHelper.GetChildrenCount(parent);
            if (count == 0) return null;

            for (int i = 0; i < count; i++)
            {
                var el = VisualTreeHelper.GetChild(parent, i) as UIElement;
                if (el == null) continue;

                if (el.Uid == uid) return el;

                el = FindChildUid(el, uid);
                if (el != null) return el;
            }
            return null;
        }

        public static DependencyObject FindVisualParent(DependencyObject child, string name)
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null)
            {
                var p = ((FrameworkElement)child).Parent;
                if (p == null)
                    return null;
                parentObject = p;
            }

            // check if the parent matches the type we’re looking for
            DependencyObject parent = ((FrameworkElement)parentObject).Name == name || ((FrameworkElement)parentObject).Uid == name ? parentObject : null;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent(parentObject, name);
            }
        }

        public static DependencyObject FindLastParent(DependencyObject child, string name)
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null)
            {
                var p = ((FrameworkElement)child).Parent;
                if (p == null)
                    return child;
                parentObject = p;

            }

            // check if the parent matches the type we’re looking for
            DependencyObject parent = ((FrameworkElement)parentObject).Name == name || ((FrameworkElement)parentObject).Uid == name ? parentObject : null;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindLastParent(parentObject, name);
            }
        }
    }
}
