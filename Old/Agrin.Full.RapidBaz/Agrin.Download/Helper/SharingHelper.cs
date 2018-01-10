using Agrin.Download.Web;
using Agrin.Download.Web.Connections;
using Agrin.Download.Web.Connections.ShareSite;
using Agrin.Download.Web.Link;
using Agrin.LinkExtractor.Aparat;
using Agrin.LinkExtractor.Instagram;
using Agrin.LinkExtractor.RapidBaz;
using Agrin.LinkExtractor.ZeeTVCom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Helper
{
    public static class SharingHelper
    {
        public static bool IsSharingByLink(string address)
        {
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(new Uri(address).Host))
                return true;
            else if (AparatFindDownloadLink.IsAparatLink(address))
                return true;
            else if (InstagramFindDownloadLink.IsInstagramLink(address))
                return true;
            else if (ZeeTVComFindDownloadLink.IsZeeTVLink(address))
                return true;
            else if (RapidBazFindDownloadLink.IsRapidBazLink(address))
                return true;
            return false;
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
            if (Agrin.LinkExtractor.DownloadUrlResolver.IsYoutubeLink(address))
                return new YoutubeConnectionInfo(address, linkWebRequest);
            else if (AparatFindDownloadLink.IsAparatLink(address))
                return new AparatConnectionInfo(address, linkWebRequest);
            else if (InstagramFindDownloadLink.IsInstagramLink(address))
                return new InstagramConnectionInfo(address, linkWebRequest);
            else if (ZeeTVComFindDownloadLink.IsZeeTVLink(address))
                return new ZeeTVComConnectionInfo(address, linkWebRequest);
            else if (RapidBazFindDownloadLink.IsRapidBazLink(address))
                return new RapidBazConnectionInfo(address, linkWebRequest);

            return null;
        }
    }
}
