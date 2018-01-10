using Agrin.Download.Web.Link;
using Agrin.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data.Settings
{
    [Serializable]
    public class AppUserAccountsSetting
    {
        public List<NetworkCredentialInfo> Items { get; set; }
        public static NetworkCredentialInfo FindFromAddress(string uriAddress)
        {
            if (uriAddress == null || ApplicationSetting.Current == null || ApplicationSetting.Current.UserAccountsSetting == null || ApplicationSetting.Current.UserAccountsSetting.Items == null)
                return null;
            uriAddress = uriAddress.ToLower();
            foreach (var netItem in ApplicationSetting.Current.UserAccountsSetting.Items.ToArray())
            {
                try
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
                catch (Exception e)
                {
                    AutoLogger.LogError(e, "App User Setting Error", true);
                }


            }
            return null;
        }
    }
}
