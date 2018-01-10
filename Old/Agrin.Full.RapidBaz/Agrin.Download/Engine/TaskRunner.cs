using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.Engine
{
    public class TaskRunner : IDisposable
    {
        public Action CompleteDownloadAction { get; set; }

        TaskInfo _currentTask;
        public TaskInfo CurrentTask
        {
            get { return _currentTask; }
            set { _currentTask = value; }
        }

        ManualResetEvent resetEvent = new ManualResetEvent(false);
        public TaskRunner(TaskInfo task)
        {
            CurrentTask = task;
        }

        public void Start()
        {
            CurrentTask.State = TaskState.Started;
            AsyncActions.Action(() =>
            {
                Func<bool> CanStart = () =>
                {
                    bool canDownload = false;
                    foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                    {
                        if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                            continue;
                        var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);

                        if (linkInfo != null && !linkInfo.IsComplete && !CurrentTask.SkippedItems.Contains(linkInfo))
                        {
                            canDownload = true;
                            break;
                        }
                    }
                    return canDownload;
                };
                Func<bool> CanStop = () =>
                {
                    bool canStop = false;
                    foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                    {
                        if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                            continue;
                        var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);

                        if (linkInfo != null && !linkInfo.IsManualStop)
                        {
                            canStop = true;
                            break;
                        }
                    }
                    return canStop;
                };


                if (CurrentTask.IsStartNow)
                {
                    if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink))
                    {
                        isDispose = !CanStart();
                    }
                }
                else if (CurrentTask.IsDayOfWeek)
                {
                    var now = GetNextWeekday(DateTime.Now, CurrentTask.DayOfWeek);
                    var time = now - DateTime.Now;
                    bool seted = false;
                    var list = CurrentTask.DateTimes.ToList();
                    list.Sort();
                    foreach (var item in list)
                    {
                        if (time.Add(item.TimeOfDay).Ticks > 0)
                        {
                            seted = true;
                            time = time.Add(item.TimeOfDay);
                            break;
                        }
                    }
                    if (time.Ticks < 0 || !seted)
                        Dispose();
                    else
                    {
                        CurrentTask.State = TaskState.WaitingForWork;
                        resetEvent.WaitOne(time);
                    }
                }
                else if (CurrentTask.IsTimeAndDays)
                {
                    var time = CurrentTask.TimeAndDays - DateTime.Now;
                    if (time.Ticks > 0)
                    {
                        CurrentTask.State = TaskState.WaitingForWork;
                        resetEvent.WaitOne(time);
                    }
                    else
                        Dispose();
                }
                else
                {
                    bool seted = false;
                    var list = CurrentTask.DateTimes.ToList();
                    list.Sort();
                    DateTime now = DateTime.Now;
                    TimeSpan time = TimeSpan.MinValue;
                    foreach (var item in list)
                    {
                        if (now < item)
                        {
                            seted = true;
                            time = item - now;
                            break;
                        }
                    }
                    if (time.Ticks < 0 || !seted)
                        Dispose();
                    else
                    {
                        CurrentTask.State = TaskState.WaitingForWork;
                        resetEvent.WaitOne(time);
                    }
                }

                if (!isDispose)
                {
                    if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink))
                    {
                        if (!CanStop())
                        {
                            CurrentTask.State = TaskState.Stoped;
                            return;
                        }
                    }
                    CurrentTask.State = TaskState.Working;
                    ApplicationTaskManager.Current.StartAction(CurrentTask);
                }
                else
                {
                    if (CurrentTask != null)
                        CurrentTask.State = TaskState.Stoped;
                }
            });
        }


        ManualResetEvent resetEventOfBreak = new ManualResetEvent(false);
        //started Link of this runner
        List<LinkInfo> startedLinks = new List<LinkInfo>();

        public void StartDownloader()
        {
            if (CurrentTask.TaskItemInfoes.Count == 0)
                ApplicationTaskManager.Current.ComepleteAction(CurrentTask);
            else if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink))
            {
                foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                {
                    if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                        continue;
                    var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);
                    if (linkInfo != null && linkInfo.IsDownloading)
                    {
                        ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
                    }
                    CompleteDownloadAction();
                }
            }
            else if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink))
            {
                StartDownloadLinkManagement();
            StartNew:
                List<LinkInfo> notCompleteLinks = new List<LinkInfo>();

                foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                {
                    if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                        continue;
                    var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);
                    if (linkInfo != null && !CurrentTask.SkippedItems.Contains(linkInfo))
                    {
                        if (linkInfo.DownloadingProperty.State != ConnectionState.Complete)
                        {
                            if (linkInfo.IsDownloading)
                            {
                                var RsieOne = resetEventOfBreak;
                                linkInfo.FinishAction = (iplay) =>
                                {
                                    Action set = null;
                                    if (!isDispose)
                                    {
                                        set = () =>
                                        {
                                            resetEventOfBreak.Set();
                                        };
                                        try
                                        {
                                            if (linkInfo.IsManualStop && !CurrentTask.SkippedItems.Contains(linkInfo))
                                                CurrentTask.SkippedItems.Add(linkInfo);
                                        }
                                        catch (Exception e)
                                        {
                                        }
                                    }

                                    linkInfo.FinishAction = null;
                                    return set;
                                };
                                if (linkInfo.IsDownloading)
                                {
                                    try
                                    {
                                        resetEventOfBreak.WaitOne();
                                        if (!isDispose)
                                            resetEventOfBreak.Reset();
                                        else
                                            break;
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                }
                                if (linkInfo.DownloadingProperty.State == ConnectionState.Complete)
                                {
                                    if (!isDispose)
                                        goto StartNew;
                                }
                                else
                                    notCompleteLinks.Add(linkInfo);
                            }
                            else
                            {
                                notCompleteLinks.Add(linkInfo);
                            }
                        }
                    }
                    if (isDispose)
                        break;
                }
                if (!isDispose)
                {
                    if (notCompleteLinks.Count > 0)
                    {
                        //link Player
                        foreach (var linkInfo in notCompleteLinks)
                        {
                            if (linkInfo.DownloadingProperty.State != ConnectionState.Complete && ApplicationLinkInfoManager.Current.LinkInfoes.ToArray().Contains(linkInfo) && !CurrentTask.SkippedItems.Contains(linkInfo))
                            {
                                startedLinks.Add(linkInfo);
                                ApplicationHelperMono.EnterDispatcherThreadAction(() =>
                                {
                                    ApplicationLinkInfoManager.Current.PlayLinkInfo(linkInfo);
                                });
                                break;
                            }
                        }
                        goto StartNew;
                    }
                    else
                    {
                        isPlayManagement = false;
                        CompleteDownloadAction();
                    }
                }

            }
        }

        Dictionary<LinkInfo, long> downloadedSizeOflinks = new Dictionary<LinkInfo, long>();
        bool isPlayManagement = false;
        void StartDownloadLinkManagement()
        {
            if (isPlayManagement)
                return;
            isPlayManagement = true;
            AsyncActions.Action(() =>
            {
                do
                {
                    Thread.Sleep(new TimeSpan(0, 5, 0));
                    if (!isDispose)
                    {
                        foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                        {
                            if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                                continue;
                            var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);
                            if (linkInfo.IsDownloading)
                            {
                                if (downloadedSizeOflinks.ContainsKey(linkInfo))
                                {
                                    if (downloadedSizeOflinks[linkInfo] == linkInfo.DownloadingProperty.DownloadedSize)
                                    {
                                        CurrentTask.TaskItemInfoes.Remove(linkID);
                                        CurrentTask.TaskItemInfoes.Add(linkID);
                                        ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
                                    }
                                    else
                                        downloadedSizeOflinks[linkInfo] = linkInfo.DownloadingProperty.DownloadedSize;
                                }
                                else
                                {
                                    downloadedSizeOflinks.Add(linkInfo, linkInfo.DownloadingProperty.DownloadedSize);
                                }
                            }
                        }
                    }
                }
                while (!isDispose);
                isPlayManagement = false;
            });
        }

        static DateTime GetNextWeekday(DateTime start, List<DayOfWeek> days)
        {
            var today = start.DayOfWeek;
            int daysToAdd = 7;
            foreach (var day in days)
            {
                var d = ((int)day - (int)start.DayOfWeek + 7) % 7;
                if (d <= daysToAdd)
                    daysToAdd = d;
            }

            return start.AddDays(daysToAdd);
        }

        bool isDispose = false;
        public void Dispose()
        {
            if (isDispose)
                return;
            CurrentTask.State = TaskState.Stoped;
            isDispose = true;
            resetEvent.Set();
            resetEvent.Dispose();
            resetEventOfBreak.Set();
            resetEventOfBreak.Dispose();
            foreach (var item in startedLinks.ToArray())
            {
                if (item.IsDownloading)
                    ApplicationLinkInfoManager.Current.StopLinkInfo(item);
            }
            startedLinks.Clear();
            resetEvent = null;
            CurrentTask = null;
        }
    }
}
