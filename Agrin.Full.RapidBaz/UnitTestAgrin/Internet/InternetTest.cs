using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace UnitTestAgrin.Internet
{
    /// <summary>
    /// Summary description for InternetTest
    /// </summary>
    [TestClass]
    public class InternetTest
    {
        [TestMethod]
        public void GetIpAddress()
        {
            var ip = GetPublicIP();
            string howtogeek = "google.com";

            IPAddress[] addresslist = Dns.GetHostAddresses(howtogeek);
            List<string> ipAddress = new List<string>();
            foreach (IPAddress theaddress in addresslist)
            {
                ipAddress.Add(theaddress.ToString());
            }

            string GeoipDb = @"D:\BaseProjects\Agrin Download Manager\Agrin.Full.RapidBaz\Agrin.Windows.UI\bin\Debug\Resources\IpToCountry\GeoLiteCity.dat";
            //open the database
            LookupService ls = new LookupService(GeoipDb, LookupService.GEOIP_MEMORY_CACHE);
            //get country of the ip address
            var ok = ls.getDatabaseInfo();
            Country c = ls.getCountry(ipAddress.First());
            var code = c.getCode();
            var name = c.getName();
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

        public static string GetPublicIP()
        {
            return new System.Net.WebClient().DownloadString("https://ipinfo.io/ip").Trim();
        }

        public string GetClientIP()
        {
            // Get the client IP from the call context.
            object data = CallContext.GetData("ClientIP");

            // If the data is null or not a string, then return an empty string.
            if (data == null || !(data is IPAddress))
                return string.Empty;

            // Return the data as a string.
            return ((IPAddress)data).ToString();
        }
    }
}
