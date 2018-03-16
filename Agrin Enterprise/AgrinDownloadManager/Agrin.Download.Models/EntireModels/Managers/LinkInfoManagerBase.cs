using Agrin.Download.CoreModels.Link;
using Agrin.Download.Engines;
using Agrin.Download.ShortModels.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.EntireModels.Managers
{
    public abstract class LinkInfoManagerBase
    {
        public abstract List<LinkInfoCore> LinkInfoes { get; }
        /// <summary>
        /// 
        /// </summary>
        public static LinkInfoManagerBase Current { get; set; }

        public SpeedEngineHelper CurrentSpeedEngineHelper { get; set; }

        public LinkInfoShort CreateInstance(string link)
        {
            return LinkInfoCore.CreateInstance(link).AsShort();
        }

        public void Play(LinkInfoCore linkInfoCore)
        {
            if (linkInfoCore.CanPlay)
            {
                linkInfoCore.Play();
                CurrentSpeedEngineHelper?.Resume();
            }
        }

        public void Stop(LinkInfoCore linkInfoCore, bool isFromTask = false)
        {
            if (!isFromTask)
            {
                var tasks = TaskScheduleManagerBase.Current.GetTaskSchedulerInfoesByLinkId(linkInfoCore.Id).Where(y => y.Status == Agrin.Download.CoreModels.Task.TaskStatus.Processing);
                if (tasks.Count() > 0)
                {
                    foreach (var task in tasks)
                    {
                        task.Stop();
                    }
                }
            }
            if (linkInfoCore.CanStop)
                linkInfoCore.Stop();
        }

        public abstract void DeleteRange(IEnumerable<LinkInfoCore> linkInfoes);
    }
}
