using Agrin.Download.CoreModels.Link;
using Agrin.Download.Engines;
using Agrin.Download.ShortModels.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.EntireModels.Managers
{
    public class LinkInfoManager
    {
        private static LinkInfoManager _current;

        public static LinkInfoManager Current
        {
            get
            {
                if (_current == null)
                    _current = new LinkInfoManager();
                return _current;
            }
            set
            {
                _current = value;
            }
        }

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

        public void Stop(LinkInfoCore linkInfoCore)
        {
            if (linkInfoCore.CanStop)
                linkInfoCore.Stop();
        }
    }
}
