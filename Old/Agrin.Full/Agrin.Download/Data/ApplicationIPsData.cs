using Agrin.Data.Mapping;
using Agrin.Download.Data.Serializition;
using Agrin.Framesoft;
using Agrin.Framesoft.Helper;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data
{
    [Serializable]
    public class ApplicationIPsData
    {
        [field: NonSerialized]
        public static ApplicationIPsData Current { get; set; }

        public List<IPPropertiesSerialize> HostIPData { get; set; }
        public Dictionary<string, byte[]> FlagsByCountryCodes { get; set; }

        public static byte[] GetFlagByHostName(string hostName)
        {
            hostName = hostName.ToLower();
            foreach (var item in Current.HostIPData)
            {
                if (item.Host != null && item.Host.ToLower() == hostName)
                    return GetFlagByCountryCode(item.CountryCode);
            }
            return null;
        }

        public static IPPropertiesSerialize GetIPPropertiesSerialize(string host_Ip)
        {
            host_Ip = host_Ip.ToLower();
            foreach (var item in Current.HostIPData)
            {
                if (item.Host != null && item.Host.ToLower() == host_Ip)
                    return item;
            }
            return null;
        }

        public static byte[] GetFlagByCountryCode(string countryCode)
        {
            if (Current.FlagsByCountryCodes.ContainsKey(countryCode))
                return Current.FlagsByCountryCodes[countryCode];
            return null;
        }

        static object lockObj = new object();
        public static byte[] GetOrDownloadFlagByCountryCode(string countryCode)
        {
            if (Current.FlagsByCountryCodes.ContainsKey(countryCode))
                return Current.FlagsByCountryCodes[countryCode];
            else
            {
                AsyncActions.Action(() =>
                {
                    lock (lockObj)
                    {
                        if (Current.FlagsByCountryCodes.ContainsKey(countryCode))
                        {
                            return;
                        }
                        var flag = new System.Net.WebClient().DownloadData("http://www.agrindownloadmanager.ir/Client/GetFlagByCountryCode/" + countryCode);
                        if (!Current.FlagsByCountryCodes.ContainsKey(countryCode))
                            Current.FlagsByCountryCodes.Add(countryCode, flag);
                    }
                });
            }
            return null;
        }

        static object lockOBJ = new object();
        public static IPPropertiesSerialize AddNewHostIP(string host_Ip)
        {
            lock (lockOBJ)
            {
                if (string.IsNullOrEmpty(host_Ip) || host_Ip == "localhost")
                    return null;
                host_Ip = host_Ip.ToLower();
                if (!string.IsNullOrEmpty(host_Ip))
                {
                    var findItem = GetIPPropertiesSerialize(host_Ip);
                    if (findItem != null)
                        return findItem;
                }

                var data = DataSerializationHelper.GetRequestData<IpProperties>("http://framesoft.ir/client/GetIpProperties/" + host_Ip);
                if (data.Data != null)
                {
                    IPPropertiesSerialize map = new IPPropertiesSerialize();
                    map.Host = host_Ip;
                    Mapper.Map<IpProperties, IPPropertiesSerialize>(data.Data, map);
                    if (!string.IsNullOrEmpty(host_Ip))
                        Current.HostIPData.Add(map);
                    if (!Current.FlagsByCountryCodes.ContainsKey(data.Data.CountryCode))
                    {
                        if (data.Data.Flag == null)
                            data.Data.Flag = new byte[] { };
                        Current.FlagsByCountryCodes.Add(data.Data.CountryCode, data.Data.Flag);
                    }
                    if (!string.IsNullOrEmpty(host_Ip))
                        SerializeData.SaveApplicationIPsDataToFile();
                    return map;
                }
            }
            return null;
        }
    }
}
