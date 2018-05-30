using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public class DirectLinkInfo
    {
        [Key]
        public int Id { get; set; }
        public string Address { get; set; }

        public int UserId { get; set; }
        public int FileId { get; set; }

        public bool IsComplete { get; set; }

        public UserInfo UserInfo { get; set; }

        [ForeignKey("FileId")]
        public DirectFileInfo DirectFileInfo { get; set; }
    }
}
