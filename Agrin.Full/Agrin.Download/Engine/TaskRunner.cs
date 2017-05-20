using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Agrin.Download.Engine
{
    public class TaskRunner : IDisposable
    {
        static TaskRunner()
        {
            WifiTryToConnectEngine();
        }
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
                    AutoLogger.LogTextTest($"CurrentTask.IsStartNow");
                    if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink))
                    {
                        isDispose = !CanStart();
                    }
                }
                else if (CurrentTask.IsDayOfWeek)
                {
                    AutoLogger.LogTextTest($"CurrentTask.IsDayOfWeek");
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
                        WaitTime(time);
                    }
                }
                else if (CurrentTask.IsTimeAndDays)
                {
                    AutoLogger.LogTextTest($"CurrentTask.IsTimeAndDays");
                    var time = CurrentTask.TimeAndDays - DateTime.Now;
                    if (time.Ticks > 0)
                    {
                        CurrentTask.State = TaskState.WaitingForWork;
                        WaitTime(time);
                    }
                    else
                        Dispose();
                }
                else
                {
                    AutoLogger.LogTextTest($"CurrentTask.else");

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
                    {
                        Dispose();
                    }
                    else
                    {
                        CurrentTask.State = TaskState.WaitingForWork;
                        WaitTime(time);
                    }
                }

                if (CurrentTask != null)
                    AutoLogger.LogTextTest($"CurrentTask.isDispose {CurrentTask.Name} {isDispose}");

                if (!isDispose)
                {
                    if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink))
                    {
                        if (!CanStop())
                        {
                            CurrentTask.State = TaskState.Stoped;
                            ApplicationTaskManager.Current.CompleteAndRestartTaskRunner(CurrentTask);
                            AutoLogger.LogTextTest($"CurrentTask.CompleteAndRestartTaskRunner {CurrentTask.Name}");
                            return;
                        }
                    }
                    CurrentTask.State = TaskState.Working;
                    ApplicationTaskManager.Current.StartAction?.Invoke(CurrentTask);
                }
                else
                {
                    if (CurrentTask != null)
                        CurrentTask.State = TaskState.Stoped;
                }
            });
        }

        void WaitTime(TimeSpan time)
        {
            resetEvent.WaitOne(time);
        }

        ManualResetEvent resetEventOfBreak = new ManualResetEvent(false);
        //started Link of this runner
        List<LinkInfo> startedLinks = new List<LinkInfo>();
        public void StartDownloader()
        {
            AutoLogger.LogTextTest($"TaskRunner StartDownloader {isDispose}");
            if (isDispose)
                return;
            if (CurrentTask.TaskItemInfoes.Count == 0)
                ApplicationTaskManager.Current.ComepleteAction?.Invoke(CurrentTask);
            else if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StopLink))
            {
                AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StopLink");

                foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                {
                    AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StopLink " + linkID.Mode.ToString());
                    if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                        continue;
                    var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);
                    AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StopLink is null? " + (linkInfo == null) + "is downloading ? " + linkInfo.IsDownloading);
                    if (linkInfo != null && linkInfo.IsDownloading)
                    {
                        ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
                    }
                }
                CompleteDownloadAction?.Invoke();
            }
            else if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.StartLink))
            {
                AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StartLink");
                StartDownloadLinkManagement();
                StartNew:
                List<LinkInfo> notCompleteLinks = new List<LinkInfo>();

                foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                {
                    AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StartLink " + linkID.Mode.ToString());
                    if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                        continue;
                    var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);
                    AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StartLink is null? " + (linkInfo == null) + "is not SkippedItems ? " + !CurrentTask.SkippedItems.Contains(linkInfo));
                    if (linkInfo != null && !CurrentTask.SkippedItems.Contains(linkInfo))
                    {
                        AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StartLink state? " + linkInfo.DownloadingProperty.State);
                        if (linkInfo.DownloadingProperty.State != ConnectionState.Complete)
                        {
                            AutoLogger.LogTextTest($"TaskRunner TaskUtilityModeEnum.StartLink IsDownloading? " + linkInfo.IsDownloading);
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
                                            Log.AutoLogger.LogError(e, "StartDownloader");
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
                                        Log.AutoLogger.LogError(e, "StartDownloader 2");
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
                    AutoLogger.LogTextTest($"TaskRunner notCompleteLinks {CurrentTask.Name} {notCompleteLinks.Count}");
                    if (notCompleteLinks.Count > 0)
                    {
                        //link Player
                        foreach (var linkInfo in notCompleteLinks)
                        {
                            AutoLogger.LogTextTest($"TaskRunner notCompleteLinks state {linkInfo.DownloadingProperty.State} contian: {ApplicationLinkInfoManager.Current.LinkInfoes.ToArray().Contains(linkInfo)} no skip: {!CurrentTask.SkippedItems.Contains(linkInfo)}");
                            if (linkInfo.DownloadingProperty.State != ConnectionState.Complete && ApplicationLinkInfoManager.Current.LinkInfoes.ToArray().Contains(linkInfo) && !CurrentTask.SkippedItems.Contains(linkInfo))
                            {
                                startedLinks.Add(linkInfo);
                                AutoLogger.LogTextTest($"TaskRunner notCompleteLinks {CurrentTask.Name} PlayLinkInfo {!linkInfo.IsDownloading}");
                                ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                                {
                                    AutoLogger.LogTextTest($"TaskRunner notCompleteLinks {CurrentTask.Name} PlayLinkInfo ok {!linkInfo.IsDownloading}");
                                    if (!linkInfo.IsDownloading)
                                        ApplicationLinkInfoManager.Current.PlayLinkInfo(linkInfo);
                                });
                                Thread.Sleep(1000);
                                break;
                            }
                        }
                        goto StartNew;
                    }
                    else
                    {
                        isPlayManagement = false;
                        CompleteDownloadAction?.Invoke();
                    }
                }
            }
            else if (CurrentTask.TaskUtilityActions.Contains(TaskUtilityModeEnum.DeactiveTasks))
            {
                AutoLogger.LogTextTest($"TaskRunner DeactiveTasks {CurrentTask.Name} {CurrentTask.TaskItemInfoes.Count}");
                //CompleteDownloadAction?.Invoke();
                foreach (var linkID in CurrentTask.TaskItemInfoes.ToArray())
                {
                    if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                        continue;
                    var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);
                    AutoLogger.LogTextTest($"TaskRunner DeactiveTasks {linkInfo != null}");
                    if (linkInfo != null)
                    {
                        var taskInfoes = ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(linkInfo);
                        AutoLogger.LogTextTest($"TaskRunner DeactiveTasks taskInfoes {taskInfoes != null}");
                        if (taskInfoes != null)
                        {
                            AutoLogger.LogTextTest($"TaskRunner DeactiveTasks taskInfoes count {taskInfoes.Count}");
                            foreach (var taskInfo in taskInfoes)
                            {
                                ApplicationTaskManager.Current.DeActiveTask(taskInfo);
                            }
                        }

                        if (linkInfo.CanStop)
                            ApplicationLinkInfoManager.Current.StopLinkInfo(linkInfo);
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

        public static Action<bool> ChangeWifiStateAction { get; set; }

        static bool lastInternetStatus = false;
        public static void WifiTryToConnectEngine()
        {
            AutoLogger.LogText("WifiTryToConnectEngine Started");
            AsyncActions.Action(() =>
            {
                while (true)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                    try
                    {
                        var find = (from x in ApplicationTaskManager.Current.TaskInfoes.ToArray() where x.State == TaskState.Working && x.TaskUtilityActions.Contains(TaskUtilityModeEnum.WiFiOn) select x).FirstOrDefault();
                        if (find != null)
                        {
                            var isConnected = CheckIsConnectedToInternet();
                            AutoLogger.LogText($"WifiTryToConnectEngine isConnected {isConnected}");
                            if (!isConnected)
                            {
                                AutoLogger.LogText($"WifiTryToConnectEngine try to reconnect wifi");
                                ChangeWifiStateAction?.Invoke(false);
                                Thread.Sleep(new TimeSpan(0, 0, 3));
                                ChangeWifiStateAction?.Invoke(true);
                                Thread.Sleep(new TimeSpan(0, 0, 10));
                                TryAgainToDownloadLinkInfo(find);
                            }
                            else if (!lastInternetStatus)
                            {
                                TryAgainToDownloadLinkInfo(find);
                            }
                            lastInternetStatus = isConnected;
                        }
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, "WifiTryToConnectEngine error! ");
                    }
                }
            });
        }

        static void TryAgainToDownloadLinkInfo(TaskInfo task)
        {
            foreach (var linkID in task.TaskItemInfoes.ToArray())
            {
                if (linkID.Mode != Web.Tasks.TaskItemMode.LinkInfo)
                    continue;
                var linkInfo = ApplicationLinkInfoManager.Current.FindLinkInfoByID((int)linkID.Value);
                if (linkInfo != null && ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.Contains(linkInfo))
                {
                    AutoLogger.LogText($"WifiTryToConnectEngine TryAgainToPlay");
                    linkInfo.TryAgainToPlay();
                }
            }
        }

        static bool CheckIsConnectedToInternet()
        {
            try
            {
                Ping myPing = new Ping();
                var host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
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
                if (item.CanStop)
                    ApplicationLinkInfoManager.Current.StopLinkInfo(item);
            }
            startedLinks.Clear();
            resetEvent = null;
            CurrentTask = null;
        }
    }
}
