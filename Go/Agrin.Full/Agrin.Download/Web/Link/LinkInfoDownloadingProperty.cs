using Agrin.Helper.Collections;
using Agrin.Helper.ComponentModel;
using Agrin.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Web.Link
{
    public class LinkInfoDownloadingProperty : ANotifyPropertyChanged
    {
        #region Constructors
        public LinkInfoDownloadingProperty(LinkInfo parent)
        {
            _parent = parent;
        }
        public LinkInfoDownloadingProperty()
        {

        }

        #endregion

        #region Commands
        public Action<LinkInfo, bool> SelectionChanged;
        public Action ErrorAction;
        #endregion

        #region Events

        #endregion

        #region Fields

        #endregion

        #region Properties

        public bool IsForceDownload { get; set; }

        /// <summary>
        /// تعداد کانکشن ها
        /// </summary>
        int _connectionCount = 8;
        public int ConnectionCount
        {
            get { return _connectionCount; }
            set
            {
                _connectionCount = value;
                if (State == ConnectionState.Downloading)
                    AsyncActions.Action(() => Parent.ConncetionsCheck(false));
            }
        }

        /// <summary>
        /// تاریخ آخرین دانلود
        /// </summary>
        DateTime _dateLastDownload = DateTime.Now;
        public DateTime DateLastDownload
        {
            get { return _dateLastDownload; }
            set { _dateLastDownload = value; OnPropertyChanged("DateLastDownload"); }
        }
        /// <summary>
        /// تاریخ ساخت لینک
        /// </summary>
        DateTime _createDateTime = DateTime.Now;
        public DateTime CreateDateTime
        {
            get { return _createDateTime; }
            set { _createDateTime = value; OnPropertyChanged("CreateDateTime"); }
        }

        /// <summary>
        /// حجم فایل
        /// </summary>
        double _size = -2;
        public double Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged("Size");
                OnPropertyChanged("GetPercent");
                if (Parent != null)
                    Parent.OnPropertyChanged("IsSizeValue");
            }
        }

        ObtimizeList<double> _ListSpeedByteDownloaded = new ObtimizeList<double>(20);

        public ObtimizeList<double> ListSpeedByteDownloaded
        {
            get { return _ListSpeedByteDownloaded; }
            set { _ListSpeedByteDownloaded = value; }
        }

        public ConcurrentDictionary<string, string> CustomHeaders { get; set; } = new ConcurrentDictionary<string, string>();

        double _SpeedByteDownloaded;
        public double SpeedByteDownloaded
        {
            get { return _SpeedByteDownloaded; }
            set
            {
                if (Size < 0)
                    return;
                if (value < 0)
                {
                    value = 0;
                }
                _SpeedByteDownloaded = value;
                OnPropertyChanged("SpeedByteDownloaded");
                ListSpeedByteDownloaded.Add(value);
            }
        }

        TimeSpan _TimeRemaining = new TimeSpan();

        public TimeSpan TimeRemaining
        {
            get { return _TimeRemaining; }
            set { _TimeRemaining = value; OnPropertyChanged("TimeRemaining"); }
        }

        public string GetPercent
        {
            get
            {
                return Size < 0 ? ApplicationHelperBase.GetAppResource("Unknown_Language", true) : String.Format("{0:00.00%}", (double)DownloadedSize / Size);
            }
        }
        /// <summary>
        /// لینک اصلی
        /// </summary>
        LinkInfo _parent;
        public LinkInfo Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged("Parent"); }
        }
        /// <summary>
        /// سایت های شیر برای اینکه کانکشن ها دوباره لینک رو دریافت نکنن و از کش استفاده کنند
        /// </summary>
        public string UriCache { get; set; }

        /// <summary>
        /// قابلیت ایست
        /// </summary>
        ResumeCapabilityEnum _resumeCapability = ResumeCapabilityEnum.Unknown;
        public ResumeCapabilityEnum ResumeCapability
        {
            get { return _resumeCapability; }
            set { _resumeCapability = value; }
        }

        /// <summary>
        /// الگوریتم دانلود فایل
        /// </summary>
        private AlgoritmEnum _downloadAlgoritm = AlgoritmEnum.Unknown;
        public AlgoritmEnum DownloadAlgoritm
        {
            get { return _downloadAlgoritm; }
            set { _downloadAlgoritm = value; }
        }

        /// <summary>
        /// وضعیت لینک
        /// </summary>
        public ConnectionState _state;
        public ConnectionState State
        {
            get { return _state; }
            set
            {
                _state = value; OnPropertyChanged("State");
                if (Parent != null)
                {
                    Parent.ValidateCommands();
                }
            }
        }

        /// <summary>
        /// حجم دانلود شده
        /// </summary>
        long _downloadedSize;
        public long DownloadedSize
        {
            get { return _downloadedSize; }
            set
            {
                _downloadedSize = value;
                OnPropertyChanged("DownloadedSize");
                OnPropertyChanged("GetPercent");
                if (Parent != null)
                    Parent.OnPropertyChanged("GetPercent");
                Agrin.Download.Manager.ApplicationLinkInfoManager.Current.ChangedDownloadedSize();
            }
        }

        /// <summary>
        /// لینک انتخاب شده
        /// </summary>
        bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); if (SelectionChanged != null) SelectionChanged(Parent, value); }
        }

        /// <summary>
        /// رنج های شروع دانلود لینک
        /// </summary>
        private List<long> _downloadRangePositions = new List<long>();
        public List<long> DownloadRangePositions
        {
            get { return _downloadRangePositions; }
            set { _downloadRangePositions = value; }
        }

        string _ReconnectTimer = "0:0";

        public string ReconnectTimer
        {
            get { return _ReconnectTimer; }
            set { _ReconnectTimer = value; OnPropertyChanged("ReconnectTimer"); }
        }

        #endregion

        #region Methods

        object lockState = new object();
        /// <summary>
        /// تغییر وضعیت لینک
        /// </summary>
        /// <param name="state"></param>
        public void ConnectionSetedState()
        {
            List<LinkWebRequest> connections = Parent.Connections.ToList();
            lock (lockState)
            {
                bool isComplete = true, isConnecting = false, isPaused = true, isStop = true;
                if (State == ConnectionState.Waiting)
                    return;
                int errorCount = 0;
                bool isConnectingBreak = false;
                int count = connections.Count;
                foreach (var item in connections)
                {
                    if (item.State == ConnectionState.Downloading)
                    {
                        State = ConnectionState.Downloading;
                        return;
                    }
                    else if (item.State == ConnectionState.Connecting || item.State == ConnectionState.CreatingRequest)
                    {
                        isConnecting = true;
                        isStop = isComplete = isPaused = false;
                        isConnectingBreak = true;
                    }

                    if (!isConnectingBreak)
                    {
                        if (item.State != ConnectionState.Complete)
                            isComplete = false;
                        else
                            count--;
                        if (item.State != ConnectionState.Stoped && item.State != ConnectionState.Complete)
                            isStop = false;
                        if (item.State != ConnectionState.Paused && item.State != ConnectionState.Complete)
                            isPaused = false;
                        if (item.State == ConnectionState.Error || (errorCount > 0 && (item.State == ConnectionState.Stoped || item.State == ConnectionState.Paused)))
                            errorCount++;
                    }

                }
                //List<ConnectionState> states = new List<ConnectionState>();
                //foreach (var item in connections)
                //{
                //    states.Add(item.State);
                //}
                if (errorCount == count && errorCount != 0 && State != ConnectionState.Complete)
                {
                    State = ConnectionState.Error;
                    if (ErrorAction != null)
                        ErrorAction();
                }

                if (isComplete)
                {
                    if (Size <= DownloadedSize)
                    {
                        //State = ConnectionState.Complete;
                        Parent.CheckForCompleteLink(false);
                    }
                }
                else if (isConnecting)
                    State = ConnectionState.Connecting;
                else if (isPaused)
                    State = ConnectionState.Paused;
                else if (isStop)
                    State = ConnectionState.Stoped;
            }
        }

        #endregion


    }
}
