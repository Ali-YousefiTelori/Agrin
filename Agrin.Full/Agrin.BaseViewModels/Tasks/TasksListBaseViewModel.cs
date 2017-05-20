using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Download.Web.Tasks;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Tasks
{
    public class TasksListBaseViewModel : ANotifyPropertyChanged
    {
        public FastCollection<TaskInfo> TaskInfoes
        {
            get
            {
                if (ApplicationTaskManager.Current == null)
                    return null;
                return ApplicationTaskManager.Current.TaskInfoes;
            }
        }

        FastCollection<LinkInfoItem> _LinkSelections;

        public FastCollection<LinkInfoItem> LinkSelections
        {
            get
            {
                if (_LinkSelections == null)
                    _LinkSelections = new FastCollection<LinkInfoItem>(ApplicationHelperBase.DispatcherThread);
                return _LinkSelections;
            }
            set { _LinkSelections = value; }
        }

        bool _IsShowSelectionLinks = false;

        public bool IsShowSelectionLinks
        {
            get { return _IsShowSelectionLinks; }
            set { _IsShowSelectionLinks = value; OnPropertyChanged("IsShowSelectionLinks"); }
        }

        void ActiveTask(TaskInfo task)
        {
            ApplicationTaskManager.Current.ActiveTask(task);
        }

        void DeActiveTask(TaskInfo task)
        {
            ApplicationTaskManager.Current.DeActiveTask(task);
        }


        public void ActiveOrDeActiveTask(TaskInfo task)
        {
            if (task.IsActive)
                DeActiveTask(task);
            else
                ActiveTask(task);
        }

        TaskInfo _SelectedTask { get; set; }
        public void ShowSelectionLinksList(TaskInfo task)
        {
            _SelectedTask = task;
            LinkSelections.Clear();
            RefreshLinksItems();
            IsShowSelectionLinks = true;
        }


        public void RefreshLinksItems()
        {
            var linkslist = ApplicationLinkInfoManager.Current.LinkInfoes.Where(x => !x.IsComplete).ToList();
            int i = 0;
            foreach (var item in linkslist)
            {
                LinkSelections.Add(new LinkInfoItem() { IsChecked = false, Link = item, Index = i });
                i++;
            }
            int index = 0;
            foreach (var item in _SelectedTask.TaskItemInfoes)
            {
                if (item.Mode != Download.Web.Tasks.TaskItemMode.LinkInfo)
                    continue;
                var sel = LinkSelections.Where(x => x.Link.PathInfo.Id == (int)item.Value).FirstOrDefault();
                if (sel != null)
                {
                    sel.IsChecked = true;
                    sel.Index = index;
                    index++;
                }
            }
            LinkSelections.SortByAscending(x => x.Index);
        }

        public void SaveSelectionLinks()
        {
            var listLinks = LinkSelections.OrderBy(x => x.Index).ToList();
            listLinks = (from x in listLinks where x.IsChecked select x).ToList();
            var newList = listLinks.Select(x => x.Link.PathInfo.Id).Select<int, TaskItemInfo>(x => new TaskItemInfo() { Mode = TaskItemMode.LinkInfo, Value = x }).ToList();
            ApplicationTaskManager.Current.ChangeListOfTask(_SelectedTask,newList);
            IsShowSelectionLinks = false;
            ApplicationTaskManager.Current.SaveData();
        }

        public static void DeleteTask(TaskInfo task)
        {
            ApplicationTaskManager.Current.RemoveTask(task);
        }
    }
}
