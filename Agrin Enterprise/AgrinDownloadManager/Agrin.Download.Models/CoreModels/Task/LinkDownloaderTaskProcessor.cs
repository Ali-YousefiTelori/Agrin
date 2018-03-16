using Agrin.Download.CoreModels.Link;
using Agrin.Download.EntireModels.Managers;
using Agrin.Log;
using Agrin.Threads;
using SignalGo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.CoreModels.Task
{
    /// <summary>
    /// proccessor for download links
    /// </summary>
    public class LinkDownloaderTaskProcessor : ITaskProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        public LinkDownloaderTaskProcessor(TaskItemSetting setting)
        {
            Setting = setting;
        }

        /// <summary>
        /// ids of links
        /// </summary>
        public TaskItemSetting Setting { get; set; }
        /// <summary>
        /// status
        /// </summary>
        public TaskStatus Status { get; set; } = TaskStatus.Stopped;

        /// <summary>
        /// is disposed
        /// </summary>
        public bool IsDispose { get; set; }
        /// <summary>
        /// parent of task
        /// </summary>
        public TaskSchedulerInfo Parent { get; set; }

        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            Stop();
            Status = TaskStatus.Disabled;
            IsDispose = true;
        }

        /// <summary>
        /// finished
        /// </summary>
        public void Finished()
        {
            Status = TaskStatus.Finished;
            Parent.OneTaskFinished();
        }

        /// <summary>
        /// start
        /// </summary>
        public void Start()
        {
            this.RunInLock(() =>
            {
                if (Status == TaskStatus.Disabled || Status == TaskStatus.Finished || Status == TaskStatus.Processing)
                    return;

                Status = TaskStatus.Processing;
                IEnumerable<LinkInfoCore> linkInfos = LinkInfoManagerBase.Current.LinkInfoes.Where(x => Setting.Ids.Contains(x.Id)).OrderBy(x => Setting.Ids.IndexOf(x.Id));

                AsyncActions.Run(() =>
                {
                    if (Setting.DelayTimeToStart != null)
                        Thread.Sleep(Setting.DelayTimeToStart.Value);

                    while (!IsDispose && Status == TaskStatus.Processing)
                    {
                        if (linkInfos.Count(x => !x.IsDispose && (x.CanPlayInActual || x.IsDownloadingInActual)) == 0)
                        {
                            Finished();
                            break;
                        }
                        this.RunInLock(() =>
                        {
                            if (!IsDispose && Status == TaskStatus.Processing)
                            {
                                var counOfDownloading = linkInfos.Count(x => !x.IsDispose && x.IsDownloadingInActual);
                                if (counOfDownloading < Setting.ConcurrentCount)
                                {
                                    foreach (var linkInfo in linkInfos.Where(x => !x.IsDispose && x.CanPlay).Take(Setting.ConcurrentCount.Value - counOfDownloading))
                                    {
                                        LinkInfoManagerBase.Current.Play(linkInfo);
                                    }
                                }
                            }
                        });
                        Thread.Sleep(1000);
                    }
                }, (ex) =>
                {
                    Status = TaskStatus.Error;
                    AutoLogger.LogError(ex, "LinkDownloaderTaskProcessor Start");
                });
            });
        }

        /// <summary>
        /// stop
        /// </summary>
        public void Stop()
        {
            this.RunInLock(() =>
            {
                if (Status != TaskStatus.Disabled && Status != TaskStatus.Finished)
                {
                    Status = TaskStatus.Stopped;
                    IEnumerable<LinkInfoCore> linkInfos = LinkInfoManagerBase.Current.LinkInfoes.Where(x => Setting.Ids.Contains(x.Id));
                    foreach (var linkInfo in linkInfos.Where(x => !x.IsDispose && x.CanStop))
                    {
                        LinkInfoManagerBase.Current.Stop(linkInfo, true);
                    }
                }
            });
        }
    }
}
