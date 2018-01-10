using Agrin.Download.Data;
using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Manager
{
    public class ApplicationLinkInfoManager
    {
        #region Static Properties

        static ApplicationLinkInfoManager _Current;
        public static ApplicationLinkInfoManager Current
        {
            get { return ApplicationLinkInfoManager._Current; }
            set { ApplicationLinkInfoManager._Current = value; }
        }

        #endregion

        #region Properties

        public Action<long> ChangedDownloadedSizeAction { get; set; }

        public Action<LinkInfo, bool> SelectionChanged { get; set; }

        ConcurrentList<int> _Ids = new ConcurrentList<int>();
        public ConcurrentList<int> Ids
        {
            get { return _Ids; }
            set { _Ids = value; }
        }

        FastCollection<LinkInfo> _LinkInfoes;
        public FastCollection<LinkInfo> LinkInfoes
        {
            get
            {
                if (_LinkInfoes == null)
                    _LinkInfoes = new FastCollection<LinkInfo>(ApplicationHelperBase.DispatcherThread);
                return _LinkInfoes;
            }
            set { _LinkInfoes = value; }
        }

        public Action<LinkInfo> RemovedFromDownloadingLinkInfoes { get; set; }
        FastCollection<LinkInfo> _DownloadingLinkInfoes;
        public FastCollection<LinkInfo> DownloadingLinkInfoes
        {
            get
            {
                if (_DownloadingLinkInfoes == null)
                    _DownloadingLinkInfoes = new FastCollection<LinkInfo>(ApplicationHelperBase.DispatcherThread);
                return _DownloadingLinkInfoes;
            }
            set { _DownloadingLinkInfoes = value; }
        }

        long _DownloadedStopedLinksSize = 0;

        public long DownloadedStopedLinksSize
        {
            get { return _DownloadedStopedLinksSize; }
            set
            {
                if (isResetingDownloadedSize)
                    return;
                _DownloadedStopedLinksSize = value;
                ChangedDownloadedSize();
            }
        }

        public long DownloadedDownlosingLinksSize { get; set; }

        #endregion

        #region Methods

        void RemoveFromDownloadingLinkInfoes(LinkInfo linkInfo)
        {
            DownloadingLinkInfoes.Remove(linkInfo);
            if (RemovedFromDownloadingLinkInfoes != null)
                RemovedFromDownloadingLinkInfoes(linkInfo);
        }

        public void PlayLinkInfo(LinkInfo linkInfo)
        {
            ApplicationServiceData.AddItem(linkInfo.PathInfo.Id);
            if (!DownloadingLinkInfoes.Contains(linkInfo))
                DownloadingLinkInfoes.Add(linkInfo);
            _DownloadedStopedLinksSize -= linkInfo.DownloadingProperty.DownloadedSize;
            linkInfo.Play(true);
            linkInfo.DisposeAction = () =>
            {
                RemoveFromDownloadingLinkInfoes(linkInfo);
                linkInfo.DisposeAction = null;
            };
        }

        public void PauseLinkInfo(LinkInfo linkInfo)
        {
            linkInfo.Pause();
            ApplicationServiceData.RemoveItem(linkInfo.PathInfo.Id);
            RemoveFromDownloadingLinkInfoes(linkInfo);
        }

        public void StopLinkInfo(LinkInfo linkInfo)
        {
            linkInfo.Stop(true);
            linkInfo.SaveThisLink();
            linkInfo.SaveBackUpThisLink();
            DownloadedStopedLinksSize += linkInfo.DownloadingProperty.DownloadedSize;
            RemoveFromDownloadingLinkInfoes(linkInfo);
            ApplicationServiceData.RemoveItem(linkInfo.PathInfo.Id);
        }

        public void DisposeLinkInfo(LinkInfo linkInfo)
        {
            linkInfo.Dispose();
            ApplicationServiceData.RemoveItem(linkInfo.PathInfo.Id);
        }

        public void AddSelection(LinkInfo linkInfo)
        {
            linkInfo.DownloadingProperty.SelectionChanged = (info, sel) =>
            {
                SelectionChanged?.Invoke(linkInfo, sel);
            };
        }

        public void AddRangeDeserializedData(List<LinkInfo> linkInfoes)
        {
            LinkInfoes.AddRange(linkInfoes);
            foreach (var item in linkInfoes)
            {
                if (ApplicationSetting.Current.IsSettingForAllLinks && item.Management.IsApplicationSetting)
                {
                    DeSerializeData.AppSettingToLinkInfo(item);
                }
                AddSelection(item);
                Ids.Add(item.PathInfo.Id);
            }
        }

        public void AddLinkInfo(LinkInfo linkInfo, GroupInfo groupInfo, bool isPlay)
        {
            int id = FindNewId();
            linkInfo.PathInfo.Id = id;
            Ids.Add(id);
            if (ApplicationSetting.Current.IsSettingForNewLinks || ApplicationSetting.Current.IsSettingForAllLinks)
            {
                DeSerializeData.AppSettingToLinkInfo(linkInfo);
            }

            LinkInfoes.Insert(0, linkInfo);
            ApplicationGroupManager.Current.SetGroupByLinkInfo(linkInfo, groupInfo);
            if (isPlay)
            {
                PlayLinkInfo(linkInfo);
            }
            AddSelection(linkInfo);
            SerializeData.SaveLinkInfoesToFile();
        }

        public LinkInfo FindLinkInfoByID(int id)
        {
            foreach (var item in LinkInfoes.ToList())
            {
                if (item.PathInfo.Id == id)
                    return item;
            }
            return null;
        }

        public void AddRangeLinkInfo(List<LinkInfo> linkInfoes, GroupInfo groupInfo, bool isPlay)
        {
            LinkInfoes.AddRange(linkInfoes);
            foreach (var item in linkInfoes)
            {
                ApplicationGroupManager.Current.SetGroupByLinkInfo(item, groupInfo);
                int id = FindNewId();
                item.PathInfo.Id = id;
                Ids.Add(id);
                if (ApplicationSetting.Current.IsSettingForNewLinks)
                {
                    DeSerializeData.AppSettingToLinkInfo(item);
                }
                if (isPlay)
                    PlayLinkInfo(item);
                AddSelection(item);

            }
            SerializeData.SaveLinkInfoesToFile();
        }

        public void DeleteRangeLinkInfo(List<LinkInfo> linkInfoes)
        {
            long downloadedSize = 0;
            foreach (var item in linkInfoes)
            {
                ApplicationTaskManager.Current.RemoveLinkInfoFromStartTasks(item);
                ApplicationTaskManager.Current.RemoveLinkInfoFromStopedTasks(item);
                ApplicationServiceData.RemoveItem(item.PathInfo.Id);
                item.Dispose();
                downloadedSize += item.DownloadingProperty.DownloadedSize;
                item.Destruction();
            }
            LinkInfoes.RemoveRange(linkInfoes);
            ApplicationSetting.Current.OldDownloadedSize += downloadedSize;
            ResetDownloadedSize();
            SerializeData.SaveApplicationSettingToFile();
            SerializeData.SaveLinkInfoesToFile();
            ApplicationNotificationManager.Current.CleanUp();
            ApplicationBalloonManager.Current.CleanUp();
        }

        public int FindNewId()
        {
            int id = 0;
            if (Ids.Count == 0)
                id = 0;
            else
                id = Ids.Max() + 1;
            while (System.IO.Directory.Exists(System.IO.Path.Combine(Agrin.IO.Helper.MPath.SaveDataPath, id.ToString())))
            {
                id++;
            }
            return id;
        }


        public Action<LinkInfo> NotSupportResumableLinkAction { get; set; }
        public void AddNewLinkInfoToNotSupportResumable(LinkInfo linkInfo)
        {
            if (NotSupportResumableLinkAction != null)
                NotSupportResumableLinkAction(linkInfo);
        }

        public void ClearDataForNotSupportResumableLink(LinkInfo linkInfo)
        {
            StopLinkInfo(linkInfo);
            ApplicationSetting.Current.OldDownloadedSize += linkInfo.DownloadingProperty.DownloadedSize;
            SerializeData.SaveApplicationSettingToFile();
            var cons = linkInfo.Connections.ToList();
            if (cons.Count == 0)
                return;
            foreach (var item in cons)
            {
                IOHelper.DeleteFile(item.SaveFileName);
                item.DownloadedSize = 0;
            }
            for (int i = 1; i < cons.Count; i++)
            {
                cons[i].Stop(true);
                linkInfo.Connections.RemoveAt(i);
            }
            if (linkInfo.DownloadingProperty.Size >= 0)
                cons[0].EndPosition = cons[0].Length = linkInfo.DownloadingProperty.Size;
            else
            {
                cons[0].EndPosition = -2;
                cons[0].Length = 0;
                linkInfo.DownloadingProperty.Size = -2;
            }
            linkInfo.DownloadingProperty.DownloadedSize = 0;
            linkInfo.SaveThisLink();
        }

        bool isResetingDownloadedSize = false;
        public void ResetDownloadedSize()
        {
            isResetingDownloadedSize = true;
            long downloadedSize = 0;
            _DownloadedStopedLinksSize = 0;
            downloadedSize += DownloadedStopedLinksSize;
            foreach (var item in LinkInfoes)
            {
                if (!item.IsDownloading)
                {
                    downloadedSize += item.DownloadingProperty.DownloadedSize;
                }
            }
            isResetingDownloadedSize = false;
            DownloadedStopedLinksSize += downloadedSize;
            ChangedDownloadedSize();
        }

        object changedLock = new object();
        public void ChangedDownloadedSize()
        {
            if (!isResetingDownloadedSize && ChangedDownloadedSizeAction != null)
            {
                lock (changedLock)
                {
                    long downloadedSize = ApplicationSetting.Current.OldDownloadedSize;
                    downloadedSize += DownloadedStopedLinksSize;
                    foreach (var item in LinkInfoes.ToList())
                    {
                        if (item.IsDownloading)
                        {
                            downloadedSize += item.DownloadingProperty.DownloadedSize;
                        }
                    }
                    ChangedDownloadedSizeAction(downloadedSize);
                }
            }
        }

        public void ChangeSaveDataPath(string target, Action<Exception, Exception> complete, Action<string, string, int, int> progress)
        {
            var activeTasks = new List<TaskInfo>();
            foreach (var item in ApplicationTaskManager.Current.TaskInfoes.ToList())
            {
                activeTasks.Add(item);
                ApplicationTaskManager.Current.DeActiveTask(item);
            }

            var downloadings = DownloadingLinkInfoes.ToList();
            foreach (var item in downloadings)
            {
                StopLinkInfo(item);
            }

            Action reInit = () =>
            {
                foreach (var item in downloadings)
                {
                    PlayLinkInfo(item);
                }
                foreach (var item in activeTasks)
                {
                    ApplicationTaskManager.Current.ActiveTask(item);
                }
            };
            target = System.IO.Path.Combine(target, "ADM");
            DirectoryMoveHelper directoryMoveHelper = new DirectoryMoveHelper(MPath.SaveDataPath, target);
            directoryMoveHelper.CompleteAction = () =>
            {
                ApplicationSetting.Current.PathSetting.SaveDataPath = MPath.SaveDataPath = target;
                SerializeData.SaveApplicationSettingToFile();
                foreach (var item in LinkInfoes.ToList())
                {
                    item.PathInfo.ConnectionsSavedAddress = null;
                    foreach (var connections in item.Connections.ToList())
                    {
                        connections.SaveFileName = null;
                    }
                }
                reInit();
                complete(null, null);
            };
            directoryMoveHelper.MovedAction = (s, t, pos) =>
            {
                progress?.Invoke(s, t, directoryMoveHelper.FileCount, pos);
            };
            directoryMoveHelper.ErrorMovingAction = (ex1, ex2) =>
            {
                reInit();
                complete(ex1, ex2);
            };
            directoryMoveHelper.Move();
        }

        #endregion
    }
}
