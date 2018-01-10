using Agrin.BaseViewModels.Group;
using Agrin.BaseViewModels.Link;
using Agrin.BaseViewModels.Tasks;
using Agrin.Download.Engine;
using Agrin.Helper.ComponentModel;
using Agrin.RapidBaz.Users;
using Agrin.ViewModels.Helper.ComponentModel;
using Agrin.ViewModels.Managers;
using Agrin.ViewModels.RapidBaz;
using Agrin.Windows.UI.ViewModels.Pages;
using Agrin.Windows.UI.Views.Link;
using Agrin.Windows.UI.Views.Lists;
using Agrin.Windows.UI.Views.RapidBaz;
using Agrin.Windows.UI.Views.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Agrin.Windows.UI.ViewModels.Toolbox
{
    public class TabMenuControlViewModel : ANotifyPropertyChanged
    {
        public static TabMenuControlViewModel This { get; set; }
        public TabMenuControlViewModel()
        {
            This = this;
            LinksCommand = new RelayCommand(Links);
            CompleteLinksCommand = new RelayCommand(CompleteLinks);
            NotCompleteLinksCommand = new RelayCommand(NotCompleteLinks);
            ErrorLinksCommand = new RelayCommand(ErrorLinks);
            DownloadingLinksCommand = new RelayCommand(DownloadingLinks);
            QueueLinksCommand = new RelayCommand(QueueLinks);

            GroupsCommand = new RelayCommand(ShowGroups);

            LoginRapidBazCommand = new RelayCommand(LoginRapidBaz);
            QueueRapidBazCommand = new RelayCommand(QueueRapidBaz);
            CompleteRapidBazCommand = new RelayCommand(CompleteRapidBaz);
            FolderExplorerRapidBazCommand = new RelayCommand(FolderExplorerRapidBaz);
            TasksListCommand = new RelayCommand(TasksList);
            ShowGroupLinksCommand = new RelayCommand<bool>(ShowGroupLinks);
            

            ApplicationAboutCommand = new RelayCommand(ApplicationAbout);
            FeedBackCommand = new RelayCommand(FeedBack);
            RapidBazInformationCommand = new RelayCommand(RapidBazInformation);

            Initialize();
            UserManager.LoginChanged -= UserManager_LoginChanged;
            UserManager.LoginChanged += UserManager_LoginChanged;
        }


        public RelayCommand LinksCommand { get; set; }
        public RelayCommand CompleteLinksCommand { get; set; }
        public RelayCommand NotCompleteLinksCommand { get; set; }
        public RelayCommand ErrorLinksCommand { get; set; }
        public RelayCommand DownloadingLinksCommand { get; set; }
        public RelayCommand QueueLinksCommand { get; set; }
        public RelayCommand<bool> ShowGroupLinksCommand { get; set; }

        public RelayCommand LoginRapidBazCommand { get; set; }
        public RelayCommand QueueRapidBazCommand { get; set; }
        public RelayCommand CompleteRapidBazCommand { get; set; }
        public RelayCommand FolderExplorerRapidBazCommand { get; set; }
       
        public RelayCommand ApplicationAboutCommand { get; set; }
        public RelayCommand FeedBackCommand { get; set; }
        public RelayCommand RapidBazInformationCommand { get; set; }

        public RelayCommand GroupsCommand { get; set; }

        public RelayCommand TasksListCommand { get; set; }

        private TabItem _SelectedTabItem;

        public TabItem SelectedTabItem
        {
            get { return _SelectedTabItem; }
            set
            {
                _SelectedTabItem = value;
                OnPropertyChanged("SelectedTabItem");
            }
        }

        int _SelectedIndex = 0;

        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                _SelectedIndex = value;
                OnPropertyChanged("SelectedIndex");
                if (value == 0)
                    PagesManagerViewModel.This.SetIndex(PagesManagerViewModel.linkPagesManager);
                else if (value == 1)
                    PagesManagerViewModel.This.SetIndex(PagesManagerViewModel.groupsPagesManager);
                else if (value == 2)
                    PagesManagerViewModel.This.SetIndex(PagesManagerViewModel.rapidBazPagesManager);
                else if (value == 3)
                    PagesManagerViewModel.This.SetIndex(PagesManagerViewModel.taskManager);
                else if (value == 4)
                    PagesManagerViewModel.This.SetIndex(PagesManagerViewModel.settingsPagesManager);
                else if (value == 5)
                    PagesManagerViewModel.This.SetIndex(PagesManagerViewModel.aboutPagesManager);
            }
        }

        string _LoginText = "ورود کاربر";

        public string LoginText
        {
            get { return _LoginText; }
            set { _LoginText = value; OnPropertyChanged("LoginText"); }
        }

        bool _IsLinksSelected = true;

        public bool IsLinksSelected
        {
            get { return _IsLinksSelected; }
            set
            {
                _IsLinksSelected = value;
                OnPropertyChanged("IsLinksSelected");
                if (value)
                {
                    IsCompleteLinksSelected = IsErrorLinksSelected = IsNotCompleteLinksSelected = IsDownloadingLinksSelected = IsQueueLinksSelected = false;
                }
            }
        }

        bool _IsCompleteLinksSelected = false;

        public bool IsCompleteLinksSelected
        {
            get { return _IsCompleteLinksSelected; }
            set
            {
                _IsCompleteLinksSelected = value;
                OnPropertyChanged("IsCompleteLinksSelected");
            }
        }

        bool _IsErrorLinksSelected = false;

        public bool IsErrorLinksSelected
        {
            get { return _IsErrorLinksSelected; }
            set
            {
                _IsErrorLinksSelected = value;
                OnPropertyChanged("IsErrorLinksSelected");
            }
        }

        bool _IsQueueLinksSelected = false;

        public bool IsQueueLinksSelected
        {
            get { return _IsQueueLinksSelected; }
            set { _IsQueueLinksSelected = value; OnPropertyChanged("IsQueueLinksSelected"); }
        }

        bool _IsDownloadingLinksSelected = false;

        public bool IsDownloadingLinksSelected
        {
            get { return _IsDownloadingLinksSelected; }
            set { _IsDownloadingLinksSelected = value; OnPropertyChanged("IsDownloadingLinksSelected"); }
        }

        bool _IsNotCompleteLinksSelected;

        public bool IsNotCompleteLinksSelected
        {
            get { return _IsNotCompleteLinksSelected; }
            set
            {
                _IsNotCompleteLinksSelected = value;
                OnPropertyChanged("IsNotCompleteLinksSelected");
            }
        }

        bool _IsLoginRapidBaz = true;

        public bool IsLoginRapidBaz
        {
            get { return _IsLoginRapidBaz; }
            set { _IsLoginRapidBaz = value; OnPropertyChanged("IsLoginRapidBaz"); }
        }

        public bool IsLogin
        {
            get
            {
                return UserManager.IsLogin;
            }
        }

        void UserManager_LoginChanged()
        {
            OnPropertyChanged("IsLogin");
            if (IsLogin)
            {
                LoginText = "مشخصات کاربر";
            }
            else
            {
                LoginText = "ورود کاربر";
            }
        }

        bool _isQueueRapidBaz = false;

        public bool IsQueueRapidBaz
        {
            get { return _isQueueRapidBaz; }
            set { _isQueueRapidBaz = value; OnPropertyChanged("IsQueueRapidBaz"); }
        }

        bool _isCompleteRapidBazList = false;

        public bool IsCompleteRapidBazList
        {
            get { return _isCompleteRapidBazList; }
            set { _isCompleteRapidBazList = value; OnPropertyChanged("IsCompleteRapidBazList"); }
        }

        bool _IsFolderExplorerRapidBaz = false;

        public bool IsFolderExplorerRapidBaz
        {
            get { return _IsFolderExplorerRapidBaz; }
            set { _IsFolderExplorerRapidBaz = value; OnPropertyChanged("IsFolderExplorerRapidBaz"); }
        }

        void ShowGroupLinks(bool value)
        {
            Agrin.Windows.UI.ViewModels.Lists.LinksViewModel.This.ShowGroupLinks(value);
        }

        void Initialize()
        {
            AddLinksBaseViewModel.BackItemClick = () =>
            {
                OnlyLinks();
            };

            AddGroupBaseViewModel.BackItemClick = () =>
            {
                ShowGroups();
            };

            AddTaskBaseViewModel.BackItemClick = () =>
            {
                TasksList();
            };
        }

        void SelectLinkPageItem<T>()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.linkPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<T>());
        }

        void SelectRapidPageItem<T>()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.rapidBazPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<T>());
        }

        void SelectTaskManagerItem<T>()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.taskManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<T>());
        }

        void SelectGroupManagerItem<T>()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.groupsPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<T>());
        }

        void SelectHelpPageItem<T>()
        {
            ((PagesManagerViewModel)PagesManagerViewModel.aboutPagesManager.DataContext).SetIndex(PagesManagerHelper.FindPageItem<T>());
        }

        private void OnlyLinks()
        {
            SelectLinkPageItem<Links>();
        }

        private void Links()
        {
            OnlyLinks();
            IsLinksSelected = true;
            SearchEngine.FilterBy(isAll: true);
        }

        void FilterAll()
        {
            IsLinksSelected = false;
            SearchEngine.FilterBy(false, IsCompleteLinksSelected, IsErrorLinksSelected, IsNotCompleteLinksSelected, IsDownloadingLinksSelected, IsQueueLinksSelected);
        }

        private void CompleteLinks()
        {
            SelectLinkPageItem<Links>();
            FilterAll();
        }

        private void NotCompleteLinks()
        {
            SelectLinkPageItem<Links>();
            FilterAll();
        }

        private void ErrorLinks()
        {
            SelectLinkPageItem<Links>();
            FilterAll();
        }

        private void QueueLinks()
        {
            SelectLinkPageItem<Links>();
            FilterAll();
        }

        private void DownloadingLinks()
        {
            SelectLinkPageItem<Links>();
            FilterAll();
        }

        private void CompleteRapidBaz()
        {
            SelectRapidPageItem<CompleteListRapidBaz>();
            IsCompleteRapidBazList = true;
            CompleteListRapidBazViewModel.CheckRefresh();
        }

        private void QueueRapidBaz()
        {
            SelectRapidPageItem<QueueListRapidBaz>();
            IsQueueRapidBaz = true;
            QueueListRapidBazViewModel.CheckRefresh(null);
        }

        public void LoginRapidBaz()
        {
            SelectRapidPageItem<LoginPageDesignRapidBaz>();
            IsLoginRapidBaz = true;
        }

        private void TasksList()
        {
            SelectTaskManagerItem<TasksList>();
        }

        private void FolderExplorerRapidBaz()
        {
            SelectRapidPageItem<FolderFilesExplorer>();
            IsFolderExplorerRapidBaz = true;
            FolderFilesExplorerViewModel.CheckRefresh();
        }

        private void ShowGroups()
        {
            SelectGroupManagerItem<Groups>();
        }

        private void ApplicationAbout()
        {
            SelectHelpPageItem<Agrin.Windows.UI.Views.Help.About>();
        }

        private void FeedBack()
        {
            SelectHelpPageItem<Agrin.Windows.UI.Views.Help.FeedBack>();
        }

        private void RapidBazInformation()
        {
            SelectHelpPageItem<Agrin.Windows.UI.Views.Help.RapidBazAbout>();
        }
    }
}
