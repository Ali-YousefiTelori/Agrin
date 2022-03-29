using Agrin.Download.Web.Link;
using Agrin.IO.Helper;
using Agrin.LinkExtractor.ZeeTVCom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web.Connections.ShareSite
{
    public class ZeeTVComConnectionInfo : AConnectionInfo
    {
        public ZeeTVComConnectionInfo(string address, LinkWebRequest parentLinkWebRequest)
        {
            ParentLinkWebRequest = parentLinkWebRequest;
            ParentLinkWebRequest.UriDownload = new Uri(address);
        }

        public override bool CreateRequestData()
        {
            try
            {
                lock (writeLock)
                {
                    if (isDispose)
                        return false;
                    _saveStream = IOHelperBase.Current.OpenFileStreamForWrite(ParentLinkWebRequest.SaveFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }
                if (isDispose)
                    return false;
                ParentLinkWebRequest.DownloadedSize = _saveStream.Length;
                if (ParentLinkWebRequest.EndPosition != -2 && _saveStream.Length == ParentLinkWebRequest.EndPosition - ParentLinkWebRequest.StartPosition)
                {
                    ParentLinkWebRequest.Complete();
                    return false;
                }
                else if (_saveStream.Length > ParentLinkWebRequest.Length)
                {
                    _saveStream.SetLength((long)ParentLinkWebRequest.Length);
                    ParentLinkWebRequest.DownloadedSize = _saveStream.Length;
                    ParentLinkWebRequest.Complete();
                    return false;
                }
                _saveStream.Seek(0, SeekOrigin.End);
                var address = ParentLinkWebRequest.Parent.GetNewAddress();
                string uri = address.Address;
                var proxy = ParentLinkWebRequest.Parent.GetNewProxy(address);
                if (ZeeTVComFindDownloadLink.IsZeeTVLink(address.Address))
                {
                    if (ParentLinkWebRequest.Parent.DownloadingProperty.UriCache == null)
                    {
                        SetState(ConnectionState.FindingAddressFromSharing);
                        string normalURL = address.Address;
                        var link = ZeeTVComFindDownloadLink.FindLinkFromSite(normalURL, proxy);

                        if (isDispose)
                            return false;
                        ParentLinkWebRequest.Parent.DownloadingProperty.UriCache = uri = link;

                    }
                    else
                        uri = ParentLinkWebRequest.Parent.DownloadingProperty.UriCache;
                }

                SetState(ConnectionState.CreatingRequest);

                _request = (HttpWebRequest)WebRequest.Create(uri);
                _request.Timeout = 60000;
                GetHttpWebRequest.AllowAutoRedirect = false;
                _request.Proxy = proxy;
                GetHttpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
                GetHttpWebRequest.CookieContainer = ParentLinkWebRequest.RequestCookieContainer;
                var authentication = ParentLinkWebRequest.Parent.GetUserAuthentication(address.Address);
                if (authentication != null)
                    _request.Headers.Add(authentication[0], authentication[1]);

                if (_request.Credentials != null)
                    _request.UseDefaultCredentials = false;

                GetHttpWebRequest.InitializeCustomHeaders(ParentLinkWebRequest.Parent.DownloadingProperty.CustomHeaders);

                if (ParentLinkWebRequest.StartPosition + _saveStream.Length > 0 && ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes)
                {
                    long rng = (long)ParentLinkWebRequest.StartPosition + _saveStream.Length;
                    //#if(MobileApp || XamarinApp)
                    //                Agrin.Download.Helper.LinkHelper.AddRange(rng, _request);

                    //#else
                    Agrin.Download.Helper.LinkHelper.AddRange(rng, _request);
                    //long rng = (long)(ParentLinkWebRequest.StartPosition + _saveStream.Length);
                    //_request.AddRange(rng);
                    //#endif
                    //Parent.DownloadingProperty.DownloadRangePositions.Capacity = int.MaxValue;
                    lock (LinkWebRequest.lockStatic)
                    {
                        if (!ParentLinkWebRequest.Parent.DownloadingProperty.DownloadRangePositions.Contains(rng))
                            ParentLinkWebRequest.Parent.DownloadingProperty.DownloadRangePositions.Add(rng);
                    }

                    ParentLinkWebRequest.Parent.SaveThisLink();
                    //AddThisRange = rng;
                }
                else
                {
                    if (ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability != ResumeCapabilityEnum.Yes && ParentLinkWebRequest.Parent.DownloadingProperty.DownloadedSize != 0)
                    {
                        NotResumableSupport();
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is AggregateException)
                {
                    AggregateException c = e as AggregateException;
                    if (c.InnerExceptions != null && c.InnerExceptions.Count > 0)
                    {
                        if (c.InnerExceptions[0].InnerException != null)
                        {
                            e = c.InnerExceptions[0].InnerException;
                        }
                        else
                            e = c.InnerExceptions[0];
                    }
                }
                StopException(e);
                return false;
            }
            return true;
        }
    }
}
