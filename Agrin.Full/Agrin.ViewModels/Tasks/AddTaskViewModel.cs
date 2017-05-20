using Agrin.BaseViewModels.Tasks;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Tasks
{
    public class AddTaskViewModel : AddTaskBaseViewModel
    {
        public AddTaskViewModel()
        {
            AddTaskCommand = new RelayCommand(AddTask, CanAddTask);
            BackCommand = new RelayCommand(Back);
            ShowSelectionLinksListCommand = new RelayCommand(ShowSelectionLinksList);
        }

        public RelayCommand AddTaskCommand { get; set; }
        public RelayCommand ShowSelectionLinksListCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
    }
}
