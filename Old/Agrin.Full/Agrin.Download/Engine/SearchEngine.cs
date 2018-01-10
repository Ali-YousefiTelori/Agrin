using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Engine
{
    public static class SearchEngine
    {
        static SearchEngine()
        {
            ApplicationLinkInfoManager.Current.LinkInfoes.CollectionChanged += LinkInfoes_CollectionChanged;
        }

        static void LinkInfoes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Search();
        }

        static bool _isAll = true, _isComplete, _isError, _isNoComplete, _isDownloading, _isQueue;
        public static void FilterBy(bool isAll = false, bool isComplete = false, bool isError = false, bool isNoComplete = false, bool isDownloading = false, bool isQueue = false)
        {
            _isAll = isAll;
            _isComplete = isComplete;
            _isError = isError;
            _isNoComplete = isNoComplete;
            _isDownloading = isDownloading;
            _isQueue = isQueue;

            Search();
        }

        static bool IsLinkInfoInQueue(LinkInfo linkInfo)
        {
            if (linkInfo.IsAddedToTaskForStart || linkInfo.IsAddedToTaskForStop)
            {
                foreach (var taskInfo in ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(linkInfo))
                {
                    if (taskInfo.State == TaskState.WaitingForWork || taskInfo.State == TaskState.Working)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<LinkInfo> GetList()
        {
            //if (!_isQueue)
            //{
            var ie = from x in ApplicationLinkInfoManager.Current.LinkInfoes where (_isAll || (_isComplete && x.DownloadingProperty.State == ConnectionState.Complete) || (_isError && x.DownloadingProperty.State == ConnectionState.Error) || (_isDownloading && !x.IsManualStop) || (_isNoComplete && x.DownloadingProperty.State != ConnectionState.Error && x.DownloadingProperty.State != ConnectionState.Complete) || (_isQueue && IsLinkInfoInQueue(x))) select x;
            return ie.ToList();
            //}
            //else
            //{
            //    List<LinkInfo> links = new List<LinkInfo>();
            //    foreach (var linkInfo in ApplicationLinkInfoManager.Current.LinkInfoes)
            //    {
            //        if (linkInfo.IsAddedToTaskForStart || linkInfo.IsAddedToTaskForStop)
            //        {
            //            foreach (var taskInfo in ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(linkInfo))
            //            {
            //                if (taskInfo.State == TaskState.WaitingForWork || taskInfo.State == TaskState.Working)
            //                {
            //                    links.Add(linkInfo);
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    return links;
            //}
        }

        private static FastCollection<LinkInfo> _Items;
        public static FastCollection<LinkInfo> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new FastCollection<LinkInfo>(ApplicationHelperBase.DispatcherThread);
                return SearchEngine._Items;
            }
            set { SearchEngine._Items = value; }
        }

        static string _SearchText = "";
        public static string SearchText
        {
            get { return SearchEngine._SearchText; }
            set { SearchEngine._SearchText = value; }
        }

        static GroupInfo _CurrentGroup;
        public static GroupInfo CurrentGroup
        {
            get { return _CurrentGroup; }
            set { _CurrentGroup = value; }
        }

        public static bool IsAppLoading { get; set; }
        public static bool IsNotify { get; set; }
        public static void Search()
        {
            try
            {
                if (IsAppLoading)
                    return;
                List<LinkInfo> adding = new List<LinkInfo>();
                List<LinkInfo> removing = new List<LinkInfo>();

                string text = String.IsNullOrEmpty(SearchText) ? SearchText : SearchText.ToLower();
                List<LinkInfo> allForSearchItems = null;
                if (IsNotify && ApplicationNotificationManager.Current.CurrentNotification != null)
                    allForSearchItems = ApplicationNotificationManager.Current.CurrentNotification.Items.ToList();
                else
                    allForSearchItems = GetList();

                foreach (var item in Items)
                {
                    if (!allForSearchItems.Contains(item))
                        removing.Add(item);
                }
                foreach (var item in allForSearchItems)
                {
                    if (item.PathInfo.CurrentGroupInfo.IsSelected && (String.IsNullOrEmpty(text) || item.Management.ContainsAddress(text)) && (CurrentGroup == null || CurrentGroup == item.PathInfo.CurrentGroupInfo))
                    {
                        if (!Items.Contains(item))
                            adding.Add(item);
                    }
                    else
                    {
                        if (Items.Contains(item))
                            removing.Add(item);
                    }
                }

                Items.RemoveRangeNotChanged(removing);
                Items.InsertRangeNotChanged(0, adding);
                Items.OnCollectionChanged();
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Search", true);
            }
        }

        public static void Search(LinkInfo info)
        {
            try
            {
                if (IsAppLoading || info == null)
                    return;
                string text = String.IsNullOrEmpty(SearchText) ? SearchText : SearchText.ToLower();
                List<LinkInfo> allForSearchItems = null;
                if (IsNotify && ApplicationNotificationManager.Current.CurrentNotification != null)
                    allForSearchItems = ApplicationNotificationManager.Current.CurrentNotification.Items.ToList();
                else
                    allForSearchItems = GetList();

                if (!allForSearchItems.Contains(info))
                {
                    Items.Remove(info);
                }
                if (info.PathInfo.CurrentGroupInfo.IsSelected && (String.IsNullOrEmpty(text) || info.Management.ContainsAddress(text)) && (CurrentGroup == null || CurrentGroup == info.PathInfo.CurrentGroupInfo))
                {
                    if (!Items.Contains(info))
                        Items.Add(info);
                }
                else
                {
                    if (Items.Contains(info))
                        Items.Remove(info);
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Search 2", true);
            }

        }

        public static void Show(List<LinkInfo> items)
        {
            Items.Clear();
            Items.AddRange(items);
        }
    }
}
