using Agrin.Download.Data.Settings;
using Agrin.Download.Web;
using Agrin.Download.Web.Connections;
using Agrin.Download.Web.Connections.ShareSite;
using Agrin.Download.Web.Link;
using Agrin.IO.Helper;
using Agrin.LinkExtractor;
using Agrin.LinkExtractor.Aparat;
using Agrin.LinkExtractor.Facebook;
using Agrin.LinkExtractor.Helpers;
using Agrin.LinkExtractor.Instagram;
using Agrin.LinkExtractor.Models;
using Agrin.LinkExtractor.RadioJavan;
using Agrin.LinkExtractor.ZeeTVCom;
using System;
using System.Collections.Generic;
using YoutubeExtractor;

namespace Agrin.Download.Helper
{
    public static class SharingHelper
    {
        public static bool IsVideoSharing(string url, bool isSelectQuality = false)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (DownloadUrlResolver.IsYoutubeLink(url))
                return true;
            else if (AparatFindDownloadLink.IsAparatLink(url))
                return true;
            else if (RadioJavanFindDownloadLink.IsRadioJavanLink(url))
                return true;
            else if (InstagramFindDownloadLink.IsInstagramLink(url) && !isSelectQuality)
                return true;
            else if (FacebookFindDownloadLink.IsFacebookLink(url))
                return true;
            return false;
        }

        public static SharingNameEnum GetVideoSharingName(string url)
        {
            if (DownloadUrlResolver.IsYoutubeLink(url))
                return SharingNameEnum.Youtube;
            else if (AparatFindDownloadLink.IsAparatLink(url))
                return SharingNameEnum.Aparat;
            else if (RadioJavanFindDownloadLink.IsRadioJavanLink(url))
                return SharingNameEnum.RadioJavan;
            else if (FacebookFindDownloadLink.IsFacebookLink(url))
                return SharingNameEnum.Facebook;
            return SharingNameEnum.None;
        }

        public static AConnectionInfo GetConnectionFromAlgoritm(string address, AlgoritmEnum algoritm, LinkWebRequest linkWebRequest)
        {
            if (algoritm == AlgoritmEnum.Page)
                return new PageConnectionInfo(address, linkWebRequest);
            else if (algoritm == AlgoritmEnum.Sharing)
                return GetSharingAlgoritm(address, linkWebRequest);
            return new NormalConnectionInfo(address, linkWebRequest);
        }

        public static AConnectionInfo GetSharingAlgoritm(string address, LinkWebRequest linkWebRequest)
        {
            if (DownloadUrlResolver.IsYoutubeLink(address))
                return new YoutubeConnectionInfo(address, linkWebRequest);
            else if (AparatFindDownloadLink.IsAparatLink(address))
                return new AparatConnectionInfo(address, linkWebRequest);
            else if (RadioJavanFindDownloadLink.IsRadioJavanLink(address))
                return new RadioJavanConnectionInfo(address, linkWebRequest);
            else if (InstagramFindDownloadLink.IsInstagramLink(address))
                return new InstagramConnectionInfo(address, linkWebRequest);
            else if (ZeeTVComFindDownloadLink.IsZeeTVLink(address))
                return new ZeeTVComConnectionInfo(address, linkWebRequest);
            else if (FacebookFindDownloadLink.IsFacebookLink(address))
                return new FacebookConnectionInfo(address, linkWebRequest);
            return null;
        }

        public static List<PublicSharingInfo> GetSharingInfo(string address)
        {
            List<PublicSharingInfo> items = new List<PublicSharingInfo>();
            var sName = GetVideoSharingName(address);
            if (sName == SharingNameEnum.Youtube)
            {
                var youtubes = DownloadUrlResolver.GetDownloadUrls(address);
                foreach (var item in youtubes)
                {
                    string text = "";
                    if (item.AudioType == AudioType.Unknown && string.IsNullOrEmpty(item.AudioExtension))
                        text = " ، بدون صدا ";
                    if (item.VideoType == VideoType.Unknown && string.IsNullOrEmpty(item.VideoExtension))
                        text += "، بدون تصویر";
                    items.Add(new PublicSharingInfo() { Text = "نوع : " + item.VideoType + " کیفیت " + item.Resolution + text, Data = item });
                }
            }
            else if (sName == SharingNameEnum.Aparat)
            {
                var aparats = Agrin.LinkExtractor.Aparat.AparatFindDownloadLink.FindLinkFromSite(address, ApplicationSetting.Current.ProxySetting.GetFirstActiveProxy()?.GetProxy());
                foreach (var item in aparats.Qualities)
                {
                    items.Add(new PublicSharingInfo() { Text = "نوع : " + MPath.GetFileExtention(item.Link).TrimStart('.') + " " + item.Quality, Data = item });
                }
            }
            else if (sName == SharingNameEnum.RadioJavan)
            {
                var aparats = Agrin.LinkExtractor.RadioJavan.RadioJavanFindDownloadLink.FindLinkFromSite(address, ApplicationSetting.Current.ProxySetting.GetFirstActiveProxy()?.GetProxy());
                foreach (var item in aparats.Qualities)
                {
                    items.Add(new PublicSharingInfo() { Text = "نوع : " + MPath.GetFileExtention(item.Link).TrimStart('.') + " " + item.Quality, Data = item });
                }
            }
            else if (sName == SharingNameEnum.RadioJavan)
            {
                var aparats = Agrin.LinkExtractor.RadioJavan.RadioJavanFindDownloadLink.FindLinkFromSite(address, ApplicationSetting.Current.ProxySetting.GetFirstActiveProxy()?.GetProxy());
                foreach (var item in aparats.Qualities)
                {
                    items.Add(new PublicSharingInfo() { Text = "نوع : " + MPath.GetFileExtention(item.Link).TrimStart('.') + " " + item.Quality, Data = item });
                }
            }
            else if (sName == SharingNameEnum.Facebook)
            {
                var facebooks = FacebookFindDownloadLink.FindLinkFromSite(address, ApplicationSetting.Current.ProxySetting.GetFirstActiveProxy()?.GetProxy());
                foreach (var item in facebooks)
                {
                    items.Add(new PublicSharingInfo() { Text = "کیفیت : " + item.LinkType, Data = item });
                }
            }
            return items;
        }

        public static object GetSharingIndex(string address, IList<PublicSharingInfo> sharingLinks, int sharingLinksSelectedIndex)
        {
            if (sharingLinks.Count == 0 || sharingLinksSelectedIndex == -1)
                return null;
            if (DownloadUrlResolver.IsYoutubeLink(address))
                return ((VideoInfo)sharingLinks[sharingLinksSelectedIndex].Data).FormatCode;
            else if (AparatFindDownloadLink.IsAparatLink(address))
                return ((AparatQualityInfo)sharingLinks[sharingLinksSelectedIndex].Data).Quality;
            else if (RadioJavanFindDownloadLink.IsRadioJavanLink(address))
                return ((RadioJavanQualityInfo)sharingLinks[sharingLinksSelectedIndex].Data).Quality;
            else if (FacebookFindDownloadLink.IsFacebookLink(address))
                return (int)((FacebookQualityInfo)sharingLinks[sharingLinksSelectedIndex].Data).LinkType;

            return null;
        }

    }
}
