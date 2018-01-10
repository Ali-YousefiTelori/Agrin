namespace Agrin.ViewModels.UI.DragDrop
{
    using Agrin.ViewModels.UI.DragDrop.Utilities;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    public class DropInfo
    {
        public DropInfo(object sender, DragEventArgs e, Agrin.ViewModels.UI.DragDrop.DragInfo dragInfo, string dataFormat)
        {
            this.Data = e.Data.GetDataPresent(dataFormat) ? e.Data.GetData(dataFormat) : e.Data;
            this.DragInfo = dragInfo;
            this.VisualTarget = sender as UIElement;
            if (sender is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl) sender;
                UIElement itemContainerAt = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl));
                this.VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
                if (itemContainerAt != null)
                {
                    ItemsControl control2 = ItemsControl.ItemsControlFromItemContainer(itemContainerAt);
                    this.InsertIndex = control2.ItemContainerGenerator.IndexFromContainer(itemContainerAt);
                    this.TargetCollection = control2.ItemsSource ?? control2.Items;
                    this.TargetItem = control2.ItemContainerGenerator.ItemFromContainer(itemContainerAt);
                    this.VisualTargetItem = itemContainerAt;
                    if (this.VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (e.GetPosition(itemContainerAt).Y > (itemContainerAt.RenderSize.Height / 2.0))
                        {
                            this.InsertIndex++;
                        }
                    }
                    else if (e.GetPosition(itemContainerAt).X > (itemContainerAt.RenderSize.Width / 2.0))
                    {
                        this.InsertIndex++;
                    }
                }
                else
                {
                    this.TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    this.InsertIndex = itemsControl.Items.Count;
                }
            }
        }

        public object Data { get; private set; }

        public Agrin.ViewModels.UI.DragDrop.DragInfo DragInfo { get; private set; }

        public Type DropTargetAdorner { get; set; }

        public DragDropEffects Effects { get; set; }

        public int InsertIndex { get; private set; }

        public IEnumerable TargetCollection { get; private set; }

        public object TargetItem { get; private set; }

        public UIElement VisualTarget { get; private set; }

        public UIElement VisualTargetItem { get; private set; }

        public Orientation VisualTargetOrientation { get; private set; }
    }
}

