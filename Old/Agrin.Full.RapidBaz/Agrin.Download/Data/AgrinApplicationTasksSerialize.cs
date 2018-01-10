using Agrin.Download.Data.Serializition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data
{
    [Serializable]
    public class AgrinApplicationTasksSerialize
    {
        public List<TaskInfoSerialize> TaskInfoes { get; set; }
    }
}
