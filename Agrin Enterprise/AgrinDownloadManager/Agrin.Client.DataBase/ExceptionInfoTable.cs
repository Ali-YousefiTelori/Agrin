using Agrin.Download.CoreModels.Link;
using Agrin.Download.DataBaseModels;
using Agrin.Foundation;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Client.DataBase
{
    public class ExceptionInfoTable : DataBaseFoundation<ExceptionInfo>
    {
        public ExceptionInfoTable()
        {
            if (Current != null)
                throw new NotSupportedException("you cannot create database table two time!");
            Current = this;
        }

        public override void Add(ExceptionInfo exceptionInfo)
        {
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
            {
                exceptionInfo.HashCode = exceptionInfo.CalculateHash();

                var erros = db.GetCollection<ExceptionInfo>("ExceptionInfoes");
                var find = erros.FindOne(x => x.LinkId == exceptionInfo.LinkId && x.HashCode == exceptionInfo.HashCode);
                erros.EnsureIndex(x => x.Id);
                if (find == null)
                {
                    erros.Insert(exceptionInfo);
                }
                else
                {
                    find.CountOfError++;
                    find.LastDateTimeErrorDetected = exceptionInfo.LastDateTimeErrorDetected;
                    erros.Update(find);

                }
            }
        }

        public override void Delete(ExceptionInfo data)
        {
            throw new NotImplementedException();
        }

        public override List<TResult> GetList<TResult>()
        {
            throw new NotImplementedException();
        }

        public override List<ExceptionInfo> GetList()
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

        public override void Update(ExceptionInfo data)
        {
            throw new NotImplementedException();
        }
    }
}
