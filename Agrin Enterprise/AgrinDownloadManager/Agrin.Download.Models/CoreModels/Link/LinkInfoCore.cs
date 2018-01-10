﻿using Agrin.ComponentModels;
using Agrin.Download.EntireModels.Link;
using Agrin.Download.ShortModels.Link;
using Agrin.Download.Web;
using Agrin.Foundation;
using Agrin.IO.Helpers;
using Agrin.IO.Mixer;
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
        volatile ConcurrentList<LinkInfoRequestCore> _Connections = null;
        volatile bool _IsCopyingFile = false;
        volatile bool _IsManualStop = true;
        long _Size = -2;//-2 = not get download size and -1 = dont know what is download size
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
            }
        }

        /// <summary>
        /// when link can do complete method
        /// </summary>
        public bool CanComplete
        {
            get
            {
                return Connections.Count(x => x.IsComplete) == Connections.Count;
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
                OnPropertyChanged(nameof(IsComplete));
            }
        }

        /// <summary>
        /// when link is copying file
        /// </summary>
        public bool IsError
        {
            get
            {
                return _IsManualError || Connections.Count(x => x.IsError) == Connections.Count;
            }
            set
            {
                _IsManualError = value;
            }
        }

        /// <summary>
        /// can play
        /// </summary>
        public bool CanPlay
        {
            get
            {
                return IsManualStop && !IsCopyingFile;
            }
        }

        /// <summary>
        /// can stop
        /// </summary>
        public bool CanStop
        {
            get
            {
                return !CanPlay && !IsCopyingFile;
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
                else if (IsError)
                    return ConnectionStatus.Error;
                else if (CanComplete && ValidateLinkCompletion())
                    return ConnectionStatus.Complete;

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
                OnPropertyChanged("IsManualStop");
            }
        }

        /// <summary>
        /// downloaded size of connection
        /// </summary>
        public long DownloadedSize
        {
            get
            {
                return Connections.Sum(x => x.DownloadedSize);
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
            }
        }
        /// <summary>
        /// if size geted from web request
        /// </summary>
        public bool IsGetSize { get => _IsGetSize; set => _IsGetSize = value; }

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

        internal void Complete()
        {
            this.RunInLock(() =>
            {
                var linkInfoShort = (LinkInfoShort)this;
                try
                {
                    if (IsCopyingFile)
                        return;
                    IsCopyingFile = true;
                    //تغییر برای بخش سکیوریتی اندروید
                    //if (string.IsNullOrEmpty(linkInfoShort.PathInfo.SecurityDirectorySavePath) && !System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(linkInfoShort.PathInfo.FullSaveAddress)))
                    //    IOHelper.CreateDirectory(System.IO.Path.GetDirectoryName(linkInfoShort.PathInfo.FullSaveAddress));

                    foreach (var item in Connections.ToArray())
                    {
                        item.DownloadedSize = 0;
                    }

                    List<string> files = new List<string>();

                    long fileLen = 0;

                    foreach (var item in LinkHelper.SortByPosition(Connections))
                    {
                        files.Add(item.SaveConnectionFileName);
                    }

                    //میکسر لینک که قبلاً ذخیره شده بود
                    MixerInfo mixerInfo = MixerInfo.LoadFromFile(linkInfoShort.PathInfo.MixerSavePath, linkInfoShort.PathInfo.SecurityMixerSavePath, null);//, linkInfoShort.PathInfo.BackUpMixerSavePath

                    MixerTypeEnum mixerType = MixerTypeEnum.NoSpace;

                    if (mixerInfo == null)
                    {
                        if (System.IO.File.Exists(linkInfoShort.PathInfo.FullSaveAddress))
                        {
                            using (var fs = IOHelper.OpenFileStreamForWrite(linkInfoShort.PathInfo.DirectorySavePath, System.IO.FileMode.OpenOrCreate, fileName: linkInfoShort.PathInfo.FileName))
                            {
                                fs.SetLength(0);
                            }
                        }
                        mixerType = MixerInfo.GenerateAutoMixerByDriveSize(linkInfoShort.PathInfo.FullSaveAddress, (long)linkInfoShort.Size, files);
                        if (mixerType == MixerTypeEnum.NoSpace)
                            throw new Exception("فضای کافی برای ذخیره سازی بر روی دیسک خود ندارید. حداقل فضای کافی برای تکمیل سازی فایل 10 مگابایت می باشد.");
                        mixerInfo = MixerInfo.InstanceMixerByType(mixerType, linkInfoShort.PathInfo.MixerSavePath, null);//linkInfoShort.PathInfo.MixerBackupSavePath
                    }
                    else
                        mixerType = mixerInfo.MixerType;
                    //موتور میکس کننده
                    FileMixer mixer = FileMixer.GetMixerByType(mixerType, files);
                    mixerInfo.FilePath = linkInfoShort.PathInfo.DirectorySavePath;
                    mixerInfo.FileName = linkInfoShort.PathInfo.FileName;

                    mixer.OnChangedDataAction = () =>
                    {
                        //linkInfoShort.DownloadedSize = mixer.MixedSize;
                    };


                    mixer.Start(mixerInfo);

                    fileLen = mixer.Size;

                    if (fileLen != linkInfoShort.Size && linkInfoShort.Size > 0)
                    {
                        throw new Exception("File length is not true! try again.");
                    }
                    IsComplete = true;
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
                    AutoLogger.LogError(e, "Complete Error");
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
                }
                finally
                {
                    IsCopyingFile = false;
                    Save();
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
                if (!IsDownloading)
                    return;
                if (Connections.Count > 0 && ((ShortModels.Link.LinkInfoShort)this).LinkInfoDownloadCore.ResumeCapability != ResumeCapabilityEnum.Yes)
                {
                    var connection = Connections.First();
                    if (connection.CanPlay)
                        connection.Play();
                    return;
                }
                var completeCount = Connections.Count(x => x.IsComplete);
                bool cannewwhenEnd = Connections.Count == 0 || (Size - DownloadedSize) > 1024 * 1024 || completeCount == Connections.Count;
                var canAddConection = cannewwhenEnd && (DownloadedSize < Size || !IsGetSize);
                if (canAddConection && (Connections.Count - completeCount) < ((ShortModels.Link.LinkInfoShort)this).LinkInfoDownloadCore.GetConcurrentConnectionCount())
                {
                    var connection = CreateNewRequestCore();
                    connection.Play();
                }
            });
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
                IsManualStop = false;
                if (Connections.Count(x => x.CanPlay) > 0)
                {
                    foreach (var item in Connections.Where(x => x.CanPlay).Take(((ShortModels.Link.LinkInfoShort)this).LinkInfoDownloadCore.GetConcurrentConnectionCount()))
                    {
                        item.Play();
                    }
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
            //lock (ObjectLocker.TakeAnObject(this))
            //{

            //}
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
    }
}
