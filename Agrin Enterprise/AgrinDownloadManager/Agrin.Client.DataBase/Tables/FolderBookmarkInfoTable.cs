using Agrin.Client.DataBase.Models;
using Agrin.Foundation;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Agrin.Client.DataBase.Tables
{
    public class FolderBookmarkInfoTable : DataBaseFoundation<FolderBookmarkInfo>
    {
        public FolderBookmarkInfoTable()
        {
            if (Current != null)
                throw new NotSupportedException("you cannot create database table two time!");
            Current = this;
        }

        public override void Add(FolderBookmarkInfo data)
        {
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<FolderBookmarkInfo>("FolderBookmarkInfoes");
                links.EnsureIndex(x => x.Id);
                links.Insert(data);
            }
        }

        public override void Delete(FolderBookmarkInfo data)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<FolderBookmarkInfo>("FolderBookmarkInfoes");
                links.Delete(x => x.Id == data.Id);
            }
        }

        public override List<TResult> GetList<TResult>()
        {
            throw new NotImplementedException();
        }

        public override void Initialize<T>()
        {
            Initialize();
        }

        public override void Initialize()
        {

        }

        public override FolderBookmarkInfo FindItem(Expression<Func<FolderBookmarkInfo, bool>> predicate)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var items = db.GetCollection<FolderBookmarkInfo>("FolderBookmarkInfoes");
                return items.FindOne(predicate);
            }
        }

        public override void Update(FolderBookmarkInfo data)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<FolderBookmarkInfo>("FolderBookmarkInfoes");
                links.Update(data);
            }
        }

        public override List<FolderBookmarkInfo> GetList()
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var items = db.GetCollection<FolderBookmarkInfo>("FolderBookmarkInfoes");
                return items.FindAll().ToList();
            }
        }

        public override void Delete(Expression<Func<FolderBookmarkInfo, bool>> predicate)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<FolderBookmarkInfo>("FolderBookmarkInfoes");
                links.Delete(predicate);
            }
        }
    }
}
