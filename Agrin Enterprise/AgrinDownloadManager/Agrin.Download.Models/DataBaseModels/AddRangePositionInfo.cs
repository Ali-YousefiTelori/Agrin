using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.DataBaseModels
{
    /// <summary>
    /// when link use add range this position save for repair link
    /// </summary>
    public class AddRangePositionInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [BsonId]
        public int Id { get; set; }
        /// <summary>
        /// id of link
        /// </summary>
        public int LinkId { get; set; }
        /// <summary>
        /// position of link
        /// </summary>
        public long Position { get; set; }
    }
}
