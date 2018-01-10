namespace Agrin.ViewModels.UI.DragDrop.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public static class ItemsControlExtensions
    {
        public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector)
            {
                return (bool) itemsControl.GetType().GetProperty("CanSelectMultipleItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(itemsControl, null);
            }
            return ((itemsControl is ListBox) && (((ListBox) itemsControl).SelectionMode != SelectionMode.Single));
        }

        public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
        {
            Type itemContainerType = itemsControl.GetItemContainerType();
            if (itemContainerType != null)
            {
                return (UIElement) child.GetVisualAncestor(itemContainerType);
            }
            return null;
        }

        public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
        {
            UIElement child = itemsControl.InputHitTest(position) as UIElement;
            if (child != null)
            {
                return itemsControl.GetItemContainer(child);
            }
            return null;
        }

        public static Type GetItemContainerType(this ItemsControl itemsControl)
        {
            if (itemsControl.Items.Count > 0)
            {
                IEnumerable<ItemsPresenter> visualDescendents = itemsControl.GetVisualDescendents<ItemsPresenter>();
                foreach (ItemsPresenter presenter in visualDescendents)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(presenter, 0), 0);
                    if ((child != null) && (itemsControl.ItemContainerGenerator.IndexFromContainer(child) != -1))
                    {
                        return child.GetType();
                    }
                }
            }
            return null;
        }

        public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
        {
            DependencyObject child = VisualTreeHelper.GetChild(itemsControl.GetVisualDescendent<ItemsPresenter>(), 0);
            PropertyInfo property = child.GetType().GetProperty("Orientation", typeof(Orientation));
            if (property != null)
            {
                return (Orientation) property.GetValue(child, null);
            }
            return Orientation.Vertical;
        }

        public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector)
            {
                return ((MultiSelector) itemsControl).SelectedItems;
            }
            if (itemsControl is ListBox)
            {
                ListBox box = (ListBox) itemsControl;
                if (box.SelectionMode == SelectionMode.Single)
                {
                    return Enumerable.Repeat<object>(box.SelectedItem, 1);
                }
                return box.SelectedItems;
            }
            if (itemsControl is TreeView)
            {
                return Enumerable.Repeat<object>(((TreeView) itemsControl).SelectedItem, 1);
            }
            if (itemsControl is Selector)
            {
                return Enumerable.Repeat<object>(((Selector) itemsControl).SelectedItem, 1);
            }
            return Enumerable.Empty<object>();
        }
    }
}

