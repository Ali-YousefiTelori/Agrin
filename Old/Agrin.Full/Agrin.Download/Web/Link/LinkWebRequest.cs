using Agrin.Download.Data.Serializition;
using Agrin.Download.Engine.Interfaces;
using Agrin.Download.Helper;
using Agrin.Download.Web.Connections;
using Agrin.Helper.ComponentModel;
using Agrin.IO;
using Agrin.IO.Helper;
using Agrin.IO.Strings;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Agrin.Download.Web.Link
{
    public class LinkWebRequest : ANotifyPropertyChanged
    {
        public LinkWebRequest(LinkInfo parent, string address)
        {
            Parent = parent;
            UriDownload = new Uri(address);
        }

        #region Commands

        public Action Connected;
        public Action Completed;
        public Action CommandBindingChanged { get; set; }

        #endregion

        #region Events

        #endregion

        #region Fields


        object lockObject = new object();

        object playLockObject = new object();

        #endregion

        #region Properties

        AConnectionInfo CurrentConnectionInfo { get; set; }

        public bool CanStop
        {
            get
            {
                return Parent.IsDownloading && !IsPause && State != ConnectionState.Stoped && State != ConnectionState.CopyingFile && State != ConnectionState.Complete && State != ConnectionState.Error && State != ConnectionState.Waiting;
            }
        }

        public bool CanPlay
        {
            get
            {
                return !Parent.IsManualStop && !IsPause && (State == ConnectionState.Error || State == ConnectionState.Paused || State == ConnectionState.Stoped);
            }
        }

        private int _connectionId;

        public int ConnectionId
        {
            get { return _connectionId; }
            set { _connectionId = value; OnPropertyChanged("ConnectionId"); }
        }

        public bool IsDownloading
        {
            get
            {
                return LinkInfo.IsDownloadingCheck(State);
            }
        }

        public ConnectionState _state;

        public ConnectionState State
        {
            get { return _state; }
            set
            {
                if (_state == ConnectionState.Complete && value == ConnectionState.Stoped)
                {
                    Parent.DownloadingProperty.ConnectionSetedState();
                    return;
                }

                var ghable = _state;
                _state = value;
                OnPropertyChanged("State");
                if (CommandBindingChanged != null)
                    CommandBindingChanged();
                Parent.DownloadingProperty.ConnectionSetedState();
                if ((ghable != ConnectionState.Downloading && value == ConnectionState.Downloading) || (ghable == ConnectionState.Downloading && value != ConnectionState.Downloading))
                    Parent.ResetBufferSizeAndLimiter();
            }
        }

        WebExceptionStatus _WebErrorStatus = WebExceptionStatus.Success;
        public WebExceptionStatus WebErrorStatus
        {
            get { return _WebErrorStatus; }
            set { _WebErrorStatus = value; }
        }

        HttpStatusCode _HttpStatusErrorCode;
        public HttpStatusCode HttpStatusErrorCode
        {
            get { return _HttpStatusErrorCode; }
            set { _HttpStatusErrorCode = value; }
        }

        FtpStatusCode _FtpStatusErrorCode;
        public FtpStatusCode FtpStatusErrorCode
        {
            get { return _FtpStatusErrorCode; }
            set { _FtpStatusErrorCode = value; }
        }

        private double _startPosition;

        public double StartPosition
        {
            get { return _startPosition; }
            set
            {
                _startPosition = value;
                OnPropertyChanged("StartPosition");
                Length = EndPosition - StartPosition;
            }
        }

        private double _endPosition = -2;

        public double EndPosition
        {
            get { return _endPosition; }
            set
            {
                _endPosition = value;
                OnPropertyChanged("EndPosition");
                Length = EndPosition - StartPosition;
            }
        }

        private double _Length;

        public double Length
        {
            get { return _Length; }
            set { _Length = value; OnPropertyChanged("Length"); }
        }

        static object lockDownload = new object();
        private long _downloadedSize;
        public long DownloadedSize
        {
            get
            {
                return _downloadedSize;
            }
            set
            {
                lock (lockDownload)
                {
                    long newSizeValue = value - _downloadedSize;
                    Parent.DownloadingProperty.DownloadedSize += newSizeValue;
                    _downloadedSize = value;

                    OnPropertyChanged("DownloadedSize");
                }
                //Parent.SetDownloadedSize();
            }
        }

        Uri _uriDownload;

        public Uri UriDownload
        {
            get { return _uriDownload; }
            set { _uriDownload = value; OnPropertyChanged("UriDownload"); }
        }

        Stopwatch _ReConnectTimer;
        public Stopwatch ReConnectTimer
        {
            get { return _ReConnectTimer; }
            set { _ReConnectTimer = value; }
        }

        CookieContainer _requestCookieContainer;

        public CookieContainer RequestCookieContainer
        {
            get { return _requestCookieContainer; }
            set { _requestCookieContainer = value; }
        }

        int _bufferRead;
        public int BufferRead
        {
            get { return _bufferRead; }
            set
            {
                _bufferRead = value;
            }
        }

        LinkInfo _parent;

        public LinkInfo Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged("Parent"); }
        }

        Task _currentThread;

        public Task CurrentThread
        {
            get { return _currentThread; }
            set { _currentThread = value; }
        }

        string _SaveFileName;

        public string SaveFileName
        {
            get
            {
                if (_SaveFileName == null)
                    _SaveFileName = System.IO.Path.Combine(Parent.PathInfo.ConnectionsSavedAddress, ConnectionId.ToString() + ".AT");
                if (!Directory.Exists(Path.GetDirectoryName(_SaveFileName)))
                    IOHelper.CreateDirectory(Path.GetDirectoryName(_SaveFileName));
                return _SaveFileName;
            }
            set { _SaveFileName = value; OnPropertyChanged("SaveFileName"); }
        }

        bool _isPause;

        public bool IsPause
        {
            get { return _isPause; }
            set
            {
                _isPause = value;
                OnPropertyChanged("IsPause");
                if (CommandBindingChanged != null)
                    CommandBindingChanged();
            }
        }

        #endregion

        #region Methods
        public static object lockStatic = new object();

        public void ChangeAlgoritm(AlgoritmEnum algoritm)
        {
            State = ConnectionState.Waiting;
            Parent.DownloadingProperty.DownloadAlgoritm = algoritm;
            Parent.SaveThisLink();
            switch (algoritm)
            {
                case AlgoritmEnum.Page:
                    {
                        //int index = Parent.Connections.IndexOf(this);
                        var connectionInfo = new PageConnectionInfo(UriDownload, this);
                        //connectionInfo.ParentLinkWebRequest.ConnectionId = ConnectionId;
                        //connectionInfo.ParentLinkWebRequest.Parent = Parent;
                        //connectionInfo.ParentLinkWebRequest.Parent.Connections.RemoveAt(index);
                        //connectionInfo.ParentLinkWebRequest.Parent.Connections.Add(connectionInfo);
                        //connectionInfo.Parent.Connections.Reset();
                        //Parent.SaveThisLink();
                        if (CurrentConnectionInfo != null)
                        {
                            connectionInfo._request = CurrentConnectionInfo._request;
                            connectionInfo._response = CurrentConnectionInfo._response;
                            connectionInfo._saveStream = CurrentConnectionInfo._saveStream;
                            connectionInfo._responseStream = CurrentConnectionInfo._responseStream;
                        }
                        CurrentConnectionInfo = connectionInfo;
                        State = ConnectionState.Downloading;
                        connectionInfo.DownloadData();
                        break;
                    }
                //case AlgoritmEnum.Limit:
                //    {
                //        var state = State;
                //        if (CanStop)
                //            Pause();
                //        ConnectionInfo connectionInfo;
                //        int index = Parent.Connections.IndexOf(this);
                //        connectionInfo = new LimitConnectionInfo(UriDownload);
                //        connectionInfo.ConnectionId = ConnectionId;
                //        connectionInfo.Parent = Parent;
                //        connectionInfo.Parent.Connections.RemoveAt(index);
                //        connectionInfo.Parent.Connections.Add(connectionInfo);
                //        Parent.SaveThisLink();
                //        connectionInfo._request = _request;
                //        connectionInfo._response = _response;
                //        connectionInfo._saveStream = _saveStream;
                //        connectionInfo._responseStream = _responseStream;
                //        if (state == ConnectionState.Downloading && State == ConnectionState.Paused)
                //            connectionInfo.DownloadData();

                //        break;
                //    }
            }

            //this._request = null;
            //this._response = null;
            //this._saveStream = null;
            //this._responseStream = null;
            this.UriDownload = null;

        }
        public bool IsPauseCheck()
        {
            lock (lockObject)
            {
                if (IsPause && State == ConnectionState.Pausing)
                {
                    StopComplete(StopEnum.Pause);
                }
                return _manualStop || IsPause || _state == ConnectionState.Stoped;
            }
        }

        int _limitPerSecound;
        public int LimitPerSecound
        {
            get { return _limitPerSecound; }
            set { _limitPerSecound = value; }
        }

        public bool islimit = false;
        public int GetParentReadBuffer()
        {
            islimit = Parent.Management.IsLimit;
            if (islimit && BufferRead > LimitPerSecound)
                return LimitPerSecound;
            return BufferRead;
        }


        //public virtual void DownloadData()
        //{
        //    //try
        //    //{
        //    Log.AutoLogger.AppedLog(Log.StateMode.Start, "Agrin.Download.Web.Creator.ConnectionInfo", "DownloadData", new object[] { });

        //    BufferRead = Parent.Management.ReadBuffer;
        //    byte[] _read = new byte[BufferRead];
        //    int readCount = _responseStream.Read(_read, 0, _read.Length);
        //    double getComplete;
        //    //if (AddThisRange != -1)
        //    //{
        //    //    ThreadInfo.LinkInfo.DownloadingProperty.AccebtRangs.Add(AddThisRange);
        //    //    ThreadInfo.LinkInfo.SaveRangeThisLink();
        //    //    AddThisRange = -1;
        //    //}
        //    do
        //    {
        //    SaveData:
        //        if (IsPauseCheck())
        //            break;
        //        else if (State != ConnectionState.Downloading)
        //            break;
        //        ReConnectTimer = Stopwatch.StartNew();
        //        getComplete = Length - _saveStream.Position;
        //        _saveStream.Write(_read, 0, readCount);

        //        _saveStream.Flush();
        //        DownloadedSize = _saveStream.Position;
        //        if (Length > DownloadedSize)
        //        {
        //            if (BufferRead > getComplete)
        //            {
        //                getComplete = (int)(Length - _saveStream.Position);
        //                if (getComplete != 0)
        //                    readCount = _responseStream.Read(_read, 0, (int)getComplete);
        //            }
        //            else
        //            {
        //                if (BufferRead != _read.Length)
        //                    _read = new byte[BufferRead];
        //                readCount = _responseStream.Read(_read, 0, BufferRead);
        //                //if (!Parent.Management.IsLimit)
        //                BufferRead = Parent.Management.ReadBuffer;
        //                //else
        //                //{
        //                //limit
        //                //Buffer = (_threadInfo.LinkInfo.Management.LimitSizePerTime / 10) + _threadInfo.LinkInfo.Management.LimitSizePerTime % 10;
        //                //Thread.Sleep(100);
        //                //}
        //            }
        //            if (readCount == 0)
        //                if (_saveStream.Length == Length)
        //                {
        //                    Complete();
        //                    break;
        //                }
        //            goto SaveData;
        //        }
        //        else
        //        {
        //            if (_saveStream.Length > Length && Length > 0)
        //            {
        //                DownloadedSize = (long)Length;
        //                _saveStream.SetLength((long)Length);
        //            }
        //            if (_saveStream.Length == Length)
        //            {
        //                Complete();
        //                break;
        //            }
        //            //else
        //            //{
        //            //    //if (!Pausing)
        //            //    //    StopComplete();
        //            //    break;
        //            //}
        //        }
        //    } while (true);
        //    Log.AutoLogger.AppedLog(Log.StateMode.End, "Agrin.Download.Web.Creator.ConnectionInfo", "DownloadData", new object[] { });

        //    //ReConnectTimer = null;
        //    //if (Pausing)
        //    //    Paused();

        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    System.Diagnostics.Debugger.Break();
        //    //}
        //}
        public void Play(bool isManual)
        {
            if (isManual)
                ManualPlay();
            else
                AutoPlay();
        }

        void ManualPlay()
        {
            Parent.forErrorEvent.Set();
            _manualStop = false;
            AutoPlay();
        }

        void AutoPlay()
        {
            ReConnectTimer = null;
            Parent.forErrorEvent.Set();
            if (_manualStop)
                return;
            State = ConnectionState.CreatingRequest;
            AsyncActions.Action(() =>
            {
                lock (playLockObject)
                {
                    if (_manualStop)
                        return;
                    if (IsPause)
                    {
                        //if (State == ConnectionState.Paused)
                        //{
                        //    IsPause = false;
                        //    State = ConnectionState.Downloading;
                        //    if (Connected != null)
                        //        Connected();
                        //    DownloadData();
                        //}
                    }
                    else
                    {
                        if (CurrentConnectionInfo != null)
                        {
                            DisposeConnection();
                        }
                        CurrentConnectionInfo = SharingHelper.GetConnectionFromAlgoritm(Parent.PathInfo.Address, Parent.DownloadingProperty.DownloadAlgoritm, this);
                        //
                    }
                }
                if (CurrentConnectionInfo != null)
                {
                    var create = CurrentConnectionInfo.CreateRequestData();
                    if (CurrentConnectionInfo != null && create && State != ConnectionState.Complete)
                        CurrentConnectionInfo.Connect();
                }
            }, (e) =>
            {
                var con = CurrentConnectionInfo;
                if (_manualStop || con == null || con.isDispose)
                {
                    return;
                }
                Agrin.Log.AutoLogger.LogError(e, "AutoPlay");
                if (LinkInfo.IsDownloadingCheck(State))
                {
                    Parent.Management.AddError(e);
                    StopComplete(StopEnum.Error);
                }
                else
                    AutoStop();
            });
        }

        public void Complete()
        {
            DisposeConnection();
            if (DownloadedSize != Length)
            {
                _downloadedSize = (long)Length;
                OnPropertyChanged("DownloadedSize");
            }
            State = ConnectionState.Complete;
            if (Completed != null)
                Completed();
        }

        public void StopComplete(StopEnum state)
        {
            if (state == StopEnum.Pause)
                State = ConnectionState.Paused;
            else if (state == StopEnum.Error)
            {
                AutoStop();
                State = ConnectionState.Error;
            }
            else if (state == StopEnum.Stop)
                AutoStop();
            else if (state == StopEnum.Dispose)
                ManualStop();
        }

        public void Pause()
        {
            lock (lockObject)
            {
                State = ConnectionState.Pausing;
                IsPause = true;
            }
        }

        public void CancelManualStop()
        {
            _manualStop = false;
        }

        bool _manualStop = false;
        void ManualStop(bool isbrokenLink = false, bool isError = false)
        {
            _manualStop = true;
            AutoStop(isbrokenLink);
        }

        void AutoStop(bool isbrokenLink = false, bool isError = false)
        {
            IsPause = true;
            DisposeConnection();
            if (isbrokenLink)
            {
                State = ConnectionState.BrokenLink;
                Parent.DownloadingProperty.UriCache = null;
            }
            else
            {
                if (isError)
                    State = ConnectionState.Error;
                else
                    State = ConnectionState.Stoped;
            }
            IsPause = false;
        }

        public void Stop(bool isManual, bool isbrokenLink = false, bool isError = false)
        {
            ReConnectTimer = null;
            lock (playLockObject)
            {
                if (isManual)
                    ManualStop(isbrokenLink, isError);
                else
                    AutoStop();
            }
        }

        public void Dispose()
        {
            //ManualStop();
        }

        public void ReConnect()
        {
            lock(Parent.lockValue)
            {
                ReConnectTimer = null;
                Stop(false);
                AutoPlay();
            }
        }

        public void DisposeConnection()
        {
            if (CurrentConnectionInfo != null)
                CurrentConnectionInfo.Dispose();
            CurrentConnectionInfo = null;
        }

        #endregion

        #region Static Methods

        //public static AConnectionInfo GetConnectionFromAlgoritm(AlgoritmEnum algoritm)
        //{
        //    if (algoritm == AlgoritmEnum.Page)
        //        return new PageConnectionInfo();
        //    return new NormalConnectionInfo();
        //}


        #endregion

    }

    public enum StopEnum
    {
        Stop,
        Pause,
        Error,
        Dispose
    }
}
