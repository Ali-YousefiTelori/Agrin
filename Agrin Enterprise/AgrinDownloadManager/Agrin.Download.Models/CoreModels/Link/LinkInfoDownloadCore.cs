using Agrin.ComponentModels;
using Agrin.Download.ShortModels.Link;
using Agrin.IO;
using Agrin.IO.Helpers;
using Agrin.Log;
using Agrin.Models;
using Agrin.Models.Settings;
using Agrin.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.Download.CoreModels.Link
{
    /// <summary>
    /// core of downloading link info
    /// </summary>
    public abstract class LinkInfoDownloadCore : NotifyPropertyChanged
    {
        volatile LinkInfoShort _LinkInfo;
        volatile ResumeCapabilityEnum _ResumeCapability = ResumeCapabilityEnum.Unknown;
        byte? _ConcurrentConnectionCount = null;
        /// <summary>
        /// link info of this properties
        /// </summary>
        public LinkInfoShort LinkInfo { get => _LinkInfo; set => _LinkInfo = value; }
        /// <summary>
        /// support resumable for link
        /// </summary>
        public ResumeCapabilityEnum ResumeCapability
        {
            get => _ResumeCapability; set
            {
                _ResumeCapability = value;
                LinkInfo.OnBasicDataChanged?.Invoke();
            }
        }

        /// <summary>
        /// linkinfo playing maximum connection count
        /// </summary>
        public byte? ConcurrentConnectionCount
        {
            get
            {
                return _ConcurrentConnectionCount;
            }
            set
            {
                if (value == 0)
                    throw new Exception("Connection count cannot be zero!");
                _ConcurrentConnectionCount = value;
            }
        }


        public byte GetConcurrentConnectionCount()
        {
            if (_ConcurrentConnectionCount == null)
                return ApplicationSettingsInfo.Current.SpeedSettingInfo.MaximumConnectionCount;
            return _ConcurrentConnectionCount.Value;
        }

        public static Action<int,long> AddStartedRangePositionAction { get; set; }

        /// <summary>
        /// add Started Range Position
        /// </summary>
        /// <param name="position"></param>
        public void AddStartedRangePosition(long position)
        {
            this.RunInLock(() =>
            {
                AddStartedRangePositionAction?.Invoke(LinkInfo.Id, position);
                    //string errorsFileName = "";
                    //if (!string.IsNullOrEmpty(LinkInfo.PathInfo.SecurityTemporarySavePath))
                    //    errorsFileName = PathHelper.CombineSecurityPathWithNoSecurity(LinkInfo.PathInfo.TemporarySavePath, "RangePositions.agn");
                    //else
                    //    errorsFileName = PathHelper.Combine(LinkInfo.PathInfo.TemporarySavePath, "RangePositions.agn");

                //List<long> items = new List<long>();
                //try
                //{
                //    if (File.Exists(errorsFileName))
                //    {
                //        using (var reader = new StreamReader(IOHelperBase.OpenFileStreamForRead(errorsFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                //        {
                //            var oldItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(reader.ReadToEnd());
                //            if (oldItems != null)
                //                items.AddRange(oldItems);
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    AutoLogger.LogError(ex, "AddStartedRangePosition");
                //}

                //items.Add(position);

                //try
                //{
                //    using (var writer = IOHelperBase.OpenFileStreamForWrite(errorsFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite).GetStream())
                //    {
                //        var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                //        var bytes = Encoding.ASCII.GetBytes(serialized);
                //        writer.Write(bytes, 0, bytes.Length);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    AutoLogger.LogError(ex, "AddStartedRangePosition 2");
                //}
            });
        }
    }
}
