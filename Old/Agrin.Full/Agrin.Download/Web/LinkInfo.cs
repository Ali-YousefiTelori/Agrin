using Agrin.Download.Data;
using Agrin.Download.Data.Serializition;
using Agrin.Download.Data.Settings;
using Agrin.Download.Engine.Interfaces;
using Agrin.Download.Exceptions;
using Agrin.Download.Helper;
using Agrin.Download.Manager;
using Agrin.Download.Web.Connections;
using Agrin.Download.Web.Link;
using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.IO.Helper;
using Agrin.IO.Mixer;
using Agrin.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;

namespace Agrin.Download.Web
{
    public class LinkInfo : ANotifyPropertyChanged, IPlayInfo
    {
        #region Constructors

        public LinkInfo(string address)
        {
            PathInfo = new LinkInfoPath(address, this);
            if (SharingHelper.IsVideoSharing(address))
                DownloadingProperty.DownloadAlgoritm = AlgoritmEnum.Sharing;
            Management.AddMultiLink(new MultiLinkAddress() { Address = address, IsSelected = true }, false);
        }

        public LinkInfo()
        {

        }

        #endregion

        #region Commands

        public Func<IPlayInfo, Action> FinishAction { get; set; }
        //public Action CommandBindingChanged { get; set; }
        public Action DisposeAction { get; set; }

        #endregion

        #region Fields

        // public object lockObject = new object();

        #endregion

        #region Properties

        LinkInfoDownloadingProperty _downloadingProperty;

        public LinkInfoDownloadingProperty DownloadingProperty
        {
            get
            {
                if (_downloadingProperty == null)
                    _downloadingProperty = new LinkInfoDownloadingProperty(this);
                return _downloadingProperty;
            }
            set { _downloadingProperty = value; }
        }

        LinkInfoProperties _properties;
        public LinkInfoProperties Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        LinkInfoPath _pathInfo;
        public LinkInfoPath PathInfo
        {
            get { return _pathInfo; }
            set { _pathInfo = value; }
        }

        LinkInfoManagement _management;
        public LinkInfoManagement Management
        {
            get
            {
                if (_management == null)
                    _management = new LinkInfoManagement() { Parent = this };
                return _management;
            }
            set { _management = value; _management.Parent = this; }
        }

        public static bool IsDownloadingCheck(ConnectionState state)
        {
            return state == ConnectionState.Downloading || state == ConnectionState.Connecting || state == ConnectionState.ConnectingToSharing || state == ConnectionState.CreatingRequest || state == ConnectionState.FindingAddressFromSharing || state == ConnectionState.GetAddressFromSharing || state == ConnectionState.GetingData || state == ConnectionState.LoginToSharing || state == ConnectionState.SharingTimer || state == ConnectionState.CheckForSupportResumable;
        }

        public bool IsDownloading
        {
            get
            {
                return IsDownloadingCheck(DownloadingProperty.State) || DownloadingProperty.ReconnectTimer != "0:0";
            }
        }

        public bool CanStop
        {
            get
            {
                return !IsManualStop && (IsDownloading || DownloadingProperty.State == ConnectionState.Error || DownloadingProperty.State == ConnectionState.Paused || DownloadingProperty.State == ConnectionState.Stoped);
            }
        }

        public bool CanDelete
        {
            get
            {
                return DownloadingProperty.State != ConnectionState.CopyingFile;
            }
        }

        public bool CanPlay
        {
            get
            {
                return IsManualStop && (DownloadingProperty.State == ConnectionState.Error || DownloadingProperty.State == ConnectionState.Paused || DownloadingProperty.State == ConnectionState.Stoped);
            }
        }

        public bool CanPause
        {
            get
            {
                return DownloadingProperty.State == ConnectionState.Downloading;
            }
        }

        public bool IsComplete
        {
            get
            {
                return DownloadingProperty.State == ConnectionState.Complete;
            }
        }

        public bool IsError
        {
            get
            {
                return DownloadingProperty.State == ConnectionState.Error;
            }
        }

        public bool IsSizeValue
        {
            get
            {
                return DownloadingProperty.Size >= 0;
            }
        }

        public string GetPercent
        {
            get
            {
                return DownloadingProperty.GetPercent;
            }
        }

        public void ValidateCommands()
        {
            OnPropertyChanged("IsError");
            OnPropertyChanged("IsComplete");
            OnPropertyChanged("CanPause");
            OnPropertyChanged("CanPlay");
            OnPropertyChanged("CanDelete");
            OnPropertyChanged("CanStop");
            OnPropertyChanged("IsDownloading");
            if (DownloadingProperty.State == ConnectionState.Downloading && reconnectGetCount != -1)
                reconnectGetCount = -1;
        }

        FastCollection<LinkWebRequest> _connections;
        public FastCollection<LinkWebRequest> Connections
        {
            get
            {
                if (_connections == null)
                    _connections = new FastCollection<LinkWebRequest>(ApplicationHelperBase.DispatcherThread);
                return _connections;
            }
            set { _connections = value; }
        }

