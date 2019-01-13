using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// visit cards or channels of users
    /// </summary>
    public class VisitCardInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// title of visit card
        /// </summary>
        [StringLength(100)]
        public string Title { get; set; }
        /// <summary>
        /// description of visitcard
        /// </summary>
        [StringLength(350)]
        public string Description { get; set; }
        /// <summary>
        /// user id that have role
        /// </summary>
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
        public ICollection<FileInfo> FileInfoes { get; set; }
    }
}
