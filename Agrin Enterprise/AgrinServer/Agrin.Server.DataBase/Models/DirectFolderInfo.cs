using Agrin.Server.DataBase.Models.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public class DirectFolderInfo
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public DirectFolderInfo ParentFolderInfo { get; set; }
        public ICollection<DirectFolderInfo> Children { get; set; }
        public ICollection<DirectFolderToUserRelationInfo> DirectFolderToUserRelationInfoes { get; set; }
    }
}