        bool _isGettingHostInfoProperties = false;
        IPPropertiesSerialize _HostInfoProperties = null;
        public IPPropertiesSerialize HostInfoProperties
        {
            get
            {
                try
                {
                    if (_HostInfoProperties != null)
                        return _HostInfoProperties;
                    if (_isGettingHostInfoProperties)
                        return _HostInfoProperties;
                    _isGettingHostInfoProperties = true;
                    AsyncActions.Action(() =>
                    {
                        int count = 0;
                        while (true)
                        {
                            try
                            {
                                Uri uri = new Uri(Management.GetLastMultiLinkAddress());
                                var ip = ApplicationIPsData.AddNewHostIP(uri.Host);

                                if (ip != null)
                                {
                                    _HostInfoProperties = ip;
                                    _HostFlag = ApplicationIPsData.GetFlagByCountryCode(ip.CountryCode);
                                    ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                                    {
                                        OnPropertyChanged("HostInfoProperties");
                                        OnPropertyChanged("HostFlag");
                                        OnPropertyChanged("CountryCodeKey");
                                    });
                                    break;
                                }
                                Thread.Sleep(5000);
                            }
                            catch (Exception e)
                            {
                                AutoLogger.LogError(e, "HostInfoProperties");
                                if (_isDispose || count > 10)
                                    break;
                                Thread.Sleep(5000);
                            }
                            count++;
                        }
                        _isGettingHostInfoProperties = false;
                    });
                    return _HostInfoProperties;
                }
                catch
                {
                    return _HostInfoProperties;
                }
            }
            set
            {
                _HostInfoProperties = value;
                OnPropertyChanged("HostInfoProperties");
                OnPropertyChanged("HostFlag");
                OnPropertyChanged("CountryCodeKey");
            }
        }

        byte[] _HostFlag = null;
        public byte[] HostFlag
        {
            get
            {
                if (_HostFlag == null)
                {
                    var hostIP = HostInfoProperties;
                    if (hostIP != null)
                    {
                        _HostFlag = ApplicationIPsData.GetFlagByCountryCode(hostIP.CountryCode);
                    }
                }
                return _HostFlag;
            }
        }

        /// <summary>
        /// کلید تبادل برای تبدیل به تصویر و نگهداری فقط تصویر در حافظه و جلوگیری از کپی شدن ان
        /// BytesToImageConverter استفاده شده نمونه در رابط کاربری با استفاده از کانورتور
        /// </summary>
        public string CountryCodeKey
        {
            get
            {
                if (HostInfoProperties == null)
                    return null;
                return "IpProperties" + HostInfoProperties.CountryCode;
            }
        }

        bool _isGettingFileIcon = false;

        byte[] _FileIcon = null;

        public byte[] FileIcon
        {
            get
            {
                if (_FileIcon != null)
                    return _FileIcon;
                if (_isGettingFileIcon)
                    return _FileIcon;
                _isGettingFileIcon = true;
                AsyncActions.Action(() =>
                {
                    _FileIcon = Agrin.IO.FileStatic.GetFileIcon(PathInfo.FileExtension);
                    if (_FileIcon != null)
                        OnPropertyChanged("FileIcon");
                    _isGettingFileIcon = false;
                }, (ex) =>
                {
                    _isGettingFileIcon = false;
                });
                return _FileIcon;
            }
            set
            {
                _FileIcon = value;
                OnPropertyChanged("FileIcon");
            }
        }

        bool _IsAddedToTaskForStart = false;

        public bool IsAddedToTaskForStart
        {
            get { return _IsAddedToTaskForStart; }
            set
            {
                _IsAddedToTaskForStart = value;
                OnPropertyChanged("IsAddedToTaskForStart");
                task_StateChanged(null);
            }
        }

        bool _IsAddedToTaskForStop = false;

        public bool IsAddedToTaskForStop
        {
            get { return _IsAddedToTaskForStop; }
            set
            {
                _IsAddedToTaskForStop = value;
                OnPropertyChanged("IsAddedToTaskForStop");
                task_StateChanged(null);
            }
        }

        List<TaskInfo> createdTaskStateChnaged = new List<TaskInfo>();
        public bool IsWaitingForPlayQueue
        {
            get
            {
                if (!IsManualStop)
                    return false;
                foreach (var task in ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(this))
                {
                    task.StateChanged -= task_StateChanged;
                    task.StateChanged += task_StateChanged;
                    createdTaskStateChnaged.Remove(task);
                    createdTaskStateChnaged.Add(task);
                    if (task.State == TaskState.Working || task.State == TaskState.WaitingForWork)
                        return true;
                }
                return false;
            }
        }

        void ClearTaskStateChnages()
        {
            foreach (var item in createdTaskStateChnaged.ToArray())
            {
                item.StateChanged -= task_StateChanged;
            }
            createdTaskStateChnaged.Clear();
        }

        void CheckTaskStateChnages()
        {
            var findList = ApplicationTaskManager.Current.FindTaskInfoByLinkInfo(this);
            foreach (var task in createdTaskStateChnaged.ToArray())
            {
                if (!findList.Contains(task))
                {
                    createdTaskStateChnaged.Remove(task);
                    task.StateChanged -= task_StateChanged;
                }
            }
        }

        void task_StateChanged(TaskInfo taskInfo)
        {
            OnPropertyChanged("IsWaitingForPlayQueue");
            CheckTaskStateChnages();
        }

        bool _ShowSelectedNumberVisibility = false;

        public bool ShowSelectedNumberVisibility
        {
            get { return _ShowSelectedNumberVisibility; }
            set { _ShowSelectedNumberVisibility = value; OnPropertyChanged("ShowSelectedNumberVisibility"); }
        }

        private string _SelectedIndexNumber = "0";

