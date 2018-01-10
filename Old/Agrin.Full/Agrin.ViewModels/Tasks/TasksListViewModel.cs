using Agrin.BaseViewModels.Tasks;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Download.Web.Tasks;
using Agrin.Helper.Collections;
using Agrin.ViewModels.Helper.ComponentModel;
using Agrin.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Agrin.ViewModels.Tasks
{
    public class TasksListViewModel : TasksListBaseViewModel
    {
        public TasksListViewModel()
        {
            UpItemCommand = new RelayCommand<ListBoxItem>(UpItem);
            DownItemCommand = new RelayCommand<ListBoxItem>(DownItem);
            ActiveOrDeActiveTaskCommand = new RelayCommand<TaskInfo>(ActiveOrDeActiveTask);
            ShowSelectionLinksListCommand = new RelayCommand<Grid>(ShowSelectionLinksList);
            SaveSelectionLinksCommand = new RelayCommand(SaveSelectionLinksParent);
        }

        public RelayCommand<ListBoxItem> UpItemCommand { get; set; }
        public RelayCommand<ListBoxItem> DownItemCommand { get; set; }
        public RelayCommand<TaskInfo> ActiveOrDeActiveTaskCommand { get; set; }
        public RelayCommand<Grid> ShowSelectionLinksListCommand { get; set; }
        public RelayCommand SaveSelectionLinksCommand { get; set; }

        private void ShowSelectionLinksList(Grid obj)
        {
            base.ShowSelectionLinksList(obj.DataContext as TaskInfo);
            _CurrentListBox = obj.Children[0] as ListBox;
        }

        ListBox _CurrentListBox { get; set; }
        private void SaveSelectionLinksParent()
        {
            base.SaveSelectionLinks();
            BindingOperations.GetBindingExpression(_CurrentListBox, ListBox.ItemsSourceProperty).UpdateTarget();
        }

        TaskItemInfo FindItemByLinkInfoItem(LinkInfoItem linkInfo)
        {
            foreach (var item in linkInfo.ParentTask.TaskItemInfoes)
            {
                if (item.Mode == TaskItemMode.LinkInfo && item.Value == linkInfo.Link.PathInfo.Id)
                    return item;
            }
            return null;
        }

        private void DownItem(ListBoxItem LItem)
        {
            var listBox = ViewsHelper.FindVisualParent(LItem, typeof(ListBox));
            var item = LItem.DataContext as LinkInfoItem;
            int id = item.Link.PathInfo.Id;
            var taskItem = FindItemByLinkInfoItem(item);
            int oldIndex = item.ParentTask.TaskItemInfoes.IndexOf(taskItem);
            int newIndex = 0;
            if (oldIndex != item.ParentTask.TaskItemInfoes.Count - 1)
                newIndex = oldIndex + 1;
            else
                newIndex = 0;
            item.ParentTask.TaskItemInfoes.RemoveAt(oldIndex);
            item.ParentTask.TaskItemInfoes.Insert(newIndex, taskItem);
            BindingOperations.GetBindingExpression(listBox, ListBox.ItemsSourceProperty).UpdateTarget();
            ApplicationTaskManager.Current.SaveData();
        }

        private void UpItem(ListBoxItem LItem)
        {
            var listBox = ViewsHelper.FindVisualParent(LItem, typeof(ListBox));
            var item = LItem.DataContext as LinkInfoItem;
            int id = item.Link.PathInfo.Id;
            var taskItem = FindItemByLinkInfoItem(item);
            int oldIndex = item.ParentTask.TaskItemInfoes.IndexOf(taskItem);
            int newIndex = 0;
            if (oldIndex != 0)
                newIndex = oldIndex - 1;
            else
                newIndex = item.ParentTask.TaskItemInfoes.Count - 1;
            item.ParentTask.TaskItemInfoes.RemoveAt(oldIndex);
            item.ParentTask.TaskItemInfoes.Insert(newIndex, taskItem);
            BindingOperations.GetBindingExpression(listBox, ListBox.ItemsSourceProperty).UpdateTarget();
            ApplicationTaskManager.Current.SaveData();
        }
    }
}
