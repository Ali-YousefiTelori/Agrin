namespace Agrin.ViewModels.UI.DragDrop
{
    using Agrin.ViewModels.UI.DragDrop.Utilities;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    public static class DragDrop
    {
        public static readonly DependencyProperty DragAdornerTemplateProperty = DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate), typeof(Agrin.ViewModels.UI.DragDrop.DragDrop));
        public static readonly DependencyProperty DragHandlerProperty = DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(Agrin.ViewModels.UI.DragDrop.DragDrop));
        public static readonly DependencyProperty DropHandlerProperty = DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(Agrin.ViewModels.UI.DragDrop.DragDrop));
        public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(Agrin.ViewModels.UI.DragDrop.DragDrop), new UIPropertyMetadata(false, new PropertyChangedCallback(Agrin.ViewModels.UI.DragDrop.DragDrop.IsDragSourceChanged)));
        public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(Agrin.ViewModels.UI.DragDrop.DragDrop), new UIPropertyMetadata(false, new PropertyChangedCallback(Agrin.ViewModels.UI.DragDrop.DragDrop.IsDropTargetChanged)));
        private static IDragSource m_DefaultDragHandler;
        private static IDropTarget m_DefaultDropHandler;
        private static Agrin.ViewModels.UI.DragDrop.DragAdorner m_DragAdorner;
        private static DragInfo m_DragInfo;
        private static Agrin.ViewModels.UI.DragDrop.DropTargetAdorner m_DropTargetAdorner;
        private static DataFormat m_Format = DataFormats.GetDataFormat("Agrin.ViewModels.UI.DragDrop");

        private static void CreateDragAdorner()
        {
            DataTemplate dragAdornerTemplate = GetDragAdornerTemplate(m_DragInfo.VisualSource);
            if (dragAdornerTemplate != null)
            {
                UIElement content = (UIElement)Application.Current.MainWindow.Content;
                UIElement adornment = null;
                if ((m_DragInfo.Data is IEnumerable) && !(m_DragInfo.Data is string))
                {
                    if (((IEnumerable)m_DragInfo.Data).Cast<object>().Count<object>() <= 10)
                    {
                        ItemsControl control = new ItemsControl
                        {
                            ItemsSource = (IEnumerable)m_DragInfo.Data,
                            ItemTemplate = dragAdornerTemplate
                        };
                        Border border = new Border
                        {
                            Child = control
                        };
                        adornment = border;
                    }
                }
                else
                {
                    ContentPresenter presenter = new ContentPresenter
                    {
                        Content = m_DragInfo.Data,
                        ContentTemplate = dragAdornerTemplate
                    };
                    adornment = presenter;
                }
                if (adornment != null)
                {
                    adornment.Opacity = 0.5;
                    DragAdorner = new Agrin.ViewModels.UI.DragDrop.DragAdorner(content, adornment);
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
                return;
            if (sender is ItemsControl)
            {
                var itemsCtrl = sender as ItemsControl;
                dynamic slItems = itemsCtrl;
                int count = slItems.SelectedItems.Count;
                if (count == slItems.Items.Count)
                    return;
            }
            if (HitTestScrollBar(sender, e))
            {
                m_DragInfo = null;
            }
            else
            {
                m_DragInfo = new DragInfo(sender, e);
                ItemsControl itemsControl = sender as ItemsControl;
                if ((((m_DragInfo.VisualSourceItem != null) && (itemsControl != null)) && itemsControl.CanSelectMultipleItems()) && itemsControl.GetSelectedItems().Cast<object>().Contains<object>(m_DragInfo.SourceItem))
                {
                    e.Handled = true;
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (m_DragInfo != null)
            {
                m_DragInfo = null;
            }
        }

        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (m_DragInfo != null)
            {
                Point dragStartPosition = m_DragInfo.DragStartPosition;
                Point position = e.GetPosition(null);
                if ((Math.Abs((double)(position.X - dragStartPosition.X)) > SystemParameters.MinimumHorizontalDragDistance) || (Math.Abs((double)(position.Y - dragStartPosition.Y)) > SystemParameters.MinimumVerticalDragDistance))
                {
                    IDragSource dragHandler = GetDragHandler(m_DragInfo.VisualSource);
                    if (dragHandler != null)
                    {
                        dragHandler.StartDrag(m_DragInfo);
                    }
                    else
                    {
                        DefaultDragHandler.StartDrag(m_DragInfo);
                    }
                    if ((m_DragInfo.Effects != DragDropEffects.None) && (m_DragInfo.Data != null))
                    {
                        DataObject data = new DataObject(m_Format.Name, m_DragInfo.Data);
                        System.Windows.DragDrop.DoDragDrop(m_DragInfo.VisualSource, data, m_DragInfo.Effects);
                        m_DragInfo = null;
                    }
                }
                else
                {

                }
            }
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTarget_PreviewDragOver(sender, e);
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            DropTargetAdorner = null;
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            DropInfo dropInfo = new DropInfo(sender, e, m_DragInfo, m_Format.Name);
            IDropTarget dropHandler = GetDropHandler((UIElement)sender);
            if (dropHandler != null)
            {
                dropHandler.DragOver(dropInfo);
            }
            else
            {
                DefaultDropHandler.DragOver(dropInfo);
            }
            if (dropInfo.Effects != DragDropEffects.None)
            {
                if ((DragAdorner == null) && (m_DragInfo != null))
                {
                    CreateDragAdorner();
                }
                if (DragAdorner != null)
                {
                    DragAdorner.MousePosition = e.GetPosition(DragAdorner.AdornedElement);
                    DragAdorner.InvalidateVisual();
                }
            }
            else
            {
                DragAdorner = null;
            }
            if (sender is ItemsControl)
            {
                UIElement visualDescendent = ((ItemsControl)sender).GetVisualDescendent<ItemsPresenter>();
                if (dropInfo.DropTargetAdorner == null)
                {
                    DropTargetAdorner = null;
                }
                else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                {
                    DropTargetAdorner = Agrin.ViewModels.UI.DragDrop.DropTargetAdorner.Create(dropInfo.DropTargetAdorner, visualDescendent);
                }
                if (DropTargetAdorner != null)
                {
                    DropTargetAdorner.DropInfo = dropInfo;
                    DropTargetAdorner.InvalidateVisual();
                }
            }
            e.Effects = dropInfo.Effects;
            e.Handled = true;
            Scroll((DependencyObject)sender, e);
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            DropInfo dropInfo = new DropInfo(sender, e, m_DragInfo, m_Format.Name);
            IList selectionList = null;
            if (m_DragInfo.VisualSource != null)
            {
                dynamic d = m_DragInfo.VisualSource;
                var li = d.SelectedItems as IList;
                if (li != null)
                    selectionList = li.Cast<object>().ToList();
            }

            IDropTarget dropHandler = GetDropHandler((UIElement)sender);
            DragAdorner = null;
            DropTargetAdorner = null;
            if (dropHandler != null)
            {
                dropHandler.Drop(dropInfo);
            }
            else
            {
                DefaultDropHandler.Drop(dropInfo);
            }
            if (m_DragInfo.VisualSource != null && selectionList != null)
            {
                dynamic d = m_DragInfo.VisualSource;
                foreach (var item in selectionList)
                {
                    d.SelectedItems.Add(item);
                }
                foreach (var item in d.Items)
                {
                    item.Index = d.Items.IndexOf(item);
                }
            }
            e.Handled = true;
        }

        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }

        public static IDragSource GetDragHandler(UIElement target)
        {
            return (IDragSource)target.GetValue(DragHandlerProperty);
        }

        public static IDropTarget GetDropHandler(UIElement target)
        {
            return (IDropTarget)target.GetValue(DropHandlerProperty);
        }

        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        private static bool HitTestScrollBar(object sender, MouseButtonEventArgs e)
        {
            return (VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender)).VisualHit.GetVisualAncestor<ScrollBar>() != null);
        }

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            if ((bool)e.NewValue)
            {
                element.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DragSource_PreviewMouseLeftButtonDown);
                element.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DragSource_PreviewMouseLeftButtonUp);
                element.PreviewMouseMove += new MouseEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DragSource_PreviewMouseMove);
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DragSource_PreviewMouseLeftButtonDown);
                element.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DragSource_PreviewMouseLeftButtonUp);
                element.PreviewMouseMove -= new MouseEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DragSource_PreviewMouseMove);
            }
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            if ((bool)e.NewValue)
            {
                element.AllowDrop = true;
                element.PreviewDragEnter += new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDragEnter);
                element.PreviewDragLeave += new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDragLeave);
                element.PreviewDragOver += new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDragOver);
                element.PreviewDrop += new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDrop);
            }
            else
            {
                element.AllowDrop = false;
                element.PreviewDragEnter -= new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDragEnter);
                element.PreviewDragLeave -= new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDragLeave);
                element.PreviewDragOver -= new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDragOver);
                element.PreviewDrop -= new DragEventHandler(Agrin.ViewModels.UI.DragDrop.DragDrop.DropTarget_PreviewDrop);
            }
        }

        private static void Scroll(DependencyObject o, DragEventArgs e)
        {
            ScrollViewer visualDescendent = o.GetVisualDescendent<ScrollViewer>();
            if (visualDescendent != null)
            {
                Point position = e.GetPosition(visualDescendent);
                double num = Math.Min((double)(visualDescendent.FontSize * 2.0), (double)(visualDescendent.ActualHeight / 2.0));
                if ((position.X >= (visualDescendent.ActualWidth - num)) && (visualDescendent.HorizontalOffset < (visualDescendent.ExtentWidth - visualDescendent.ViewportWidth)))
                {
                    visualDescendent.LineRight();
                }
                else if ((position.X < num) && (visualDescendent.HorizontalOffset > 0.0))
                {
                    visualDescendent.LineLeft();
                }
                else if ((position.Y >= (visualDescendent.ActualHeight - num)) && (visualDescendent.VerticalOffset < (visualDescendent.ExtentHeight - visualDescendent.ViewportHeight)))
                {
                    visualDescendent.LineDown();
                }
                else if ((position.Y < num) && (visualDescendent.VerticalOffset > 0.0))
                {
                    visualDescendent.LineUp();
                }
            }
        }

        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        public static void SetDragHandler(UIElement target, IDragSource value)
        {
            target.SetValue(DragHandlerProperty, value);
        }

        public static void SetDropHandler(UIElement target, IDropTarget value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        public static IDragSource DefaultDragHandler
        {
            get
            {
                if (m_DefaultDragHandler == null)
                {
                    m_DefaultDragHandler = new Agrin.ViewModels.UI.DragDrop.DefaultDragHandler();
                }
                return m_DefaultDragHandler;
            }
            set
            {
                m_DefaultDragHandler = value;
            }
        }

        public static IDropTarget DefaultDropHandler
        {
            get
            {
                if (m_DefaultDropHandler == null)
                {
                    m_DefaultDropHandler = new Agrin.ViewModels.UI.DragDrop.DefaultDropHandler();
                }
                return m_DefaultDropHandler;
            }
            set
            {
                m_DefaultDropHandler = value;
            }
        }

        private static Agrin.ViewModels.UI.DragDrop.DragAdorner DragAdorner
        {
            get
            {
                return m_DragAdorner;
            }
            set
            {
                if (m_DragAdorner != null)
                {
                    m_DragAdorner.Detatch();
                }
                m_DragAdorner = value;
            }
        }

        private static Agrin.ViewModels.UI.DragDrop.DropTargetAdorner DropTargetAdorner
        {
            get
            {
                return m_DropTargetAdorner;
            }
            set
            {
                if (m_DropTargetAdorner != null)
                {
                    m_DropTargetAdorner.Detatch();
                }
                m_DropTargetAdorner = value;
            }
        }
    }
}