        public string SelectedIndexNumber
        {
            get { return _SelectedIndexNumber; }
            set { _SelectedIndexNumber = value; OnPropertyChanged("SelectedIndexNumber"); }
        }


        #endregion

        #region Events

        #endregion

        #region Methods

        public void Initialize()
        {
            GetSelectionLinks();
        }

        public bool CanAddNewConnection()
        {
            if (this.DownloadingProperty.State == ConnectionState.Waiting || this.DownloadingProperty.State == ConnectionState.Stoped || this.DownloadingProperty.State == ConnectionState.CopyingFile || this.DownloadingProperty.ResumeCapability != ResumeCapabilityEnum.Yes)//!this.Management.CanStart
                return false;

            int count = 0;
            foreach (var item in this.Connections.ToList())
            {
                if (item.State != ConnectionState.Downloading && item.State != ConnectionState.Complete)
                    return false;
                if (item.State != ConnectionState.Complete)
                    count++;
            }
            if (count >= DownloadingProperty.ConnectionCount)
                return false;
            //long downSize = 0;
            //foreach (var item in Connections.ToList())
            //{
            //    downSize += (long)item.Length;
            //}
            if (this.DownloadingProperty.Size - this.DownloadingProperty.DownloadedSize < 1048576)
                return false;
            return true;
        }
        //object lockDownloadSize = new object();
        public void SetDownloadedSize()
        {
            long downSize = 0;
            foreach (var item in Connections.ToList())
            {
                downSize += item.DownloadedSize;
            }
            this.DownloadingProperty.DownloadedSize = downSize;
        }

        public object lockValue = new object();
        public LinkWebRequest CreateNewConnectionInfo(bool isManual)
        {
            lock (lockValue)
            {
                LinkWebRequest max = null;
                LinkWebRequest retItem = null;
                //if (Connections.Count == 0)
                //{
                //    retItem = new LinkThreadInfo() { LinkInfo = this, StartPosition = StartPosition, EndPosition = EndPosition == 0 ? -2 : EndPosition, ThreadId = 1 };
                //    DownloadingProperty.ThreadLinks.Add(retItem);
                //    SaveThisLink();
                //    return retItem;
                //}
                if (DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes)
                {
                    foreach (var item in Connections.ToList())
                    {
                        if (max == null)
                            max = item;
                        else if (max.Length - max.DownloadedSize < item.Length - item.DownloadedSize)
                            max = item;
                    }
                    double downSizeMande = max.Length - max.DownloadedSize;
                    double size = (long)(downSizeMande / 2);
                    max.EndPosition = max.StartPosition + max.DownloadedSize + size + (downSizeMande % 2);
                    retItem = new LinkWebRequest(this, PathInfo.Address);
                    retItem.StartPosition = max.EndPosition;
                    retItem.EndPosition = max.EndPosition + size;
                    AddConnection(retItem, isManual);
                }

                //((Agrin_Engine.Helper.ANotifyPropertyChanged)BindingProperty).OnPropertyChanged("Size");

                return retItem;
            }
        }
        //class NewTestData
        //{
        //    public int id { get; set; }
        //}
        //List<NewTestData> data = new List<NewTestData>();
        static object dddNewConnectionLock = new object();
        public void AddConnection(LinkWebRequest connectionInfo, bool isManual)
        {
            lock (dddNewConnectionLock)
            {
                connectionInfo.Parent = this;
                int id = 1;
                if (Connections.Count > 0)
                {
                    id = Connections.Max((c) => c.ConnectionId) + 1;
                }
                connectionInfo.ConnectionId = id;
                Connections.Add(connectionInfo);
                connectionInfo.Connected = connectionInfo.Completed = new Action(() =>
                {
                    ApplicationHelperBase.EnterDispatcherThreadActionBegin(() =>
                    {
                        CheckForCreateConection(isManual);
                    });
                });
                var list = Connections.ToArray();
                List<int> cons = new List<int>();
                foreach (var item in list)
                {
                    if (!cons.Contains(item.ConnectionId))
                    {
                        cons.Add(item.ConnectionId);
                    }
                    else
                    {
                        AutoLogger.LogText("ConnectionID FatalError!");
                    }
                }
                SaveThisLink(true);
                SaveBackUpThisLink(true);
            }
        }

        public void CheckForCreateConection(bool isManual)
        {
            if (CanAddNewConnection() && CanPlayConnections(DownloadingProperty.State))
            {
                var con = CreateNewConnectionInfo(isManual);
                con.Play(isManual);
            }
            else
                ConncetionsCheck(isManual);
        }

        public bool CanPlayConnections(ConnectionState state)
        {
            return state == ConnectionState.Downloading || state == ConnectionState.Connecting || state == ConnectionState.ConnectingToSharing;
        }

        int errorCount = 0;
        public ManualResetEvent forErrorEvent = new ManualResetEvent(false);

        public void Play(bool isManual)
        {
            forErrorEvent.Set();
            if (isManual)
                ManualPlay();
            else
                AutoPlay(false, false);
        }

        bool isTryAgainToPlay = false;
        /// <summary>
        /// زمانی که لینک خطا خورده و تایم زده تا منتظر سعی مجدد باشه این گزینه تایم رو از ابتدا شروع میکنه و سعی میکنه مجدد سعی کنه تا دانلود کنه
        /// </summary>
        public void TryAgainToPlay()
        {
            if (!forErrorEvent.WaitOne(0))
            {
                reconnectGetCount = -1;
                AutoLogger.LogText($"LinkInfo TryAgainToPlay");
                errorCount = 0;
                isTryAgainToPlay = true;
                forErrorEvent.Set();
            }
        }

