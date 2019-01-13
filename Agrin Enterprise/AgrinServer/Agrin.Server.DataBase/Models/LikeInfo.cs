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
    /// tables of like
    /// </summary>
    public class LikeInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// user id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// request idea id
        /// </summary>
        public int? RequestIdeaId { get; set; }

        [ForeignKey("RequestIdeaId")]
        public RequestIdeaInfo RequestIdeaInfo { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
    }
}
