using Agrin.Download.Data;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.Download.Manager
{
    public class ApplicationGroupManager
    {
        #region Static Properties And Methods

        static ApplicationGroupManager _Current;
        public static ApplicationGroupManager Current
        {
            get { return ApplicationGroupManager._Current; }
            set { ApplicationGroupManager._Current = value; }
        }

        #endregion

        #region Properties

        GroupInfo _NoGroup = new GroupInfo() { Name = "بدون گروه", SavePath = MPath.DownloadsPath, Id = 0 };
        public GroupInfo NoGroup
        {
            get { return _NoGroup; }
            set { _NoGroup = value; }
        }

        GroupInfo _AllGroup;
        public GroupInfo AllGroup
        {
            get { return _AllGroup; }
            set { _AllGroup = value; }
        }

        FastCollection<GroupInfo> _GroupInfoes;
        public FastCollection<GroupInfo> GroupInfoes
        {
            get
            {
                if (_GroupInfoes == null)
                    _GroupInfoes = new FastCollection<GroupInfo>(ApplicationHelperMono.DispatcherThread);
                return _GroupInfoes;
            }
            set { _GroupInfoes = value; }
        }

        #endregion

        #region Methods
        long GetNewId()
        {
            if (GroupInfoes.Count == 0)
                return 1;
            return GroupInfoes.Max(x => x.Id) + 1;
        }

        public void AddGroupInfo(GroupInfo groupInfo)
        {
            groupInfo.Id = GetNewId();
            GroupInfoes.Add(groupInfo);
            if (AddedGroups != null)
                AddedGroups(new List<GroupInfo>() { groupInfo });

            SerializeData.SaveGroupInfoesToFile();
        }

        public void AddRangeGroupInfoes(List<GroupInfo> groupInfoes)
        {
            GroupInfoes.AddRange(groupInfoes);
            if (AddedGroups != null)
                AddedGroups(groupInfoes);
            SerializeData.SaveGroupInfoesToFile();
        }

        public void DeleteGroupInfo(GroupInfo groupInfo)
        {
            GroupInfoes.Remove(groupInfo);
            if (RemovedGroups != null)
                RemovedGroups(new List<GroupInfo>() { groupInfo });
            SerializeData.SaveGroupInfoesToFile();
        }
        public void DeleteRangeGroupInfo(List<GroupInfo> groupInfoes)
        {
            GroupInfoes.RemoveRange(groupInfoes);
            if (RemovedGroups != null)
                RemovedGroups(groupInfoes);
            SerializeData.SaveGroupInfoesToFile();
        }
        public void SaveEditGroup(GroupInfo group)
        {
            SerializeData.SaveGroupInfoesToFile();
        }

        public GroupInfo FindGroupByFileName(string fileName)
        {
            var newFileName = Agrin.IO.FileStatic.GetLinksFileName(fileName);
            if (!string.IsNullOrEmpty(newFileName))
                fileName = newFileName;
            string ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(ext))
                return NoGroup;
            ext = ext.ToLower().TrimStart(new char[] { '.' });
            foreach (var item in GroupInfoes)
            {
                if (item.Extentions.Select<string, string>(x => x.ToLower().TrimStart(new char[] { '.' })).Contains(ext))
                {
                    return item;
                }
            }
            return NoGroup;
        }
        public GroupInfo FindGroupByName(string groupName, bool nullable = false)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                if (nullable)
                    return null;
                else
                    return NoGroup;
            }

            foreach (var item in GroupInfoes)
            {
                if (item.Name == groupName)
                {
                    return item;
                }
            }
            return NoGroup;
        }

        public void SetGroupByLinkInfo(LinkInfo linkInfo, GroupInfo groupInfo)
        {
            GroupInfo findGroup = FindGroupByFileName(linkInfo.PathInfo.FileName);
            if (findGroup != null && (groupInfo == findGroup || groupInfo == null))
            {
                linkInfo.PathInfo.UserGroupInfo = null;
                linkInfo.PathInfo.ApplicationGroupInfo = findGroup;
            }
            else
            {
                linkInfo.PathInfo.UserGroupInfo = groupInfo;
                linkInfo.PathInfo.ApplicationGroupInfo = findGroup;
            }
        }

        public void ChangedGroupSavePath(GroupInfo group)
        {
            foreach (var item in ApplicationLinkInfoManager.Current.LinkInfoes.ToArray())
            {
                if (group == item.PathInfo.CurrentGroupInfo)
                    item.PathInfo.OnPropertyChanged("SavePath");
            }
        }

        #endregion

        #region Actions
        public Action<object> ChangedGroups;
        public Action<List<GroupInfo>> AddedGroups;
        public Action<List<GroupInfo>> RemovedGroups;
        #endregion


    }
}
