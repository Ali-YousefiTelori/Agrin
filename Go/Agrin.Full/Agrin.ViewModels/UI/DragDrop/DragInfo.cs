namespace Agrin.ViewModels.UI.DragDrop
{
    using Agrin.ViewModels.UI.DragDrop.Utilities;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class DragInfo
    {
        public DragInfo(object sender, MouseButtonEventArgs e)
        {
            this.DragStartPosition = e.GetPosition(null);
            this.Effects = DragDropEffects.None;
            this.MouseButton = e.ChangedButton;
            this.VisualSource = sender as UIElement;
            if (sender is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl) sender;
                UIElement itemContainer = itemsControl.GetItemContainer((UIElement) e.OriginalSource);
                if (itemContainer != null)
                {
                    ItemsControl control2 = ItemsControl.ItemsControlFromItemContainer(itemContainer);
                    this.SourceCollection = control2.ItemsSource ?? control2.Items;
                    this.SourceItem = control2.ItemContainerGenerator.ItemFromContainer(itemContainer);
                    this.SourceItems = itemsControl.GetSelectedItems();
                    if (this.SourceItems.Cast<object>().Count<object>() <= 1)
                    {
                        this.SourceItems = Enumerable.Repeat<object>(this.SourceItem, 1);
                    }
                    this.VisualSourceItem = itemContainer;
                }
                else
                {
                    this.SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                }
            }
            if (this.SourceItems == null)
            {
                this.SourceItems = Enumerable.Empty<object>();
            }
        }

        public object Data { get; set; }

        public Point DragStartPosition { get; private set; }

        public DragDropEffects Effects { get; set; }

        public System.Windows.Input.MouseButton MouseButton { get; private set; }

        public IEnumerable SourceCollection { get; private set; }

        public object SourceItem { get; private set; }

        public IEnumerable SourceItems { get; private set; }

        public UIElement VisualSource { get; private set; }

        public UIElement VisualSourceItem { get; private set; }
    }
}

