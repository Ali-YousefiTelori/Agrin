using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Toolbox
{
    public class TasksToolbarBaseViewModel : ANotifyPropertyChanged
    {
        public virtual IEnumerable<TaskInfo> GetSelectedTasks()
        {
            return null;
        }

        public virtual void AddTask()
        {

        }
    }
}
