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

        public static void AddLinkInfo(List<string> uriAddress, GroupInfo groupInfo, string userSavePath, string userName, string password, bool isPlay, int shareingIndex)
        {
            foreach (var item in uriAddress)
            {
                AddLinkInfo(item, groupInfo, userSavePath, userName, password, isPlay, shareingIndex);
            }
        }

        public static void AddLinkInfo(string uriAddress, GroupInfo groupInfo, string userSavePath, string userName, string password, bool isPlay, int shareingIndex)
        {
            LinkInfo linkInfo = new LinkInfo(uriAddress);
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(uriAddress) && shareingIndex != -1)
                linkInfo.Management.SharingSettings.Add(shareingIndex);
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

            ApplicationLinkInfoManager.Current.AddLinkInfo(linkInfo, groupInfo, isPlay);
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
