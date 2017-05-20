using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.UI.ViewModels.Lists;
using Agrin.UI.ViewModels.Pages;
using Agrin.ViewModels.Helper.ComponentModel;

namespace Agrin.UI.ViewModels.Toolbox
{
    public class MenuControlViewModel
    {
        public MenuControlViewModel()
        {
            AddLinkInfoCommand = new RelayCommand(AddLinkInfo);
            AddGroupInfoCommand = new RelayCommand(AddGroupInfo);
            TaskManagerCommand = new RelayCommand(TaskManager);
            ExitAppCommand = new RelayCommand(ExitApp);
            AppAboutCommand = new RelayCommand(AppAbout);
            FeedbackCommand = new RelayCommand(Feedback);
            LoginRapidbazCommand = new RelayCommand(LoginRapidbaz);
            ListOfRapidbazCommand = new RelayCommand(ListOfRapidbaz);
        }


        public RelayCommand AddLinkInfoCommand { get; set; }
        public RelayCommand AddGroupInfoCommand { get; set; }
        public RelayCommand TaskManagerCommand { get; set; }
        public RelayCommand ExitAppCommand { get; set; }
        public RelayCommand AppLearningCommand { get; set; }
        public RelayCommand AppAboutCommand { get; set; }
        public RelayCommand FeedbackCommand { get; set; }
        public RelayCommand LoginRapidbazCommand { get; set; }
        public RelayCommand ListOfRapidbazCommand { get; set; }

        private void ExitApp()
        {
            MainWindow.This.Close();
        }

        private void TaskManager()
        {
            PagesManagerViewModel.SetIndex(5);
            MainWindow.This.IsShowPage = true;
        }

        private void AddGroupInfo()
        {
            AddGroup();
        }

        public static void AddGroup(bool isEditMode = false)
        {
            if (!isEditMode)
                AddGroupViewModel.This.Clear();
            AddGroupViewModel.This.IsEditMode = isEditMode;
            PagesManagerViewModel.SetIndex(1);
            MainWindow.This.IsShowPage = true;
        }

        public static void EditGroupInfo(GroupInfo groupInfo)
        {
            AddGroupViewModel.This.EditGroupInfo = groupInfo;
            AddGroup(true);
        }

        private void AddLinkInfo()
        {
            ToolbarViewModel.This.AddLinkPageCommand.Execute();
        }
        private void AppAbout()
        {
            PagesManagerViewModel.SetIndex(2);
            MainWindow.This.IsShowPage = true;
        }
        private void Feedback()
        {
            PagesManagerViewModel.SetIndex(3);
            MainWindow.This.IsShowPage = true;
        }

        private void LoginRapidbaz()
        {
            PagesManagerViewModel.SetIndex(6);
            MainWindow.This.IsShowPage = true;
        }

        private void ListOfRapidbaz()
        {
            PagesManagerViewModel.SetIndex(7);
            MainWindow.This.IsShowPage = true;
            if (!RapidBazGetListViewModel.This.CanRefresh())
                return;
            if (RapidBazGetListViewModel.MustRefresh)
                RapidBazGetListViewModel.This.RefreshList();
        }
    }
}
