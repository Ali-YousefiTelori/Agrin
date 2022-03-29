using Agrin.Download.Data;
using Agrin.Download.Engine;
using Agrin.Download.Web;
using Agrin.Download.Web.Tasks;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Manager
{
    public class ApplicationTaskManager
    {
        public ApplicationTaskManager(Action<TaskInfo> startAction, Action<TaskInfo> comepleteAction)
        {
            StartAction = (task) =>
            {
                startAction(task);
                AutoLogger.LogTextTest($"ApplicationTaskManager.StartAction {task.TaskMode.First()} {ActivatedTasks.ContainsKey(task)}");

                if (task.TaskMode.First() == TaskModeEnum.Utility)
                {
                    if (UserCompleteAction != null)
                        UserCompleteAction();

                    ComepleteAction?.Invoke(task);
                    if (ActivatedTasks.ContainsKey(task))
                    {
                        ActivatedTasks[task].Dispose();
                        ActivatedTasks[task] = new TaskRunner(task);
                        ActivatedTasks[task].Start();
                    }
                }
                else
                {
                    //must start taskDownloadManager
                    if (ActivatedTasks.ContainsKey(task))
                    {
                        var runner = ActivatedTasks[task];
                        runner.CompleteDownloadAction = () =>
                        {
                            CompleteAndRestartTaskRunner(task);
                        };
                        runner.StartDownloader();
                    }
                }
            };

            ComepleteAction = comepleteAction;
        }

        public void CompleteAndRestartTaskRunner(TaskInfo task)
        {
            UserCompleteAction?.Invoke();
            ComepleteAction?.Invoke(task);
            if (ActivatedTasks.ContainsKey(task))
            {
                ActivatedTasks[task].Dispose();
                ActivatedTasks[task] = new TaskRunner(task);
                ActivatedTasks[task].Start();
            }
        }

        private static ApplicationTaskManager _Current;
        public static ApplicationTaskManager Current
        {
            get { return ApplicationTaskManager._Current; }
            set
            {
                ApplicationTaskManager._Current = value;
            }
        }

        TaskInfo _MainLinkTask = null;
        public TaskInfo MainLinkTask
        {
            get
            {
                lock (lockObj)
                {
                    if (_MainLinkTask == null)
                    {
                        foreach (var item in TaskInfoes.ToArray())
                        {
                            if (item.IsMainTask)
                            {
                                _MainLinkTask = item;
                                break;
                            }
                        }
                        if (_MainLinkTask == null)
                        {
                            var task = new TaskInfo() { Name = "صف اصلی", IsStartNow = true, IsActive = true, IsMainTask = true };
                            task.TaskUtilityActions.Add(TaskUtilityModeEnum.StartLink);
                            AddTask(task);
                            _MainLinkTask = task;
                        }
                    }

                    return _MainLinkTask;
                }
            }
        }

        public Action UserCompleteAction { get; set; }
        public Action<TaskInfo> ComepleteAction { get; set; }
        internal Action<TaskInfo> StartAction { get; set; }

        FastCollection<TaskInfo> _TaskInfoes;
        public FastCollection<TaskInfo> TaskInfoes
        {
            get
            {
                if (_TaskInfoes == null)
                    _TaskInfoes = new FastCollection<TaskInfo>(ApplicationHelperBase.DispatcherThread);
                return _TaskInfoes;
            }
            set { _TaskInfoes = value; }
        }

        private Dictionary<TaskInfo, TaskRunner> ActivatedTasks = new Dictionary<TaskInfo, TaskRunner>();

        public bool IsDeserializing { get; set; }
        object lockObj = new object();
        public void AddTask(TaskInfo taskInfo, bool isDeserializing = false)
        {
            lock (lockObj)
            {
                taskInfo.ID = GetNewID();
                if (isDeserializing)
                    TaskInfoes.Add(taskInfo);
                else
                    TaskInfoes.Insert(0, taskInfo);
                SetLinkInfoesTaskValidation(taskInfo);
                if (taskInfo.IsActive)
                    ActiveTask(taskInfo);
                SaveData();
            }
        }

        public void SetLinkInfoesTaskValidation(TaskInfo taskInfo)
        {
            foreach (var item in taskInfo.TaskItemInfoes)
            {
                if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                {
                    LinkInfo link = ApplicationLinkInfoManager.Current.FindLinkInfoByID(item.Value);
                    if (link != null)
                    {
                        if (taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink))
                            link.IsAddedToTaskForStart = true;
                        else if (taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink) || taskInfo.TaskUtilityActions.Contains(TaskUtilityModeEnum.DeactiveTasks))
                            link.IsAddedToTaskForStop = true;
                        link.Management.ChangedTaskInfo();
                    }
                }
            }
        }

        public void ChangedLinkInfoTaskValidation(LinkInfo linkInfo)
        {
            bool isStart = false, isStop = false;
            foreach (var task in TaskInfoes.ToArray())
            {
                foreach (var item in task.TaskItemInfoes)
                {
                    if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                    {
                        LinkInfo link = ApplicationLinkInfoManager.Current.FindLinkInfoByID(item.Value);
                        if (link != null && linkInfo.PathInfo.Id == link.PathInfo.Id)
                        {
                            if (task.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink))
                                isStart = true;
                            else if (task.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink) || task.TaskUtilityActions.Contains(TaskUtilityModeEnum.DeactiveTasks))
                                isStop = true;
                        }
                    }
                    if (isStart && isStop)
                        break;
                }
                if (isStart && isStop)
                    break;
            }
            linkInfo.IsAddedToTaskForStart = isStart;
            linkInfo.IsAddedToTaskForStop = isStop;
            linkInfo.Management.ChangedTaskInfo();
        }

        public void ClearNoItemsTask()
        {
            foreach (var task in TaskInfoes.ToArray())
            {
                if (task.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink) || task.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink) || task.TaskUtilityActions.Contains(TaskUtilityModeEnum.DeactiveTasks))
                {
                    if (task.TaskItemInfoes.Count == 0)
                        RemoveTask(task);
                }
            }
        }

        public void RemoveLinkInfoFromStopedTasks(LinkInfo linkInfo)
        {
            List<TaskInfo> changed = new List<TaskInfo>();
            foreach (var task in TaskInfoes.ToArray())
            {
                if (task.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink) || task.TaskUtilityActions.Contains(TaskUtilityModeEnum.DeactiveTasks))
                {
                    foreach (var item in task.TaskItemInfoes.ToArray())
                    {
                        if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                        {
                            if (linkInfo.PathInfo.Id == item.Value)
                            {
                                task.TaskItemInfoes.Remove(item);
                                changed.Add(task);
                            }
                        }
                    }
                }
            }

            linkInfo.IsAddedToTaskForStop = false;
            ClearNoItemsTask();
            SerializeData.SaveApplicationTaskToFile();
            linkInfo.Management.ChangedTaskInfo();
            ChangedAllTaskItemsIndexes(changed);
        }

        public void RemoveLinkInfoFromStartTasks(LinkInfo linkInfo)
        {
            List<TaskInfo> changed = new List<TaskInfo>();
            foreach (var task in TaskInfoes.ToArray())
            {
                if (!task.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink))
                    continue;
                foreach (var item in task.TaskItemInfoes.ToArray())
                {
                    if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                    {
                        if (linkInfo.PathInfo.Id == item.Value)
                        {
                            task.TaskItemInfoes.Remove(item);
                            changed.Add(task);
                        }
                    }
                }
            }

            linkInfo.IsAddedToTaskForStart = false;
            ClearNoItemsTask();
            SerializeData.SaveApplicationTaskToFile();
            linkInfo.Management.ChangedTaskInfo();
            ChangedAllTaskItemsIndexes(changed);
        }

        public void ChangedAllTaskItemsIndexes(List<TaskInfo> tasks)
        {
            foreach (var task in tasks)
            {
                foreach (var item in task.TaskItemInfoes.ToArray())
                {
                    if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                    {
                        LinkInfo linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID(item.Value);
                        if (linkInfo != null)
                            linkInfo.Management.ChangedTaskInfo();
                    }
                }
            }
        }

        public void RemoveTaskChangedValidation(TaskInfo taskInfo)
        {
            foreach (var item in taskInfo.TaskItemInfoes)
            {
                if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                {
                    LinkInfo link = ApplicationLinkInfoManager.Current.FindLinkInfoByID(item.Value);
                    if (link != null)
                    {
                        ChangedLinkInfoTaskValidation(link);
                    }
                }
            }
        }

        public List<TaskInfo> FindTaskInfoByLinkInfo(LinkInfo linkInfo)
        {
            List<TaskInfo> list = new List<TaskInfo>();
            foreach (var task in TaskInfoes.ToArray())
            {
                foreach (var item in task.TaskItemInfoes.ToArray())
                {
                    if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                    {
                        if (linkInfo.PathInfo.Id == item.Value)
                        {
                            list.Add(task);
                            break;
                        }
                    }
                }
            }
            return list;
        }

        public List<TaskInfo> FindTaskInfoByLinkInfoID(int id)
        {
            List<TaskInfo> list = new List<TaskInfo>();
            foreach (var task in TaskInfoes.ToArray())
            {
                foreach (var item in task.TaskItemInfoes.ToArray())
                {
                    if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                    {
                        if (id == item.Value)
                        {
                            list.Add(task);
                            break;
                        }
                    }
                }
            }
            return list;
        }

        public int FindLinkInfoIndexFromTaskInfo(LinkInfo linkInfo, TaskInfo task)
        {
            int index = 1;
            foreach (var item in task.TaskItemInfoes.ToArray())
            {
                if (item.Mode == Web.Tasks.TaskItemMode.LinkInfo)
                {
                    if (linkInfo.PathInfo.Id ==item.Value)
                    {
                        return index;
                    }
                    index++;
                }
            }

            return 0;
        }

        public void ChangeListOfTask(TaskInfo taskInfo, List<TaskItemInfo> taskItems)
        {
            var changedList = taskItems.ToList();
            Func<TaskItemInfo, bool> existInNewList = (item) =>
            {
                foreach (var changed in changedList)
                {
                    if (changed.Mode == TaskItemMode.LinkInfo && changed.Value == item.Value)
                        return true;
                }
                return false;
            };
            foreach (var item in taskInfo.TaskItemInfoes)
            {
                if (item.Mode == TaskItemMode.LinkInfo)
                {
                    var link = ApplicationLinkInfoManager.Current.FindLinkInfoByID(item.Value);
                    if (link != null)
                        ChangedLinkInfoTaskValidation(link);
                }
            }
            taskInfo.TaskItemInfoes = taskItems;
            foreach (var item in changedList.ToArray())
            {
                if (item.Mode == TaskItemMode.LinkInfo)
                {
                    if (existInNewList(item))
                        changedList.Remove(item);
                }
            }
        }

        public TaskInfo MoveUpFromTask(LinkInfo linkInfo)
        {
            var taskInfo = FindTaskInfoByLinkInfo(linkInfo).FirstOrDefault();
            foreach (var item in taskInfo.TaskItemInfoes.ToList())
            {
                if (item.Mode == TaskItemMode.LinkInfo)
                {
                    if (item.Value == linkInfo.PathInfo.Id)
                    {
                        if (taskInfo.TaskItemInfoes.Last() == item)
                        {
                            taskInfo.TaskItemInfoes.Remove(item);
                            taskInfo.TaskItemInfoes.Insert(0, item);
                        }
                        else
                        {
                            int index = taskInfo.TaskItemInfoes.IndexOf(item) + 1;
                            taskInfo.TaskItemInfoes.Remove(item);
                            taskInfo.TaskItemInfoes.Insert(index, item);
                        }
                    }
                }
            }
            return taskInfo;
        }

        public TaskInfo MoveDownFromTask(LinkInfo linkInfo)
        {
            var taskInfo = FindTaskInfoByLinkInfo(linkInfo).FirstOrDefault();
            foreach (var item in taskInfo.TaskItemInfoes.ToList())
            {
                if (item.Mode == TaskItemMode.LinkInfo)
                {
                    if (item.Value == linkInfo.PathInfo.Id)
                    {
                        if (taskInfo.TaskItemInfoes.First() == item)
                        {
                            taskInfo.TaskItemInfoes.Remove(item);
                            taskInfo.TaskItemInfoes.Add(item);
                        }
                        else
                        {
                            int index = taskInfo.TaskItemInfoes.IndexOf(item) - 1;
                            taskInfo.TaskItemInfoes.Remove(item);
                            taskInfo.TaskItemInfoes.Insert(index, item);
                        }

                    }
                }
            }
            return taskInfo;
        }

        public int GetNewID()
        {
            int id = TaskInfoes.Count > 0 ? TaskInfoes.Max(x => x.ID) : 0;
            id++;
            return id;
        }

        public void RemoveTask(TaskInfo taskInfo)
        {
            lock (lockObj)
            {
                TaskInfoes.Remove(taskInfo);
                RemoveTaskChangedValidation(taskInfo);
                if (ActivatedTasks.ContainsKey(taskInfo))
                    DeActiveTask(taskInfo);
                if (taskInfo.IsMainTask)
                    _MainLinkTask = null;
                SaveData();
            }
        }

        public void ActiveTask(TaskInfo taskInfo)
        {
            AutoLogger.LogTextTest($"ActiveTask {taskInfo.Name} {ActivatedTasks.ContainsKey(taskInfo)}");
            if (!ActivatedTasks.ContainsKey(taskInfo))
            {
                taskInfo.SkippedItems.Clear();
                taskInfo.IsActive = true;
                ActivatedTasks.Add(taskInfo, null);
                ActivatedTasks[taskInfo] = new TaskRunner(taskInfo);
                ActivatedTasks[taskInfo].Start();
            }
            SaveData();
        }

        public void DeActiveTask(TaskInfo taskInfo)
        {
            AutoLogger.LogTextTest($"DeActiveTask {taskInfo.Name} {ActivatedTasks.ContainsKey(taskInfo)}");
            if (ActivatedTasks.ContainsKey(taskInfo))
            {
                ActivatedTasks[taskInfo].Dispose();
                ActivatedTasks.Remove(taskInfo);
                taskInfo.IsActive = false;
            }
            SaveData();
        }

        public TaskRunner GetTaskRunnerOfTaskInfo(TaskInfo taskInfo)
        {
            if (ActivatedTasks.ContainsKey(taskInfo))
                return ActivatedTasks[taskInfo];
            return null;
        }

        public void SaveData()
        {
            if (!IsDeserializing)
                SerializeData.SaveApplicationTaskToFile();
        }
    }
}
