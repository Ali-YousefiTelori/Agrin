using Agrin.Download.DataBaseModels;
using Agrin.Foundation;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Client.DataBase
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
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                var positions = db.GetCollection<AddRangePositionInfo>("AddRangePositionInfoes");
                positions.Insert(addRangePositionInfo);
            }
        }

        public override void Delete(AddRangePositionInfo data)
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