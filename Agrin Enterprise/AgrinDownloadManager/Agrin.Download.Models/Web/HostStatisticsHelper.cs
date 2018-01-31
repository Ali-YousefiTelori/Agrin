using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web
{
    public class HostStatisticsInfo
    {
        [JsonProperty("as")]
        public string As { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("isp")]
        public string ISP { get; set; }
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lon")]
        public double Lon { get; set; }
        [JsonProperty("org")]
        public string Org { get; set; }
        [JsonProperty("query")]
        public string Qquery { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("regionName")]
        public string RegionName { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
    }

    public static class HostStatisticsHelper
    {
        public static HostStatisticsInfo GetHostStatistics(string hostName)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://ip-api.com/json/" + hostName);
            using (var response = webRequest.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader =new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        if (!json.Contains("\"country\""))
                            throw new Exception("error to GetHostStatistics");
                        return JsonConvert.DeserializeObject<HostStatisticsInfo>(json);
                    }
                }
            }
        }
    }
}
