using Agrin.Download.Web.Link;
using Agrin.IO;
using Agrin.IO.Helper;
using Agrin.IO.Strings;
using Agrin.LinkExtractor.RapidBaz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Agrin.Download.Web.Connections.ShareSite
{
    public class RapidBazConnectionInfo : AConnectionInfo
    {
        public RapidBazConnectionInfo(string address, LinkWebRequest parentLinkWebRequest)
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
                string fn = Path.GetFileName(uri);
                uri = uri.Replace(fn, "test");

                var proxy = ParentLinkWebRequest.Parent.GetNewProxy(address);

                SetState(ConnectionState.CreatingRequest);

                _request = (HttpWebRequest)WebRequest.Create(uri);
                _request.Timeout = 60000;
                _request.AllowAutoRedirect = true;
                _request.Proxy = proxy;
                _request.ServicePoint.ConnectionLimit = int.MaxValue;
                _request.CookieContainer = ParentLinkWebRequest.RequestCookieContainer;
                Func<string, string, string[]> GetCredentialCache = (user, pass) =>
                {
                    String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass));
                    return new string[] { "Authorization", "Basic " + encoded };
                };
#if(!MobileApp)
                var authentication = GetCredentialCache(RapidBaz.Users.UserManager.CurrentUser.UserName, RapidBaz.Users.UserManager.CurrentUser.Password);
                if (authentication != null)
                    _request.Headers.Add(authentication[0], authentication[1]);
