using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Users
{
    public class RapidUserInfo
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonProperty("dls_total")]
        public string FilesCount { get; set; }

        [JsonProperty("spc_used")]
        public string UploadSize { get; set; }
        [JsonProperty("spc_total")]
        public string TotalSize { get; set; }

        [JsonProperty("pts_buy")]
        public string ScoreBuy { get; set; }
        [JsonProperty("pts_dls")]
        public string ScoreDownload { get; set; }

        public string UploadSizeText
        {
            get
            {
                return GetSizeString(UploadSize);
            }
        }

        public string TotalSizeText
        {
            get
            {
                return GetSizeString(TotalSize);
            }
        }

        public string RemainingSizeText
        {
            get
            {
                return GetSizeString((long.Parse(TotalSize) - long.Parse(UploadSize)).ToString());
            }
        }

        string GetSizeString(string stringSize)
        {
            try
            {
                long size = long.Parse(stringSize);
                if (size > 1024)
                    return (size / 1024) + " گیگابایت";
                else
                    return size + " مگابایت";
            }
            catch
            {

            }
            return stringSize;
        }
    }
}
