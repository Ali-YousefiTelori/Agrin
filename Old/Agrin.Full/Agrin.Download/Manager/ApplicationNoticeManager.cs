using Agrin.Download.Data;
using Agrin.Framesoft.Messages;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Manager
{
    public enum NoticeMode
    {
        PublicMessage = 0,//پیام مدیر برای همه ی کاربران
        UserMessage = 1,//پیام مدیر برای کاربر مورد نظر
        YourMessage = 2 //پیامی که کاربر از طریق نرم افزار به مدیر ارسال میکند
    }

    [Serializable]
    public class NoticeInfo
    {
        public NoticeMode Mode { get; set; }
        public ILinkMessage Data { get; set; }
        public bool IsRead { get; set; }
    }

    public class ApplicationNoticeManager
    {
        static ApplicationNoticeManager _Current;

        public static ApplicationNoticeManager Current
        {
            get { return ApplicationNoticeManager._Current; }
            set { ApplicationNoticeManager._Current = value; }
        }

        public Action<NoticeInfo> NoticeAddedAction;

        private FastCollection<NoticeInfo> _Items;
        public FastCollection<NoticeInfo> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new FastCollection<NoticeInfo>(ApplicationHelperBase.DispatcherThread);
                return _Items;
            }
            set { _Items = value; }
        }

        public int NotReadCount
        {
            get
            {
                int count = 0;
                foreach (var item in Items.ToArray())
                {
                    if (!item.IsRead)
                        count++;
                }
                return count;
            }
        }

        public void DeSerialiedData(NoticeInfo[] items)
        {
            Items.AddRange(items.Take(10));
        }

        public bool ExistID(NoticeInfo noticeInfo)
        {
            int id = 0;
            if (noticeInfo.Mode == NoticeMode.PublicMessage || noticeInfo.Mode == NoticeMode.YourMessage)
            {
                id = ((Framesoft.Messages.PublicMessageInfoReceiveData)noticeInfo.Data).MessageID;
            }
            else if (noticeInfo.Mode == NoticeMode.UserMessage)
            {
                id = ((Framesoft.Messages.UserMessageInfoData)noticeInfo.Data).MessageID;
            }
            foreach (var info in Items)
            {
                int newID = 0;
                if (info.Mode == NoticeMode.PublicMessage || info.Mode == NoticeMode.YourMessage)
                {
                    newID = ((Framesoft.Messages.PublicMessageInfoReceiveData)info.Data).MessageID;
                }
                else if (info.Mode == NoticeMode.UserMessage)
                {
                    newID = ((Framesoft.Messages.UserMessageInfoData)info.Data).MessageID;
                }
                if (id == newID)
                    return true;
            }
            return false;
        }

        public void AddNotice(NoticeInfo item)
        {
            if (ExistID(item))
                return;
            Items.Insert(0, item);
            if (NoticeAddedAction != null)
                NoticeAddedAction(item);
            SerializeData.SaveNoticesToFile();
        }

        public void SetReadNotic(NoticeInfo item)
        {
            item.IsRead = true;
            SerializeData.SaveNoticesToFile();
        }

        public DateTime GetMaximumUserMessageDateTime()
        {
            DateTime dt = DateTime.MinValue;
            foreach (var item in Items)
            {
                if (item.Mode == NoticeMode.UserMessage)
                {
                    var newDT = ((UserMessageInfoData)item.Data).MessageDateTime;
                    if (newDT > dt)
                        dt = newDT;
                }
            }
            return dt;
        }

        public DateTime GetMaximumPublicMessageDateTime()
        {
            DateTime dt = DateTime.MinValue;
            foreach (var item in Items)
            {
                if (item.Mode == NoticeMode.PublicMessage)
                {
                    var newDT = ((PublicMessageInfoReceiveData)item.Data).MessageDateTime;
                    if (newDT > dt)
                        dt = newDT;
                }
            }
            return dt;
        }
    }
}
