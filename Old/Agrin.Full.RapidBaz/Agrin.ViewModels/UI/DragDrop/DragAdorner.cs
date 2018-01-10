namespace Agrin.ViewModels.UI.DragDrop
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    internal class DragAdorner : Adorner
    {
        private AdornerLayer m_AdornerLayer;
        private UIElement m_Adornment;
        private Point m_MousePosition;

        public DragAdorner(UIElement adornedElement, UIElement adornment) : base(adornedElement)
        {
            this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.m_AdornerLayer.Add(this);
            this.m_Adornment = adornment;
            base.IsHitTestVisible = false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.m_Adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public void Detatch()
        {
            this.m_AdornerLayer.Remove(this);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            return new GeneralTransformGroup { Children = { base.GetDesiredTransform(transform), new TranslateTransform(this.MousePosition.X - 4.0, this.MousePosition.Y - 4.0) } };
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.m_Adornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.m_Adornment.Measure(constraint);
            return this.m_Adornment.DesiredSize;
        }

        public Point MousePosition
        {
            get
            {
                return this.m_MousePosition;
            }
            set
            {
                if (this.m_MousePosition != value)
                {
                    this.m_MousePosition = value;
                    this.m_AdornerLayer.Update(base.AdornedElement);
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
    }
}

