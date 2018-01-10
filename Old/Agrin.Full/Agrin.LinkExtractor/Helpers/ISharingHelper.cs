using Agrin.LinkExtractor.Facebook;
using Agrin.LinkExtractor.Instagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.LinkExtractor.Helpers
{
    public enum SharingNameEnum
    {
        Aparat,
        Youtube,
        RadioJavan,
        Facebook,
        None
    }

    /// <summary>
    /// کلاسی که باید داده هاش تغییر کنه کلاس 
    /// Agrin.Download.Helper.SharingHelper
    /// هست
    /// </summary>
    public interface ISharingHelper
    {
        //bool IsVideoSharing(string url, bool isSelectQuality = false);

        //SharingNameEnum GetVideoSharingName(string url);
    }
}
