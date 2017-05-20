using Agrin.BaseViewModels.Tasks;
using Agrin.BaseViewModels.Toolbox;
using Agrin.Download.Web;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Toolbox
{
    public class TasksToolbarViewModel : TasksToolbarBaseViewModel
    {
        public TasksToolbarViewModel()
        {
            AddTaskCommand = new RelayCommand(AddTask);
            MessageActionCommand = new RelayCommand(MessageAction);
            DeleteTaskCommand = new RelayCommand(DeleteTaskAction, CanDeleteAction);
        }

        public RelayCommand AddTaskCommand { get; set; }
        public RelayCommand MessageCommand { get; set; }
        public RelayCommand DeleteTaskCommand { get; set; }


        bool _IsShowMessage = false;

        public bool IsShowMessage
        {
            get { return _IsShowMessage; }
            set { _IsShowMessage = value; OnPropertyChanged("IsShowMessage"); }
        }

        string _Message = "آیا میخواهید وظیفه ی انتخاب شده را حذف کنید؟ در این صورت عملیات وظیفه متوقف خواهد شد";

        public string Message
        {
            get { return _Message; }
            set { _Message = value; OnPropertyChanged("Message"); }
        }

        string _MessageTitle = "حذف وظیفه";

        public string MessageTitle
        {
            get { return _MessageTitle; }
            set { _MessageTitle = value; }
        }

        private void MessageAction()
        {
            var sel = GetSelectedTasks();
            if (sel == null)
                return;
            var first = sel.FirstOrDefault();
            if (first == null)
                return;
            TasksListBaseViewModel.DeleteTask(first);
            IsShowMessage = false;
        }

        bool CanDeleteAction()
        {
            var sel = GetSelectedTasks();
            if (sel == null)
                return false;
            var first = sel.FirstOrDefault();
            if (first == null)
                return false;
            return true;
        }

        private void DeleteTaskAction()
        {
            IsShowMessage = true;
        }

        public RelayCommand MessageActionCommand { get; set; }
    }
}
