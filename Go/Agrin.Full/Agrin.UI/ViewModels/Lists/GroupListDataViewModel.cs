using Agrin.Download.Engine;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.UI.ViewModels.Pages;
using Agrin.UI.Views.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Agrin.UI.ViewModels.Lists
{
    public class GroupListDataViewModel : ANotifyPropertyChanged<GroupListData>
    {
        public GroupListDataViewModel()
        {
            if (MainWindow.This == null)
                return;
            PinCommand = new RelayCommand(Pin);
            MainWindow.This.IsGroupPin = true;
            Pin();

            //ApplicationGroupManager.Current.AddGroupInfo(new GroupInfo() { Name = "بازی ها" });
            //ApplicationGroupManager.Current.AddGroupInfo(new GroupInfo() { Name = "صوتی و تصویری" });
            //ApplicationGroupManager.Current.AddGroupInfo(new GroupInfo() { Name = "نرم افزار ها" });
            //ApplicationGroupManager.Current.AddGroupInfo(new GroupInfo() { Name = "موسیقی ها" });
            //foreach (var item in ApplicationGroupManager.Current.GroupInfoes)
            //{
            //    item.SavePath = System.IO.Path.Combine(Agrin.IO.Helper.MPath.DownloadsPath, item.Name);
            //}
            GroupInfo allGroups = ApplicationGroupManager.Current.NoGroup;
            allGroups.SelectionChanged = () =>
                {
                    SearchEngine.IsAppLoading = true;
                    foreach (var item in ApplicationGroupManager.Current.GroupInfoes)
                    {
                        item.IsSelected = allGroups.IsSelected;
                    }
                    SearchEngine.IsAppLoading = false;
                };
            ApplicationGroupManager.Current.AllGroup = allGroups;
            Groups.Add(allGroups);
            Groups.AddRange(ApplicationGroupManager.Current.GroupInfoes);
            ApplicationGroupManager.Current.AddedGroups = (list) =>
            {
                Groups.AddRange(list);
            };
            ApplicationGroupManager.Current.RemovedGroups = (list) =>
            {
                Groups.RemoveRange(list);
            };
            SelectedGroup = allGroups;
            isloaded = false;
            SetViewElement = () =>
            {
                ViewElement.mainGroupList.ItemContainerGenerator.StatusChanged += (sender, e) =>
                {
                    if (ViewElement.mainGroupList.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                    {
                        isloaded = true;
                        ViewElement.mainGroupList.ScrollIntoView(Groups[0]);
                        ListBoxItem mainItem = (ListBoxItem)ViewElement.mainGroupList.ItemContainerGenerator.ContainerFromItem(Groups[0]);
                        mainItem.Style = (Style)ViewElement.FindResource("mainBoxItem");
                        mainItem.IsSelected = true;
                    }
                };
            };
            AddGroupCommand = new RelayCommand(AddGroupExecute);
            DeleteGroupCommand = new RelayCommand(DeleteGroup, CanDeleteGroup);
            This = this;
        }

        bool isloaded = true;
        public static GroupListDataViewModel This;

        FastCollection<GroupInfo> _Groups;

        public FastCollection<GroupInfo> Groups
        {
            get
            {
                if (_Groups == null)
                    _Groups = new FastCollection<GroupInfo>(ApplicationHelper.DispatcherThread);
                return _Groups;
            }
            set { _Groups = value; }
        }

        public RelayCommand PinCommand { get; set; }
        public RelayCommand AddGroupCommand { get; set; }
        public RelayCommand DeleteGroupCommand { get; set; }

        ControlTemplate _PinStyle;
        public ControlTemplate PinStyle
        {
            get { return _PinStyle; }
            set { _PinStyle = value; OnPropertyChanged("PinStyle"); }
        }

        GroupInfo _SelectedGroup;
        public GroupInfo SelectedGroup
        {
            get { return _SelectedGroup; }
            set
            {
                if (!isloaded)
                    return;
                _SelectedGroup = value;
                OnPropertyChanged("SelectedGroup");
                if (value == ApplicationGroupManager.Current.NoGroup)
                    SearchEngine.CurrentGroup = null;
                else
                    SearchEngine.CurrentGroup = value;
                SearchEngine.Search();
            }
        }

        bool _IsEditMode;
        public bool IsEditMode
        {
            get { return _IsEditMode; }
            set { _IsEditMode = value; OnPropertyChanged("IsEditMode"); }
        }

        void Pin()
        {
            MainWindow.This.IsGroupPin = !MainWindow.This.IsGroupPin;
            PinStyle = MainWindow.This.IsGroupPin ? ApplicationHelper.GetAppResource<ControlTemplate>("UnPin_TemplateStyle") : ApplicationHelper.GetAppResource<ControlTemplate>("Pin_TemplateStyle");
        }

        private void AddGroupExecute()
        {
            AddGroup(false);
        }

        private void AddGroup(bool isEditMode = false)
        {
            if (!isEditMode)
                AddGroupViewModel.This.Clear();
            AddGroupViewModel.This.IsEditMode = isEditMode;
            PagesManagerViewModel.SetIndex(1);
            MainWindow.This.IsShowPage = true;
            
        }

        private bool CanDeleteGroup()
        {
            return SelectedGroup != null && SelectedGroup != ApplicationGroupManager.Current.AllGroup;
        }

        private void DeleteGroup()
        {
            ApplicationGroupManager.Current.DeleteGroupInfo(SelectedGroup);
        }
        public void EditGroupInfo(GroupInfo groupInfo)
        {
            AddGroupViewModel.This.EditGroupInfo = groupInfo;
            AddGroup(true);
        }
    }
}
