using Agrin.Download.CoreModels.Link;
using Agrin.Foundation;
using Agrin.Models.Serialization.Link;
using CrazyMapper;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Agrin.Client.DataBase
{
    public class LinkInfoTable : DataBaseFoundation<LinkInfoCore>
    {
        public LinkInfoTable()
        {
            if (Current != null)
                throw new NotSupportedException("you cannot create database table two time!");
            Current = this;
        }

        public override void Update(LinkInfoCore linkInfoCore)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<LinkInfoSerialization>("LinkInfoes");
                var item = Mapper.Map<LinkInfoSerialization>(linkInfoCore);
                links.Update(item);
            }
        }

        public override void Initialize<T>() 
        {
            var links = GetList<T>();
            foreach (var item in links)
            {
                AgrinClientContext.MainLoadedLinkInfoes.Add(item);
            }
            AgrinClientContext.LinkInfoes = new ObservableCollection<LinkInfoCore>(AgrinClientContext.MainLoadedLinkInfoes.OrderBy(x => x.LastDownloadedDateTime));
        }

        public override void Add(LinkInfoCore linkInfoCore)
        {
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<LinkInfoSerialization>("LinkInfoes");
                var item = Mapper.Map<LinkInfoSerialization>(linkInfoCore);
                links.EnsureIndex(x => x.Id);
                links.Insert(item);
                linkInfoCore.Id = item.Id;
                AgrinClientContext.LinkInfoes.Insert(0, linkInfoCore);
                AgrinClientContext.MainLoadedLinkInfoes.Add(linkInfoCore);
            }
        }

        public override List<T> GetList<T>()
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<LinkInfoSerialization>("LinkInfoes");
                return AgrinClientContext.MapList<LinkInfoSerialization, T>(links.FindAll().ToList());
            }
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