#endif
                _request.KeepAlive = true;
                _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

                if (ParentLinkWebRequest.StartPosition + _saveStream.Length > 0 && ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes)
                {
                    long rng = (long)ParentLinkWebRequest.StartPosition + _saveStream.Length;

                    Agrin.Download.Helper.LinkHelper.AddRange(rng, _request);

                    lock (LinkWebRequest.lockStatic)
                    {
                        if (!ParentLinkWebRequest.Parent.DownloadingProperty.DownloadRangePositions.Contains(rng))
                            ParentLinkWebRequest.Parent.DownloadingProperty.DownloadRangePositions.Add(rng);
                    }

                    ParentLinkWebRequest.Parent.SaveThisLink();
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

        public void RepairLink()
        {
#if(!MobileApp)

            var repair = Agrin.Download.Engine.RapidBazEngineHelper.Retry(ParentLinkWebRequest.Parent.PathInfo.Address);
#endif
            }

        public override void Connect()
        {
            SetState(ConnectionState.Connecting);
            //_responseStream = new FileStream("d:\\DMail Sender.rar", FileMode.Open,FileAccess.ReadWrite);
            //_responseStream.Seek((long)StartPosition, SeekOrigin.Begin);
            //if (EndPosition == -2)
            //{
            //    EndPosition = _responseStream.Length;
            //    if (Parent.Connections.Count == 1)
            //        Parent.DownloadingProperty.Size = EndPosition - StartPosition;
            //}
            //Parent.PathInfo.UserFileName = "DMS.rar";
            //Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
            //State = ConnectionState.Downloading;
            //if (Connected != null)
            //    Connected();
            //DownloadData();
            //return;
            if (isDispose)
                return;
            try
            {
                //Thread.Sleep(3000);
                if (isDispose)
                    return;
                Func<string, bool> getError = (uri) =>
                {
                    string error = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(uri, "error:", ":", true);
                    if (string.IsNullOrEmpty(error))
                    {
                        error = Agrin.IO.Strings.Text.GetTextBetweenTwoValue(uri, "error:", "", true);
                    }
                    string code = error;
                    if (code == "410")
                        return true;
                    error = RapidBazFindDownloadLink.GetErrorTextByErrorNumber(error);
                    if (code == "4042")
                    {
                        RepairLink();
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        throw new Exception(@"در حال کار بر روی سرور هستیم
پیام خطای سرور زمانی رخ می دهد که درخواست شما برای سرور خوانا نباشد
لطفا لینک خود را چک کنید و دوباره لینک خود را از منبع خود دریافت کنید.
اگر مطمئن هستید که لینک شما مشکلی ندارد لطفا لینک خود را ذخیره کرده و بعدا امتحان کنید.
ممکن است در حال حاضر به خاطر فشار بالا سرور قادر به پاسخگویی به درخواست شما نباشد." + System.Environment.NewLine + uri);
                    }
                    else
                        throw new Exception(error);
                };
                HttpWebRequest request = (HttpWebRequest)_request;
                try
                {
                    _response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    if (isDispose)
                        return;
                    var e410 = getError(ex.Response.ResponseUri.OriginalString);
                    if (e410)
                    {
                        ParentLinkWebRequest.ReConnect();
                        return;
                    }
                }
                if (_response.ContentLength <= 0)
                {
                    var e410 = getError(_response.ResponseUri.OriginalString);
                    if (e410)
                    {
                        ParentLinkWebRequest.ReConnect();
                        return;
                    }
                }
                ParentLinkWebRequest.ReConnectTimer = Stopwatch.StartNew();
                if (ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes)
                {
                    if (_response.Headers.AllKeys.Select(c => c.ToLower()).ToArray().Contains("content-range"))
                    {
                        if (!_response.Headers["Content-Range"].Contains((ParentLinkWebRequest.StartPosition + _saveStream.Length).ToString()))
                        {
                            SetState(ConnectionState.CheckForSupportResumable);
                            var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address, authentication);
                            if (checkValue == LinkaddressCheckMode.True)
                                ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
                            else if (checkValue != LinkaddressCheckMode.Exception)
                                NotResumableSupport();
                            else
                            {
                                throw new Exception("Error in Checking Resumable Support");
                            }
                        }
                    }
                    else if (_response.Headers.AllKeys.Select(c => c.ToLower()).ToArray().Contains("accept-ranges"))
                    {
                        if (!_response.Headers["accept-ranges"].Contains((ParentLinkWebRequest.StartPosition + _saveStream.Length).ToString()))
                        {
                            SetState(ConnectionState.CheckForSupportResumable);
                            var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address, authentication);
                            if (checkValue == LinkaddressCheckMode.True)
                                ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
                            else if (checkValue != LinkaddressCheckMode.Exception)
                                NotResumableSupport();
                            else
                            {
                                throw new Exception("Error in Checking Resumable Support");
                            }
                        }
                    }
                    else
                    {
                        SetState(ConnectionState.CheckForSupportResumable);
                        var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address, authentication);
                        if (checkValue == LinkaddressCheckMode.True)
                            ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
                        else if (checkValue != LinkaddressCheckMode.Exception)
                            NotResumableSupport();
                        else
                        {
                            throw new Exception("Error in Checking Resumable Support");
                        }
                    }
                }
                if (ParentLinkWebRequest.ConnectionId == 2 && _response.ContentLength == ParentLinkWebRequest.Parent.DownloadingProperty.Size)
                {
                    SetState(ConnectionState.CheckForSupportResumable);
                    var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address, authentication);
                    if (checkValue == LinkaddressCheckMode.True)
                        ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
                    else if (checkValue != LinkaddressCheckMode.Exception)
                        NotResumableSupport();
                    else
                    {
                        throw new Exception("Error in Checking Resumable Support");
                    }
                }
                if (ParentLinkWebRequest.EndPosition == -2)
                {
                    ParentLinkWebRequest.EndPosition = _response.ContentLength;
                    if (ParentLinkWebRequest.Parent.Connections.Count == 1)
                        ParentLinkWebRequest.Parent.DownloadingProperty.Size = ParentLinkWebRequest.EndPosition - ParentLinkWebRequest.StartPosition;
                }

                if (ParentLinkWebRequest.ConnectionId == 1)
                {
                    GetHeaders();
                    if (ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Unknown)
                    {
                        SetState(ConnectionState.CheckForSupportResumable);
                        var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address, authentication);
                        if (checkValue == LinkaddressCheckMode.True)
                            ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
                        else if (checkValue != LinkaddressCheckMode.Exception)
                            NotResumableSupport();
                        else
                            throw new Exception("Error in Checking Resumable Support");
                    }
                }
                else
                {

                }
                string newUri = _response.ResponseUri.ToString();
                bool canAddNewUri = true;
                foreach (var item in ParentLinkWebRequest.Parent.Management.MultiLinks)
                {
                    if (item.Address == newUri)
                    {
                        canAddNewUri = false;
                        break;
                    }
                }
                if (canAddNewUri)
                {
                    ParentLinkWebRequest.Parent.Management.AddMultiLink(new MultiLinkAddress() { Address = newUri, IsSelected = true, IsApplicationAdded = true });
                }

                _responseStream = _response.GetResponseStream();
                if (ParentLinkWebRequest.EndPosition < 0 && ParentLinkWebRequest.Parent.DownloadingProperty.DownloadAlgoritm == AlgoritmEnum.Unknown)
                {
                    ParentLinkWebRequest.ChangeAlgoritm(AlgoritmEnum.Page);
                }
                else if (!ParentLinkWebRequest.IsPauseCheck())
                {
                    SetState(ConnectionState.Downloading);

                    if (ParentLinkWebRequest.Connected != null)
                        ParentLinkWebRequest.Connected();
                    DownloadData();
                }
            }
            catch (Exception e)
            {
                if (isDispose)
                    return;
                if (ParentLinkWebRequest.Parent.DownloadingProperty.UriCache != null)
                    ParentLinkWebRequest.Parent.DownloadingProperty.UriCache = null;

                StopException(e);
            }
        }

        internal override void GetHeaders()
        {
            try
            {
                //if (_threadInfo == null)
                //    return;
                bool mustSave = false;
                string lower = "";
                string contentType = "";
                //LinkInfoBindingToAdd bindingFileName = ((LinkInfoBindingToAdd)_threadInfo.LinkInfo.BindingProperty);
                foreach (string item in _response.Headers)
                {
                    lower = item.ToLower();
                    string estrin = _response.Headers[lower];
                    switch (lower)
                    {
                        case "content-disposition":
                            {
                                if (ParentLinkWebRequest.ConnectionId > 1 || _response.Headers.Keys.Cast<string>().Contains("Content-Range"))
                                    break;
                                string fileName = Decodings.FullDecodeString(MPath.GetFileName(estrin)).Trim(new char[] { '"' });
                                if (fileName == "test")
                                    break;
                                if (!String.IsNullOrEmpty(fileName))
                                {
                                    mustSave = ParentLinkWebRequest.Parent.PathInfo.AddressFileName != fileName;
                                    ParentLinkWebRequest.Parent.PathInfo.AddressFileName = fileName;
                                }
                                break;
                            }
                        case "content-type":
                            {
                                contentType = estrin;
                                break;
                            }
                        case "accept-ranges":
                            {
                                if (ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability != ResumeCapabilityEnum.No)
                                {
                                    var value = _response.Headers[lower];
                                    if (value != null && value.ToLower().Contains("bytes"))
                                    {
                                        if (value.Contains(ParentLinkWebRequest.StartPosition.ToString()) || ParentLinkWebRequest.StartPosition == 0)
                                            ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
                                    }
                                    else
                                        ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.No;
                                }

                                break;
                            }
                        case "content-range":
                            {
                                if (ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability != ResumeCapabilityEnum.No)
                                {
                                    var value = _response.Headers[lower];
                                    if (value != null && value.ToLower().Contains(ParentLinkWebRequest.StartPosition.ToString()))
                                        ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
                                }
                                break;
                            }
                    }
                }

                if (String.IsNullOrEmpty(ParentLinkWebRequest.Parent.PathInfo.AddressFileName))
                {
                    string getfileName = FileStatic.GetLinksFileName(_response.ResponseUri.OriginalString);
                    ParentLinkWebRequest.Parent.PathInfo.AddressFileName = getfileName.Trim(new char[] { '"' });
                }
                if (!String.IsNullOrEmpty(contentType))
                {
                    string ext = MPath.GetDefaultExtension(contentType);
                    if (!ParentLinkWebRequest.Parent.PathInfo.AddressFileName.ToLower().Contains(ext.ToLower()))
                    {
                        var extF = Path.GetExtension(ParentLinkWebRequest.Parent.PathInfo.AddressFileName);
                        if (extF != null)
                            extF = extF.ToLower();
                        if (extF == ".htm" || extF == ".html")
                        {
                            var fn = Path.GetFileNameWithoutExtension(ParentLinkWebRequest.Parent.PathInfo.AddressFileName);
                            ParentLinkWebRequest.Parent.PathInfo.AddressFileName = fn + ext;
                        }
                    }

                }

                if (mustSave)
                    ParentLinkWebRequest.Parent.SaveThisLink();
            }
            catch (Exception e)
            {
                if (isDispose)
                    return;
                try
                {
                    Agrin.Log.AutoLogger.LogError(e, "GetHeaders");
                    if (ParentLinkWebRequest.State == ConnectionState.Connecting)
                    {
                        ParentLinkWebRequest.Parent.Management.AddError(e);
                        ParentLinkWebRequest.StopComplete(StopEnum.Error);
                    }
                }
                catch (Exception c)
                {
                    if (isDispose)
                        return;
                    Agrin.Log.AutoLogger.LogError(c, "GetHeaders 2");
                }
                //System.Diagnostics.Debugger.Break();
            }
        }
    }
}
