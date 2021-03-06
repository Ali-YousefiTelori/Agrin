using Agrin.Download.DataBaseModels;
using Agrin.Foundation;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Agrin.Client.DataBase.Tables
{
    public class AddRangePositionInfoTable : DataBaseFoundation<AddRangePositionInfo>
    {
        public AddRangePositionInfoTable()
        {
            if (Current != null)
                throw new NotSupportedException("you cannot create database table two time!");
            Current = this;
        }

        public override void Add(AddRangePositionInfo addRangePositionInfo)
        {
            lock (AgrinClientContext.LockOBJ)
            {
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var positions = db.GetCollection<AddRangePositionInfo>("AddRangePositionInfoes");
                    positions.Insert(addRangePositionInfo);
                }
            }
        }

        public override void Delete(AddRangePositionInfo data)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Expression<Func<AddRangePositionInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override AddRangePositionInfo FindItem(Expression<Func<AddRangePositionInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override List<TResult> GetList<TResult>()
        {
            throw new NotImplementedException();
        }

        public override List<AddRangePositionInfo> GetList()
        {
            throw new NotImplementedException();
        }

        public override void Initialize<TResult>()
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Update(AddRangePositionInfo data)
        {
            throw new NotImplementedException();
        }
    }
}