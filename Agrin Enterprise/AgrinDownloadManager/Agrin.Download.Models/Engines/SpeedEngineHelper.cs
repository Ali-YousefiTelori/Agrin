using Agrin.Download.CoreModels.Link;
using Agrin.Download.EntireModels.Managers;
using SignalGo.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.Engines
{
    public class SpeedEngineHelper
    {
        public static SpeedEngineHelper Current { get; set; }
        ConcurrentList<LinkInfoCore> ListOfLinks { get; set; }
        ManualResetEvent ManualResetEvent { get; set; }
        public SpeedEngineHelper(ConcurrentList<LinkInfoCore> listOfLinks)
        {
            ListOfLinks = listOfLinks;
        }

        public bool IsPause { get; set; }
        public void Resume()
        {
            if (ManualResetEvent != null)
            {
                IsPause = false;
                ManualResetEvent.Set();
                ManualResetEvent.Reset();
                return;
            }
            ManualResetEvent = new ManualResetEvent(false);
            ManualResetEvent.Reset();

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    var items = ListOfLinks.Where(x => x.IsDownloading).Select(x => x.AsShort()).ToList();
                    IsPause = items.Count == 0;
                    foreach (var linkInfo in items)
                    {
                        linkInfo.AvarageDownloadedSizePerSecound.Put(linkInfo.DownloadedSize - linkInfo.PreviousDownloadedSize);
                        
                        if (linkInfo.AvarageDownloadedSizePerSecound.Count != 0)
                        {
                            var avarage = linkInfo.AvarageDownloadedSizePerSecound.Read().Average();
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
                    }
                    Thread.Sleep(1000);
                    if (IsPause)
                    {
                        ManualResetEvent.Reset();
                        ManualResetEvent.WaitOne();
                    }
                }
            });
            thread.Start();
        }

        public void Pause()
        {
            IsPause = true;
        }
    }
}
