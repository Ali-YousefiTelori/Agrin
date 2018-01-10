using Agrin.Download.Helper;
using Agrin.Download.Manager;
using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Lists
{
    public class LinkInfoesDownloadingManagerBaseViewModel
    {
        public LinkInfoesDownloadingManagerBaseViewModel()
        {

        }

        public static List<LinkInfo> AddLinkInfo(List<string> uriAddress, GroupInfo groupInfo, string userSavePath, string userSecurityPath, string userName, string password, bool isPlay, object shareingValue, Dictionary<string, string> customHeaders = null)
        {
            List<LinkInfo> items = new List<LinkInfo>();
            foreach (var item in uriAddress)
            {
                items.Add(AddLinkInfo(item, groupInfo, userSavePath, userSecurityPath, userName, password, isPlay, shareingValue, customHeaders));
            }
            return items;
        }

        public static LinkInfo AddLinkInfo(string uriAddress, GroupInfo groupInfo, string userSavePath, string userSecurityPath, string userName, string password, bool isPlay, object shareingValue, Dictionary<string, string> customHeaders = null)
        {
            LinkInfo linkInfo = new LinkInfo(uriAddress);
            if (SharingHelper.IsVideoSharing(uriAddress) && shareingValue != null)
                linkInfo.Management.SharingSettings.Add(shareingValue);
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                linkInfo.Management.NetworkUserPass = new Download.Web.Link.NetworkCredentialInfo() { UserName = userName, Password = password };
            if (!string.IsNullOrEmpty(userSavePath))
            {
                if (groupInfo != null)
                {
                    if (!Agrin.IO.Helper.MPath.EqualPath(groupInfo.SavePath, userSavePath))
                    {
                        linkInfo.PathInfo.UserSavePath = userSavePath;
                    }
                }
                else
                    linkInfo.PathInfo.UserSavePath = userSavePath;
            }

            if (!string.IsNullOrEmpty(userSecurityPath))
            {
                linkInfo.PathInfo.UserSecurityPath = userSecurityPath;
            }
            if (customHeaders != null)
            {
                if (linkInfo.DownloadingProperty.CustomHeaders == null)
                    linkInfo.DownloadingProperty.CustomHeaders = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
                foreach (var item in customHeaders)
                {
                    linkInfo.DownloadingProperty.CustomHeaders.TryAdd(item.Key, item.Value);
                }
            }
            ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, groupInfo, isPlay);
            return linkInfo;
            //linkInfo.DownloadingProperty.SelectionChanged = (link, sel) =>
            //{
            //    if (sel)
            //        AddSelection(link);
            //    else
            //        RemoveSelection(link);
            //};
            //linkInfo.CommandBindingChanged = () =>
            //{
            //    ApplicationHelper.EnterDispatcherThreadActionBegin(() =>
            //    {
            //        CommandManager.InvalidateRequerySuggested();
            //    });
            //};
        }

    }
}
