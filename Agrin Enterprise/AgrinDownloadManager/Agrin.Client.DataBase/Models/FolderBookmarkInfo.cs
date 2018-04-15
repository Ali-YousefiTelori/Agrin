using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Client.DataBase.Models
{
    public class FolderBookmarkInfo
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
