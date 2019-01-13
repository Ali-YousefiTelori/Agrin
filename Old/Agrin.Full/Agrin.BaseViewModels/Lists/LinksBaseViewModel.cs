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
            set
            {
                _AddTimeMinutes = value;
                OnPropertyChanged("AddTimeMinutes");
            }
        }

        int _AddTimeHours = DateTime.Now.Hour;

        public int AddTimeHours
        {
            get { return _AddTimeHours; }
            set { _AddTimeHours = value; OnPropertyChanged("AddTimeHours"); }
        }

        int _AddDateYear = DateTime.Now.Year;

        public int AddDateYear
        {
            get { return _AddDateYear; }
            set { _AddDateYear = value; }
        }

        int _AddDateMonth = DateTime.Now.Month;

        public int AddDateMonth
        {
            get { return _AddDateMonth; }
            set { _AddDateMonth = value; }
        }

        int _AddDateDay = DateTime.Now.Day;

        public int AddDateDay
        {
            get { return _AddDateDay; }
            set { _AddDateDay = value; }
        }

        int _StopTimeForMinutes = 5;

        public int StopTimeForMinutes
        {
            get { return _StopTimeForMinutes; }
            set { _StopTimeForMinutes = value; OnPropertyChanged("StopTimeForMinutes"); }
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

        bool _IsStopTask = false;

        public bool IsStopTask
        {
            get { return _IsStopTask; }
            set { _IsStopTask = value; OnPropertyChanged("IsStopTask"); }
        }

        List<TaskUtilityModeEnum> _TaskUtilityActions = new List<TaskUtilityModeEnum>();
        public List<TaskUtilityModeEnum> TaskUtilityActions
        {
            get { return _TaskUtilityActions; }
            set { _TaskUtilityActions = value; }
        }

        List<TaskUtilityModeEnum> _LinkCompleteTaskUtilityActions = new List<TaskUtilityModeEnum>();
        public List<TaskUtilityModeEnum> LinkCompleteTaskUtilityActions
        {
            get { return _LinkCompleteTaskUtilityActions; }
            set { _LinkCompleteTaskUtilityActions = value; }
        }

        public bool CanActiveTaskByDateTimes(DateTime dt)
        {
            if (dt < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0))
                return false;
            return true;
        }

        public virtual DateTime GetDateTimeForAddTask()
        {
#if (MobileApp || __ANDROID__)
            return new DateTime(AddDateYear, AddDateMonth, AddDateDay, AddTimeHours, AddTimeMinutes, 0);
#else
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, AddTimeHours, AddTimeMinutes, 0);
#endif
        }

        public virtual DateTime GetDateTimeForStopTask()
        {
#if (MobileApp || __ANDROID__)
            return new DateTime(AddDateYear, AddDateMonth, AddDateDay, StopTimeHours, StopTimeMinutes, 0);
#else
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, StopTimeHours, StopTimeMinutes, 0);
#endif
        }

        public void AddTimeTask(List<LinkInfo> listLinks)
        {
            int newID = ApplicationTaskManager.Current.GetNewID();
            string taskName = Agrin.IO.Strings.Text.IntToLetters(newID);
            var task = new TaskInfo() { Name = taskName, IsActive = true, TaskItemInfoes = listLinks.Select(x => x.PathInfo.Id).Select<int, TaskItemInfo>(x => new TaskItemInfo() { Mode = TaskItemMode.LinkInfo, Value = x }).ToList() };

            if (StartNow)
            {
                task.IsStartNow = true;
            }
            else
            {
                var dt = GetDateTimeForAddTask();

                if (AddTimeMinutes == DateTime.Now.Minute && AddTimeHours == DateTime.Now.Hour && AddDateYear == DateTime.Now.Year && AddDateMonth == DateTime.Now.Month && AddDateDay == DateTime.Now.Day)
                {
                    task.IsStartNow = true;
                }
                else
                {
                    //dt = dt.AddSeconds(-dt.Second);
                    //dt = dt.AddMinutes(-dt.Minute);
                    //dt = dt.AddHours(-dt.Hour);
                    //dt = dt.AddMinutes(AddTimeMinutes);
                    //dt = dt.AddHours(AddTimeHours);
#if (!MobileApp && !__ANDROID__)
                    if (dt < DateTime.Now)
                        dt = dt.AddDays(1);
#endif
                    task.DateTimes.Add(dt);
                }
            }
            task.TaskUtilityActions.Add(TaskUtilityModeEnum.StartLink);
            task.TaskUtilityActions.AddRange(TaskUtilityActions);
            task.LinkCompleteTaskUtilityActions.AddRange(LinkCompleteTaskUtilityActions);
            ApplicationTaskManager.Current.AddTask(task);
            LinkCompleteTaskUtilityActions.Clear();
            TaskUtilityActions.Clear();
        }

        public void AddTimeTask()
        {
            var listLinks = GetSelectedItems().ToList();
            AddTimeTask(listLinks);
        }

        public void AddStopTimeTask(List<LinkInfo> listLinks)
        {
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
            }
            else
            {
                dt = GetDateTimeForStopTask();
#if (!MobileApp && !__ANDROID__)
                if (dt < DateTime.Now)
                    dt = dt.AddDays(1);
#endif
                //dt = dt.AddSeconds(-dt.Second);
                //dt = dt.AddMinutes(-dt.Minute);
                //dt = dt.AddHours(-dt.Hour);
                //dt = dt.AddMinutes(StopTimeMinutes);
                //dt = dt.AddHours(StopTimeHours);
                //if (dt < DateTime.Now)
                //    dt = dt.AddDays(1);
            }

            task.DateTimes.Add(dt);
            if (IsStopForMinutes)
                task.TaskUtilityActions.Add(TaskUtilityModeEnum.StartLink);
            else
            {
                if (IsStopTask)
                {
                    task.TaskUtilityActions.Add(TaskUtilityModeEnum.DeactiveTasks);
                }
                else
                {
                    task.TaskUtilityActions.Add(TaskUtilityModeEnum.StopLink);
                }
            }
            task.TaskUtilityActions.AddRange(TaskUtilityActions);
            task.LinkCompleteTaskUtilityActions.AddRange(LinkCompleteTaskUtilityActions);
            ApplicationTaskManager.Current.AddTask(task);
            LinkCompleteTaskUtilityActions.Clear();
            TaskUtilityActions.Clear();
        }

        public void AddStopTimeTask()
        {
            var listLinks = GetSelectedItems().ToList();
            AddStopTimeTask(listLinks);
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
                        item.SaveThisLink();
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

        public void RepairLink()
        {
            try
            {
                Agrin.Download.Engine.Repairer.LinkRepairer.LinkRepairerProcessAction = (val, max, state) =>
                {
                    //if (state == Download.Engine.Repairer.LinkRepairerState.FindConnectionProblems)
                    //{
                    //    activity.RunOnUiThread(() =>
                    //    {
                    //        txtState.Text = ViewUtility.FindTextLanguage(this, "FindConnectionProblems_Language");
                    //        progressMain.Progress = 1;
                    //    });
                    //}
                    //else if (state == Download.Engine.Repairer.LinkRepairerState.FindPositionOfProblems)
                    //{
                    //    activity.RunOnUiThread(() =>
                    //    {
                    //        txtState.Text = ViewUtility.FindTextLanguage(this, "FindPositionOfProblems_Language");
                    //        progressMain.Progress = 2;
                    //    });
                    //}
                    //else if (state == Download.Engine.Repairer.LinkRepairerState.DownloadingProblems)
                    //{
                    //    activity.RunOnUiThread(() =>
                    //    {
                    //        txtState.Text = ViewUtility.FindTextLanguage(this, "DownloadingProblems_Language");
                    //        progressMain.Progress = 3;
                    //    });
                    //}
                    //else if (state == Download.Engine.Repairer.LinkRepairerState.FixingProblems)
                    //{
                    //    activity.RunOnUiThread(() =>
                    //    {
                    //        txtState.Text = ViewUtility.FindTextLanguage(this, "FixingProblems_Language");
                    //        progressMain.Progress = 4;
                    //    });
                    //}

                    //activity.RunOnUiThread(() =>
                    //{
                    //    int newMax = 0, newVal = 0;
                    //    if (max > int.MaxValue)
                    //    {
                    //        newMax = int.MaxValue;
                    //        long nval = max - int.MaxValue;
                    //        if (val - nval > 0)
                    //        {
                    //            newVal = (int)(val - nval);
                    //        }
                    //        else
                    //            newVal = 0;
                    //    }
                    //    else
                    //    {
                    //        newMax = (int)max;
                    //        newVal = (int)val;
                    //    }
                    //    progressStage.Max = newMax;
                    //    progressStage.Progress = newVal;
                    //});
                };
                int i = 0;
                //var repair = Agrin.Download.Engine.Repairer.LinkRepairer.RepairFile(SelectedItem.PathInfo.BackUpCompleteAddress, SelectedItem.PathInfo.FullAddressFileName);
                
                var repairByCheckSum = Agrin.Download.Engine.Repairer.LinkRepairer.RepairFileByCheckSum(@"G:\Downloads\errorCheckSum.CheckSum", @"G:\Downloads\trueCheckSum.CheckSum", SelectedItem.PathInfo.FullAddressFileName, SelectedItem.PathInfo.BackUpCompleteAddress);
            }
            catch (Exception e)
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
            return linkInfo != null && linkInfo.DownloadingProperty.State == ConnectionState.Complete && (System.IO.File.Exists(linkInfo.PathInfo.FullAddressFileName) || !string.IsNullOrEmpty(linkInfo.PathInfo.SecurityFileSavePath));
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
            obj.SaveThisLink();
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

        public static bool CanPlayForce(LinkInfo linkInfo)
        {
            string p = linkInfo.GetPercent.Replace("%", "");
            double percent = 0.0;
            double.TryParse(p, out percent);
            if (percent <= 95.0)
            {
                return false;
            }
            return true;
        }

        public void PlayForceLinkInfo(LinkInfo linkInfo)
        {
            if (!CanPlayForce(linkInfo))
            {
                return;
            }
            linkInfo.DownloadingProperty.IsForceDownload = true;
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
            ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(deleteItems);
        }

        public virtual void DeleteCompleteLinks()
        {
            List<LinkInfo> deleteItems = new List<LinkInfo>();
            foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToArray())
            {
                if (item.CanDelete && item.IsComplete)
                    deleteItems.Add(item);
            }
            ApplicationLinkInfoManager.Current.DeleteRangeLinkInfo(deleteItems);
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

        public bool CanChangeLinkSaveLocationCommand()
        {
            return SelectedItem != null && SelectedItem.DownloadingProperty.State != ConnectionState.CopyingFile && SelectedItem.DownloadingProperty.State != ConnectionState.Complete;
        }

        public bool CanChangeSelectedLinksSaveLocationCommand()
        {
            var items = GetSelectedItems();
            if (items == null)
                return false;
            foreach (var item in GetSelectedItems())
            {
                var can = item.DownloadingProperty.State != ConnectionState.CopyingFile && item.DownloadingProperty.State != ConnectionState.Complete;
                if (!can)
                    return false;
            }
            return true;
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
