using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Agrin.Framesoft;

namespace UnitTestAgrin.IO
{
    [TestClass]
    public class FileTest
    {
        [TestMethod]
        public void FindFileNameTest()
        {
            try
            {
                string text = GetLinksFileName("ali.rar");
                text = GetLinksFileName("c:\\dsdfdf\\ali.rar");
                text = GetLinksFileName("aa\\ali.rar");
                text = GetLinksFileName("aa//ali.rar");
                text = GetLinksFileName("/ali.rar");
                text = GetLinksFileName("http://tinyez.tv/dl/dl/x/X-Men.5.First.Class.Dubbed.Audio.TinyMoviez_us.mp3?hash=21b6c0af06e4c059360ba6c3201ee536_187730_3853&s=");
                text = GetLinksFileName("http://tinyez.tv/dl/dl/x/X-Men.5.First.Class.Dubbed.Audio.TinyMoviez_us.mp3");
                text = GetLinksFileName("http://cdn.p30download.com/?b=p30dl-software&f=");
                text = GetLinksFileName("");
            }
            catch
            {

            }
        }

        [TestMethod]
        public void IpToCountrySerialize()
        {
            string folder = @"D:\BaseProjects\Agrin Download Manager\Agrin.Full.RapidBaz\Agrin.Windows.UI\bin\Debug\Resources\IpToCountry";
            string file = System.IO.Path.Combine(folder, "IpToCountry.txt");
            var lines = System.IO.File.ReadAllLines(file);
            List<IpProperties> ips = new List<IpProperties>();

            foreach (var item in lines)
            {
                string[] values = item.Split(',');
                //ips.Add(new IpProperties() { CountryCode = values[4].Trim('"'), From = long.Parse(values[0].Trim('"')), To = long.Parse(values[1].Trim('"')) });
            }
        }

        public static string GetLinksFileName(string newFileName)
        {
            string decode = Agrin.IO.Strings.Decodings.FullDecodeString(newFileName.Trim().Trim(new char[] { '/', '\\' }));
            Uri uri = null;
            string fileName = null;
            if (Uri.TryCreate(decode, UriKind.Absolute, out uri))
            {
                fileName = Agrin.IO.Strings.Decodings.FullDecodeString(System.IO.Path.GetFileName(uri.AbsolutePath));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Agrin.IO.Strings.Decodings.FullDecodeString(System.IO.Path.GetFileName(decode));
                    if (fileName.Contains("="))
                    {
                        int l = fileName.LastIndexOf('=');
                        if (l < fileName.Length - 1)
                        {
                            string name = fileName.Substring(l + 1, fileName.Length - l - 1);
                            if (!string.IsNullOrEmpty(name))
                                fileName = name;
                        }

                    }
                }
            }
            else
                fileName = System.IO.Path.GetFileName(decode);

            foreach (var item in System.IO.Path.GetInvalidPathChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            if (string.IsNullOrEmpty(fileName))
                return "notName.html";
            else if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
                return fileName + ".html";
            return fileName;
        }
    }
}
