using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web.Tasks
{
    public enum TaskItemMode
    {
        LinkInfo,
        TaskInfo,
        RapidBazID
    }

    
    public class TaskItemInfo
    {
        public object Value { get; set; }
        public TaskItemMode Mode { get; set; }
    }
}
