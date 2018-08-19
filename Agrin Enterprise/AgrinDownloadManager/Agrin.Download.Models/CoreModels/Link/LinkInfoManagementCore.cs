using Agrin.ComponentModels;
using Agrin.Download.ShortModels.Link;
using Agrin.Helper.Net;
using Agrin.IO;
using Agrin.IO.Helpers;
using Agrin.Log;
using Agrin.Models.Link;
using Agrin.Threads;
using SignalGo.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.Download.CoreModels.Link
{
    /// <summary>
    /// manage link data
    /// </summary>
    public class LinkInfoManagementCore : NotifyPropertyChanged
    {
        volatile AgrinCustomWebException _LastException;
        ConcurrentList<LinkAddressInfo> _MultiLinkAddresses;
        volatile bool _IsLimit = false;
        volatile LinkInfoShort _LinkInfo;

        /// <summary>
        /// parent of link info
        /// </summary>
        public LinkInfoShort LinkInfo { get => _LinkInfo; set => _LinkInfo = value; }

        /// <summary>
        /// last exception of link info
        /// </summary>
        public AgrinCustomWebException LastException
        {
            get
            {
                return _LastException;
            }
            private set
            {
                _LastException = value;
                OnPropertyChanged("LastException");
            }
        }

        /// <summary>
        /// multi link from application system
        /// </summary>
        public ConcurrentList<LinkAddressInfo> MultiLinkAddresses
        {
            get
            {
                if (_MultiLinkAddresses == null)
                {
                    _MultiLinkAddresses = new ConcurrentList<LinkAddressInfo>
                    {
                        new LinkAddressInfo() { Address = LinkInfo.PathInfo.MainUriAddress, IsApplicationAdded = false, IsEnabled = true}
                    };
                }
                return _MultiLinkAddresses;
            }
        }

        /// <summary>
        /// when download is limited
        /// </summary>
        public bool IsLimit { get => _IsLimit; set => _IsLimit = value; }

        /// <summary>
        /// add new LinkAddressInfo to multilink
        /// </summary>
        /// <param name="address"></param>
        /// <param name="save"></param>
        public void AddNewLinkAddressInfo(LinkAddressInfo address, bool save = true)
        {
            MultiLinkAddresses.Add(address);
            if (save)
                LinkInfo.Save();
        }

        public static Action<int, Exception> LinkInfoAddErrorAction { get; set; }
        /// <summary>
        /// add error in application Error List
        /// </summary>
        /// <param name="error"></param>
        public void AddError(Exception error)
        {
            LinkInfoAddErrorAction?.Invoke(LinkInfo.Id, error);
            //try
            //{
            //    //Logger.WriteLine("AddError", error);
            //    //List<AgrinCustomWebException> items = GetErrors(out string errorsFileName);
            //    //this.RunInLock(() =>
            //    //{
            //    //    var last = items.LastOrDefault();
            //    //    if (last != null && last.ExceptionData != null && last.ExceptionData.Message == error.Message)
            //    //        return;
            //    //    LastException = AgrinCustomWebException.ExceptionToSerializable(error);
            //    //    var find = (from x in items where x.ExceptionData.ToString() == LastException.ExceptionData.ToString() select x).FirstOrDefault();
            //    //    if (find != null)
            //    //    {
            //    //        find.Count++;
            //    //        find.CreatedDateTime = DateTime.Now;
            //    //    }
            //    //    else
            //    //        items.Add(LastException);
            //    //    if (items.Count > 20)
            //    //    {
            //    //        items.RemoveAt(0);
            //    //    }

            //    //    try
            //    //    {
            //    //        using (var writer = IOHelperBase.OpenFileStreamForWrite(errorsFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite).GetStream())
            //    //        {
            //    //            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(items);
            //    //            var bytes = Encoding.UTF8.GetBytes(serialized);
            //    //            writer.Write(bytes, 0, bytes.Length);
            //    //        }
            //    //    }
            //    //    catch (Exception ex)
            //    //    {
            //    //        AutoLogger.LogError(ex, "AddError 2");
            //    //    }
            //    //});

            //}
            //catch (Exception e)
            //{
            //    AutoLogger.LogError(e, "AddError 3");
            //}
        }

        /// <summary>
        /// get list of errors
        /// </summary>
        /// <param name="errorsFileName"></param>
        /// <returns></returns>
        //public List<AgrinCustomWebException> GetErrors(out string errorsFileName)
        //{
        //    errorsFileName = "";
        //    List<AgrinCustomWebException> items = new List<AgrinCustomWebException>();
        //    try
        //    {
        //        errorsFileName = this.RunInLock(() =>
        //        {
        //            string fName = "";
        //            if (!string.IsNullOrEmpty(LinkInfo.PathInfo.SecurityTemporarySavePath))
        //                fName = PathHelper.CombineSecurityPathWithNoSecurity(LinkInfo.PathInfo.TemporarySavePath, "Errors.agn");
        //            else
        //                fName = PathHelper.Combine(LinkInfo.PathInfo.TemporarySavePath, "Errors.agn");

        //            try
        //            {
        //                using (var reader = new StreamReader(IOHelperBase.OpenFileStreamForRead(fName, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
        //                {
        //                    var oldItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AgrinCustomWebException>>(reader.ReadToEnd());
        //                    items.AddRange(oldItems);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                AutoLogger.LogError(ex, "GetErrors");
        //            }
        //            return fName;
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        AutoLogger.LogError(e, "GetErrors 2");
        //    }
        //    return items;
        //}

    }
}
