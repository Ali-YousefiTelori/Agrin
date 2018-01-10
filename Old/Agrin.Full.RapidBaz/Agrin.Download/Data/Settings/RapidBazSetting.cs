using Agrin.Download.Manager;
using Agrin.Download.Web;
using Agrin.Helper.Collections;
using Agrin.RapidBaz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class RapidBazSetting
    {
        public bool IsSaveSetting { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsRefreshAutomatic { get; set; }
        public long RefreshTime { get; set; }
        public bool IsAddToLinksAutomatic { get; set; }

        /// <summary>
        /// queueID must Check From Manager if complete must Download in adm
        /// </summary>
        List<RapidBazLinkInfo> _DownloadAfterCompleteInRapidBaz;
        public List<RapidBazLinkInfo> DownloadAfterCompleteInRapidBaz
        {
            get
            {
                if (_DownloadAfterCompleteInRapidBaz == null)
                    _DownloadAfterCompleteInRapidBaz = new List<RapidBazLinkInfo>();
                return _DownloadAfterCompleteInRapidBaz;
            }
            set { _DownloadAfterCompleteInRapidBaz = value; }
        }

        List<string> _DownloadedRapidBazItems = new List<string>();

        public List<string> DownloadedRapidBazItems
        {
            get
            {
                if (_DownloadedRapidBazItems == null)
                    _DownloadedRapidBazItems = new List<string>();
                return _DownloadedRapidBazItems;
            }
            set { _DownloadedRapidBazItems = value; }
        }

        public static FastCollection<RapidItemInfo> QueueItems { get; set; }

        public void DownloadAfterComplete(string queueID, bool isQueue)
        {
            if (DownloadAfterCompleteInRapidBaz.Where(x => x.QueueID == queueID).FirstOrDefault() == null)
            {
                DownloadAfterCompleteInRapidBaz.Add(new RapidBazLinkInfo() { QueueID = queueID, SendToQueueAfterComplete = isQueue });
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            }
            //QueueIdChangedFromList(queueID);
        }

        public void RemoveAfterComplete(string queueID)
        {
            var item = DownloadAfterCompleteInRapidBaz.Where(x => x.QueueID == queueID).FirstOrDefault();
            if (item != null)
            {
                DownloadAfterCompleteInRapidBaz.Remove(item);
                Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            }
            //QueueIdChangedFromList(item.QueueID);
        }

        public void RemoveAfterComplete(RapidBazLinkInfo item)
        {
            DownloadAfterCompleteInRapidBaz.Remove(item);
            Agrin.Download.Data.SerializeData.SaveApplicationSettingToFile();
            //QueueIdChangedFromList(item.QueueID);
        }

        public void AddToDownloadList(string uriAddress, RapidBazLinkInfo item)
        {
            LinkInfo linkInfo = new LinkInfo(uriAddress);
            ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, null, true);
            RemoveAfterComplete(item);
        }

        //public bool IsExistInDownloadList(string queueID)
        //{
        //    if (QueueItems == null)
        //        return false;
        //    foreach (var item in QueueItems.ToArray())
        //    {
        //        if (item.ID == queueID)
        //            return true;
        //    }
        //    return false;
        //}
        public bool IsExistInDownloadList(string queueID)
        {
            foreach (var item in DownloadAfterCompleteInRapidBaz.ToArray())
            {
                if (item.QueueID == queueID)
                    return true;
            }
            return false;
        }

        //void QueueIdChangedFromList(string queueID)
        //{
        //    if (QueueItems == null)
        //        return;
        //    RapidItemInfo rapidItem = null;
        //    foreach (var item in QueueItems.ToArray())
        //    {
        //        if (item.ID == queueID)
        //        {
        //            rapidItem = item;
        //            break;
        //        }
        //    }
        //    if (rapidItem != null)
        //    {
        //        bool isToList = false;
        //        foreach (var item in DownloadAfterCompleteInRapidBaz.ToArray())
        //        {
        //            if (item.QueueID == rapidItem.ID)
        //            {
        //                isToList = true;
        //                break;
        //            }
        //        }
        //        rapidItem.IsDownloadOnOrOff = isToList;
        //    }
        //}

        public void AddRangeDownloadAfterComplete(List<RapidBazLinkInfo> list)
        {
            DownloadAfterCompleteInRapidBaz.AddRange(list);
            //foreach (var item in list)
            //{
            //    QueueIdChangedFromList(item.QueueID);
            //}
        }

        public void Initialize()
        {
            RapidItemInfo.DownloadAfterCompleteChangedAction = (item, val) =>
            {
                if (val)
                    DownloadAfterComplete(item.ID, true);
                else
                    RemoveAfterComplete(item.ID);
            };

            RapidItemInfo.DownloadAfterCompleteChangedFunction = (item) =>
            {
                return IsExistInDownloadList(item.ID);
            };

            RapidItemInfo.GetFlagByCountryCode = (code) =>
            {
                return ApplicationIPsData.GetOrDownloadFlagByCountryCode(code); ;
            };
        }
    }
}
