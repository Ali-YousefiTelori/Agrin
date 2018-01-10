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
    public class LinkInfoItem : ANotifyPropertyChanged
    {
        public TaskInfo ParentTask { get; set; }

        public LinkInfo Link { get; set; }
        bool _IsChecked = false;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; OnPropertyChanged("IsChecked"); }
        }

        int _index = 0;
        public int Index
        {
            get { return _index; }
            set { _index = value; OnPropertyChanged("Index"); }
        }
    }

    public class AddTaskBaseViewModel : ANotifyPropertyChanged
    {
        public static Action BackItemClick { get; set; }

        string _TaskName;

        public string TaskName
        {
            get { return _TaskName; }
            set
            {
                _TaskName = value;
                OnPropertyChanged("TaskName");
            }
        }

        bool _IsNowDateTime = true;

        public bool IsNowDateTime
        {
            get { return _IsNowDateTime; }
            set
            {
                _IsNowDateTime = value;
                OnPropertyChanged("IsNowDateTime");
            }
        }

        bool _IsTodayDateTime;

        public bool IsTodayDateTime
        {
            get { return _IsTodayDateTime; }
            set
            {
                _IsTodayDateTime = value;
                OnPropertyChanged("IsTodayDateTime");
            }
        }

        bool _IsWeekdayDateTime;

        public bool IsWeekdayDateTime
        {
            get { return _IsWeekdayDateTime; }
            set
            {
                _IsWeekdayDateTime = value;
                OnPropertyChanged("IsWeekdayDateTime");
            }
        }

        bool _IsSelectDateTime;

        public bool IsSelectDateTime
        {
            get { return _IsSelectDateTime; }
            set
            {
                _IsSelectDateTime = value;
                OnPropertyChanged("IsSelectDateTime");
            }
        }

        int _minutes = 0;

        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                OnPropertyChanged("Minutes");
            }
        }

        int _Hours = 0;
        public int Hours
        {
            get { return _Hours; }
            set
            {
                _Hours = value;
                OnPropertyChanged("Hours");
            }
        }

        int _selectedTaskModeIndex = 0;

        public int SelectedTaskModeIndex
        {
            get { return _selectedTaskModeIndex; }
            set
            {
                _selectedTaskModeIndex = value;
                OnPropertyChanged("SelectedTaskModeIndex");
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

        #region Tasks

        bool _StartLinks = true;

        public bool StartLinks
        {
            get { return _StartLinks; }
            set
            {
                _StartLinks = value;
                OnPropertyChanged("StartLinks");
                if (value)
                    StopLinks = false;
                OnPropertyChanged("CanAddLinks");
            }
        }

        bool _StopLinks = false;

        public bool StopLinks
        {
            get { return _StopLinks; }
            set
            {
                _StopLinks = value;
                OnPropertyChanged("StopLinks");
                if (value)
                    StartLinks = false;
                OnPropertyChanged("CanAddLinks");
            }
        }

        bool _StartTasks = false;

        public bool StartTasks
        {
            get { return _StartTasks; }
            set
            {
                _StartTasks = value;
                OnPropertyChanged("StartTasks");
                if (value)
                {
                    StopTasks = false;
                    DeActiveTasks = false;
                }
                OnPropertyChanged("CanAddTasks");
            }
        }

        bool _StopTasks = false;

        public bool StopTasks
        {
            get { return _StopTasks; }
            set
            {
                _StopTasks = value;
                OnPropertyChanged("StopTasks");
                if (value)
                {
                    StartTasks = false;
                    DeActiveTasks = false;
                }
                OnPropertyChanged("CanAddTasks");
            }
        }

        bool _DeActiveTasks = false;

        public bool DeActiveTasks
        {
            get { return _DeActiveTasks; }
            set
            {
                _DeActiveTasks = value;
                OnPropertyChanged("DeActiveTasks");
                if (value)
                {
                    StartTasks = false;
                    StopTasks = false;
                }
                OnPropertyChanged("CanAddTasks");
            }
        }

        bool _IsSunday = false;

        public bool IsSunday
        {
            get { return _IsSunday; }
            set { _IsSunday = value; OnPropertyChanged("IsSunday"); }
        }

        bool _IsMonday = false;

        public bool IsMonday
        {
            get { return _IsMonday; }
            set { _IsMonday = value; OnPropertyChanged("IsMonday"); }
        }

        bool _IsTuesday = false;

        public bool IsTuesday
        {
            get { return _IsTuesday; }
            set { _IsTuesday = value; OnPropertyChanged("IsTuesday"); }
        }

        bool _IsWednesday = false;

        public bool IsWednesday
        {
            get { return _IsWednesday; }
            set { _IsWednesday = value; OnPropertyChanged("IsWednesday"); }
        }

        bool _IsThursday = false;

        public bool IsThursday
        {
            get { return _IsThursday; }
            set { _IsThursday = value; OnPropertyChanged("IsThursday"); }
        }

        bool _IsFriday = false;

        public bool IsFriday
        {
            get { return _IsFriday; }
            set { _IsFriday = value; OnPropertyChanged("IsFriday"); }
        }

        bool _IsSaturday = false;

        public bool IsSaturday
        {
            get { return _IsSaturday; }
            set { _IsSaturday = value; OnPropertyChanged("IsSaturday"); }
        }

        DateTime _SelectedDateTime = DateTime.Now;

        public DateTime SelectedDateTime
        {
            get { return _SelectedDateTime; }
            set { _SelectedDateTime = value; OnPropertyChanged("SelectedDateTime"); }
        }

        #endregion

        public bool CanAddLinks
        {
            get
            {
                return StopLinks || StartLinks;
            }
        }

        public bool CanAddTasks
        {
            get
            {
                return StopTasks || DeActiveTasks || StartTasks;
            }
        }


        public bool CanAddTask()
        {
            return !string.IsNullOrWhiteSpace(TaskName);
        }

        public void AddTask()
        {
            var listLinks = LinkSelections.OrderBy(x => x.Index).ToList();
            listLinks = (from x in listLinks where x.IsChecked select x).ToList();
            var task = new TaskInfo() { Name = TaskName, IsActive = true, TaskItemInfoes = listLinks.Select(x => x.Link.PathInfo.Id).Select<int, TaskItemInfo>(x => new TaskItemInfo() { Mode = TaskItemMode.LinkInfo, Value = x }).ToList() };
            task.IsStartNow = IsNowDateTime;
            if (IsTodayDateTime)
            {
                var dt = DateTime.Now;
                dt = dt.AddMinutes(-dt.Minute);
                dt = dt.AddHours(-dt.Hour);
                dt = dt.AddMinutes(Minutes);
                dt = dt.AddHours(Hours);
                task.DateTimes.Add(dt);
            }
            else if (IsWeekdayDateTime)
            {
                task.IsDayOfWeek = true;
                if (IsFriday)
                    task.DayOfWeek.Add(DayOfWeek.Friday);
                if (IsSunday)
                    task.DayOfWeek.Add(DayOfWeek.Sunday);
                if (IsMonday)
                    task.DayOfWeek.Add(DayOfWeek.Monday);
                if (IsSaturday)
                    task.DayOfWeek.Add(DayOfWeek.Saturday);
                if (IsThursday)
                    task.DayOfWeek.Add(DayOfWeek.Thursday);
                if (IsTuesday)
                    task.DayOfWeek.Add(DayOfWeek.Tuesday);
                if (IsWednesday)
                    task.DayOfWeek.Add(DayOfWeek.Wednesday);
                var dt = DateTime.Now;
                dt = dt.AddMinutes(-dt.Minute);
                dt = dt.AddHours(-dt.Hour);
                dt = dt.AddMinutes(Minutes);
                dt = dt.AddHours(Hours);
                task.DateTimes.Add(dt);
            }
            else if (IsSelectDateTime)
            {
                var dt = SelectedDateTime;
                dt = dt.AddMinutes(-dt.Minute);
                dt = dt.AddHours(-dt.Hour);
                dt = dt.AddMinutes(Minutes);
                dt = dt.AddHours(Hours);
                task.DateTimes.Add(dt);
            }

            if (StartLinks)
                task.TaskUtilityActions.Add(TaskUtilityModeEnum.StartLink);
            else if (StopLinks)
                task.TaskUtilityActions.Add(TaskUtilityModeEnum.StopLink);
            else if (StartTasks)
                task.TaskUtilityActions.Add(TaskUtilityModeEnum.ActiveTasks);
            else if (StartTasks)
                task.TaskUtilityActions.Add(TaskUtilityModeEnum.DeactiveTasks);
            ApplicationTaskManager.Current.AddTask(task);
            Back();
        }

        public void ClearItems()
        {
            RefreshLinksItems();
            TaskName = "";
        }

        public void Back()
        {
            ClearItems();
            if (BackItemClick != null)
                BackItemClick();
        }

        public void ShowSelectionLinksList()
        {
            RefreshLinksItems();
            IsShowSelectionLinks = true;
        }


        public void RefreshLinksItems()
        {
            var linkslist = ApplicationLinkInfoManager.Current.LinkInfoes.Where(x => !x.IsComplete).ToList();
            foreach (var item in LinkSelections.ToList())
            {
                if (linkslist.Contains(item.Link))
                {
                    linkslist.Remove(item.Link);
                }
                else
                {
                    LinkSelections.Remove(item);
                }
            }
            int i = linkslist.Count;
            foreach (var item in linkslist)
            {
                LinkSelections.Add(new LinkInfoItem() { IsChecked = false, Link = item, Index = i });
                i++;
            }
        }
    }
}