        void ManualPlay()
        {
            IsManualStop = false;
            AutoPlay(false, true);
        }

        int reconnectGetCount = -1;
        TimeSpan GetNewTimeForReConnect()
        {
            reconnectGetCount++;
            //if (reconnectGetCount == 0)
            //    return new TimeSpan(0, 0, 5);
            //else if (reconnectGetCount == 1)
            //    return new TimeSpan(0, 0, 35);
            //else if (reconnectGetCount == 2)
            //    return new TimeSpan(0, 1, 15);
            //else if (reconnectGetCount == 3)
            //    return new TimeSpan(0, 5, 0);
            //else
            //    return new TimeSpan(0, 15, 0);
            if (reconnectGetCount == 0)
                return new TimeSpan(0, 0, 3);
            else if (reconnectGetCount == 1)
                return new TimeSpan(0, 0, 5);
            else if (reconnectGetCount == 2)
                return new TimeSpan(0, 0, 10);
            else if (reconnectGetCount == 3)
                return new TimeSpan(0, 0, 20);
            else if (reconnectGetCount == 4)
                return new TimeSpan(0, 0, 40);
            else if (reconnectGetCount == 5)
                return new TimeSpan(0, 1, 0);
            else if (reconnectGetCount == 6)
                return new TimeSpan(0, 1, 30);
            else if (reconnectGetCount == 7)
                return new TimeSpan(0, 2, 0);
            else if (reconnectGetCount == 8)
                return new TimeSpan(0, 5, 0);
            else
                return new TimeSpan(0, 10, 0);
        }

        public object lockReconnect = new object();
        void AutoPlay(bool isError, bool manualPlay)
        {
            if (IsManualStop)
                return;
            if (!isError)
                errorCount = 0;
            DownloadingProperty.ErrorAction = () =>
            {
                AsyncActions.Action(() =>
                {
                    lock (lockReconnect)
                    {
                        if (IsManualStop || DownloadingProperty.ReconnectTimer != "0:0")
                            return;
                        errorCount++;
                        forErrorEvent.Reset();
                        var time = GetNewTimeForReConnect();
                        DownloadingProperty.ReconnectTimer = (int)time.TotalMinutes + ":" + time.Seconds;
                        bool userSign = false;
                        while (!userSign && !IsManualStop)
                        {
                            userSign = forErrorEvent.WaitOne(1000);
                            time = time.Add(new TimeSpan(0, 0, -1));
                            DownloadingProperty.ReconnectTimer = (int)time.TotalMinutes + ":" + time.Seconds;
                            if (time.TotalSeconds == 0 || !IsError)
                                break;
                        }
                        DownloadingProperty.ReconnectTimer = "0:0";
                        if (!isTryAgainToPlay)
                        {
                            if (userSign || IsManualStop)
                                return;
                        }
                        isTryAgainToPlay = false;

                        lock (lockValue)
                        {
                            ApplicationHelperBase.EnterDispatcherThreadAction(() =>
                            {
                                if (Manager.ApplicationLinkInfoManager.Current.DownloadingLinkInfoes.ToArray().Contains(this) && !IsManualStop && (errorCount < Management.TryAginCount || Management.IsTryExtreme))
                                {
                                    if (DownloadingProperty.State == ConnectionState.Error)
                                        AutoPlay(true, false);
                                }
                                else
                                {
                                    ApplicationNotificationManager.Current.Add(NotificationMode.Error, this);
                                    ManualStop(true);
                                }
                            });
                        }
                    }
                });
            };
            DownloadingProperty.DateLastDownload = DateTime.Now;
            Initialize();
            if (Connections.Count == 0)
            {
                //CheckForCreateConection(manualPlay);
                lock (lockValue)
                {
                    AddConnection(new LinkWebRequest(this, PathInfo.Address), manualPlay);
                }
            }
            //***
            //bool canCheckForComplete = Connections.Count != 0;

            //foreach (var item in Connections.ToList())
            //{
            //    if (item.State != ConnectionState.Complete)
            //    {
            //        canCheckForComplete = false;
            //        item.Play(manualPlay);
            //    }
            //    if (item.Connected == null)
            //        item.Connected = item.Completed = new Action(() =>
            //        {
            //            ApplicationHelperMono.EnterDispatcherThreadActionBegin(() =>
            //            {
            //                CheckForCreateConection(manualPlay);
            //            });
            //        });
            //}
            //if (canCheckForComplete)
            //{
            //    AsyncActions.Action(() =>
            //    {
            //        Complete();
            //    });
            //}
            //***
            if (!CheckForCompleteLink(manualPlay))
                CheckForCreateConection(manualPlay);
        }

        //public void Play()
        //{
        //    ManualPlay();
        //}

