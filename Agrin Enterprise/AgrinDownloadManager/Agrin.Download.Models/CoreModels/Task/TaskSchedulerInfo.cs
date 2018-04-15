using Agrin.Download.EntireModels.Managers;
using Agrin.Foundation;
using Agrin.Log;
using LiteDB;
using SignalGo.Shared;
using SignalGo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.CoreModels.Task
{
    /// <summary>
    /// settings of link
    /// </summary>
    public class TaskItemSetting
    {
        /// <summary>
        /// type
        /// </summary>
        public TaskType Type { get; set; }
        /// <summary>
        /// Ids of Links
        /// </summary>
        public List<int> Ids { get; set; } = new List<int>();
        /// <summary>
        /// count of play links they are concurrent
        /// for example play 2 links
        /// </summary>
        public int? ConcurrentCount { get; set; } = 1;
        /// <summary>
        /// delay a time when task is starting
        /// </summary>
        public TimeSpan? DelayTimeToStart { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TaskSchedulerInfo : IDisposable
    {
        /// <summary>
        /// get a task proccessor
        /// </summary>
        public static Func<TaskItemSetting, ITaskProcessor> GetTaskProcessorFunction { get; set; }
        /// <summary>
        /// id
        /// </summary>
        [BsonId]
        public int Id { get; set; }
        /// <summary>
        /// delay a time when task is starting
        /// </summary>
        public TimeSpan? DelayTimeToStart { get; set; }
        /// <summary>
        /// start DateTime
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// end date Time
        /// </summary>
        public DateTime? EndDateTime { get; set; }
        /// <summary>
        /// when Scheduler is starting
        /// </summary>
        public List<TaskItemSetting> TasksInStart { get; set; } = new List<TaskItemSetting>();
        /// <summary>
        /// when Scheduler is ending
        /// </summary>
        public List<TaskItemSetting> TasksInEnd { get; set; } = new List<TaskItemSetting>();
        /// <summary>
        /// status of Scheduler
        /// </summary>
        public TaskStatus Status { get; set; } = TaskStatus.Stopped;
        /// <summary>
        /// processors of tasks
        /// </summary>
        [BsonIgnore]
        public List<ITaskProcessor> TaskProcessorsInStart { get; set; } = new List<ITaskProcessor>();
        /// <summary>
        /// processors of tasks
        /// </summary>
        [BsonIgnore]
        public List<ITaskProcessor> TaskProcessorsInEnd { get; set; } = new List<ITaskProcessor>();
        /// <summary>
        /// if task is deleted
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// start the Scheduler
        /// </summary>
        public void Start()
        {
            if (Status == TaskStatus.Disabled || Status == TaskStatus.Finished || Status == TaskStatus.Processing)
                return;
            Status = TaskStatus.Processing;
            Initialize();
            if (TaskProcessorsInStart.Count == TaskProcessorsInStart.Count(x => x.Status == TaskStatus.Finished))
            {
                Status = TaskStatus.Finished;
                Save();
                return;
            }

            if (DelayTimeToStart == null)
            {
                foreach (var item in TaskProcessorsInStart)
                {
                    item.Start();
                }
            }
            else
            {
                AsyncActions.Run(() =>
                {
                    Thread.Sleep(DelayTimeToStart.Value);
                    foreach (var item in TaskProcessorsInStart)
                    {
                        item.Start();
                    }
                }, (ex) =>
                {
                    Status = TaskStatus.Error;
                    Save();
                    AutoLogger.LogError(ex, "TaskSchedulerInfo Start");
                });
            }
        }

        /// <summary>
        /// when one task finished
        /// </summary>
        public void OneTaskFinished()
        {
            if (Status == TaskStatus.Processing)
            {
                if (TaskProcessorsInStart.Count == TaskProcessorsInStart.Count(x => x.Status == TaskStatus.Finished))
                {
                    Status = TaskStatus.Finished;
                    if (TaskProcessorsInEnd.Count > 0)
                    {
                        AsyncActions.Run(() =>
                        {
                            foreach (var item in TaskProcessorsInEnd)
                            {
                                item.Start();
                            }
                        }, (ex) =>
                        {
                            Status = TaskStatus.Error;
                            Save();
                            AutoLogger.LogError(ex, "TaskSchedulerInfo OneTaskFinished");
                        });
                    }
                    Save();
                }
            }
        }

        /// <summary>
        /// stop the Scheduler
        /// </summary>
        public void Stop()
        {
            Initialize();
            foreach (var item in TaskProcessorsInStart)
            {
                item.Stop();
            }
            foreach (var item in TaskProcessorsInEnd)
            {
                item.Stop();
            }
            Status = TaskStatus.Stopped;
            Save();
        }

        /// <summary>
        /// init the proccessors
        /// </summary>
        public void Initialize()
        {
            if (TaskProcessorsInStart.Count == 0)
            {
                foreach (var item in TasksInStart)
                {
                    var instance = GetTaskProcessorFunction?.Invoke(item);
                    if (instance != null)
                    {
                        instance.Parent = this;
                        TaskProcessorsInStart.Add(instance);
                    }
                    else if (item.Type == TaskType.StartLinks)
                    {
                        TaskProcessorsInStart.Add(new LinkDownloaderTaskProcessor(item) { Parent = this });
                    }
                }
            }
            if (TaskProcessorsInEnd.Count == 0)
            {
                foreach (var item in TasksInEnd)
                {
                    var instance = GetTaskProcessorFunction?.Invoke(item);
                    if (instance != null)
                    {
                        instance.Parent = this;
                        TaskProcessorsInEnd.Add(instance);
                    }
                }
            }
        }

        /// <summary>
        /// save task
        /// </summary>
        public void Save()
        {
            DataBaseFoundation<TaskSchedulerInfo>.Current.Update(this);
            TaskScheduleManagerBase.TaskScheduleChanged?.Invoke();
        }

        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            Status = TaskStatus.Disabled;
            foreach (var item in TaskProcessorsInStart)
            {
                item.Dispose();
            }
            foreach (var item in TaskProcessorsInEnd)
            {
                item.Dispose();
            }
            Save();
        }
    }
}
