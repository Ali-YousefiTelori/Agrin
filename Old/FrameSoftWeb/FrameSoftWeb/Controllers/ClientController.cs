using Agrin.Framesoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FrameSoftWeb.Helper;
using Agrin.Framesoft.Helper;

namespace FrameSoftWeb.Controllers
{
    public class ClientController : Controller
    {
        public string IPRequestHelper(string url)
        {
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

            using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
            {
                string responseRead = responseStream.ReadToEnd();
                responseStream.Close();
                return responseRead;
            }
        }

        public IpProperties GetCountryByIP(string ipAddress)
        {
            string ipResponse = IPRequestHelper("http://ip-api.com/xml/" + ipAddress);
            using (TextReader sr = new StringReader(ipResponse))
            {
                using (System.Data.DataSet dataBase = new System.Data.DataSet())
                {
                    IpProperties ipProperties = new IpProperties();
                    dataBase.ReadXml(sr);
                    ipProperties.Status = dataBase.Tables[0].Rows[0][0].ToString();
                    ipProperties.Country = dataBase.Tables[0].Rows[0][1].ToString();
                    ipProperties.CountryCode = dataBase.Tables[0].Rows[0][2].ToString();
                    ipProperties.Region = dataBase.Tables[0].Rows[0][3].ToString();
                    ipProperties.RegionName = dataBase.Tables[0].Rows[0][4].ToString();
                    ipProperties.City = dataBase.Tables[0].Rows[0][5].ToString();
                    ipProperties.Zip = dataBase.Tables[0].Rows[0][6].ToString();
                    ipProperties.Lat = dataBase.Tables[0].Rows[0][7].ToString();
                    ipProperties.Lon = dataBase.Tables[0].Rows[0][8].ToString();
                    ipProperties.TimeZone = dataBase.Tables[0].Rows[0][9].ToString();
                    ipProperties.ISP = dataBase.Tables[0].Rows[0][10].ToString();
                    ipProperties.ORG = dataBase.Tables[0].Rows[0][11].ToString();
                    ipProperties.AS = dataBase.Tables[0].Rows[0][12].ToString();
                    ipProperties.Query = dataBase.Tables[0].Rows[0][13].ToString();

                    return ipProperties;
                }
            }
        }
        //
        // GET: /Client/

        public ActionResult GetIpProperties(string ipOrDomain)
        {
            try
            {
                if (string.IsNullOrEmpty(ipOrDomain))
                    ipOrDomain = ControllerHelper.GetIPAddress();
                var prop = GetCountryByIP(ipOrDomain);
                var dir = Server.MapPath("~/Resources/Images/Flags/");
                var path = Path.Combine(dir, prop.CountryCode + ".gif");
                if (System.IO.File.Exists(path))
                    prop.Flag = System.IO.File.ReadAllBytes(path);
                return Content(this.SetResultTextData(DataSerializationHelper.EncryptObject(prop)));
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue(e.Message));
            }
        }

        public ActionResult GetFlagByCountryCode(string code)
        {
            try
            {
                var dir = Server.MapPath("~/Resources/Images/Flags/");
                var path = Path.Combine(dir, code + ".gif");
                return new DownloadResult(this, path);
            }
            catch (Exception e)
            {
                return Content(this.SetResultTextValue("Error"));
            }
        }
    }
}
