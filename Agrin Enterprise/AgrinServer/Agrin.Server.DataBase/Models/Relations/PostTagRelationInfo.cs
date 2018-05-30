using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models.Relations
{
    public class PostTagRelationInfo
    {
        public int PostId { get; set; }
        public int TagId { get; set; }

        [ForeignKey("PostId")]
        public PostInfo PostInfo { get; set; }

        [ForeignKey("TagId")]
        public TagInfo TagInfo { get; set; }
    }
}
