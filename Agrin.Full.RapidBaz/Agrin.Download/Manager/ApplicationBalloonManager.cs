using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.Manager
{
    public enum BalloonMode
    {
        Show,
        Hide,
        Changed
    }
    public class ApplicationBalloonManager
    {
        private static ApplicationBalloonManager _Current;
        public static ApplicationBalloonManager Current
        {
            get { return ApplicationBalloonManager._Current; }
            set { ApplicationBalloonManager._Current = value; }
        }

        public Action<BalloonMode> ShowBalloonAction;

        LinkInfo _CurrentLinkInfo;
        public LinkInfo CurrentLinkInfo
        {
            get { return _CurrentLinkInfo; }
            set { _CurrentLinkInfo = value; }
        }

        FastCollection<LinkInfo> _BalloonLinkInfoes;
        public FastCollection<LinkInfo> BalloonLinkInfoes
        {
            get
            {
                if (_BalloonLinkInfoes == null)
                    _BalloonLinkInfoes = new FastCollection<LinkInfo>(ApplicationHelperMono.DispatcherThread);
                return _BalloonLinkInfoes;
            }
            set { _BalloonLinkInfoes = value; }
        }

        public void AddBalloon(LinkInfo linkInfo)
        {
            BalloonLinkInfoes.Add(linkInfo);
            if (CurrentLinkInfo == null)
                Next();
            if (ShowBalloonAction != null)
                ShowBalloonAction(BalloonMode.Show);
        }

        public void Clear()
        {
            CurrentLinkInfo = null;
            HideBalloon();
            BalloonLinkInfoes.Clear();
        }

        public void CleanUp()
        {
            List<LinkInfo> removes = new List<LinkInfo>();
            foreach (var item in BalloonLinkInfoes)
            {
                if (!ApplicationLinkInfoManager.Current.LinkInfoes.Contains(item))
                    removes.Add(item);
            }

            BalloonLinkInfoes.RemoveRange(removes);
            if (BalloonLinkInfoes.Count == 0)
                HideBalloon();
            else if (CurrentLinkInfo == null || !BalloonLinkInfoes.Contains(CurrentLinkInfo))
            {
                CurrentLinkInfo = BalloonLinkInfoes.First();
                if (ShowBalloonAction != null)
                    ShowBalloonAction(BalloonMode.Changed);
            }
        }

        public void ShowBalloon()
        {
            if (ShowBalloonAction != null)
                ShowBalloonAction(BalloonMode.Show);
            if (CurrentLinkInfo == null || BalloonLinkInfoes.Contains(CurrentLinkInfo))
                Next();
        }

        public void HideBalloon()
        {
            if (ShowBalloonAction != null)
                ShowBalloonAction(BalloonMode.Hide);
        }

        public void ShowAndSelect(LinkInfo item)
        {
            CurrentLinkInfo = item;
            if (ShowBalloonAction != null)
                ShowBalloonAction(BalloonMode.Show);
        }

        public void Next()
        {
            if (BalloonLinkInfoes.Count == 0)
                return;
            int index = CurrentLinkInfo != null ? BalloonLinkInfoes.IndexOf(CurrentLinkInfo) : -1;
            if (index == -1)
                index = 0;
            else
                index++;
            if (index >= BalloonLinkInfoes.Count)
                index = 0;
            CurrentLinkInfo = BalloonLinkInfoes[index];
        }

        public void Back()
        {
            if (BalloonLinkInfoes.Count == 0)
                return;
            int index = CurrentLinkInfo != null ? BalloonLinkInfoes.IndexOf(CurrentLinkInfo) : -1;
            index--;
            if (index < 0)
                index = BalloonLinkInfoes.Count - 1;
            CurrentLinkInfo = BalloonLinkInfoes[index];
        }
    }
}
