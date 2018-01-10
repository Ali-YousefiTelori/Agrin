using Agrin.BaseViewModels.MenuToolbar;
using Agrin.Download.Data;
using Agrin.Download.Engine;
using Agrin.Download.Helper;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Download.Web.Tasks;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Lists
{
    public class LinksBaseViewModel : ListBaseViewModel<LinkInfo>
    {
        public LinksBaseViewModel()
        {

        }

        public FastCollection<LinkInfo> Items
        {
            get
            {
                return SearchEngine.Items;
            }
        }

        public FastCollection<GroupInfo> GrouInfoes
        {
            get
            {
                return ApplicationGroupManager.Current.GroupInfoes;
            }
        }

        LinkInfo _SelectedItem;
        public LinkInfo SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        string _SeachAddress;

        public string SeachAddress
        {
            get { return _SeachAddress; }
            set { _SeachAddress = value; OnPropertyChanged("SeachAddress"); }
        }

        int _AddTimeMinutes = DateTime.Now.Minute;

        public int AddTimeMinutes
        {
            get { return _AddTimeMinutes; }
            set { _AddTimeMinutes = value; OnPropertyChanged("AddTimeMinutes"); }
        }

        int _AddTimeHours = DateTime.Now.Hour;

        public int AddTimeHours
        {
            get { return _AddTimeHours; }
            set { _AddTimeHours = value; OnPropertyChanged("AddTimeHours"); }
        }

        int _StopTimeForMinutes = 5;

        public int StopTimeForMinutes
        {
            get { return _StopTimeForMinutes; }
            set { _StopTimeForMinutes = value; OnPropertyChanged("StopTimeForMinutes"); }
        }

        int _StopTimeForTimeHours = 0;

        public int StopTimeForTimeHours
        {
            get { return _StopTimeForTimeHours; }
            set { _StopTimeForTimeHours = value; OnPropertyChanged("StopTimeForTimeHours"); }
        }

        int _StopTimeHours = 0;

        public int StopTimeHours
        {
            get { return _StopTimeHours; }
            set { _StopTimeHours = value; OnPropertyChanged("StopTimeHours"); }
        }

        int _StopTimeMinutes = 0;

        public int StopTimeMinutes
        {
            get { return _StopTimeMinutes; }
            set { _StopTimeMinutes = value; OnPropertyChanged("StopTimeMinutes"); }
        }

        bool _StartNow = false;

        public bool StartNow
        {
            get { return _StartNow; }
            set { _StartNow = value; OnPropertyChanged("StartNow"); }
        }

        bool _IsStopForMinutes = true;

        public bool IsStopForMinutes
        {
            get { return _IsStopForMinutes; }
            set { _IsStopForMinutes = value; OnPropertyChanged("IsStopForMinutes"); }
        }

        bool _IsDeleteFromDisk = false;

        public bool IsDeleteFromDisk
        {
            get { return _IsDeleteFromDisk; }
            set { _IsDeleteFromDisk = value; OnPropertyChanged("IsDeleteFromDisk"); }
        }

        public void AddTimeTask()
        {
            var listLinks = GetSelectedItems().ToList();
            int newID = ApplicationTaskManager.Current.GetNewID();
            string taskName = Agrin.IO.Strings.Text.IntToLetters(newID);
            var task = new TaskInfo() { Name = taskName, IsActive = true, TaskItemInfoes = listLinks.Select(x => x.PathInfo.Id).Select<int, TaskItemInfo>(x => new TaskItemInfo() { Mode = TaskItemMode.LinkInfo, Value = x }).ToList() };

            if (StartNow)
            {
                task.IsStartNow = true;
            }
            else
            {
                var dt = DateTime.Now;
                if (AddTimeMinutes == dt.Minute && AddTimeHours == dt.Hour)
                {
                    task.IsStartNow = true;
                }
                else
                {
                    dt = dt.AddSeconds(-dt.Second);
                    dt = dt.AddMinutes(-dt.Minute);
                    dt = dt.AddHours(-dt.Hour);
                    dt = dt.AddMinutes(AddTimeMinutes);
                    dt = dt.AddHours(AddTimeHours);
                    if (dt < DateTime.Now)
                        dt = dt.AddDays(1);
                    task.DateTimes.Add(dt);
                }
            }


            task.TaskUtilityActions.Add(TaskUtilityModeEnum.StartLink);
            ApplicationTaskManager.Current.AddTask(task);
        }

        public void AddStopTimeTask()
        {
            var listLinks = GetSelectedItems().ToList();
            if (IsStopForMinutes)
            {
                foreach (var item in listLinks)
                {
                    ApplicationLinkInfoManager.Current.StopLinkInfo(item);
                }
            }
            int newID = ApplicationTaskManager.Current.GetNewID();
            string taskName = Agrin.IO.Strings.Text.IntToLetters(newID);
            var task = new TaskInfo() { Name = taskName, IsActive = true, TaskItemInfoes = listLinks.Select(x => x.PathInfo.Id).Select<int, TaskItemInfo>(x => new TaskItemInfo() { Mode = TaskItemMode.LinkInfo, Value = x }).ToList() };
            var dt = DateTime.Now;

            if (IsStopForMinutes)
            {
                dt = dt.AddMinutes(StopTimeForMinutes);
                dt = dt.AddHours(StopTimeForTimeHours);
            }
            else
            {
                dt = dt.AddSeconds(-dt.Second);
                dt = dt.AddMinutes(-dt.Minute);
                dt = dt.AddHours(-dt.Hour);
                dt = dt.AddMinutes(StopTimeMinutes);
                dt = dt.AddHours(StopTimeHours);
                if (dt < DateTime.Now)
                    dt = dt.AddDays(1);
            }

            task.DateTimes.Add(dt);
            if (IsStopForMinutes)
                task.TaskUtilityActions.Add(TaskUtilityModeEnum.StartLink);
            else
                task.TaskUtilityActions.Add(TaskUtilityModeEnum.StopLink);
            ApplicationTaskManager.Current.AddTask(task);
        }


        public void RemoveAllStartTimeTask(LinkInfo linkInfo)
        {
            ApplicationTaskManager.Current.RemoveLinkInfoFromStartTasks(linkInfo);
        }

        public void RemoveAllStopTimeTask(LinkInfo linkInfo)
        {
            ApplicationTaskManager.Current.RemoveLinkInfoFromStopedTasks(linkInfo);
        }

        public void DeleteSelectedLinksTimes()
        {
            var listLinks = GetSelectedItems().ToList();

            foreach (var linkInfo in listLinks)
            {
                if (linkInfo.IsAddedToTaskForStart)
                {
                    ApplicationTaskManager.Current.RemoveLinkInfoFromStartTasks(linkInfo);
                }
                if (linkInfo.IsAddedToTaskForStop)
                {
                    ApplicationTaskManager.Current.RemoveLinkInfoFromStopedTasks(linkInfo);
                }
            }
        }

        public void MoveUpFromTask()
        {
            var listLinks = GetSelectedItems().ToList();
            List<TaskInfo> changedTask = new List<TaskInfo>();
            foreach (var linkInfo in listLinks)
            {
                if (linkInfo.IsAddedToTaskForStart || linkInfo.IsAddedToTaskForStop)
                {
                    changedTask.Add(ApplicationTaskManager.Current.MoveUpFromTask(linkInfo));
                }
            }
            ApplicationTaskManager.Current.ChangedAllTaskItemsIndexes(changedTask);
            SerializeData.SaveApplicationTaskToFile();
        }

        public void MoveDownFromTask()
        {
            var listLinks = GetSelectedItems().ToList();
            List<TaskInfo> changedTask = new List<TaskInfo>();

            foreach (var linkInfo in listLinks)
            {
                if (linkInfo.IsAddedToTaskForStart || linkInfo.IsAddedToTaskForStop)
                {
                    changedTask.Add(ApplicationTaskManager.Current.MoveDownFromTask(linkInfo));
                }
            }
            ApplicationTaskManager.Current.ChangedAllTaskItemsIndexes(changedTask);
            SerializeData.SaveApplicationTaskToFile();
        }

        public bool CanDeleteSelectedLinksTimes()
        {
            var listLinks = GetSelectedItems().ToList();

            foreach (var item in listLinks)
            {
                if (item.IsAddedToTaskForStart || item.IsAddedToTaskForStop)
                    return true;
            }
            return false;
        }

        public bool CanDeleteGroupInfo(object obj)
        {
            return obj != ApplicationGroupManager.Current.NoGroup;
        }

        public void DeleteGroupInfo(object obj)
        {
            if (obj is GroupInfo)
            {
                ApplicationGroupManager.Current.DeleteGroupInfo(obj as GroupInfo);
                foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToArray())
                {
                    if (item.PathInfo.CurrentGroupInfo == obj)
                    {
                        ApplicationGroupManager.Current.SetGroupByLinkInfo(item, null);
                    }
                }
            }
        }

        
        //public bool CanEditGroupInfo(GroupInfo obj)
        //{
        //    return obj != ApplicationGroupManager.Current.NoGroup;
        //}

        //public void EditGroupInfo(GroupInfo obj)
        //{
        //    MenuBaseViewModel.EditGroupInfo(obj);
        //}

        public void AddGroupInfo()
        {

        }

        List<TaskInfo> GetSelectionTaskInfoes()
        {
            var listLinks = GetSelectedItems().ToList();
            List<TaskInfo> tasks = new List<TaskInfo>();
            foreach (var link in listLinks)
            {
                var task = ApplicationTaskManager.Current.FindTaskInfoByLinkInfoID(link.PathInfo.Id).FirstOrDefault();
                if (task != null && !tasks.Contains(task))
                {
                    tasks.Add(task);
                }
            }
            return tasks;
        }

        public bool CanStopSelectionTask()
        {
            foreach (var item in GetSelectionTaskInfoes())
            {
                if (item.IsActive)
                    return true;
            }
            return false;
        }

        public void StopSelectionTask()
        {
            foreach (var item in GetSelectionTaskInfoes())
            {
                ApplicationTaskManager.Current.DeActiveTask(item);
            }
        }

        public bool CanPlaySelectionTask()
        {
            foreach (var item in GetSelectionTaskInfoes())
            {
                if (!item.IsActive)
                    return true;
            }
            return false;
        }

        public void PlaySelectionTask()
        {
            foreach (var item in GetSelectionTaskInfoes())
            {
                ApplicationTaskManager.Current.ActiveTask(item);
            }
        }

        public void ReportLink(string savePath)
        {
            try
            {
                LinkHelper.CreateLinkInfoReport(SelectedItem, savePath);

            }
            catch
            {
            }
        }

        public virtual void OpenFileLocation()
        {
            
        }

        public virtual void OpenFile()
        {
            
        }


        public bool CanOpenFile()
        {
            return LinksBaseViewModel.CanOpenFile(SelectedItem);
        }

        public static bool CanOpenFile(LinkInfo linkInfo)
        {
            return linkInfo != null && linkInfo.DownloadingProperty.State == ConnectionState.Complete && System.IO.File.Exists(linkInfo.PathInfo.FullAddressFileName);
        }

        public void SendToGroupInfo(GroupInfo obj, List<LinkInfo> links)
        {
            foreach (var item in links)
            {
                item.PathInfo.UserGroupInfo = obj;
            }
            Agrin.Download.Data.SerializeData.SaveLinkInfoesToFile();
        }
        //public bool CanSendToGroupInfo(GroupInfo obj, List<LinkInfo> links)
        //{
        //    return Toolbox.ToolbarViewModel.CanDeleteLinks();
        //}

        public virtual void ChangeUserSavePath(LinkInfo obj, string fileName)
        {
            //var dialog = new SaveFileDialog() { InitialDirectory = obj.PathInfo.SavePath, FileName = obj.PathInfo.FileName, Filter = "*.*|All Files" };
            //if (dialog.ShowDialog().Value)
            //{
            obj.PathInfo.UserFileName = Path.GetFileName(fileName);
            obj.PathInfo.UserSavePath = Path.GetDirectoryName(fileName);
            //}
        }

        public bool CanChangeUserSavePath(LinkInfo obj)
        {
            return obj != null && obj.CanDelete;
        }

        public void LinkInfoSetting(LinkInfo item)
        {
            //Toolbox.ToolbarViewModel.ShowSetting(item);
        }

        public void Search()
        {
            SearchEngine.SearchText = SeachAddress;
            SearchEngine.Search();
        }

        public void PlayLinkInfo()
        {
            LinksBaseViewModel.PlayLinkInfo(SelectedItem);
        }

        public bool CanPlayLinkInfo()
        {
            return LinksBaseViewModel.CanPlayLinkInfo(SelectedItem);
        }

        public void StopLinkInfo()
        {
            LinksBaseViewModel.StopLinkInfo(SelectedItem);
        }

        public bool CanStopLinkInfo()
        {
            return LinksBaseViewModel.CanStopLinkInfo(SelectedItem);
        }

        public static void PlayLinkInfo(LinkInfo linkInfo)
        {
            ApplicationLinkInfoManager.Current.PlayLinkInfo(linkInfo);
        }

        public static bool CanPlayLinkInfo(LinkInfo linkInfo)
        {
            return linkInfo != null && linkInfo.CanPlay;
        }

        public static void StopLinkInfo(LinkInfo linkInfo)
        {
            ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
        }

        public static bool CanStopLinkInfo(LinkInfo linkInfo)
        {
            return linkInfo != null && linkInfo.CanStop;
        }

        public bool CanPlayLinks()
        {
            foreach (var item in GetSelectedItems())
            {
                if (item.CanPlay)
                    return true;
            }

            return false;
        }

        public void PlayLinks()
        {
            foreach (var item in GetSelectedItems())
            {
                if (item.CanPlay)
                    LinksBaseViewModel.PlayLinkInfo(item);
            }
        }

        public bool CanStopLinks()
        {
            foreach (var item in GetSelectedItems())
            {
                if (item.CanStop)
                    return true;
            }

            return false;
        }

        public void StopLinks()
        {
            foreach (var item in GetSelectedItems())
            {
                if (item.CanStop)
                    LinksBaseViewModel.StopLinkInfo(item);
            }
        }

        public bool CanDeleteLinks()
        {
            foreach (var item in GetSelectedItems())
            {
                if (item.CanDelete)
                    return true;
            }

            return false;
        }

        public virtual void DeleteLinks()
        {
            List<LinkInfo> deleteItems = new List<LinkInfo>();
            foreach (var item in GetSelectedItems())
            {
                if (item.CanDelete)
                    deleteItems.Add(item);
            }
            ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(deleteItems, IsDeleteFromDisk);
        }

        public bool CanDeleteLink(LinkInfo link)
        {
            return link != null && link.CanDelete;
        }

        public virtual void DeleteLink(LinkInfo link)
        {
            ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(new List<LinkInfo>() { link }, IsDeleteFromDisk);
        }

        public virtual void SettingLinks()
        {

        }

        public virtual string CopyLinkLocation()
        {
            StringBuilder links = new StringBuilder();
            foreach (var item in GetSelectedItems())
            {
                links.AppendLine(item.PathInfo.Address);
            }
            return links.ToString();
        }

        public bool CanReconnectSelectedLinks()
        {
            foreach (var item in GetSelectedItems())
            {
                if (item.CanStop)
                    return true;
            }
            return false;
        }

        public void ReconnectSelectedLinks()
        {
            foreach (var item in GetSelectedItems())
            {
                if (item.CanStop)
                    item.Reconnect(true);
            }
        }

        //public bool CanAddAndPlayLinkInfo()
        //{
        //    Uri uri;
        //    return Uri.TryCreate(SeachAddress, UriKind.Absolute, out uri);
        //}

        //public void AddAndPlayLinkInfo()
        //{
        //    //LinkInfoesDownloadingManagerViewModel.This.AddLinkInfo(SeachAddress, null, null, null, null, true);
        //    SeachAddress = "";
        //}

        //public bool CanAddLinkInfo()
        //{
        //    return CanAddAndPlayLinkInfo();
        //}

        //public void AddLinkInfo()
        //{
        //    //LinkInfoesDownloadingManagerViewModel.This.AddLinkInfo(SeachAddress, null, null, null, null, false);
        //    SeachAddress = "";
        //}

        //public void ResetGridViewGroups(SelectedToolboxGroupGridEnum mode)
        //{
        //    ViewElement.mainDataGrid.Items.GroupDescriptions.Clear();
        //    if (mode == SelectedToolboxGroupGridEnum.Groups)
        //        ViewElement.mainDataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription("PathInfo.CurrentGroupInfo"));
        //    else if (mode == SelectedToolboxGroupGridEnum.Tasks)
        //        ViewElement.mainDataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription("PathInfo.CurrentQueueInfo"));
        //}

        //public void RefreshGridGrouping(object item)
        //{
        //    IEditableCollectionView view = ViewElement.mainDataGrid.Items as IEditableCollectionView;
        //    view.EditItem(item);
        //    view.CommitEdit();
        //}
    }
}
