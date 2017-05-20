namespace Agrin.ViewModels.UI.DragDrop
{
    using Agrin.ViewModels.UI.DragDrop.Utilities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    internal class DefaultDropHandler : IDropTarget
    {
        protected static bool CanAcceptData(DropInfo dropInfo)
        {
            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                return (GetList(dropInfo.TargetCollection) != null);
            }
            if (dropInfo.DragInfo.SourceCollection is ItemCollection)
            {
                return false;
            }
            return (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data) && !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem));
        }

        public virtual void DragOver(DropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Copy;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }

        public virtual void Drop(DropInfo dropInfo)
        {
            int insertIndex = dropInfo.InsertIndex;
            IList list = GetList(dropInfo.TargetCollection);
            IEnumerable enumerable = ExtractData(dropInfo.Data);
            if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
            {
                IList list2 = GetList(dropInfo.DragInfo.SourceCollection);
                foreach (object obj2 in enumerable)
                {
                    int index = list2.IndexOf(obj2);
                    if (index != -1)
                    {
                        list2.RemoveAt(index);
                        if ((list2 == list) && (index < insertIndex))
                        {
                            insertIndex--;
                        }
                    }
                }
            }

            foreach (object obj2 in enumerable)
            {
                list.Insert(insertIndex++, obj2);
            }
        }

        protected static IEnumerable ExtractData(object data)
        {
            if (!(!(data is IEnumerable) || (data is string)))
            {
                return (IEnumerable) data;
            }
            return Enumerable.Repeat<object>(data, 1);
        }

        protected static IList GetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return (((ICollectionView) enumerable).SourceCollection as IList);
            }
            return (enumerable as IList);
        }

        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            for (ItemsControl control = ItemsControl.ItemsControlFromItemContainer(targetItem); control != null; control = ItemsControl.ItemsControlFromItemContainer(control))
            {
                if (control == sourceItem)
                {
                    return true;
                }
            }
            return false;
        }

        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            IEnumerable<Type> source = from i in target.GetType().FindInterfaces(filter, null) select i.GetGenericArguments().Single<Type>();
            if (source.Count<Type>() > 0)
            {
                Type dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return source.Any<Type>(t => t.IsAssignableFrom(dataType));
            }
            return (target is IList);
        }
    }
}

