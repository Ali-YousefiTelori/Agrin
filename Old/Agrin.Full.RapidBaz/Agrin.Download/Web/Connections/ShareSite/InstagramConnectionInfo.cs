using Agrin.Download.Web.Link;
using Agrin.LinkExtractor.Instagram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web.Connections.ShareSite
{
    public class InstagramConnectionInfo: AConnectionInfo
    {
        public InstagramConnectionInfo(string address, LinkWebRequest parentLinkWebRequest)
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
                    _saveStream = new FileStream(ParentLinkWebRequest.SaveFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
                if (InstagramFindDownloadLink.IsInstagramLink(address.Address))
                {
                    if (ParentLinkWebRequest.Parent.DownloadingProperty.UriCache == null)
                    {
                        SetState(ConnectionState.FindingAddressFromSharing);
                        string normalURL = address.Address;
                        var instaLink = Agrin.LinkExtractor.Instagram.InstagramFindDownloadLink.FindLinkFromSite(normalURL, proxy);

                        if (isDispose)
                            return false;
                        if (!string.IsNullOrEmpty(instaLink.Title))
                        {
                            ParentLinkWebRequest.Parent.PathInfo.UserFileName = Agrin.IO.Helper.MPath.GetFileNameValidChar(instaLink.Title + Path.GetExtension(instaLink.Link));
                        }
                        else
                        {
                            ParentLinkWebRequest.Parent.PathInfo.UserFileName = Agrin.IO.Helper.MPath.GetFileNameValidChar(Path.GetFileName(instaLink.Link));
                        }
                        ParentLinkWebRequest.Parent.DownloadingProperty.UriCache = uri = instaLink.Link;

                    }
                    else
                        uri = ParentLinkWebRequest.Parent.DownloadingProperty.UriCache;
                }

                SetState(ConnectionState.CreatingRequest);

                _request = (HttpWebRequest)WebRequest.Create(uri);
                _request.Timeout = 60000;
                _request.AllowAutoRedirect = true;
                _request.Proxy = proxy;
                _request.ServicePoint.ConnectionLimit = int.MaxValue;
                _request.CookieContainer = ParentLinkWebRequest.RequestCookieContainer;
                var authentication = ParentLinkWebRequest.Parent.GetUserAuthentication(address.Address);
                if (authentication != null)
                    _request.Headers.Add(authentication[0], authentication[1]);

                if (_request.Credentials != null)
                    _request.UseDefaultCredentials = false;

                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
                var parent = ParentLinkWebRequest.Parent;
                if (ParentLinkWebRequest.Parent.Management.Errors.Count > 0)
                {
                    foreach (var error in ParentLinkWebRequest.Parent.Management.Errors)
                    {
                        if (error == null)
                            continue;
                        if (error.ExceptionData is WebException)
                        {
                            if (error.OtherData.FirstOrDefault() is WebExceptionStatus && (WebExceptionStatus)error.OtherData.FirstOrDefault() == WebExceptionStatus.ConnectFailure)
                            {
                                _request.ServicePoint.Expect100Continue = false;
                                break;
                            }
                        }
                    }
                }
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
