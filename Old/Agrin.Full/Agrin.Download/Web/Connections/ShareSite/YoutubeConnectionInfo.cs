using Agrin.Download.Web.Link;
using Agrin.IO.Helper;
using Agrin.LinkExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using YoutubeExtractor;

namespace Agrin.Download.Web.Connections.ShareSite
{
    public class YoutubeConnectionInfo : AConnectionInfo
    {
        public YoutubeConnectionInfo(string address, LinkWebRequest parentLinkWebRequest)
        {
            ParentLinkWebRequest = parentLinkWebRequest;
            ParentLinkWebRequest.UriDownload = new Uri(address);
        }

        ManualResetEvent _sharingUIWaiting = new ManualResetEvent(false);
        public override bool CreateRequestData()
        {
            try
            {
                lock (writeLock)
                {
                    if (isDispose)
                        return false;
                    _saveStream = IOHelper.OpenFileStreamForWrite(ParentLinkWebRequest.SaveFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

                if (DownloadUrlResolver.IsYoutubeLink(address.Address))
                {
                    if (ParentLinkWebRequest.Parent.DownloadingProperty.UriCache == null)
                    {
                        SetState(ConnectionState.FindingAddressFromSharing);
                        string normalURL = address.Address;
                        List<VideoInfo> videos = null;
                        if (DownloadUrlResolver.TryNormalizeYoutubeUrl(address.Address, out normalURL))
                            videos = DownloadUrlResolver.GetDownloadUrls(normalURL).ToList();
                        else
                            videos = DownloadUrlResolver.GetDownloadUrls(address.Address).ToList();
                        if (ParentLinkWebRequest.Parent.Management.SharingSettings == null)
                            ParentLinkWebRequest.Parent.Management.SharingSettings = new List<object>();
                        VideoInfo video = null;
                        if (ParentLinkWebRequest.Parent.Management.SharingSettings.Count == 1)
                        {
                            video = DownloadUrlResolver.GetVideoInfoByFormatCode(videos, int.Parse(ParentLinkWebRequest.Parent.Management.SharingSettings.First().ToString()));
                            ParentLinkWebRequest.Parent.PathInfo.AddressFileName = Agrin.IO.Helper.MPath.GetFileNameValidChar(video.Title + video.VideoExtension);
                            DownloadUrlResolver.DecryptDownloadUrl(video);
                            uri = video.DownloadUrl;
                            ParentLinkWebRequest.Parent.DownloadingProperty.UriCache = uri;
                        }
                        else
                        {
                            ParentLinkWebRequest.Parent.Management.UserSelectedItemAction = (item) =>
                            {
                                VideoInfo info = item as VideoInfo;
                                DownloadUrlResolver.DecryptDownloadUrl(video);
                                uri = info.DownloadUrl;
                                _currentAddress = uri;
                                ParentLinkWebRequest.Parent.DownloadingProperty.UriCache = uri;
                                ParentLinkWebRequest.Parent.Management.SharingSettings.Add(info.FormatCode);
                                ParentLinkWebRequest.Parent.SaveThisLink();
                                _sharingUIWaiting.Set();
                            };
                            if (ParentLinkWebRequest.Parent.Management.UserMustSelectItemAction == null)
                            {
                                StopException(new Exception("user must select one Link (AVS)!"));
                                return false;
                            }
                            ParentLinkWebRequest.Parent.Management.UserMustSelectItemAction(videos.Cast<object>().ToList());

                            _sharingUIWaiting.WaitOne();
                            _sharingUIWaiting.Reset();
                        }
                    }
                    else
                        uri = ParentLinkWebRequest.Parent.DownloadingProperty.UriCache;
                }
                _currentAddress = uri;

                SetState(ConnectionState.CreatingRequest);

                _request = (HttpWebRequest)WebRequest.Create(uri);
                _request.Timeout = 60000;
                GetHttpWebRequest.AllowAutoRedirect = true;
                _request.Proxy = ParentLinkWebRequest.Parent.GetNewProxy(address);
                GetHttpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
                GetHttpWebRequest.CookieContainer = ParentLinkWebRequest.RequestCookieContainer;
                var authentication = ParentLinkWebRequest.Parent.GetUserAuthentication(address.Address);
                if (authentication != null)
                    _request.Headers.Add(authentication[0], authentication[1]);

                if (_request.Credentials != null)
                    _request.UseDefaultCredentials = false;

                GetHttpWebRequest.InitializeCustomHeaders(ParentLinkWebRequest.Parent.DownloadingProperty.CustomHeaders);

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
                                GetHttpWebRequest.ServicePoint.Expect100Continue = false;
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
