﻿using Agrin.Download.CoreModels.Link;
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
        public static Action TickCalculatedAction { get; set; }
        public static SpeedEngineHelper Current { get; set; }
        ConcurrentList<LinkInfoCore> ListOfLinks { get; set; }
        //ManualResetEvent ManualResetEvent { get; set; }
        public SpeedEngineHelper(ConcurrentList<LinkInfoCore> listOfLinks)
        {
            ListOfLinks = listOfLinks;
        }
        bool isStart = false;
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

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    var items = ListOfLinks.Where(x => x.IsDownloading || x.IsCopyingFile).Select(x => x.AsShort()).ToList();
                    bool isPause = items.Count == 0;
                    foreach (var linkInfo in items)
                    {
                        if (linkInfo.PreviousDownloadedSize > 0)
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
                    if (!isPause)
                        TickCalculatedAction?.Invoke();
                    Thread.Sleep(1000);
                    //if (IsPause)
                    //{
                    //    ManualResetEvent.Reset();
                    //    ManualResetEvent.WaitOne();
                    //    Thread.Sleep(1000);
                    //}
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        //public void Pause()
        //{
        //    IsPause = true;
        //}
    }
}