        public bool CheckForCompleteLink(bool isManual)
        {
            if (IsManualStop || DownloadingProperty.State == ConnectionState.CopyingFile)
                return false;

            var cons = Connections.ToList();
            bool isComplete = cons.Count != 0;
            if (cons.Count == 0)
                return false;
            foreach (var item in Connections.ToList())
            {
                if (item.State != ConnectionState.Complete)
                {
                    item.CancelManualStop();
                    isComplete = false;
                    ConncetionsCheck(isManual);
                }

                if (item.Connected == null)
                    item.Connected = item.Completed = new Action(() =>
                    {
                        ApplicationHelperBase.EnterDispatcherThreadActionBegin(() =>
                        {
                            CheckForCreateConection(isManual);
                        });
                    });
            }
            if (isComplete)
            {
                lock (completeLock)
                {
                    try
                    {
                        bool firstfixing = false;
                        FixedProblem:
                        var size = FixDownloadFileSize();
                        if (size != DownloadingProperty.Size && DownloadingProperty.Size >= 0)
                        {
                            if (firstfixing)
                                throw new Exception("i cannot fix size problem!");
                            //System.Text.StringBuilder text = new System.Text.StringBuilder();
                            //double allS = 0;
                            double lastEndPosition = 0;
                            LinkWebRequest lastRequest = null;
                            foreach (var item in LinkHelper.SortByPosition(Connections))
                            {
                                if (lastEndPosition != 0 && item.StartPosition != lastEndPosition)
                                {
                                    lastRequest.EndPosition = item.StartPosition;
                                }
                                lastEndPosition = item.EndPosition;
                                lastRequest = item;
                                //text.AppendLine(item.StartPosition.ToString());
                                //text.AppendLine(item.EndPosition.ToString());
                                //var newL = item.EndPosition - item.StartPosition;
                                //if (newL != item.DownloadedSize)
                                //{

                                //}
                                //text.AppendLine((newL).ToString());
                                //allS += newL;
                                //text.AppendLine("ok");
                            }
                            firstfixing = true;
                            goto FixedProblem;
                            //string val = text.ToString();
                            //return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Agrin.Log.AutoLogger.LogText("Complete FixDownloadFileSize error " + ex.Message);
                        DownloadingProperty.State = ConnectionState.Error;
                        Management.AddError(ex);
                        return false;
                    }
                }
                if (DownloadingProperty.State == ConnectionState.Complete)
                    return true;
                AsyncActions.Action(() =>
                {
                    Complete();
                }, (ex) =>
                {
                    Agrin.Log.AutoLogger.LogText("Complete isComplete " + ex.Message);
                });
                return true;
            }
            return false;
        }

        public void Pause()
        {
            DownloadingProperty.State = ConnectionState.Pausing;
            foreach (var item in Connections.ToList())
            {
                if (CanPlayConnections(item.State))
                    item.Pause();
            }
        }

        bool _IsManualStop = true;
        public bool IsManualStop
        {
            get { return _IsManualStop; }
            set
            {
                _IsManualStop = value;
                OnPropertyChanged("IsManualStop");
                task_StateChanged(null);
                ValidateCommands();
            }
        }

        public void ManualStop(bool isError = false)
        {
            IsManualStop = true;
            forErrorEvent.Set();
            reconnectGetCount = -1;
            DownloadingProperty.State = ConnectionState.Waiting;
            foreach (var item in Connections.ToList())
            {
                if (item.State != ConnectionState.Complete)
                    item.Stop(true, false, isError);
            }
            if (isError)
                DownloadingProperty.State = ConnectionState.Error;
            else
                DownloadingProperty.State = ConnectionState.Stoped;

            DownloadingProperty.SpeedByteDownloaded = 0;

            if (IsDownloading)
                ManualStop(isError);
            else if (FinishAction != null)
            {
                Action fin = FinishAction(this);
                if (fin != null)
                    AsyncActions.Action(fin);
            }
            DownloadingProperty.IsForceDownload = false;
        }

        public void AutoStop()
        {
            DownloadingProperty.State = ConnectionState.Waiting;
            foreach (var item in Connections.ToList())
            {
                if (item.State != ConnectionState.Complete)
                    item.Stop(false);
            }
            DownloadingProperty.State = ConnectionState.Stoped;
            DownloadingProperty.IsForceDownload = false;
        }

        public void Stop(bool isManual)
        {
            lock (lockConnectionCheckLock)
            {
                if (isManual)
                    ManualStop(DownloadingProperty.State == ConnectionState.Error);
                else
                    AutoStop();
            }
        }

        object lockConnectionCheckLock = new object();
        bool isConnectionCheck = false;
        public void ConncetionsCheck(bool isManual, bool checkToConnect = false)
        {
            if (IsManualStop)
                return;
            lock (lockConnectionCheckLock)
            {
                if (isConnectionCheck || IsManualStop)
                    return;
                isConnectionCheck = true;
                if (checkToConnect && !IsDownloading)
                {
                    isConnectionCheck = false;
                    return;
                }
                int downloading = 0;
                var connections = Connections.ToList();
                foreach (var item in connections)
                {
                    if (item.IsDownloading)
                        downloading++;
                }
                if (downloading >= DownloadingProperty.ConnectionCount)
                {
                    int playCount = downloading - DownloadingProperty.ConnectionCount;
                    if (playCount > 0)
                    {
                        foreach (var item in connections)
                        {
                            if (item.IsDownloading)
                            {
                                playCount--;
                                item.Stop(false);
                            }
                            if (playCount <= 0)
                                break;
                        }
                    }
                    isConnectionCheck = false;
                    return;
                }
                bool canAddThread = true;
                foreach (var item in connections)
                {
                    if (item.State == ConnectionState.Error || item.State == ConnectionState.Paused || item.State == ConnectionState.Stoped)
                    {
                        downloading++;
                        item.Play(isManual);
                    }
                    if (downloading >= DownloadingProperty.ConnectionCount)
                    {
                        isConnectionCheck = false;
                        return;
                    }
                    else if (item.State == ConnectionState.Connecting || item.State == ConnectionState.Pausing)
                        canAddThread = false;

                }
                if (canAddThread && DownloadingProperty.ResumeCapability == ResumeCapabilityEnum.Yes && downloading < DownloadingProperty.ConnectionCount && CanAddNewConnection())
                    CreateNewConnectionInfo(isManual).Play(isManual);
                isConnectionCheck = false;
            }
        }

        ConcurrentBag<MultiLinkAddress> _selectionLinks;
        public ConcurrentBag<MultiLinkAddress> SelectionLinks
        {
            get
            {
                if (_selectionLinks == null)
                    _selectionLinks = new ConcurrentBag<MultiLinkAddress>();
                return _selectionLinks;
            }
            set { _selectionLinks = value; }
        }

        ConcurrentBag<ProxyInfo> _selectionProxies;
        public ConcurrentBag<ProxyInfo> SelectionProxies
        {
            get
            {
                if (_selectionProxies == null)
                    _selectionProxies = new ConcurrentBag<ProxyInfo>();
                return _selectionProxies;
            }
            set { _selectionProxies = value; }
        }

        object listLock = new object();
        void GetSelectionLinks()
        {
            lock (listLock)
            {
                try
                {
                    MultiLinkAddress result1 = null;
                    while (!SelectionLinks.IsEmpty)
                    {
                        SelectionLinks.TryTake(out result1);
                    }

                    ProxyInfo result2;
                    while (!SelectionProxies.IsEmpty)
                    {
                        SelectionProxies.TryTake(out result2);
                    }
                }
                catch (Exception e)
                {
                    SelectionLinks = new ConcurrentBag<MultiLinkAddress>();
                    SelectionProxies = new ConcurrentBag<ProxyInfo>();
                    Agrin.Log.AutoLogger.LogError(e, "GetSelectionLinks", true);
                }

                foreach (var item in Management.MultiLinks.ToList())
                {
                    if (item.IsSelected)
                    {
                        SelectionLinks.Add(item);
                    }
                }

                foreach (var item in Management.MultiProxy.ToList())
                {
                    if (item.IsSelected)
                    {
                        SelectionProxies.Add(item);
                    }
                }
            }
        }

        int selectedLinkIndex = 0;
        public MultiLinkAddress GetNewAddress()
        {
            var items = SelectionLinks.ToList();
            if (items.Count == 0)
                GetSelectionLinks();

            lock (listLock)
            {
                if (items.Count == 0)
                    items = SelectionLinks.ToList();
                if (items.Count == 1)
                {
                    return items.FirstOrDefault();
                }
                else
                {
                    if (selectedLinkIndex >= items.Count)
                    {
                        selectedLinkIndex = 0;
                    }
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (selectedLinkIndex == i)
                        {
                            selectedLinkIndex++;
                            return items[i];
                        }
                    }
                }
                return items.FirstOrDefault();
            }
        }

