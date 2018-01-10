namespace Agrin.ViewModels.UI.DragDrop
{
    using System;

    public interface IDropTarget
    {
        void DragOver(DropInfo dropInfo);
        void Drop(DropInfo dropInfo);
    }
}

