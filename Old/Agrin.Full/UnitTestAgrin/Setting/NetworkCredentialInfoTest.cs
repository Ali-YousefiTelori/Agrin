using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestAgrin.Setting
{
    [TestClass]
    public class NetworkCredentialInfoTest
    {
        public List<NetworkCredentialInfo> Items { get; set; }
        [TestMethod]
        public void TestFindFromAddress()
        {
            Items = new List<NetworkCredentialInfo>() { new NetworkCredentialInfo() { IsUsed = true, ServerAddress = "tinyez.tv" } };
            var finded = FindFromAddress("tinyez.tv/dl/dl/x/X-Men.5.First.Class.Dubbed.Audio.TinyMoviez_us.mp3?hash=21b6c0af06e4c059360ba6c3201ee536_187730_3853&s=");

        }
        NetworkCredentialInfo FindFromAddress(string uriAddress)
        {
            uriAddress = uriAddress.ToLower();
            foreach (var netItem in Items.ToArray())
            {
                if (!netItem.IsUsed)
                    continue;
                string address = netItem.ServerAddress;
                if (!string.IsNullOrEmpty(address))
                {
                    if (address.First() == '*' && address.Last() == '*')
                    {
                        string[] array = address.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                        bool finded = true;
                        foreach (var item in array)
                        {
                            if (!uriAddress.Contains(item))
                            {
                                finded = false;
                                break;
                            }
                        }
                        if (finded)
                        {
                            return netItem;
                        }
                    }
                    else if (address.First() == '*')
                    {
                        string[] array = address.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                        bool finded = true;
                        foreach (var item in array)
                        {
                            if (!uriAddress.Contains(item))
                            {
                                finded = false;
                                break;
                            }
                        }


                        if (finded)
                        {
                            var uriReverceAddress = new string(uriAddress.Reverse().ToArray());
                            var findText = new string(array.Last().Reverse().ToArray());
                            var data = uriReverceAddress.IndexOf(findText);
                            if (data == 0)
                                return netItem;
                        }
                    }
                    else if (address.Last() == '*')
                    {
                        string[] array = address.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                        bool finded = true;
                        foreach (var item in array)
                        {
                            if (!uriAddress.Contains(item))
                            {
                                finded = false;
                                break;
                            }
                        }

                        if (finded)
                        {
                            var data = uriAddress.IndexOf(array.First());
                            if (data == 0)
                                return netItem;
                        }
                    }
                    else if (address.Contains('*'))
                    {
                        string[] array = address.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
                        bool finded = true;
                        foreach (var item in array)
                        {
                            if (!uriAddress.Contains(item))
                            {
                                finded = false;
                                break;
                            }
                        }
                        if (finded)
                        {
                            var data = uriAddress.IndexOf(array.First());
                            if (data == 0)
                            {
                                var uriReverceAddress = new string(uriAddress.Reverse().ToArray());
                                var findText = new string(array.Last().Reverse().ToArray());
                                data = uriReverceAddress.IndexOf(findText);
                                if (data == 0)
                                    return netItem;
                            }
                        }
                    }
                    else
                    {
                        Uri uri = null;
                        if (Uri.TryCreate(uriAddress, UriKind.Absolute, out uri))
                        {
                            if (uri.AbsolutePath == address || uri.AbsoluteUri == uriAddress || uri.Authority == uriAddress || uri.DnsSafeHost == uriAddress || uri.Fragment == uriAddress || uri.Host == uriAddress
                                || uri.LocalPath == uriAddress || uri.OriginalString == uriAddress || uri.Segments.Contains(uriAddress) || uri.Scheme == uriAddress)
                                return netItem;
                        }
                    }
                }

            }
            return null;
        }
    }

    public class NetworkCredentialInfo
    {
        public string ServerAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsUsed { get; set; }
    }
}
