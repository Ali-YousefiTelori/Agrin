using Agrin.Download.Web.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Serializition
{
    [Serializable]
    public class TaskItemInfoSerialize
    {
        public int Value { get; set; }
        public TaskItemMode Mode { get; set; }
    }
}