        int selectedProxyIndex = 0;
        public IWebProxy GetNewProxy(MultiLinkAddress address)
        {
            if (address.ProxyID == 1)
                return null;
            else if (address.ProxyID != 0)
            {
                foreach (var item in Management.MultiProxy.ToList())
                {
                    if (item.Id == address.ProxyID)
                        return item.GetProxy();
                }
            }
            var items = SelectionProxies.ToList();
            if (items.Count == 1)
            {
                return items.First().GetProxy();
            }
            else
            {
                if (selectedProxyIndex >= items.Count)
                {
                    selectedProxyIndex = 0;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    if (selectedProxyIndex == i)
                    {
                        selectedProxyIndex++;
                        return items[i].GetProxy();
                    }
                }
            }
            return null;
        }

        public string[] GetUserAuthentication(string address)
        {
            Func<string, string, bool, string[]> GetCredentialCache = (user, pass, isUTF8) =>
            {
                string encoded = "";
                if (isUTF8)
                    encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user + ":" + pass));
                else
                    encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass));

                //string testDecode = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(encoded));

                return new string[] { "Authorization", "Basic " + encoded };
            };

            if (Management.NetworkUserPass != null && !String.IsNullOrEmpty(Management.NetworkUserPass.UserName) && !String.IsNullOrEmpty(Management.NetworkUserPass.Password))
            {
                var auth = GetCredentialCache(Management.NetworkUserPass.UserName, Management.NetworkUserPass.Password, Management.NetworkUserPass.ServerAddress == "*" + Framesoft.Helper.UserManagerHelper.domain + "*");
                return auth;
            }
            else
            {
                var findedItem = AppUserAccountsSetting.FindFromAddress(address);
                if (findedItem != null)
                    return GetCredentialCache(findedItem.UserName, findedItem.Password, Management.NetworkUserPass.ServerAddress == "*" + Framesoft.Helper.UserManagerHelper.domain + "*");
            }
            return null;
        }

        public void ChangeConnectionsAlgoritm(AlgoritmEnum algoritm)
        {
            var state = DownloadingProperty.State;
            DownloadingProperty.State = ConnectionState.Waiting;
            foreach (var item in Connections.ToList())
            {
                item.ChangeAlgoritm(algoritm);
            }
            DownloadingProperty.State = state;
        }

        public void Reconnect(bool isManual)
        {
            Stop(isManual);
            Play(isManual);
        }

        public void ResetBufferSizeAndLimiter()
        {
            var list = Connections.ToList();
            foreach (var item in list.ToList())
            {
                if (item.State != ConnectionState.Downloading)
                    list.Remove(item);
            }
            if (list.Count == 0)
                return;
            int buffer = Management.ReadBuffer / list.Count;
            int bufferEnd = Management.ReadBuffer % list.Count;
            int limit = Management.LimitPerSecound / list.Count;
            int limitEnd = Management.LimitPerSecound % list.Count;
            foreach (var item in list)
            {
                if (Management.IsLimit)
                {
                    item.BufferRead = buffer;
                    item.LimitPerSecound = limit;
                }
                else
                {
                    item.BufferRead = Management.ReadBuffer;
                }

            }
            if (bufferEnd > 0)
                list.Last().BufferRead += bufferEnd;
            if (limitEnd > 0)
                list.Last().BufferRead += bufferEnd;
        }

        public long FixDownloadFileSize()
        {
            long size = 0;
            foreach (var item in Connections.ToArray())
            {
                using (var copystream = IOHelper.OpenFileStreamForWrite(item.SaveFileName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                {
                    if (item.EndPosition > DownloadingProperty.Size)
                    {
                        item.EndPosition = DownloadingProperty.Size;
                    }
                    if (copystream.Length > item.Length && item.Length >= 0)
                    {
                        copystream.SetLength((long)item.Length);
                        copystream.Seek(0, System.IO.SeekOrigin.Begin);
                    }

                    size += copystream.Length;
                }
            }
            return size;
        }

        object completeLock = new object();
        public void Complete()
        {
            try
            {
                lock (completeLock)
                {
                    if (DownloadingProperty.State == ConnectionState.Complete)
                        return;
                    DownloadingProperty.State = ConnectionState.CopyingFile;
                    //تغییر برای بخش سکیوریتی اندروید
                    if (string.IsNullOrEmpty(PathInfo.SecurityPath) && !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(PathInfo.FullAddressFileName)))
                        IOHelper.CreateDirectory(System.IO.Path.GetDirectoryName(PathInfo.FullAddressFileName));

                    foreach (var item in Connections.ToArray())
                    {
                        item.DownloadedSize = 0;
                    }

                    DownloadingProperty.DownloadedSize = 0;
                    List<string> files = new List<string>();
                    long fileLen = 0;
                    foreach (LinkWebRequest item in LinkHelper.SortByPosition(Connections))
                    {
                        files.Add(item.SaveFileName);
                    }

                    //میکسر لینک که قبلاً ذخیره شده بود
                    MixerInfo mixerInfo = MixerInfo.LoadFromFile(PathInfo.MixerSavePath, PathInfo.MixerBackupSavePath);
                    MixerTypeEnum mixerType = MixerTypeEnum.NoSpace;

                    if (mixerInfo == null)
                    {
                        if (System.IO.File.Exists(PathInfo.FullAddressFileName))
                        {
                            using (var fs = Agrin.IO.Helper.IOHelper.OpenFileStreamForWrite(PathInfo.SavePath, System.IO.FileMode.OpenOrCreate, fileName: PathInfo.FileName))
                            {
                                fs.SetLength(0);
                            }
                        }
                        mixerType = MixerInfo.GenerateAutoMixerByDriveSize(PathInfo.SavePath, (long)DownloadingProperty.Size, files);
                        if (mixerType == MixerTypeEnum.NoSpace)
                            throw new Exception("فضای کافی برای ذخیره سازی بر روی دیسک خود ندارید. حداقل فضای کافی برای تکمیل سازی فایل 10 مگابایت می باشد.");
                        mixerInfo = MixerInfo.InstanceMixerByType(mixerType, PathInfo.MixerSavePath, PathInfo.MixerBackupSavePath);
                    }
                    else
                        mixerType = mixerInfo.MixerType;
                    //موتور میکس کننده
                    FileMixer mixer = FileMixer.GetMixerByType(mixerType, files);
                    mixerInfo.FilePath = PathInfo.SavePath;
                    mixerInfo.FileName = PathInfo.FileName;
                    //string savePath = @"D:\Test\Mix\MixerData.agn";
                    //string saveBackupPath = @"D:\Test\Mix\MixerDataBackup.agn";
                    //if (mixerInfo.MixerPath == null)
                    //{
                    //    mixerInfo.MixerPath = savePath;
                    //    mixerInfo.MixerBackupPath = saveBackupPath;
                    //    //foreach (var item in files)
                    //    //{
                    //    //    mixerInfo.Files.Add(new FileConnection() { Path = item });
                    //    //}
                    //    mixerInfo.FilePath = @"D:\Test\Mix\mixed.rar";
                    //}

                    mixer.OnChangedDataAction = () =>
                    {
                        DownloadingProperty.DownloadedSize = mixer.MixedSize;
                    };

                    mixer.Start(mixerInfo);
                    fileLen = mixer.Size;
                    //long fileLen = 0;
                    //using (var stream = IOHelper.OpenFileStreamForWrite(PathInfo.FullAddressFileName, System.IO.FileMode.Create, fileName: string.IsNullOrEmpty(PathInfo.SecurityFileSavePath) ? PathInfo.FileName : null, newSecurityFileName: (x) => { PathInfo.SecurityFileSavePath = x; SaveThisLink(true); }, data: this))
                    //{
                    //    long fileSize = 0;
                    //    foreach (var item in Connections)
                    //    {
                    //        using (var copystream = IOHelper.OpenFileStreamForRead(item.SaveFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    //        {
                    //            fileSize += copystream.Length;
                    //        }
                    //    }
                    //    stream.SetLength(fileSize);
                    //    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //    foreach (LinkWebRequest item in LinkHelper.SortByPosition(Connections))
                    //    {
                    //        using (var copystream = IOHelper.OpenFileStreamForRead(item.SaveFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    //        {
                    //            int len = 1024 * 1024 * 2;
                    //            byte[] read = new byte[len];
                    //            int rCount;
                    //            while ((rCount = copystream.Read(read, 0, len)) > 0)
                    //            {
                    //                System.Threading.Thread.Sleep(2);
                    //                stream.Write(read, 0, rCount);
                    //                item.DownloadedSize = copystream.Position;
                    //            }
                    //        }
                    //        if (ApplicationSetting.Current.IsRemoveBackupWhenCreatingFile)
                    //        {
                    //            try
                    //            {
                    //                IOHelper.DeleteFile(item.SaveFileName);
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                AutoLogger.LogError(e, "IsRemoveBackupWhenCreatingFile");
                    //            }
                    //        }
                    //    }
                    //    fileLen = stream.Length;
                    //}
                    if (fileLen != DownloadingProperty.Size && DownloadingProperty.Size > 0)
                    {
                        throw new Exception("File length is not true! try again.");
                    }
                    PathInfo.UserSavePath = PathInfo.SavePath;
                    DownloadingProperty.SpeedByteDownloaded = 0;
                    DownloadingProperty.State = ConnectionState.Complete;
                    string bkFileName = MPath.CreateOneFileByAddress(MPath.BackUpCompleteLinksPath);
                    PathInfo.BackUpCompleteAddress = System.IO.Path.Combine(MPath.BackUpCompleteLinksPath, bkFileName);
                    SerializeData.SaveLinkInfoDataToFile(this, bkFileName, MPath.BackUpCompleteLinksPath, true);
                    SerializeData.SaveLinkInfoesToFile();
                    ApplicationNotificationManager.Current.Add(NotificationMode.Complete, this);
                    if (FinishAction != null)
                    {
                        Action fin = FinishAction(this);
                        if (fin != null)
                            AsyncActions.Action(fin);
                    }
                    Destruction();
                    if (Management.IsEndDownload)
                        ApplicationHelperBase.SetShutDown((int)Management.EndDownloadSystemMode);
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "Complete Error", true);
                DownloadingProperty.State = ConnectionState.Error;
                Management.AddError(e);
                if (e is System.IO.IOException)
                {
                    ManualStop(true);
                }
                else
                {
                    DownloadingProperty.ErrorAction?.Invoke();
                }
            }
#if (MobileApp)
            GC.Collect();
#else

            GC.Collect(5, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            GC.Collect(5, GCCollectionMode.Forced);
#endif
        }

        public void Destruction()
        {
            try
            {
                IOHelper.DeleteDirectory(PathInfo.ConnectionsSavedAddress, true);
            }
            catch (Exception er)
            {
                Agrin.Log.AutoLogger.LogError(er, "Destruction Error");
                SaveThisLink();
            }
            finally
            {
                Data.SerializeData.SaveLinkInfoesToFile();
            }
        }

        public void SaveThisLink(bool force = false)
        {
            GetSelectionLinks();
            SerializeData.SaveLinkInfoDataToFile(this, force: force);
        }

        public void SaveBackUpThisLink(bool force = false)
        {
            GetSelectionLinks();
            SerializeData.SaveLinkInfoDataToFile(this, "Link_BackUp.agn", force: force);
        }
        bool _isDispose = false;
        public void Dispose()
        {
            _isDispose = true;
            var state = DownloadingProperty.State;
            DownloadingProperty.State = ConnectionState.Waiting;
            ManualStop();
            if (state == ConnectionState.Complete)
                DownloadingProperty.State = ConnectionState.Complete;
            else if (state != ConnectionState.Error)
                DownloadingProperty.State = ConnectionState.Stoped;
            else
                DownloadingProperty.State = ConnectionState.Error;
            if (DisposeAction != null)
                DisposeAction();
            ClearTaskStateChnages();
        }

        #endregion

        internal void NotSupportResumable()
        {
            ApplicationLinkInfoManager.Current.AddNewLinkInfoToNotSupportResumable(this);
            throw new NoResumableException("Resumable Not Support");
            //DownloadingProperty.ResumeCapability = ResumeCapabilityEnum.No;
            //var cons = Connections.ToList();
            //for (int i = 1; i < cons.Count; i++)
            //{
            //    cons[i].Stop(true);
            //    Connections.RemoveAt(i);
            //}
            //cons[0].EndPosition = cons[0].Length = DownloadingProperty.Size;
        }
    }
}

//namespace System
//{
//    public static class NotifyPropertyChangedClass
//    {
//        public static void SetNotifyPropertyChangedViewElement<T>(this T obj)
//        {
//            object t = obj;
//            ANotifyPropertyChanged<T> element = (ANotifyPropertyChanged<T>)t;
//            if (element != null)
//            {
//                ((ANotifyPropertyChanged<T>)t).ViewElement = obj;
//            }
//        }
//    }

//    public sealed class AliAttrib:Attribute
//    {
//        public AliAttrib(string ali)
//        {

//        }
//    }
//}

