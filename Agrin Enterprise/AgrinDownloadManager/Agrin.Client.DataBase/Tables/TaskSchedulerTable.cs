using Agrin.Download.CoreModels.Task;
using Agrin.Foundation;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Agrin.Client.DataBase.Tables
{
    public class TaskSchedulerTable : DataBaseFoundation<TaskSchedulerInfo>
    {
        public TaskSchedulerTable()
        {
            if (Current != null)
                throw new NotSupportedException("you cannot create database table two time!");
            Current = this;
        }

        public override void Add(TaskSchedulerInfo data)
        {
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<TaskSchedulerInfo>("TaskSchedulerInfoes");
                links.EnsureIndex(x => x.Id);
                links.Insert(data);
                AgrinClientContext.TaskSchedulerInfoes.Insert(0, data);
            }
        }

        public override void Delete(TaskSchedulerInfo data)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<TaskSchedulerInfo>("TaskSchedulerInfoes");
                data.IsDeleted = true;
                links.Update(data);
                AgrinClientContext.TaskSchedulerInfoes.Remove(data);
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

        bool oneTimeLoaded = false;
        public override void Initialize()
        {
            if (oneTimeLoaded)
                return;
            oneTimeLoaded = true;
            var links = GetList();
            foreach (var item in links.OrderByDescending(x => x.Id))
            {
                AgrinClientContext.TaskSchedulerInfoes.Add(item);
            }
        }

        public override void Update(TaskSchedulerInfo data)
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<TaskSchedulerInfo>("TaskSchedulerInfoes");
                links.Update(data);
            }
        }

        public override List<TaskSchedulerInfo> GetList()
        {
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var links = db.GetCollection<TaskSchedulerInfo>("TaskSchedulerInfoes");
                return links.Find(x => !x.IsDeleted).Where(x => x.Status == TaskStatus.Stopped && x.StartDateTime != null).ToList();
            }
        }

        public override TaskSchedulerInfo FindItem(Expression<Func<TaskSchedulerInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Expression<Func<TaskSchedulerInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
