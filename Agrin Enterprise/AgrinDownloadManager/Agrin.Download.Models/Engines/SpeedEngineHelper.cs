using Agrin.Download.CoreModels.Link;
using Agrin.Log;
using Agrin.Threads;
using SignalGo.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agrin.Download.Engines
{
    public class SpeedEngineHelper
    {
        public static Action TickCalculatedAction { get; set; }
        public static SpeedEngineHelper Current { get; set; }
        private ConcurrentList<LinkInfoCore> ListOfLinks { get; set; }
        //ManualResetEvent ManualResetEvent { get; set; }
        public SpeedEngineHelper(ConcurrentList<LinkInfoCore> listOfLinks)
        {
            ListOfLinks = listOfLinks;
        }

        private bool isStart = false;
        //public bool IsPause { get; set; }
        public void Resume()
        {
            if (isStart)
            {
                //IsPause = false;
                //ManualResetEvent.Set();
                //ManualResetEvent.Reset();
                return;
            }
            isStart = true;

            Task.Run(async () =>
            {
                while (true)
                {
                    List<ShortModels.Link.LinkInfoShort> items = ListOfLinks.Where(x => x.IsDownloading || x.IsCopyingFile).Select(x => x.AsShort()).ToList();
                    bool isPause = items.Count == 0;
                    foreach (ShortModels.Link.LinkInfoShort linkInfo in items)
                    {
                        if (linkInfo.PreviousDownloadedSize > 0)
                            linkInfo.AvarageDownloadedSizePerSecound.Put(linkInfo.DownloadedSize - linkInfo.PreviousDownloadedSize);

                        if (linkInfo.AvarageDownloadedSizePerSecound.Count != 0)
                        {
                            double avarage = linkInfo.AvarageDownloadedSizePerSecound.Read().Average();
                            linkInfo.DownloadedSizePerSecound = (long)avarage;
                            if (avarage > 0)
                            {
                                if (linkInfo.Size < 0)
                                    linkInfo.TimeRemaining = null;
                                else
                                    linkInfo.TimeRemaining = new TimeSpan(0, 0, (int)((linkInfo.Size - linkInfo.DownloadedSize) / avarage));
                            }
                        }

                        linkInfo.PreviousDownloadedSize = linkInfo.DownloadedSize;
                        if (linkInfo.IsDownloading)
                        {
                            foreach (LinkInfoRequestCore connection in linkInfo.Connections.ToArray())
                            {
                                if (connection.CanPlay || connection.IsDownloading)
                                {
                                    if (connection.LastReadDateTime.AddSeconds(connection.LastReadDuration) < DateTime.Now)
                                    {
                                        try
                                        {
                                            bool canBreak = false;
                                            linkInfo.RunInLock(() =>
                                            {
                                                canBreak = linkInfo.isStopping || linkInfo.IsManualStop || linkInfo.CanPlay;
                                            });
                                            if (canBreak)
                                                break;
                                            connection.LastReadDuration += 10;
                                            connection.Stop();
                                            connection.Play();
                                        }
                                        catch (Exception ex)
                                        {
                                            AutoLogger.LogError(ex, "try to recconect");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!isPause)
                        TickCalculatedAction?.Invoke();
                    await Task.Delay(1000);
                    //if (IsPause)
                    //{
                    //    ManualResetEvent.Reset();
                    //    ManualResetEvent.WaitOne();
                    //    Thread.Sleep(1000);
                    //}
                }
            });
        }

        //public void Pause()
        //{
        //    IsPause = true;
        //}
    }
}
