using Agrin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.CoreModels.Task
{
    /// <summary>
    /// type of task
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// start links
        /// </summary>
        StartLinks = 1,
        /// <summary>
        /// stop links
        /// </summary>
        StopLinks = 2,
        /// <summary>
        /// start tasks
        /// </summary>
        StartTasks = 3,
        /// <summary>
        /// stop tasks
        /// </summary>
        StopTasks = 4,
        /// <summary>
        /// turn on wifi
        /// </summary>
        TurnOnWifi = 5,
        /// <summary>
        /// turn off wifi
        /// </summary>
        TurnOffWifi = 6,
        /// <summary>
        /// turn on device
        /// </summary>
        TurnOnDevice = 7,
        /// <summary>
        /// turn off device
        /// </summary>
        TurnOffDevice = 8,
        /// <summary>
        /// reset device
        /// </summary>
        ResetDevice = 9,

    }

    /// <summary>
    /// status of task
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// task is proccessing
        /// </summary>
        Processing,
        /// <summary>
        /// task is stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// finished task
        /// </summary>
        Finished,
        /// <summary>
        /// disabled the task
        /// </summary>
        Disabled,
        /// <summary>
        /// task were in error
        /// </summary>
        Error
    }

    /// <summary>
    /// process the task
    /// </summary>
    public interface ITaskProcessor : IObjectDisposable, IDisposable
    {
        /// <summary>
        /// parent of task processor
        /// </summary>
        TaskSchedulerInfo Parent { get; set; }
        /// <summary>
        /// status of processor
        /// </summary>
        TaskStatus Status { get; set; }
        /// <summary>
        /// start the task
        /// </summary>
        void Start();
        /// <summary>
        /// stop the task
        /// </summary>
        void Stop();
        /// <summary>
        /// finish the task
        /// </summary>
        void Finished();
    }
}
