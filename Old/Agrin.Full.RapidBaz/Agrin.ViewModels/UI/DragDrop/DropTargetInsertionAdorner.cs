namespace Agrin.ViewModels.UI.DragDrop
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class DropTargetInsertionAdorner : DropTargetAdorner
    {
        private static Pen m_Pen = new Pen(Brushes.Gray, 2.0);
        private static PathGeometry m_Triangle;

        static DropTargetInsertionAdorner()
        {
            m_Pen.Freeze();
            LineSegment segment = new LineSegment(new Point(0.0, -3.0), false);
            segment.Freeze();
            LineSegment segment2 = new LineSegment(new Point(0.0, 3.0), false);
            segment2.Freeze();
            PathFigure figure = new PathFigure {
                StartPoint = new Point(3.0, 0.0)
            };
            figure.Segments.Add(segment);
            figure.Segments.Add(segment2);
            figure.Freeze();
            m_Triangle = new PathGeometry();
            m_Triangle.Figures.Add(figure);
            m_Triangle.Freeze();
        }

        public DropTargetInsertionAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(rotation));
            drawingContext.DrawGeometry(m_Pen.Brush, null, m_Triangle);
            drawingContext.Pop();
            drawingContext.Pop();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            ItemsControl visualTarget = base.DropInfo.VisualTarget as ItemsControl;
            if (visualTarget != null)
            {
                ItemsControl control2;
                if (base.DropInfo.VisualTargetItem != null)
                {
                    control2 = ItemsControl.ItemsControlFromItemContainer(base.DropInfo.VisualTargetItem);
                }
                else
                {
                    control2 = visualTarget;
                }
                int index = Math.Min(base.DropInfo.InsertIndex, control2.Items.Count - 1);
                UIElement element = (UIElement) control2.ItemContainerGenerator.ContainerFromIndex(index);
                if (element != null)
                {
                    Point point;
                    Point point2;
                    Rect rect = new Rect(element.TranslatePoint(new Point(), base.AdornedElement), element.RenderSize);
                    double rotation = 0.0;
                    if (base.DropInfo.VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (base.DropInfo.InsertIndex == control2.Items.Count)
                        {
                            rect.Y += element.RenderSize.Height;
                        }
                        point = new Point(rect.X, rect.Y);
                        point2 = new Point(rect.Right, rect.Y);
                    }
                    else
                    {
                        if (base.DropInfo.InsertIndex == control2.Items.Count)
                        {
                            rect.X += element.RenderSize.Width;
                        }
                        point = new Point(rect.X, rect.Y);
                        point2 = new Point(rect.X, rect.Bottom);
                        rotation = 90.0;
                    }
                    drawingContext.DrawLine(m_Pen, point, point2);
                    this.DrawTriangle(drawingContext, point, rotation);
                    this.DrawTriangle(drawingContext, point2, 180.0 + rotation);
                }
            }
        }
    }
}

