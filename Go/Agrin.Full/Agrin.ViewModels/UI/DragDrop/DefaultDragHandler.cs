namespace Agrin.ViewModels.UI.DragDrop
{
    using Agrin.ViewModels.UI.DragDrop.Utilities;
    using System;
    using System.Linq;
    using System.Windows;

    public class DefaultDragHandler : IDragSource
    {
        public virtual void StartDrag(DragInfo dragInfo)
        {
            int num = dragInfo.SourceItems.Cast<object>().Count<object>();
            if (num == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First<object>();
            }
            else if (num > 1)
            {
                dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            }
            dragInfo.Effects = (dragInfo.Data != null) ? (DragDropEffects.Move | DragDropEffects.Copy) : DragDropEffects.None;
        }
    }
}

