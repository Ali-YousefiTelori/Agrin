using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Client.DataBase.Models
{
    public class SettingInfo
    {
        [BsonId]
        public int Id { get; set; }
        public string Key { get; set; }
        public string JsonValue { get; set; }
    }
}
