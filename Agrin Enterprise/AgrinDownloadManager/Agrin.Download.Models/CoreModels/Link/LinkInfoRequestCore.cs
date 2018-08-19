using Agrin.ComponentModels;
using Agrin.Download.EntireModels.Link;
using Agrin.Download.ShortModels.Link;
using Agrin.Download.Web;
using Agrin.IO;
using Agrin.IO.Helpers;
using Agrin.Models;
using Agrin.Models.Settings;
using Agrin.Threads;
using MvvmGo.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Agrin.Download.CoreModels.Link
{
    /// <summary>
    /// link info a connection request data for multi connection
    /// </summary>
    public abstract class LinkInfoRequestCore : NotifyPropertyChanged
    {
        volatile ConnectionStatus _Status = ConnectionStatus.Stoped;
        volatile bool _IsTimeToTryingReconnect = false;
        volatile int _Id = 0;
        volatile LinkInfoShort _LinkInfo;
        //long _Size;
        long _DownloadedSize;
        long _StartPosition;
        long _EndPosition = -2;
        volatile int _bufferRead = 1024 * 50;
        volatile int _limitPerSecound;
        volatile BaseConnectionInfo _BaseConnectionInfo;

        /// <summary>
        /// status of request info
        /// </summary>
        public ConnectionStatus Status
        {
            get => _Status;
            set
            {
                _Status = value;
                if (value == ConnectionStatus.Complete || IsDownloading)
                {
                    LinkInfo.ValidateLinkCompletion();
                }
                //LinkInfo.ValidateConnectionsToDownload();
                LinkInfo.ValidateUI();
                OnPropertyChanged(nameof(Status));
            }
        }

        /// <summary>
        /// if connection is downloading
        /// </summary>
        public bool IsDownloading
        {
            get
            {
                return Status == ConnectionStatus.Downloading || Status == ConnectionStatus.Connecting || Status == ConnectionStatus.ConnectingToSharing || Status == ConnectionStatus.CreatingRequest || Status == ConnectionStatus.FindingAddressFromSharing || Status == ConnectionStatus.GetAddressFromSharing || Status == ConnectionStatus.LoginToSharing || IsTimeToTryingReconnect;
            }
        }

        /// <summary>
        /// if connection is downloading
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return Status == ConnectionStatus.Complete;
            }
        }


        public int LimitPerSecound
        {
            get { return _limitPerSecound; }
            set { _limitPerSecound = value; }
        }

        public int BufferRead
        {
            get { return _bufferRead; }
            set
            {
                _bufferRead = value;
            }
        }

        /// <summary>
        /// if connection is error
        /// </summary>
        public bool IsError
        {
            get
            {
                return Status == ConnectionStatus.Error;
            }
        }

        /// <summary>
        /// get percent of downloaded size by double value
        /// </summary>
        public double PercentDouble
        {
            get
            {
                if (IsComplete || Length == 0 || Length == -2)
                    return 1.0;
                else if (Length < 0)
                    return 0.0;
                return (double)DownloadedSize / Length;
            }
        }

        /// <summary>
        /// get percent of downloaded size
        /// </summary>
        public string Percent
        {
            get
            {
                if (IsComplete || Length == 0)
                    return "100%";
                else if (Length == -2)
                    return "0%";
                return Length < 0 ? ApplicationResourceBase.Current.GetAppResource("Unknown_Language") : String.Format("{0:00.00%}", PercentDouble);
            }
        }

        /// <summary>
        /// current connection info
        /// </summary>
        public BaseConnectionInfo BaseConnectionInfo { get => _BaseConnectionInfo; set => _BaseConnectionInfo = value; }

        /// <summary>
        /// when link is going to Error and wait a time to trying reconnect
        /// </summary>
        public bool IsTimeToTryingReconnect { get => _IsTimeToTryingReconnect; set => _IsTimeToTryingReconnect = value; }
        /// <summary>
        /// link info of this properties
        /// </summary>
        public LinkInfoShort LinkInfo { get => _LinkInfo; set => _LinkInfo = value; }

        /// <summary>
        /// if user can stop this link
        /// </summary>
        public bool CanStop
        {
            get
            {
                return IsDownloading && !LinkInfo.IsCopyingFile && !IsDispose;
            }
        }

        /// <summary>
        /// if user can play this link
        /// </summary>
        public bool CanPlay
        {
            get
            {
                return !CanStop && !LinkInfo.IsCopyingFile && !IsComplete && !LinkInfo.IsManualStop && !IsDispose;
            }
        }

        /// <summary>
        /// size of connection
        /// </summary>
        //public long Size
        //{
        //    get
        //    {
        //        return Thread.VolatileRead(ref _Size);
        //    }
        //    set
        //    {
        //        Thread.VolatileWrite(ref _Size, value);
        //    }
        //}
        /// <summary>
        /// downloaded size of connection
        /// </summary>
        public long DownloadedSize
        {
            get
            {
                return Thread.VolatileRead(ref _DownloadedSize);
            }
            set
            {
                Thread.VolatileWrite(ref _DownloadedSize, value);
                LinkInfo.OnPropertyChanged(nameof(LinkInfo.DownloadedSize));
                LinkInfo.OnPropertyChanged(nameof(LinkInfo.Percent));
                LinkInfo.OnPropertyChanged(nameof(LinkInfo.PercentDouble));
                OnPropertyChanged(nameof(Percent));
                OnPropertyChanged(nameof(PercentDouble));
                OnPropertyChanged(nameof(DownloadedSize));
            }
        }

        /// <summary>
        /// start position of link
        /// </summary>
        public long StartPosition
        {
            get
            {
                return Thread.VolatileRead(ref _StartPosition);
            }
            set
            {
                Thread.VolatileWrite(ref _StartPosition, value);
            }
        }
        /// <summary>
        /// end position of link
        /// </summary>
        public long EndPosition
        {
            get
            {
                return Thread.VolatileRead(ref _EndPosition);
            }
            set
            {
                Thread.VolatileWrite(ref _EndPosition, value);
            }
        }

        /// <summary>
        /// length between start and end position
        /// </summary>
        public long Length
        {
            get
            {
                return EndPosition - StartPosition;
            }
        }

        /// <summary>
        /// security path address of Connection
        /// </summary>
        public string SecuritySaveConnectionFileName
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// save address of Connection on disk
        /// </summary>
        public string SaveConnectionFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(SecuritySaveConnectionFileName))
                    return PathHelper.CreateSecurityDirectoryIfNotExist(SecuritySaveConnectionFileName);
                var dir = Path.Combine(ApplicationSettingsInfo.Current.PathSettings.TemporaryLinkInfoSavePath, LinkInfo.IdString);
                dir = PathHelper.CreateDirectoryIfNotExist(dir);
                return System.IO.Path.Combine(dir, Id.ToString() + ".Part");
            }
        }

        /// <summary>
        /// connection id of request
        /// </summary>
        public int Id { get => _Id; set => _Id = value; }
        /// <summary>
        /// CookieContainer of request
        /// </summary>
        public CookieContainer RequestCookieContainer { get; set; }

        /// <summary>
        /// last time readed downlaod stream
        /// </summary>
        public DateTime LastReadDateTime { get; set; }
        /// <summary>
        /// duration of time to timeout from read downlaod stream
        /// </summary>
        public int LastReadDuration { get; set; } = 10;

        /// <summary>
        /// play the connection
        /// </summary>
        internal void Play()
        {
            if (!CanPlay)
                return;
            LastReadDateTime = DateTime.Now;
            DisposeConnection();
            BaseConnectionInfo = new BaseConnectionInfo(LinkInfo.PathInfo.MainUriAddress, null, null, this, LinkInfo);
            Status = ConnectionStatus.CreatingRequest;
            BaseConnectionInfo.Play();
        }
        /// <summary>
        /// stop the connection
        /// </summary>
        internal void Stop()
        {
            if (!CanStop)
                return;
            if (BaseConnectionInfo != null)
                BaseConnectionInfo.Stop();
            DisposeConnection();
        }

        /// <summary>
        /// get buffer size to read
        /// </summary>
        public int GetBufferSizeToRead
        {
            get
            {
                if (LinkInfo.LinkInfoManagementCore.IsLimit && BufferRead > LimitPerSecound)
                    return LimitPerSecound;
                return BufferRead;
            }
        }

        /// <summary>
        /// dispose connection
        /// </summary>
        public void DisposeConnection()
        {
            if (Status != ConnectionStatus.Error)
                Status = ConnectionStatus.Stoped;
            if (BaseConnectionInfo != null && !BaseConnectionInfo.IsDispose)
                BaseConnectionInfo.Dispose();
            BaseConnectionInfo = null;
        }

        /// <summary>
        /// complete link downloading
        /// </summary>
        internal void Complete()
        {
            FixCompleteProblems();
            if (DownloadedSize != Length)
            {
                _DownloadedSize = Length;
                OnPropertyChanged("DownloadedSize");
            }
            Status = ConnectionStatus.Complete;
            LinkInfo.CheckComplete();
            Logger.WriteLine("link request complete");
            //DisposeConnection();
        }

        internal void FixCompleteProblems()
        {
            //using (var saveStream = IOHelper.OpenFileStreamForWrite(SaveConnectionFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    if (saveStream.Length > Length)
            //    {
            //        saveStream.SetLength(Length);
            //    }
            //}
        }

        /// <summary>
        /// create instance
        /// </summary>
        /// <returns></returns>
        public static LinkInfoRequestCore Instance()
        {
#if (Debug || Release)
            return new LinkInfoRequest();
#elif (MobileDebug || MobileRelease)
            return new LinkInfoRequestShort();
#endif
        }
    }
}
