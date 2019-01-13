using Agrin.Download.CoreModels.Link;
using Agrin.Download.ShortModels.Link;
using Agrin.Download.Web.Requests;
using Agrin.Interfaces;
using Agrin.IO;
using Agrin.IO.Helpers;
using Agrin.IO.Streams;
using Agrin.Log;
using Agrin.Models;
using Agrin.Models.Link;
using Agrin.Threads;
using Agrin.Web;
using Agrin.Web.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading;

namespace Agrin.Download.Web
{
    /// <summary>
    /// base of Connection Info
    /// </summary>
    public class BaseConnectionInfo : IObjectDisposable
    {
        static BaseConnectionInfo()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072;
            }
            catch (Exception ex)
            {

            }
        }

        private Thread CheckResumableThread { get; set; }

        private WebRequestExchangerBase CurrentWebRequestExchanger { get; set; }

        public Stream ResponseStream
        {
            get
            {
                return CurrentWebRequestExchanger.ResponseStream;
            }
        }

        private IStreamWriter _saveStream;

        private string _currentAddress;

        private readonly IWebProxy _currentProxy;

        private readonly string _authentication;
        private bool _IsDispose = false;

        private LinkInfoRequestCore RequestCore { get; set; }
        private LinkInfoShort LinkInfo { get; set; }

        private Stopwatch _limitWatch = new Stopwatch();
        private readonly TimeSpan _perTime = new TimeSpan(0, 0, 1);

        /// <summary>
        /// if link is ftp
        /// </summary>
        public bool IsFTP
        {
            get
            {
                if (_currentAddress.ToLower().StartsWith("ftp://"))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// if object is disposed
        /// </summary>
        public bool IsDispose
        {
            get
            {
                return _IsDispose;
            }

            set
            {
                _IsDispose = value;
            }
        }

        /// <summary>
        /// status of request core
        /// </summary>
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return this.RunInLock(() => RequestCore.Status);
            }
            set
            {
                this.RunInLock(() => RequestCore.Status = value);
            }
        }

        /// <summary>
        /// constructor of connection info
        /// </summary>
        /// <param name="requestCore"></param>
        /// <param name="linkInfo"></param>
        /// <param name="currentAddress"></param>
        /// <param name="proxy"></param>
        /// <param name="authentication"></param>
        public BaseConnectionInfo(string currentAddress, string authentication, IWebProxy proxy, LinkInfoRequestCore requestCore, LinkInfoShort linkInfo)
        {
            _authentication = authentication;
            _currentProxy = proxy;
            _currentAddress = currentAddress;
            RequestCore = requestCore;
            LinkInfo = linkInfo;
        }

        /// <summary>
        /// destroy object
        /// </summary>
        ~BaseConnectionInfo()
        {

        }

        /// <summary>
        /// play connection Info
        /// </summary>
        public void Play()
        {
            ConnectionStatus = ConnectionStatus.CreatingRequest;
            Thread thread = new Thread(StartConnection)
            {
                IsBackground = false
            };

            thread.Start();
        }

        /// <summary>
        /// stop connection downloading
        /// </summary>
        public void Stop()
        {
            SetStatus(ConnectionStatus.Stoped);
            Dispose();
        }

        public void StopWithError(Exception ex)
        {
            try
            {
                if (IsDispose)
                    return;
                LinkInfo.AsShort().LinkInfoManagementCore.AddError(ex);
                SetStatus(ConnectionStatus.Error);
                LinkInfo.OnBasicDataChanged?.Invoke();
            }
            catch
            {

            }
            finally
            {
                Dispose();
            }
        }

        private void StartConnection()
        {
            try
            {
                if (CreateRequest())
                {
                    Connect();
                }
            }
            catch (Exception ex)
            {
                StopWithError(ex);
            }
        }

        /// <summary>
        /// create request
        /// </summary>
        public virtual bool CreateRequest()
        {
            if (IsDispose)
                return false;
            ConnectionStatus = ConnectionStatus.CreatingRequest;
            _saveStream = IOHelperBase.Current.OpenFileStreamForWrite(RequestCore.SaveConnectionFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            // fix hang on 99 % 
            if (_saveStream.Length > 10 && _saveStream.Length != RequestCore.Length)
                _saveStream.SetLength(_saveStream.Length - 10);
            RequestCore.DownloadedSize = _saveStream.Length;
            if (RequestCore.EndPosition != -2 && _saveStream.Length == RequestCore.Length)
            {
                Complete();
                return false;
            }
            else if (_saveStream.Length > RequestCore.Length && RequestCore.Length >= 0)
            {
                _saveStream.SetLength(RequestCore.Length);
                RequestCore.DownloadedSize = _saveStream.Length;
                if (RequestCore.EndPosition != -2)
                {
                    Complete();
                    return false;
                }
            }
            _saveStream.Seek(0, SeekOrigin.End);

            CurrentWebRequestExchanger = WebRequestExchangerBase.Create(RequestExchangerType.NetFrameworkWebRequest);
            CurrentWebRequestExchanger.CreateRequest(_currentAddress, _authentication, 60000, _currentProxy, RequestCore.RequestCookieContainer, int.MaxValue, GetCustomHeaders());

            //foreach (var error in LinkInfo.LinkInfoManagementCore.GetErrors(out string errorsFileName))
            //{
            //    if (error == null)
            //        continue;
            //    if (error.ExceptionData is WebException)
            //    {
            //        if (error.OtherData.FirstOrDefault() is WebExceptionStatus && (WebExceptionStatus)error.OtherData.FirstOrDefault() == WebExceptionStatus.ConnectFailure)
            //        {
            //            CurrentWebRequestExchanger.SetExpect100Continue(false);
            //            break;
            //        }
            //    }
            //}

            if (RequestCore.StartPosition + _saveStream.Length > 0 && LinkInfo.LinkInfoDownloadCore.ResumeCapability == ResumeCapabilityEnum.Yes)
            {
                long rng = RequestCore.StartPosition + _saveStream.Length;

                //if (isForce)
                //{
                //    _request.Headers.Add("address", address.Address);
                //    _request.Headers.Add("RNG", rng.ToString());
                //    _request.Headers.Add("Size", ParentLinkWebRequest.Parent.DownloadingProperty.Size.ToString());
                //}
                //else
                CurrentWebRequestExchanger.AddRange(rng);
                LinkInfo.LinkInfoDownloadCore.AddStartedRangePosition(rng);
            }
            else
            {
                if (LinkInfo.LinkInfoDownloadCore.ResumeCapability != ResumeCapabilityEnum.Yes && LinkInfo.DownloadedSize != 0)
                {
                    CheckResumableSupport();
                    if (LinkInfo.LinkInfoDownloadCore.ResumeCapability != ResumeCapabilityEnum.Yes)
                        LinkInfo.NotResumableSupport();
                    return true;
                }
            }
            return true;
        }

        private WebHeaderCollection GetCustomHeaders()
        {
            if (LinkInfo.LinkInfoDownloadCore is EntireModels.Link.LinkInfoDownload)
            {
                WebHeaderCollection result = new WebHeaderCollection();
                foreach (KeyValuePair<string, string> item in ((EntireModels.Link.LinkInfoDownload)LinkInfo.LinkInfoDownloadCore).CustomHeaders)
                {
                    result.Add(item.Key, item.Value);
                }
                return result;
            }
            return null;
        }
        /// <summary>
        /// connect to request
        /// </summary>
        public virtual void Connect()
        {
            if (IsDispose)
                return;
            ConnectionStatus = ConnectionStatus.Connecting;
            Stopwatch reConnectTimer = Stopwatch.StartNew();
            try
            {
                CurrentWebRequestExchanger.GetResponse();

                //if (LinkInfo.LinkInfoDownloadCore.ResumeCapability == ResumeCapabilityEnum.Unknown)
                //{
                //    if (_response.Headers["Content-Range"] != null)
                //    {
                //        if (!_response.Headers["Content-Range"].Contains((RequestCore.StartPosition + _saveStream.Length).ToString()))
                //        {
                //            CheckResumableSupport();
                //        }
                //        else
                //            LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.Yes;
                //    }
                //    else if (_response.Headers["accept-ranges"] != null)
                //    {
                //        if (!_response.Headers["accept-ranges"].Contains((RequestCore.StartPosition + _saveStream.Length).ToString()))
                //        {
                //            CheckResumableSupport();
                //        }
                //        else
                //            LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.Yes;
                //    }
                //    else
                //    {
                //        CheckResumableSupport();
                //    }
                //}
                if (IsDispose)
                    return;
                if (LinkInfo.Connections.Count == 1 && LinkInfo.LinkInfoDownloadCore.ResumeCapability == ResumeCapabilityEnum.Unknown)
                {
                    CheckResumableThread = new Thread(CheckResumableSupport) { IsBackground = false };
                    CheckResumableThread.Start();
                }
                else
                    LinkInfo.CreateRequestCoreIfNeeded();
                //if (LinkInfo.IsGetSize && CurrentWebRequestExchanger.ContentLength != LinkInfo.Size)
                //{
                //    throw new Exception("link size is not true!");
                //}
                if (RequestCore.EndPosition == -2)
                {
                    RequestCore.EndPosition = CurrentWebRequestExchanger.ContentLength;

                }

                if (!LinkInfo.IsGetSize && LinkInfo.Connections.Count == 1)
                {
                    LinkInfo.Size = CurrentWebRequestExchanger.ContentLength;
                    LinkInfo.IsGetSize = true;
                }

                if (RequestCore.Id == 1)
                {
                    GetHeaders();
                }

                string newUri = CurrentWebRequestExchanger.ResponseUri.ToString();
                bool canAddNewUri = true;
                foreach (LinkAddressInfo item in LinkInfo.LinkInfoManagementCore.MultiLinkAddresses)
                {
                    if (item.Address == newUri)
                    {
                        canAddNewUri = false;
                        break;
                    }
                }
                if (canAddNewUri && !IsDispose)
                {
                    LinkInfo.LinkInfoManagementCore.AddNewLinkAddressInfo(new LinkAddressInfo() { Address = newUri, IsEnabled = true, IsApplicationAdded = true });
                }

                CurrentWebRequestExchanger.GetResponseStream();

                ConnectionStatus = ConnectionStatus.Downloading;
                DownloadData();
            }
            catch (Exception e)
            {
                if (!IsDispose && ConnectionStatus != ConnectionStatus.Stoped)
                    LinkInfo.LinkInfoManagementCore.AddError(e);
                AutoLogger.LogError(e, "Connect");
                ConnectionStatus = ConnectionStatus.Error;
            }
        }

        internal virtual void GetHeaders()
        {
            try
            {
                bool mustSave = false;
                string contentType = "";

                if (CurrentWebRequestExchanger.ResponseHeaders["content-disposition"] != null)
                {
                    string fileName = "";
                    string contentDisposition = CurrentWebRequestExchanger.ResponseHeaders["content-disposition"].Replace("[", "").Replace("]", "");
                    try
                    {
                        ContentDisposition content = new ContentDisposition(contentDisposition);
                        fileName = content.FileName;
                    }
                    catch (Exception ex)
                    {
                        AutoLogger.LogError(ex, $"content-disposition: {CurrentWebRequestExchanger.ResponseHeaders["content-disposition"]} !");
                    }

                    fileName = Decodings.FullDecodeString(PathHelper.GetFileNameFromContentDisposition(contentDisposition));
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        mustSave = LinkInfo.PathInfo.AppFileName != fileName;
                        LinkInfo.PathInfo.AppFileName = fileName;
                    }
                }
                if (CurrentWebRequestExchanger.ResponseHeaders["content-type"] != null)
                {
                    contentType = CurrentWebRequestExchanger.ResponseHeaders["content-type"];
                }
                if (CurrentWebRequestExchanger.ResponseHeaders["accept-ranges"] != null)
                {
                    if (LinkInfo.LinkInfoDownloadCore.ResumeCapability != ResumeCapabilityEnum.No)
                    {
                        string value = CurrentWebRequestExchanger.ResponseHeaders["accept-ranges"];
                        if (value != null && value.ToLower().Contains("bytes"))
                        {
                            if (value.Contains(RequestCore.StartPosition.ToString()) || RequestCore.StartPosition == 0)
                                LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.Yes;
                        }
                        else
                            LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.No;
                    }
                }
                if (CurrentWebRequestExchanger.ResponseHeaders["content-range"] != null)
                {
                    if (LinkInfo.LinkInfoDownloadCore.ResumeCapability != ResumeCapabilityEnum.No)
                    {
                        string value = CurrentWebRequestExchanger.ResponseHeaders["content-range"];
                        if (value != null && value.ToLower().Contains(RequestCore.StartPosition.ToString()))
                            LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.Yes;
                    }
                }

                if (String.IsNullOrEmpty(LinkInfo.PathInfo.AppFileName) || (!String.IsNullOrEmpty(LinkInfo.PathInfo.AppFileName) && (LinkInfo.PathInfo.AppFileName.ToLower().EndsWith("html") || LinkInfo.PathInfo.AppFileName.ToLower().EndsWith("htm"))))
                {
                    string getfileName = PathHelper.GetLinksFileName(CurrentWebRequestExchanger.ResponseUri.OriginalString);
                    if (Path.HasExtension(getfileName))
                    {
                        mustSave = LinkInfo.PathInfo.AppFileName != getfileName;
                        LinkInfo.PathInfo.AppFileName = getfileName;
                    }
                }
                if (!String.IsNullOrEmpty(contentType))
                {
                    string ext = MimeTypeHelper.GetExtension(contentType);
                    if (string.IsNullOrEmpty(ext) || !LinkInfo.PathInfo.AppFileName.ToLower().EndsWith(ext.ToLower()))
                    {
                        string extNew = PathHelper.GetFileExtention(LinkInfo.PathInfo.AppFileName);
                        if (extNew != null)
                            extNew = extNew.ToLower();
                        if (extNew == ".htm" || extNew == ".html")
                        {
                            string newFileName = Path.GetFileNameWithoutExtension(LinkInfo.PathInfo.AppFileName) + extNew;
                            mustSave = LinkInfo.PathInfo.AppFileName != newFileName;
                            LinkInfo.PathInfo.AppFileName = newFileName;
                        }
                    }
                }

                LinkInfo.PathInfo.AppFileName = PathHelper.GetFileNameValidChar(LinkInfo.PathInfo.AppFileName);
                if (mustSave)
                    LinkInfo.Save();
            }
            catch (Exception e)
            {
                try
                {
                    AutoLogger.LogError(e, "GetHeaders");
                    if (ConnectionStatus == ConnectionStatus.Connecting)
                    {
                        if (!IsDispose)
                            LinkInfo.LinkInfoManagementCore.AddError(e);
                        ConnectionStatus = ConnectionStatus.Error;
                    }
                }
                catch (Exception c)
                {
                    AutoLogger.LogError(c, "GetHeaders 2");
                }

                //System.Diagnostics.Debugger.Break();
            }
        }

        private bool CanBreak()
        {
            if (IsDispose)
                return true;
            else if (ConnectionStatus != ConnectionStatus.Downloading)
                return true;
            return false;
        }

        private void SetStatus(ConnectionStatus status)
        {
            ConnectionStatus = status;
        }



        /// <summary>
        /// download data from connection
        /// </summary>
        public virtual void DownloadData()
        {
            RequestCore.LastReadDuration = 10;
            RequestCore.LastReadDateTime = DateTime.Now;
            double downloaded = 0;
            _limitWatch.Start();
            if (RequestCore.Length <= 0)
                throw new Exception($"what is this size? {RequestCore.EndPosition} {RequestCore.StartPosition} {LinkInfo.Connections.Count} {RequestCore.Id}");
            int _BufferRead = RequestCore.GetBufferSizeToRead;
            byte[] _read = new byte[_BufferRead];
            ReadBytes(out int readCount, ref _read, _read.Length, LinkInfo.DownloadedSize);
            downloaded += readCount;
            long getComplete;
            long saveStreamLen, saveStreamPos;
            do
            {
                SaveData:
                //ParentLinkWebRequest.ReConnectTimer = Stopwatch.StartNew();
                if (LinkInfo.LinkInfoManagementCore.IsLimit)
                {
                    if (_limitWatch.Elapsed > _perTime)
                    {
                        _limitWatch.Restart();
                        downloaded = 0;
                    }
                    if (downloaded + _BufferRead > RequestCore.LimitPerSecound)
                    {
                        _BufferRead = RequestCore.LimitPerSecound - (int)downloaded;
                        if (_BufferRead < 0)
                            _BufferRead = 0;
                    }


                    if (_BufferRead == 0)
                    {
                        Thread.Sleep(_perTime - _limitWatch.Elapsed);
                    }
                }

                getComplete = RequestCore.Length - _saveStream.Position;

                if (CanBreak())
                    break;
                _saveStream.Write(_read, 0, readCount);
                //_saveStream.Flush();
                saveStreamLen = _saveStream.Length;
                saveStreamPos = _saveStream.Position;
                RequestCore.DownloadedSize = _saveStream.Position;

                if (RequestCore.Length > RequestCore.DownloadedSize)
                {
                    if (_BufferRead > getComplete)
                    {
                        getComplete = (int)(RequestCore.Length - saveStreamPos);
                        if (getComplete != 0)
                        {
                            if (CanBreak())
                                break;
                            ReadBytes(out readCount, ref _read, (int)getComplete, LinkInfo.DownloadedSize);

                            if (LinkInfo.LinkInfoManagementCore.IsLimit)
                                downloaded += readCount;
                        }
                    }
                    else
                    {
                        if (_BufferRead != _read.Length)
                            _read = new byte[_BufferRead];

                        if (CanBreak())
                            break;
                        if (_BufferRead != 0)
                        {
                            ReadBytes(out readCount, ref _read, _BufferRead, LinkInfo.DownloadedSize);
                        }
                        else
                            readCount = 0;
                        if (IsDispose)
                            break;
                        if (LinkInfo.LinkInfoManagementCore.IsLimit)
                            downloaded += readCount;
                        _BufferRead = RequestCore.GetBufferSizeToRead;
                    }
                    if (readCount == 0)
                    {
                        if (saveStreamLen == RequestCore.Length)
                        {
                            RequestCore.DownloadedSize = RequestCore.Length;
                            Complete();
                            break;
                        }
                        else
                            throw new Exception("zero byte readed link disconnected!");
                    }
                    goto SaveData;
                }
                else
                {
                    if (saveStreamLen > RequestCore.Length && RequestCore.Length > 0)
                    {
                        RequestCore.DownloadedSize = RequestCore.Length;
                        _saveStream.SetLength(RequestCore.Length);
                        saveStreamLen = _saveStream.Length;
                    }
                    if (saveStreamLen == RequestCore.Length)
                    {
                        RequestCore.DownloadedSize = RequestCore.Length;
                        Complete();
                        break;
                    }
                }
            }
            while (!IsDispose);
        }

        /// <summary>
        /// read bytes from connection
        /// </summary>
        /// <param name="readCount"></param>
        /// <param name="bytes"></param>
        /// <param name="count"></param>
        /// <param name="range"></param>
        public void ReadBytes(out int readCount, ref byte[] bytes, int count, long range)
        {
            readCount = ResponseStream.Read(bytes, 0, count);
            if (IsDispose)
                return;
            RequestCore.LastReadDateTime = DateTime.Now;
            RequestCore.LastReadDuration = 10;
        }

        private void CheckResumableSupport()
        {
            try
            {
                if (LinkInfo.LinkInfoDownloadCore.ResumeCapability != ResumeCapabilityEnum.Unknown)
                    return;
                using (LinkResumableChecker resumableChecker = new LinkResumableChecker())
                {
                    if (resumableChecker.CheckAddressContentForSupportResumableJustHeader(_currentAddress, _authentication, _currentProxy, RequestCore.RequestCookieContainer, GetCustomHeaders()))
                        LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.Yes;
                    else
                    {
                        if (resumableChecker.CheckAddressContentForSupportResumable(_currentAddress, _authentication, _currentProxy, RequestCore.RequestCookieContainer, GetCustomHeaders()))
                            LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.Yes;
                        else
                            LinkInfo.LinkInfoDownloadCore.ResumeCapability = ResumeCapabilityEnum.No;
                    }
                }
                if (LinkInfo.LinkInfoDownloadCore.ResumeCapability == ResumeCapabilityEnum.Yes)
                    LinkInfo.CreateRequestCoreIfNeeded();

            }
            catch (Exception ex)
            {
                AutoLogger.LogError(ex, "CheckResumableSupport");
            }
        }

        private void Complete()
        {
            if (_saveStream.Length != RequestCore.Length && RequestCore.Length > 0)
            {
                _saveStream.SetLength(RequestCore.Length);
            }
            BeginDispose();
            RequestCore.Complete();
            Dispose();
        }

        private readonly bool _isBeginDispose = false;
        /// <summary>
        /// dispose file stream
        /// </summary>
        public void BeginDispose()
        {
            if (_isBeginDispose || _saveStream == null)
                return;
            _saveStream.Dispose();
        }
        /// <summary>
        /// dispose objct
        /// </summary>
        public void Dispose()
        {
            if (IsDispose)
                return;
            BeginDispose();
            this.RunInLock(() =>
            {
                ObjectLocker.ObjectDisposed(this);
                if (CheckResumableThread != null)
                    CheckResumableThread.Abort();

                if (CurrentWebRequestExchanger != null)
                    CurrentWebRequestExchanger.Dispose();

                RequestCore.BaseConnectionInfo = null;
                RequestCore = null;
                LinkInfo = null;
                CheckResumableThread = null;
                GC.SuppressFinalize(this);
                //GC.Collect();
            });
        }
    }
}
