using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using Agrin.ViewModels.Managers;
using Agrin.Windows.UI.Views.Group;
using Agrin.Windows.UI.Views.Help;
using Agrin.Windows.UI.Views.Link;
using Agrin.Windows.UI.Views.Lists;
using Agrin.Windows.UI.Views.Managers;
using Agrin.Windows.UI.Views.RapidBaz;
using Agrin.Windows.UI.Views.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agrin.Windows.UI.ViewModels.Pages
{
    public class PagesManagerViewModel : ANotifyPropertyChanged<PagesManager>
    {
        public static PagesManager linkPagesManager, groupsPagesManager, rapidBazPagesManager, settingsPagesManager, aboutPagesManager, taskManager;
        static int _level = 0;
        static PagesManagerViewModel()
        {
            if (MainWindow.This == null)
                return;
            linkPagesManager = new PagesManager();

            _level = 1;
            groupsPagesManager = new PagesManager();

            _level = 2;
            rapidBazPagesManager = new PagesManager();

            _level = 3;
            taskManager = new PagesManager();

            _level = 4;
            settingsPagesManager = new PagesManager();

            _level = 5;
            aboutPagesManager = new PagesManager();

            _level = 6;


            //AddItem(new AddLinks(), new RelayCommand(BackToList), true);
            //AddItem(new AddGroup(), new RelayCommand(BackToList), false);
            //AddItem(new AboutPage(), new RelayCommand(BackToList), false);
            //AddItem(new SendFeedBack(), new RelayCommand(BackToList), false);
            //AddItem(new LinkInfoSetting(), new RelayCommand(BackToList), false);
            //AddItem(new TaskManager(), new RelayCommand(BackToList), false);
            //AddItem(new Login(), new RelayCommand(BackToList), false);
            //AddItem(new RapidBazGetList(), new RelayCommand(BackToList), false);//7
        }

        public static PagesManagerViewModel This = null;
        public PagesManagerViewModel()
        {
            if (MainWindow.This == null)
                return;
            ViewElementInited = () =>
            {
                if (_level == 0)
                    linkPagesManager = ViewElement;
                else if (_level == 1)
                    groupsPagesManager = ViewElement;
                else if (_level == 2)
                    rapidBazPagesManager = ViewElement;
                else if (_level == 3)
                    taskManager = ViewElement;
                else if (_level == 4)
                    settingsPagesManager = ViewElement;
                else if (_level == 5)
                    aboutPagesManager = ViewElement;

                if (this.ViewElement == linkPagesManager)
                {
                    PagesManagerHelper.AddBaseTabPageItem(linkPagesManager, null, null);
                    var links = new Links();
                    CurrentControl = links;
                    PagesManagerHelper.AddPageToTabPageItem(linkPagesManager, links, null);
                    PagesManagerHelper.AddPageToTabPageItem(linkPagesManager, new AddLinks(), null);
                }
                else if (this.ViewElement == rapidBazPagesManager)
                {
                    PagesManagerHelper.AddBaseTabPageItem(rapidBazPagesManager, null, null);
                    var login = new LoginPageDesignRapidBaz();
                    CurrentControl = login;
                    PagesManagerHelper.AddPageToTabPageItem(rapidBazPagesManager, login, null);
                    PagesManagerHelper.AddPageToTabPageItem(rapidBazPagesManager, new CompleteListRapidBaz(), null);
                    PagesManagerHelper.AddPageToTabPageItem(rapidBazPagesManager, new QueueListRapidBaz(), null);
                    PagesManagerHelper.AddPageToTabPageItem(rapidBazPagesManager, new FolderFilesExplorer(), null);
                }
                else if (this.ViewElement == groupsPagesManager)
                {
                    PagesManagerHelper.AddBaseTabPageItem(groupsPagesManager, null, null);
                    var groups = new Agrin.Windows.UI.Views.Lists.Groups();
                    CurrentControl = groups;
                    PagesManagerHelper.AddPageToTabPageItem(groupsPagesManager, groups, null);
                    PagesManagerHelper.AddPageToTabPageItem(groupsPagesManager, new AddGroup(), null);
                }
                else if (this.ViewElement == taskManager)
                {
                    PagesManagerHelper.AddBaseTabPageItem(taskManager, null, null);
                    var taskList = new TasksList();
                    CurrentControl = taskList;
                    PagesManagerHelper.AddPageToTabPageItem(taskManager, taskList, null);
                    PagesManagerHelper.AddPageToTabPageItem(taskManager, new AddTask(), null);
                }
                else if (this.ViewElement == settingsPagesManager)
                {
                    PagesManagerHelper.AddBaseTabPageItem(settingsPagesManager, null, null);
                }
                else if (this.ViewElement == aboutPagesManager)
                {
                    PagesManagerHelper.AddBaseTabPageItem(aboutPagesManager, null, null);
                    var about = new Agrin.Windows.UI.Views.Help.About();
                    var rapidAbout = new RapidBazAbout();
                    CurrentControl = rapidAbout;
                    PagesManagerHelper.AddPageToTabPageItem(aboutPagesManager, about, null);
                    PagesManagerHelper.AddPageToTabPageItem(aboutPagesManager, new FeedBack(), null);
                    PagesManagerHelper.AddPageToTabPageItem(aboutPagesManager, rapidAbout, null);
                }
                else
                {
                    This = this;
                    CurrentControl = linkPagesManager;
                }
            };
        }

        FrameworkElement _CurrentControl;

        public FrameworkElement CurrentControl
        {
            get { return _CurrentControl; }
            set
            {
                _CurrentControl = value;
                OnPropertyChanged("CurrentControl");
            }
        }

        FrameworkElement _CurrentControlCollapsed;

        public FrameworkElement CurrentControlCollapsed
        {
            get { return _CurrentControlCollapsed; }
            set
            {
                _CurrentControlCollapsed = value;
                OnPropertyChanged("CurrentControlCollapsed");
            }
        }

        public void SetIndex(FrameworkElement newControl)
        {
            PagesManagerHelper.SetIndex(ViewElement, newControl, CurrentControl, CurrentControlCollapsed, ViewElement.currentControl, ViewElement.currentControlCollapsed, (item) => CurrentControl = item, (item) => CurrentControlCollapsed = item);
        }
    }
}
