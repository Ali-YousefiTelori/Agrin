namespace Agrin.ViewModels.UI.DragDrop
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Documents;

    public abstract class DropTargetAdorner : Adorner
    {
        private AdornerLayer m_AdornerLayer;

        public DropTargetAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.m_AdornerLayer.Add(this);
            base.IsHitTestVisible = false;
        }

        internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("The requested adorner class does not derive from DropTargetAdorner.");
            }
            return (DropTargetAdorner) type.GetConstructor(new Type[] { typeof(UIElement) }).Invoke(new UIElement[] { adornedElement });
        }

        public void Detatch()
        {
            this.m_AdornerLayer.Remove(this);
        }

        public Agrin.ViewModels.UI.DragDrop.DropInfo DropInfo { get; set; }
    }
}

