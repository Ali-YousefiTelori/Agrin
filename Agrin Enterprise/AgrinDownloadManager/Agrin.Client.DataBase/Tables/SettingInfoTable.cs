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
    public class SettingInfoTable : DataBaseFoundation<SettingInfo>
    {
        public SettingInfoTable()
        {
            if (Current != null)
                throw new NotSupportedException("you cannot create database table two time!");
            Current = this;
        }

        public override void Add(SettingInfo data)
        {
            lock (AgrinClientContext.LockOBJ)
            {
                // Open database (or create if not exits)
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var items = db.GetCollection<SettingInfo>("SettingInfoes");
                    var find = items.FindOne(x => x.Key == data.Key);
                    items.EnsureIndex(x => x.Id);
                    items.EnsureIndex(x => x.Key);
                    if (find == null)
                        items.Insert(data);
                    else
                    {
                        find.JsonValue = data.JsonValue;
                        items.Update(find);
                    }
                }
            }
        }

        public override void Delete(SettingInfo data)
        {
            lock (AgrinClientContext.LockOBJ)
            {
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var links = db.GetCollection<SettingInfo>("SettingInfoes");
                    links.Delete(x => x.Id == data.Id);
                }
            }
        }

        public override List<TResult> GetList<TResult>()
        {
            throw new NotImplementedException();
        }

        public override void Initialize<T>()
        {
        }

        public override void Update(SettingInfo data)
        {
            lock (AgrinClientContext.LockOBJ)
            {
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var links = db.GetCollection<SettingInfo>("SettingInfoes");
                    links.Update(data);
                }
            }
        }

        public T GetValue<T>(string key) where T : class
        {
            lock (AgrinClientContext.LockOBJ)
            {
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var links = db.GetCollection<SettingInfo>("SettingInfoes");
                    var find = links.FindOne(x => x.Key == key);
                    if (find == null)
                        return null;
                    else
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(find.JsonValue, new Newtonsoft.Json.JsonSerializerSettings()
                        {
                            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                            Error = (e, s) =>
                            {

                            }
                        });
                }
            }
        }

        public override List<SettingInfo> GetList()
        {
            lock (AgrinClientContext.LockOBJ)
            {
                using (var db = new LiteDatabase(AgrinClientContext.DataBaseFilePath))
                {
                    var links = db.GetCollection<SettingInfo>("SettingInfoes");
                    return links.FindAll().ToList();
                }
            }
        }

        public override SettingInfo FindItem(Expression<Func<SettingInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Expression<Func<SettingInfo, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {

        }
    }
}
