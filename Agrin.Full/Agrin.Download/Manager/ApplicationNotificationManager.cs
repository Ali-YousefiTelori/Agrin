using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Manager
{
    public class ApplicationNotificationManager
    {
        static ApplicationNotificationManager _Current;
        public static ApplicationNotificationManager Current
        {
            get { return ApplicationNotificationManager._Current; }
            set { ApplicationNotificationManager._Current = value; }
        }

        public Action NotifyCountChanged;
        public Action<NotificationMode, LinkInfo> NotificationInfoChanged;

        int _NotifyCount;
        public int NotifyCount
        {
            get { return _NotifyCount; }
            set
            {
                _NotifyCount = value;
                if (NotifyCountChanged != null)
                    NotifyCountChanged();
            }
        }

        FastCollection<NotificationInfo> _NotificationInfoes;

        public FastCollection<NotificationInfo> NotificationInfoes
        {
            get
            {
                if (_NotificationInfoes == null)
                    _NotificationInfoes = new FastCollection<NotificationInfo>(ApplicationHelperBase.DispatcherThread);
                return _NotificationInfoes;
            }
            set { _NotificationInfoes = value; }
        }

        private NotificationInfo _CurrentNotification;
        public NotificationInfo CurrentNotification
        {
            get { return _CurrentNotification; }
            set { _CurrentNotification = value; }
        }

        Dictionary<NotificationMode, NotificationInfo> notifyItems = new Dictionary<NotificationMode, NotificationInfo>();

        public bool ExistID(List<LinkInfo> items, int id)
        {
            foreach (var item in items)
            {
                if (item.PathInfo.Id == id)
                    return true;
            }
            return false;
        }

        object lockOBJ = new object();
        public void Add(NotificationMode mode, LinkInfo item, bool save = true)
        {
            ApplicationHelperBase.EnterDispatcherThreadAction(() =>
            {
                lock (lockOBJ)
                {
                    if (NotificationInfoChanged != null)
                        NotificationInfoChanged(mode, item);
                    if (notifyItems.ContainsKey(mode))
                    {
                        if (!ExistID(notifyItems[mode].Items.ToList(), item.PathInfo.Id))
                            notifyItems[mode].Items.Add(item);
                        notifyItems[mode].NotifyDateTime = DateTime.Now;
                    }
                    else// if (!notifyItems[mode].Items.Contains(item))
                    {
                        NotificationInfo notify = new NotificationInfo() { IsRead = false, Mode = mode };
                        notify.NotifyDateTime = DateTime.Now;
                        notify.Items.Add(item);
                        notifyItems.Add(mode, notify);
                        NotificationInfoes.Insert(0, notify);
                        NotifyCount = notifyItems.Count;
                    }
                    if (item.Management.IsShowBalloon && ApplicationSetting.Current.LinkInfoDownloadSetting.IsShowBalloon)
                    {
                        if (ApplicationBalloonManager.Current.BalloonLinkInfoes.Contains(item))
                            ApplicationBalloonManager.Current.ShowAndSelect(item);
                        else
                            ApplicationBalloonManager.Current.AddBalloon(item);
                    }
                    if (save)
                        Save();
                }
            });

        }
        object lockobj = new object();
        public void AddRangeDesrialized(List<NotificationInfo> items)
        {
            lock (lockobj)
            {
                int unRead = 0;
                foreach (var item in items)
                {
                    NotificationInfoes.Add(item);
                    if (!item.IsRead)
                    {
                        unRead++;
                        if (!notifyItems.ContainsKey(item.Mode))
                        {
                            notifyItems.Add(item.Mode, item);
                        }
                    }
                }
                NotifyCount = unRead;
                CleanUp();
            }
        }

        public void ReadNotify(NotificationInfo notificationInfo, bool save = true)
        {
            foreach (var item in notifyItems.Values.ToList())
            {
                if (notificationInfo == item)
                {
                    notificationInfo.IsRead = true;
                    notifyItems.Remove(item.Mode);
                    break;
                }
            }
            NotifyCount = notifyItems.Count;
            if (save)
                Save();
        }

        public void Save()
        {
            Agrin.Download.Data.SerializeData.SaveNotificationToFile();
        }

        public void CleanUp()
        {
            List<NotificationInfo> removeRange = new List<NotificationInfo>();
            foreach (var item in NotificationInfoes.ToArray())
            {
                foreach (var info in item.Items.ToList())
                {
                    if (!ApplicationLinkInfoManager.Current.LinkInfoes.ToArray().Contains(info))
                        item.Items.Remove(info);
                }
                if (item.Items.Count == 0)
                {
                    removeRange.Add(item);
                    ReadNotify(item, false);
                }
            }
            if (removeRange.Count > 0)
            {
                NotificationInfoes.RemoveRange(removeRange);
                Save();
            }
            NotifyCount = notifyItems.Count;
        }
    }
}
