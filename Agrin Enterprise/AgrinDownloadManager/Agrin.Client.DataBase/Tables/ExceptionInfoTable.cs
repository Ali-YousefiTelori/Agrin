using Agrin.Download.CoreModels.Link;
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
            lock (AgrinClientContext.LockOBJ)
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
        }

        public List<ExceptionInfo> GetExceptionsByLinkId(int linkId)
        {
            lock (AgrinClientContext.LockOBJ)
            {
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var erros = db.GetCollection<ExceptionInfo>("ExceptionInfoes");
                    return erros.Find(x => x.LinkId == linkId).ToList();
                }
            }
        }

        public ExceptionInfo GetExceptionByLinkId(int linkId)
        {
            lock (AgrinClientContext.LockOBJ)
            {
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var erros = db.GetCollection<ExceptionInfo>("ExceptionInfoes");
                    return erros.Find(x => x.LinkId == linkId).OrderByDescending(x => x.LastDateTimeErrorDetected).FirstOrDefault();
                }
            }
        }

        public override void Delete(ExceptionInfo data)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Expression<Func<ExceptionInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override ExceptionInfo FindItem(Expression<Func<ExceptionInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override List<TResult> GetList<TResult>()
        {
            throw new NotImplementedException();
        }

        public override List<ExceptionInfo> GetList()
        {
            lock (AgrinClientContext.LockOBJ)
            {
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var erros = db.GetCollection<ExceptionInfo>("ExceptionInfoes");
                    return erros.FindAll().ToList();
                }
            }
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
