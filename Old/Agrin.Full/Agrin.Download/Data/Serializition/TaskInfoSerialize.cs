using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class TaskInfoSerialize
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsStartNow { get; set; }
        public bool IsDayOfWeek { get; set; }
        public bool IsTimeAndDays { get; set; }
        public int TryErrorCount { get; set; }
        public bool IsTryExterme { get; set; }
        public bool IsMainTask { get; set; }
        public DateTime TimeAndDays { get; set; }
        public List<TaskItemInfoSerialize> TaskItemInfoes { get; set; }
        public List<DateTime> DateTimes { get; set; }
        public List<DayOfWeek> DayOfWeek { get; set; }
        public List<TaskModeEnum> TaskMode { get; set; }
        public List<TaskUtilityModeEnum> TaskUtilityActions { get; set; }
        public List<TaskUtilityModeEnum> LinkCompleteTaskUtilityActions { get; set; }
        public TaskState State { get; set; }
    }
}
