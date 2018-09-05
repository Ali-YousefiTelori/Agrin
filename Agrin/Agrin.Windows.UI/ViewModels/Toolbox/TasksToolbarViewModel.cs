using Agrin.Download.Web;
using Agrin.ViewModels.Managers;
using Agrin.Windows.UI.ViewModels.Pages;
using Agrin.Windows.UI.Views.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Windows.UI.ViewModels.Toolbox
{
    public class TasksToolbarViewModel : Agrin.ViewModels.Toolbox.TasksToolbarViewModel
    {
        public override IEnumerable<TaskInfo> GetSelectedTasks()
        {
            if (TasksList.This == null || TasksList.This.MainDataGrid == null)
                return new List<TaskInfo>();

            return new List<TaskInfo>() { (TasksList.This.MainDataGrid.SelectedItem as TaskInfo) };
        }

        public override void AddTask()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.taskManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<AddTask>());
        }
    }
}
