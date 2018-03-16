using Agrin.ComponentModels;
using Agrin.Download.EntireModels.Link;
using Agrin.Download.Mixers;
using Agrin.Download.ShortModels.Link;
using Agrin.Download.Web;
using Agrin.Foundation;
using Agrin.Helpers;
using Agrin.IO.Helpers;
using Agrin.Log;
using Agrin.Models;
using Agrin.Threads;
using SignalGo.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Agrin.Download.CoreModels.Link
{
    /// <summary>
    /// core of link Info
    /// </summary>
    public abstract class LinkInfoCore : NotifyPropertyChanged
    {
        public Action OnBasicDataChanged { get; set; }

        volatile ConcurrentList<LinkInfoRequestCore> _Connections = null;
        volatile bool _IsCopyingFile = false;
        volatile bool _IsManualStop = true;
        long _Size = -2;//-2 = not get download size and -1 = dont know what is download size
        long _DownloadedSizePerSecound = 0;
        long _MixedSize = 0;
        bool _IsSelected = false;
        ushort _SelectedIndex = 0;

        ConcurrentCircularBuffer<long> _AvarageDownloadedSizePerSecound = new ConcurrentCircularBuffer<long>(10);
        long _PreviousDownloadedSize;
        TimeSpan? _TimeRemaining = null;
        volatile int _Id;
        volatile bool _IsGetSize = false;
        volatile ConnectionStatus _ManualStatus = ConnectionStatus.None;
        volatile bool _IsComplete;
        volatile bool _IsManualError = false;
        DateTime _CreatedDateTime;
        DateTime _LastDownloadedDateTime;
        /// <summary>
        /// primery id of link
        /// </summary>
        public int Id { get => _Id; set => _Id = value; }

        /// <summary>
        /// string of id for ignore id.ToString method
        /// </summary>
        public string IdString
        {
            get
            {
                return Id.ToString();
            }
        }

        /// <summary>
        /// link were in error but need aa delay time to start
        /// </summary>
        public bool IsWaitForErrorTimeToPLay { get; set; }

        /// <summary>
        /// if connection is downloading
        /// </summary>
        public bool IsDownloading
        {
            get
            {
                return (Connections.Count == 0 && !IsManualStop) || Connections.Count(x => x.IsDownloading) > 0;
            }
        }

        /// <summary>
        /// mybe link is in error but time remaning need to download again
        /// </summary>
        public bool IsDownloadingInActual
        {
            get
            {
                return IsDownloading || IsWaitForErrorTimeToPLay;
            }
        }

        /// <summary>
        /// link is activee download or etc need to app server be run
        /// </summary>
        public bool IsNeedActiveInActual
        {
            get
            {
                return IsDownloadingInActual || IsCopyingFile || IsCompleting || isDestroying;
            }
        }

        /// <summary>
        /// if link can play in actual but not in this time
        /// </summary>
        public bool CanPlayInActual
        {
            get
            {
                return CanPlay || !IsComplete;
            }
        }

        /// <summary>
        /// when link is copying file
        /// </summary>
        public bool IsCopyingFile
        {
            get
            {
                return _IsCopyingFile;
            }
            private set
            {
                _IsCopyingFile = value;
                ValidateUI();
            }
        }

        /// <summary>
        /// when link rise of complete method one time
        /// </summary>
        public bool IsCompleting { get; set; }

        /// <summary>
        /// when link can do complete method
        /// </summary>
        public bool CanComplete
        {
            get
            {
                return (IsCompleting || (Connections.Count > 0 && Connections.Count(x => x.IsComplete) == Connections.Count)) && !isDestroying;
            }
        }

        /// <summary>
        /// when link is completed
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return _IsComplete;
            }
            set
            {
                _IsComplete = value;
                ValidateUI();
            }
        }

        /// <summary>
        /// when link is not completed
        /// </summary>
        public bool IsNotComplete
        {
            get
            {
                return !IsComplete;
            }
        }

        /// <summary>
        /// when link is copying file
        /// </summary>
        public bool IsError
        {
            get
            {
                return Connections.Count > 0 && (IsWaitForErrorTimeToPLay || _IsManualError || Connections.Count(x => x.IsError) == Connections.Count(x => !x.IsComplete));
            }
            set
            {
                _IsManualError = value;
                ValidateUI();
            }
        }

        /// <summary>
        /// can play
        /// </summary>
        public bool CanPlay
        {
            get
            {
                return IsManualStop && !IsCopyingFile && !IsComplete && !isDestroying;
            }
        }

        /// <summary>
        /// can stop
        /// </summary>
        public bool CanStop
        {
            get
            {
                return !CanPlay && !IsCopyingFile && !IsComplete || isDestroying;
            }
        }

        /// <summary>
        /// status of request info
        /// </summary>
        public ConnectionStatus Status
        {
            get
            {
                if (IsCopyingFile)
                    return ConnectionStatus.CopyingFile;
                else if (IsDownloading)
                {
                    if (Connections.Count(x => x.Status == ConnectionStatus.Downloading) > 0)
                        return ConnectionStatus.Downloading;
                    return ConnectionStatus.Connecting;
                }
                else if (IsComplete)
                    return ConnectionStatus.Complete;
                else if (IsError)
                    return ConnectionStatus.Error;
                else if (CanComplete && ValidateLinkCompletion())
                    return ConnectionStatus.Complete;
                else if (!IsManualStop)
                {
                    StringBuilder buildeer = new StringBuilder();
                    buildeer.AppendLine("Start What is status");
                    foreach (var item in Connections)
                    {
                        buildeer.AppendLine(item.Status.ToString());
                    }
                    buildeer.AppendLine("End What is status");
                    AutoLogger.LogText(buildeer.ToString());
                }
                return ConnectionStatus.Stoped;
            }
        }


        /// <summary>
        /// connections of this link
        /// </summary>
        public ConcurrentList<LinkInfoRequestCore> Connections
        {
            get
            {
                if (_Connections == null)
                    _Connections = new ConcurrentList<LinkInfoRequestCore>();
                return _Connections;
            }
            set => _Connections = value;
        }
        /// <summary>
        /// is manually stopped link by user
        /// </summary>
        public bool IsManualStop
        {
            get => _IsManualStop;
            set
            {
                _IsManualStop = value;
                ValidateUI();
            }
        }

        /// <summary>
        /// downloaded size of connection
        /// </summary>
        public long DownloadedSize
        {
            get
            {
                if (MixedSize > 0)
                    return MixedSize;
                return Connections.Sum(x => x.DownloadedSize);
            }
        }
        /// <summary>
        /// mixed size when link completing
        /// </summary>
        public long MixedSize
        {
            get
            {
                return _MixedSize;
            }
            set
            {
                _MixedSize = value;
                OnPropertyChanged(nameof(MixedSize));
                OnPropertyChanged(nameof(DownloadedSize));
            }
        }

        /// <summary>
        /// calculated downloaded size for download per time
        /// </summary>
        public long PreviousDownloadedSize
        {
            get
            {
                return Thread.VolatileRead(ref _PreviousDownloadedSize);
            }
            set
            {
                Thread.VolatileWrite(ref _PreviousDownloadedSize, value);
            }
        }

        /// <summary>
        /// downloaded size per one secound
        /// </summary>
        public long DownloadedSizePerSecound
        {
            get
            {
                return _DownloadedSizePerSecound;
            }
            set
            {
                _DownloadedSizePerSecound = value;
                OnPropertyChanged(nameof(DownloadedSizePerSecound));
            }
        }

        /// <summary>
        /// avarage downloaded size per one secound
        /// </summary>
        /// 
        public ConcurrentCircularBuffer<long> AvarageDownloadedSizePerSecound
        {
            get
            {
                return _AvarageDownloadedSizePerSecound;
            }
            set
            {
                _AvarageDownloadedSizePerSecound = value;
                OnPropertyChanged(nameof(AvarageDownloadedSizePerSecound));
            }
        }

        /// <summary>
        /// get percent of downloaded size by double value
        /// </summary>
        public double PercentDouble
        {
            get
            {
                if (IsComplete || Size == 0 || Size == -2)
                    return 1.0;
                else if (Size < 0)
                    return 0.0;
                return (double)DownloadedSize / Size;
            }
        }

        /// <summary>
        /// get percent of downloaded size
        /// </summary>
        public string Percent
        {
            get
            {
                if (IsComplete || Size == 0)
                    return "100%";
                else if (Size == -2)
                    return "0%";
                return Size < 0 ? ApplicationResourceBase.Current.GetAppResource("Unknown_Language") : String.Format("{0:00.00%}", PercentDouble);
            }
        }

        /// <summary>
        /// size of connection
        /// </summary>
        public long Size
        {
            get
            {
                return Thread.VolatileRead(ref _Size);
            }
            set
            {
                Thread.VolatileWrite(ref _Size, value);
                OnPropertyChanged(nameof(Size));
            }
        }
        /// <summary>
        /// if size geted from web request
        /// </summary>
        public bool IsGetSize
        {
            get => _IsGetSize; set
            {
                _IsGetSize = value;
                OnBasicDataChanged?.Invoke();
            }
        }

        public DateTime CreatedDateTime
        {
            get
            {
                return _CreatedDateTime;
            }
            set
            {
                _CreatedDateTime = value;
            }
        }

        /// <summary>
        /// index of celection for multi select
        /// </summary>
        public ushort SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                _SelectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public DateTime LastDownloadedDateTime
        {
            get
            {
                return _LastDownloadedDateTime;
            }
            set
            {
                _LastDownloadedDateTime = value;
            }
        }

        /// <summary>
        /// time remaining to complete link
        /// </summary>
        public TimeSpan? TimeRemaining
        {
            get
            {
                return _TimeRemaining;
            }

            set
            {
                _TimeRemaining = value;
                OnPropertyChanged(nameof(TimeRemaining));
            }
        }

        /// <summary>
        /// when link info selected by user
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
                if (!value)
                    SelectedIndex = 0;
            }
        }

        /// <summary>
        /// check if link is going to complete else create a new request for this
        /// </summary>
        /// <returns>if true link is complete</returns>
        internal bool ValidateLinkCompletion()
        {
            CreateRequestCoreIfNeeded();
            if (CanComplete)
            {
                if (Size >= 0 && DownloadedSize == Size)
                    return true;
            }
            return CanComplete;
        }

        public void CheckComplete()
        {
            if (CanComplete)
                Complete();
        }

        public static Action<LinkInfoShort> CompletedLinkAction { get; set; }
        FileMixer _doingMixer;
        internal void Complete()
        {
            this.RunInLock(() =>
            {
                var linkInfoShort = (LinkInfoShort)this;
                try
                {
                    if (IsCopyingFile)
                        return;
                    IsCompleting = true;
                    IsCopyingFile = true;
                    //تغییر برای بخش سکیوریتی اندروید
                    //if (string.IsNullOrEmpty(linkInfoShort.PathInfo.SecurityDirectorySavePath) && !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(linkInfoShort.PathInfo.FullSaveAddress)))
                    //    IOHelper.CreateDirectory(System.IO.Path.GetDirectoryName(linkInfoShort.PathInfo.FullSaveAddress));
                    MixedSize = 0;
                    //foreach (var item in Connections.ToArray())
                    //{
                    //    item.DownloadedSize = 0;
                    //}

                    List<string> files = new List<string>();

                    long fileLen = 0;

                    foreach (var item in LinkHelper.SortByPosition(Connections))
                    {
                        files.Add(item.SaveConnectionFileName);
                    }

                    //میکسر لینک که قبلاً ذخیره شده بود
                    var mixerInfo = MixerInfo.LoadAction(Id);//, linkInfoShort.PathInfo.BackUpMixerSavePath

                    MixerTypeEnum mixerType = MixerTypeEnum.NoSpace;

                    if (mixerInfo == null)
                    {
                        if (System.IO.File.Exists(linkInfoShort.PathInfo.FullSaveAddress))
                        {
                            using (var fs = IOHelperBase.OpenFileStreamForWrite(linkInfoShort.PathInfo.DirectorySavePath, System.IO.FileMode.OpenOrCreate, fileName: linkInfoShort.PathInfo.FileName))
                            {
                                fs.SetLength(0);
                            }
                        }
                        mixerType = MixerInfo.GenerateAutoMixerByDriveSizeAction(linkInfoShort.PathInfo.DirectorySavePath, (long)linkInfoShort.Size, files);
                        if (mixerType == MixerTypeEnum.NoSpace)
                            throw new Exception("فضای کافی برای ذخیره سازی بر روی دیسک خود ندارید. حداقل فضای کافی برای تکمیل سازی فایل 10 مگابایت می باشد.");
                        mixerInfo = new MixerInfoBase() { MixerType = mixerType, LinkId = Id };//.InstanceMixerByTypeAction(mixerType, linkInfoShort.PathInfo.MixerSavePath, null);//linkInfoShort.PathInfo.MixerBackupSavePath
                    }
                    else
                        mixerType = mixerInfo.MixerType;
                    //موتور میکس کننده
                    _doingMixer = FileMixer.GetMixerByTypeAction(mixerType, files);
                    mixerInfo.FilePath = linkInfoShort.PathInfo.DirectorySavePath;
                    mixerInfo.FileName = linkInfoShort.PathInfo.FileName;
                    if (isDestroying)
                        return;
                    _doingMixer.OnChangedDataAction = () =>
                    {
                        MixedSize = _doingMixer.MixedSize;
                        ValidateUI();
                    };


                    _doingMixer.Start(mixerInfo);
                    if (isDestroying)
                        return;
                    MixedSize = fileLen = _doingMixer.Size;

                    if (fileLen != linkInfoShort.Size && linkInfoShort.Size > 0)
                    {
                        throw new Exception("File length is not true! try again.");
                    }

                    IsComplete = true;
                    IsCompleting = false;
                    CompletedLinkAction?.Invoke(this.AsShort());
                    //linkInfoShort.PathInfo.UserDirectorySavePath = PathInfo.SavePath;
                    //linkInfoShort.LinkInfoDownloadCore.SpeedByteDownloaded = 0;
                    //string bkFileName = MPath.CreateOneFileByAddress(MPath.BackUpCompleteLinksPath);
                    //PathInfo.BackUpCompleteAddress = System.IO.Path.Combine(MPath.BackUpCompleteLinksPath, bkFileName);
                    //SerializeData.SaveLinkInfoDataToFile(this, bkFileName, MPath.BackUpCompleteLinksPath, true);
                    //SerializeData.SaveLinkInfoesToFile();
                    //ApplicationNotificationManager.Current.Add(NotificationMode.Complete, this);

                    //if (FinishAction != null)
                    //{
                    //    Action fin = FinishAction(this);
                    //    if (fin != null)
                    //        AsyncActions.Action(fin);
                    //}

                    //Destruction();
                    //if (Management.IsEndDownload)
                    //    ApplicationHelperBase.SetShutDown((int)Management.EndDownloadSystemMode);

                }
                catch (Exception e)
                {
                    AutoLogger.LogError(e, $"Complete Error {isDestroying}");
                    IsCopyingFile = false;
                    IsError = true;
                    linkInfoShort.LinkInfoManagementCore.AddError(e);
                    if (e is System.IO.IOException)
                    {
                        //ManualStop(true);
                    }
                    else
                    {
                        //DownloadingProperty.ErrorAction?.Invoke();
                    }
                    Stop();
                }
                finally
                {
                    if (!isDestroying)
                    {
                        IsCopyingFile = false;
                        Save();
                    }
                }
#if (MobileApp)
                GC.Collect();
#else

                GC.Collect(5, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                GC.Collect(5, GCCollectionMode.Forced);
#endif

            });
        }


        /// <summary>
        ///  create new request core
        /// </summary>
        /// <returns></returns>
        internal LinkInfoRequestCore CreateNewRequestCore()
        {
            return this.RunInLock(() =>
            {
                if (isDestroying)
                    return null;
                var maximumId = Connections.Select(x => x.Id).DefaultIfEmpty((ushort)0).Max();
                maximumId++;
                LinkInfoRequestCore requestCore = LinkInfoRequestCore.Instance();
                requestCore.Id = maximumId;
                var parent = (ShortModels.Link.LinkInfoShort)this;
                requestCore.LinkInfo = parent;

                LinkInfoRequestCore max = null;
                if (Connections.Count > 0 && parent.LinkInfoDownloadCore.ResumeCapability == ResumeCapabilityEnum.Yes)
                {
                    max = Connections.MaxBy(x => x.Length - x.DownloadedSize);

                    long downSizeMande = max.Length - max.DownloadedSize;
                    long size = (downSizeMande / 2);
                    max.EndPosition = max.StartPosition + max.DownloadedSize + size + (downSizeMande % 2);
                    requestCore.StartPosition = max.EndPosition;
                    requestCore.EndPosition = max.EndPosition + size;

                }
                else
                {

                }
                Logger.WriteLine("CreateNewRequestCore", $"{requestCore.StartPosition} , {requestCore.EndPosition}");

                Connections.Add(requestCore);
                Save();
                return requestCore;
            });
        }

        internal void CreateRequestCoreIfNeeded()
        {
            this.RunInLock(() =>
            {
                if (isDestroying)
                    return;
                if (!IsDownloading || IsCompleting)
                    return;
                if (Connections.Count > 0 && ((ShortModels.Link.LinkInfoShort)this).LinkInfoDownloadCore.ResumeCapability != ResumeCapabilityEnum.Yes)
                {
                    var connection = Connections.First();
                    if (isDestroying)
                        return;
                    if (connection.CanPlay)
                        connection.Play();
                    return;
                }
                var completeCount = Connections.Count(x => x.IsComplete);
                bool cannewwhenEnd = Connections.Count == 0 || (Size - DownloadedSize) > 1024 * 1024 || completeCount == Connections.Count;
                var canAddConection = cannewwhenEnd && (DownloadedSize < Size || !IsGetSize) && !Connections.Any(x => x.EndPosition < 0);
                if (canAddConection && (Connections.Count - completeCount) < ((ShortModels.Link.LinkInfoShort)this).LinkInfoDownloadCore.GetConcurrentConnectionCount())
                {
                    var connection = CreateNewRequestCore();
                    if (isDestroying)
                        return;
                    connection?.Play();
                }
                ValidateUI();
            });
        }

        internal void ValidateConnectionsToDownload()
        {
            if (!IsManualStop)
            {
                var connections = Connections.ToArray();
                var downloadingCount = connections.Count(x => x.IsDownloading);
                var takeCount = AsShort().LinkInfoDownloadCore.GetConcurrentConnectionCount() - downloadingCount;
                if (takeCount > 0)
                {
                    foreach (var item in connections.Where(x => x.CanPlay).Take(takeCount))
                    {
                        item.Play();
                    }
                }
            }
        }

        /// <summary>
        /// play the Link
        /// </summary>
        public void Play()
        {
            this.RunInLock(() =>
            {
                if (!CanPlay)
                    return;
                _IsManualError = false;
                IsManualStop = false;
                if (Connections.ToArray().Count(x => x.CanPlay) > 0 && !IsCompleting)
                {
                    foreach (var item in Connections.Where(x => x.CanPlay).Take(((ShortModels.Link.LinkInfoShort)this).LinkInfoDownloadCore.GetConcurrentConnectionCount()))
                    {
                        item.Play();
                    }
                }
                else if (CanComplete && ValidateLinkCompletion() || IsCompleting)
                {
                    CheckComplete();
                }
                else
                {
                    CreateRequestCoreIfNeeded();
                }
            });
        }

        /// <summary>
        /// stop link
        /// </summary>
        public void Stop()
        {
            this.RunInLock(() =>
            {
                if (!CanStop)
                    return;
                if (Connections.Count(x => x.CanStop) > 0)
                {
                    foreach (var item in Connections.Where(x => x.CanStop).Take(((ShortModels.Link.LinkInfoShort)this).LinkInfoDownloadCore.GetConcurrentConnectionCount()))
                    {
                        item.Stop();
                    }
                }
                IsManualStop = true;
            });
        }

        public static Action LinkValidatedAction { get; set; }


        public void ValidateUI()
        {
            OnPropertyChanged(nameof(IsDownloading));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(IsError));
            OnPropertyChanged(nameof(IsComplete));
            OnPropertyChanged(nameof(IsCopyingFile));
            OnPropertyChanged(nameof(IsManualStop));
            OnPropertyChanged(nameof(CanPlay));
            OnPropertyChanged(nameof(CanStop));
            OnPropertyChanged(nameof(Percent));
            OnPropertyChanged(nameof(DownloadedSize));
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(IsNotComplete));
            OnPropertyChanged(nameof(PercentDouble));
            LinkValidatedAction?.Invoke();
        }

        volatile bool isDestroying = false;
        public void Distroy()
        {
            isDestroying = true;
            Stop();
            if (_doingMixer != null)
                _doingMixer.Dispose();
            Dispose();
        }

        /// <summary>
        /// save link to file
        /// </summary>
        public void Save()
        {
            DataBaseFoundation<LinkInfoCore>.Current.Update(this);
        }

        internal void NotResumableSupport()
        {
            //ApplicationLinkInfoManager.Current.AddNewLinkInfoToNotSupportResumable(this);
            //throw new Exception("Resumable Not Support");
        }
        /// <summary>
        /// dispose linkinfo
        /// </summary>
        public override void Dispose()
        {
            var shortLink = (LinkInfoShort)this;
            shortLink.LinkInfoManagementCore.Dispose();
            shortLink.LinkInfoDownloadCore.Dispose();
            shortLink.Properties.Dispose();
            shortLink.PathInfo.Dispose();
            base.Dispose();
        }

        public static LinkInfoCore CreateInstance(string url)
        {
#if (Debug || Release)
            var link = new LinkInfo();
#elif (MobileDebug || MobileRelease)
            var link = new LinkInfoShort();
#endif
            link.PathInfo.MainUriAddress = url;
            return link;
        }

        public LinkInfoShort AsShort()
        {
            return (LinkInfoShort)this;
        }
    }
}
