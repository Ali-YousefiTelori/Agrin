using Agrin.Download.Data.Serializition;
using Agrin.Download.Web.Link;
using Agrin.Helper.ComponentModel;
using Agrin.IO;
using Agrin.IO.Helper;
using Agrin.IO.Strings;
using Agrin.Log;
using Agrin.RapidBaz.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Agrin.Download.Web.Connections
{
    public abstract class AConnectionInfo : IDisposable
    {
        public LinkWebRequest ParentLinkWebRequest { get; set; }

        internal HttpWebRequest _request;

        internal HttpWebResponse _response;

        internal Stream _responseStream;

        internal Stream _saveStream;

        public void NotResumableSupport()
        {
            if (isDispose)
                return;
            if (ParentLinkWebRequest.Parent.DownloadingProperty.DownloadedSize != 0)
                ParentLinkWebRequest.Parent.NotSupportResumable();
        }
        internal string[] authentication = null;
        public virtual bool CreateRequestData()
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
        GetNewAddress:
            var address = ParentLinkWebRequest.Parent.GetNewAddress();
            if (address == null && !isDispose)
            {
                goto GetNewAddress;
            }
            else if (isDispose)
                return false;
            _request = (HttpWebRequest)WebRequest.Create(address.Address);
            _request.Timeout = 60000;
            _request.AllowAutoRedirect = true;
            _request.Proxy = ParentLinkWebRequest.Parent.GetNewProxy(address);
            //_request.ServicePoint.ConnectionLimit = ParentLinkWebRequest.Parent.DownloadingProperty.ConnectionCount + 1;
            _request.ServicePoint.ConnectionLimit = int.MaxValue;
            _request.CookieContainer = ParentLinkWebRequest.RequestCookieContainer;
            authentication = ParentLinkWebRequest.Parent.GetUserAuthentication(address.Address);
            if (authentication != null)
                _request.Headers.Add(authentication[0], authentication[1]);

            //_request.CookieContainer = new CookieContainer();
            //if (_request.RequestUri.Host.ToLower().Contains("rapidbaz.com"))
            //    _request.Headers.Add("Cookie: session=" + UserManager.CurrentSession);

            //_request.Credentials = new NetworkCredential(ParentLinkWebRequest.Parent.Management.NetworkUserPass.UserName, ParentLinkWebRequest.Parent.Management.NetworkUserPass.Password);

            //if (_request.Credentials != null)
            //    _request.UseDefaultCredentials = false;

            _request.KeepAlive = true;
            //_request.UserAgent = "DriverEasy/Pro V4.7.3.6546";
            //_request.Referer = "http://dow1.drivereasy.com/down17/ra2oou5r.3fg";
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            var parent = ParentLinkWebRequest.Parent;
            if (ParentLinkWebRequest.Parent.Management.Errors.Count > 0)
            {
                foreach (var error in ParentLinkWebRequest.Parent.Management.Errors.ToArray())
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
                //Agrin.Download.Helper.LinkHelper.AddRange(0, _request);
            }
            return true;
        }

        bool GetEncryptFileName()
        {
            try
            {
                var fn = _response.Headers["HideFN"];
                if (!string.IsNullOrEmpty(fn))
                {
                    ParentLinkWebRequest.Parent.PathInfo.AddressFileName = MPath.GetFileNameValidChar(Framesoft.Helper.DataSerializationHelper.DecryptString(fn));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        internal virtual void GetHeaders()
        {
            try
            {
                //if (_threadInfo == null)
                //    return;
                bool noneedGetAddressFileName = false;
                if (ParentLinkWebRequest.Parent.PathInfo.Address.ToLower().Contains(Framesoft.Helper.UserManagerHelper.domain.ToLower()))
                {
                    noneedGetAddressFileName = GetEncryptFileName();
                }
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
                                if (noneedGetAddressFileName)
                                    break;
                                string fileName = Decodings.FullDecodeString(MPath.GetFileName(estrin)).Trim(new char[] { '"' });
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

                if (!noneedGetAddressFileName)
                {
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

        internal void SetState(ConnectionState state)
        {
            lock (writeLock)
            {
                if (!isDispose)
                    ParentLinkWebRequest.State = state;
            }
        }

        public virtual void Connect()
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
            ParentLinkWebRequest.ReConnectTimer = Stopwatch.StartNew();
            try
            {
                //Thread.Sleep(3000);
                if (isDispose)
                    return;

                HttpWebRequest request = (HttpWebRequest)_request;
                _response = (HttpWebResponse)request.GetResponse();
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
            //_request.BeginGetResponse((asynchronousResult) =>
            //{
            //    try
            //    {
            //        //Thread.Sleep(3000);
            //        if (isDispose)
            //            return;
            //        HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            //        _response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

            //        if (ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes)
            //        {
            //            if (_response.Headers.AllKeys.Select(c => c.ToLower()).ToArray().Contains("content-range"))
            //            {
            //                if (!_response.Headers["Content-Range"].Contains((ParentLinkWebRequest.StartPosition + _saveStream.Length).ToString()))
            //                {
            //                    SetState(ConnectionState.CheckForSupportResumable);
            //                    var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address);
            //                    if (checkValue == LinkaddressCheckMode.False)
            //                        ParentLinkWebRequest.Stop(true, true);
            //                    else if (checkValue != LinkaddressCheckMode.True)
            //                    {
            //                        SetState(ConnectionState.Error);
            //                        return;
            //                    }
            //                }
            //            }
            //            else if (_response.Headers.AllKeys.Select(c => c.ToLower()).ToArray().Contains("accept-ranges"))
            //            {
            //                if (!_response.Headers["accept-ranges"].Contains((ParentLinkWebRequest.StartPosition + _saveStream.Length).ToString()))
            //                {
            //                    SetState(ConnectionState.CheckForSupportResumable);
            //                    var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address);
            //                    if (checkValue == LinkaddressCheckMode.False)
            //                        ParentLinkWebRequest.Stop(true, true);
            //                    else if (checkValue != LinkaddressCheckMode.True)
            //                    {
            //                        SetState(ConnectionState.Error);
            //                        return;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                SetState(ConnectionState.CheckForSupportResumable);
            //                var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address);
            //                if (checkValue == LinkaddressCheckMode.False)
            //                    ParentLinkWebRequest.Stop(true, true);
            //                else if (checkValue != LinkaddressCheckMode.True)
            //                {
            //                    //Agrin.Log.AutoLogger.LogText(checkValue.ToString());
            //                    //Agrin.Log.AutoLogger.LogText(_request.Address.OriginalString);
            //                    SetState(ConnectionState.Error);
            //                    return;
            //                }
            //            }
            //        }
            //        if (ParentLinkWebRequest.ConnectionId == 2 && _response.ContentLength == ParentLinkWebRequest.Parent.DownloadingProperty.Size)
            //        {
            //            SetState(ConnectionState.CheckForSupportResumable);
            //            var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address);
            //            if (checkValue == LinkaddressCheckMode.False)
            //                ParentLinkWebRequest.Parent.NotSupportResumable();
            //            else if (checkValue != LinkaddressCheckMode.True)
            //            {
            //                //Agrin.Log.AutoLogger.LogText(checkValue.ToString());
            //                //Agrin.Log.AutoLogger.LogText(_request.Address.OriginalString);
            //                SetState(ConnectionState.Error);
            //                return;
            //            }
            //        }
            //        if (ParentLinkWebRequest.EndPosition == -2)
            //        {
            //            ParentLinkWebRequest.EndPosition = _response.ContentLength;
            //            if (ParentLinkWebRequest.Parent.Connections.Count == 1)
            //                ParentLinkWebRequest.Parent.DownloadingProperty.Size = ParentLinkWebRequest.EndPosition - ParentLinkWebRequest.StartPosition;
            //        }

            //        if (asynchronousResult.IsCompleted && ParentLinkWebRequest.ConnectionId == 1)
            //        {
            //            GetHeaders();
            //            if (ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Unknown)
            //            {
            //                SetState(ConnectionState.CheckForSupportResumable);
            //                var checkValue = Agrin.Download.Web.LinkChecker.CheckAddressContentSupportRange(request.Address);
            //                if (checkValue == LinkaddressCheckMode.True)
            //                {
            //                    ParentLinkWebRequest.Parent.DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.Yes;
            //                }
            //            }
            //        }
            //        else
            //        {

            //        }

            //        if (asynchronousResult.IsCompleted)
            //        {
            //            string newUri = _response.ResponseUri.ToString();
            //            bool canAddNewUri = true;
            //            foreach (var item in ParentLinkWebRequest.Parent.Management.MultiLinks)
            //            {
            //                if (item.Address == newUri)
            //                {
            //                    canAddNewUri = false;
            //                    break;
            //                }
            //            }
            //            if (canAddNewUri)
            //            {
            //                ParentLinkWebRequest.Parent.Management.MultiLinks.Add(new MultiLinkAddress() { Address = newUri, IsSelected = true, IsApplicationAdded = true });
            //                ParentLinkWebRequest.Parent.SaveThisLink();
            //            }



            //            _responseStream = _response.GetResponseStream();
            //            if (ParentLinkWebRequest.EndPosition < 0 && ParentLinkWebRequest.Parent.DownloadingProperty.DownloadAlgoritm == AlgoritmEnum.Unknown)
            //            {
            //                ParentLinkWebRequest.ChangeAlgoritm(AlgoritmEnum.Page);
            //            }
            //            else if (!ParentLinkWebRequest.IsPauseCheck())
            //            {
            //                SetState(ConnectionState.Downloading);

            //                if (ParentLinkWebRequest.Connected != null)
            //                    ParentLinkWebRequest.Connected();
            //                DownloadData();
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        if (isDispose)
            //            return;
            //        StopException(e);
            //    }

            //}, _request);
        }

        public void StopException(Exception e)
        {
            try
            {
                if (!isDispose && ParentLinkWebRequest != null)
                {
                    if (e is WebException)
                    {
                        WebException c = e as WebException;
                        ParentLinkWebRequest.WebErrorStatus = c.Status;
                        if (c.Status == WebExceptionStatus.ProtocolError)
                        {
                            ParentLinkWebRequest.HttpStatusErrorCode = ((HttpWebResponse)c.Response).StatusCode;
                            if (ParentLinkWebRequest.HttpStatusErrorCode == HttpStatusCode.Forbidden)
                                ParentLinkWebRequest.Parent.DownloadingProperty.UriCache = null;
                        }
                    }
                    else
                    {
                        ParentLinkWebRequest.WebErrorStatus = WebExceptionStatus.UnknownError;
                    }

                    if (ParentLinkWebRequest.State == ConnectionState.Downloading || ParentLinkWebRequest.State == ConnectionState.Connecting || ParentLinkWebRequest.State == ConnectionState.FindingAddressFromSharing || ParentLinkWebRequest.State == ConnectionState.CreatingRequest)
                    {
                        ParentLinkWebRequest.Parent.Management.AddError(e);
                        ParentLinkWebRequest.StopComplete(StopEnum.Error);
                    }
                }
                if (!isDispose)
                    Agrin.Log.AutoLogger.LogError(e, "Connect Exception");
            }
            catch (Exception c)
            {
                if (!isDispose)
                    Agrin.Log.AutoLogger.LogError(c, "Connect WebException 2");
            }
        }

        Stopwatch limitWatch = new Stopwatch();
        TimeSpan perTime = new TimeSpan(0, 0, 1);
        internal object writeLock = new object();
        public virtual void DownloadData()
        {
            //try
            //{
            double downloaded = 0;
            limitWatch.Start();

            int _BufferRead = ParentLinkWebRequest.GetParentReadBuffer();
            byte[] _read = new byte[_BufferRead];
            int readCount = _responseStream.Read(_read, 0, _read.Length);
            downloaded += readCount;
            double getComplete;
            long saveStreamLen, saveStreamPos;
            Func<bool> canBreack = () =>
            {
                if (isDispose || ParentLinkWebRequest.IsPauseCheck())
                    return true;
                else if (ParentLinkWebRequest.State != ConnectionState.Downloading)
                    return true;
                return false;
            };
            do
            {
            SaveData:
                ParentLinkWebRequest.ReConnectTimer = Stopwatch.StartNew();
                if (ParentLinkWebRequest.islimit)
                {
                    if (limitWatch.Elapsed > perTime)
                    {
                        limitWatch.Restart();
                        downloaded = 0;
                    }
                    if (downloaded + _BufferRead > ParentLinkWebRequest.LimitPerSecound)
                    {
                        _BufferRead = ParentLinkWebRequest.LimitPerSecound - (int)downloaded;
                        if (_BufferRead < 0)
                            _BufferRead = 0;
                    }


                    if (_BufferRead == 0)
                    {
                        Thread.Sleep(perTime - limitWatch.Elapsed);
                    }
                }

                getComplete = ParentLinkWebRequest.Length - _saveStream.Position;
                lock (writeLock)
                {
                    if (canBreack())
                        break;
                    _saveStream.Write(_read, 0, readCount);
                    saveStreamLen = _saveStream.Length;
                    saveStreamPos = _saveStream.Position;
                    ParentLinkWebRequest.DownloadedSize = _saveStream.Position;
                }

                if (ParentLinkWebRequest.Length > ParentLinkWebRequest.DownloadedSize)
                {
                    if (_BufferRead > getComplete)
                    {
                        getComplete = (int)(ParentLinkWebRequest.Length - saveStreamPos);
                        if (getComplete != 0)
                        {
                            if (canBreack())
                                break;
                            readCount = _responseStream.Read(_read, 0, (int)getComplete);

                            if (ParentLinkWebRequest.islimit)
                                downloaded += readCount;
                        }
                    }
                    else
                    {
                        if (_BufferRead != _read.Length)
                            _read = new byte[_BufferRead];

                        if (canBreack())
                            break;
                        if (_BufferRead != 0)
                            readCount = _responseStream.Read(_read, 0, _BufferRead);
                        else
                            readCount = 0;
                        if (ParentLinkWebRequest.islimit)
                            downloaded += readCount;
                        _BufferRead = ParentLinkWebRequest.GetParentReadBuffer();
                    }
                    if (readCount == 0)
                        if (saveStreamLen == ParentLinkWebRequest.Length)
                        {
                            ParentLinkWebRequest.DownloadedSize = (long)ParentLinkWebRequest.Length;
                            ParentLinkWebRequest.Complete();
                            break;
                        }
                    goto SaveData;
                }
                else
                {
                    if (saveStreamLen > ParentLinkWebRequest.Length && ParentLinkWebRequest.Length > 0)
                    {
                        ParentLinkWebRequest.DownloadedSize = (long)ParentLinkWebRequest.Length;
                        lock (writeLock)
                        {
                            _saveStream.SetLength((long)ParentLinkWebRequest.Length);
                            saveStreamLen = _saveStream.Length;
                        }
                    }
                    if (saveStreamLen == ParentLinkWebRequest.Length)
                    {
                        ParentLinkWebRequest.DownloadedSize = (long)ParentLinkWebRequest.Length;
                        ParentLinkWebRequest.Complete();
                        break;
                    }
                    //else
                    //{
                    //    //if (!Pausing)
                    //    //    StopComplete();
                    //    break;
                    //}
                }
            } while (true);
            //ReConnectTimer = null;
            //if (Pausing)
            //    Paused();

            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
        }
        internal bool isDispose = false;
        public void Dispose()
        {
            try
            {

                lock (writeLock)
                {
                    isDispose = true;

                    try { ParentLinkWebRequest.ReConnectTimer = null; }
                    catch { }

                    if (_saveStream != null)
                        _saveStream.Dispose();
                    GC.Collect();

                    ParentLinkWebRequest = null;
                }
            }
            catch (Exception e)
            {
                AutoLogger.LogError(e, " _saveStream.Dispose ");
            }

            AsyncActions.Action(() =>
            {
                limitWatch = null;
                writeLock = null;
                if (_responseStream != null)
                    _responseStream.Dispose();
                _responseStream = null;
                if (_response != null)
                    _response.Close();
                _response = null;
            }, (e) =>
            {
                AutoLogger.LogError(e, " Dispose Error! AConnection");
            });
        }
    }
}
