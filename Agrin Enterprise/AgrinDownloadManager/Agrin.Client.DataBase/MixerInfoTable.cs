using Agrin.Download.Mixers;
using Agrin.Foundation;
using Agrin.IO.HardWare;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Client.DataBase
{
    public class MixerInfoTable : DataBaseFoundation<MixerInfoBase>
    {
        public MixerInfoTable()
        {
            if (Current != null)
                throw new NotSupportedException("you cannot create database table two time!");
            Current = this;
            FileMixer.GetMixerByTypeAction = GetMixerByType;
            MixerInfo.GenerateAutoMixerByDriveSizeAction = GenerateAutoMixerByDriveSize;
            MixerInfo.LoadAction = LoadByLinkId;
            MixerInfo.SaveAction = Update;

        }

        public override void Update(MixerInfoBase mixerInfoBase)
        {
            if (mixerInfoBase.Id == 0)
                Add(mixerInfoBase);
            else
            {
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var mixers = db.GetCollection<MixerInfoBase>("MixerInfoes");
                    mixers.Update(mixerInfoBase);
                }
            }
        }


        public override void Add(MixerInfoBase mixerInfoBase)
        {
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<MixerInfoBase>("MixerInfoes");
                links.EnsureIndex(x => x.Id);
                links.Insert(mixerInfoBase);
            }
        }

        public MixerInfoBase LoadByLinkId(int linkId)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<MixerInfoBase>("MixerInfoes");
                var find = links.FindOne(x => x.LinkId == linkId);
                return find;
            }
        }


        public override void Initialize<TResult>()
        {
            throw new NotImplementedException();
        }

        public override void Delete(MixerInfoBase data)
        {
            throw new NotImplementedException();
        }

        public override List<TResult> GetList<TResult>()
        {
            throw new NotImplementedException();
        }

        public static FileMixer GetMixerByType(MixerTypeEnum type, List<string> files)
        {
            if (type == MixerTypeEnum.Normal)
                return new FileNormalMixer(files);
            else if (type == MixerTypeEnum.Deleter)
                return new FileDeleterMixer(files);
            else if (type == MixerTypeEnum.Revercer)
                return new FileRevercerMixer(files);
            return null;
        }
        

        public static MixerTypeEnum GenerateAutoMixerByDriveSize(string savePath, long fileSize, List<string> partOfFiles)
        {
            try
            {
                var freeSpace = DriveHelperBase.Current.GetFreeSizeByPath(savePath);

                if (FileNormalMixer.CanStartByThisMixer(fileSize, freeSpace))
                    return MixerTypeEnum.Normal;
                else if (FileDeleterMixer.CanStartByThisMixer(partOfFiles, freeSpace, DriveHelperBase.Current.GetRootPath(partOfFiles.First()), DriveHelperBase.Current.GetRootPath(savePath)))
                    return MixerTypeEnum.Deleter;
                else if (FileRevercerMixer.CanStartByThisMixer(freeSpace, DriveHelperBase.Current.GetRootPath(partOfFiles.First()), DriveHelperBase.Current.GetRootPath(savePath)))
                    return MixerTypeEnum.Revercer;
                return MixerTypeEnum.NoSpace;
            }
            catch (Exception ex)
            {
                return MixerTypeEnum.Normal;
            }
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override List<MixerInfoBase> GetList()
        {
            throw new NotImplementedException();
        }
        //static List<T> GetLinkInfoes<T>(Expression<Func<LinkInfoSerialization, bool>> query)
        //{
        //    using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
        //    {
        //        var links = db.GetCollection<LinkInfoSerialization>("LinkInfoes");
        //        TinyMapper.Bind(typeof(LinkInfoSerialization), typeof(T));

        //        return AgrinClientContext.MapList<LinkInfoSerialization, T>(links.Find(query).ToList());
        //    }
        //}
    }
}
