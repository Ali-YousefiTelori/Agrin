namespace Agrin.ViewModels.UI.DragDrop.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;

    public static class VisualTreeExtensions
    {
        public static T GetVisualAncestor<T>(this DependencyObject d) where T: class
        {
            for (DependencyObject obj2 = VisualTreeHelper.GetParent(d); obj2 != null; obj2 = VisualTreeHelper.GetParent(obj2))
            {
                T local = obj2 as T;
                if (local != null)
                {
                    return local;
                }
            }
            return default(T);
        }

        public static DependencyObject GetVisualAncestor(this DependencyObject d, Type type)
        {
            for (DependencyObject obj2 = VisualTreeHelper.GetParent(d); obj2 != null; obj2 = VisualTreeHelper.GetParent(obj2))
            {
                if (obj2.GetType() == type)
                {
                    return obj2;
                }
            }
            return null;
        }

        public static T GetVisualDescendent<T>(this DependencyObject d) where T: DependencyObject
        {
            return d.GetVisualDescendents<T>().FirstOrDefault<T>();
        }

        public static IEnumerable<T> GetVisualDescendents<T>(this DependencyObject d) where T: DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(d);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(d, i);
                if (child is T)
                {
                    yield return (T) child;
                }
                IEnumerator<T> enumerator = child.GetVisualDescendents<T>().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    T current = enumerator.Current;
                    yield return current;
                }
            }
        }

       
    }
}

