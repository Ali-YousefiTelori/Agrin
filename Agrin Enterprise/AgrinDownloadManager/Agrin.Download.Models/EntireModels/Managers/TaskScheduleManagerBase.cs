using Agrin.Download.CoreModels.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.EntireModels.Managers
{
    public abstract class TaskScheduleManagerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public static TaskScheduleManagerBase Current { get; set; }


        public abstract List<TaskSchedulerInfo> TaskSchedulerInfoes { get; }

        public abstract List<TaskSchedulerInfo> GetTaskSchedulerInfoesByLinkId(int linkId);

        public void RemoveLinkIdFromEveryTaskSchedulerInfoes(IEnumerable<int> linkIds)
        {
            foreach (var linkId in linkIds)
            {
                foreach (var taskScheduler in GetTaskSchedulerInfoesByLinkId(linkId))
                {
                    taskScheduler.TasksInStart.RemoveAll(y => y.Ids.Contains(linkId) && (y.Type == TaskType.StartLinks || y.Type == TaskType.StopLinks));
                    taskScheduler.TasksInEnd.RemoveAll(y => y.Ids.Contains(linkId) && (y.Type == TaskType.StartLinks || y.Type == TaskType.StopLinks));
                    taskScheduler.Save();
                }
            }
        }

        public abstract void Add(TaskSchedulerInfo taskSchedulerInfo);

        public void Start(TaskSchedulerInfo taskSchedulerInfo)
        {
            taskSchedulerInfo.Start();
        }

        public void Stop(TaskSchedulerInfo taskSchedulerInfo)
        {
            taskSchedulerInfo.Stop();
        }

        public abstract void DeleteRange(IEnumerable<TaskSchedulerInfo> taskSchedulerInfoes);
    }
}
