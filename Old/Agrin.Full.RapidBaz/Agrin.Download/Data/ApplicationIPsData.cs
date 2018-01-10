using Agrin.Data.Mapping;
using Agrin.Download.Data.Serializition;
using Agrin.Framesoft;
using Agrin.Framesoft.Helper;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Data
{
    [Serializable]
    public class ApplicationIPsData
    {
        static string dbFileName = "";
        static ApplicationIPsData()
        {
            dbFileName = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "Resources", "IpToCountry", "GeoLiteCity.dat");
        }


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
            var flag = GetFlasByCountryCode(countryCode);
            return flag;
        }

        static byte[] GetFlasByCountryCode(string countryCode)
        {
            string file = System.IO.Path.Combine(Agrin.IO.Helper.MPath.CurrentAppDirectory, "Resources", "IpToCountry", "Flags", countryCode + ".gif");
            var flag = System.IO.File.ReadAllBytes(file);
            return flag;
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
                            Agrin.RapidBaz.Models.RapidItemInfo.OnChangedFlag();
                            return;
                        }
                        //var flag = new System.Net.WebClient().DownloadData("http://www.agrindownloadmanager.ir/Client/GetFlagByCountryCode/" + countryCode);

                        var flag = GetFlasByCountryCode(countryCode);
                        if (!Current.FlagsByCountryCodes.ContainsKey(countryCode))
                            Current.FlagsByCountryCodes.Add(countryCode, flag);
                        Agrin.RapidBaz.Models.RapidItemInfo.OnChangedFlag();
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
                host_Ip = host_Ip.ToLower();
                if (!string.IsNullOrEmpty(host_Ip))
                {
                    var findItem = GetIPPropertiesSerialize(host_Ip);
                    if (findItem != null)
                        return findItem;
                }
                else if (string.IsNullOrEmpty(host_Ip))
                {
                    host_Ip = GetPublicIP();
                }
                long ip = 0;
                bool isIP = long.TryParse(host_Ip.Replace(".", ""), out ip);
                if (!isIP)
                {
                    host_Ip = GetHostIP(host_Ip);
                }
                //var data = DataSerializationHelper.GetRequestData<IpProperties>("http://framesoft.ir/client/GetIpProperties/" + host_Ip);
                var data = GetCountryByIP(host_Ip);
                if (data != null)
                {
                    IPPropertiesSerialize map = new IPPropertiesSerialize();
                    map.Host = host_Ip;
                    Mapper.Map<IpProperties, IPPropertiesSerialize>(data, map);
                    if (!string.IsNullOrEmpty(host_Ip))
                        Current.HostIPData.Add(map);
                    if (!Current.FlagsByCountryCodes.ContainsKey(data.CountryCode))
                    {
                        //if (data.Data.Flag == null)
                        //    data.Data.Flag = new byte[] { };
                        Current.FlagsByCountryCodes.Add(data.CountryCode, GetFlagByCountryCode(data.CountryCode));
                    }
                    if (!string.IsNullOrEmpty(host_Ip))
                        SerializeData.SaveApplicationIPsDataToFile();
                    return map;
                }
            }
            return null;
        }

        public static string GetPublicIP()
        {
            return new System.Net.WebClient().DownloadString("https://ipinfo.io/ip").Trim();
        }

        public static string GetHostIP(string host)
        {
            IPAddress[] addresslist = Dns.GetHostAddresses(host);
            List<string> ipAddress = new List<string>();
            foreach (IPAddress theaddress in addresslist)
            {
                ipAddress.Add(theaddress.ToString());
            }
            return ipAddress.First();
        }

        public static IpProperties GetCountryByIP(string ipAddress)
        {
            IpProperties ipProperties = new IpProperties();
            LookupService ls = new LookupService(dbFileName, LookupService.GEOIP_MEMORY_CACHE);
            //get country of the ip address
            Country c = ls.getCountry(ipAddress);
            ipProperties.CountryCode = c.getCode();
            return ipProperties;
        }
    }
}
