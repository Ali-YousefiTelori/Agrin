using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public class CommentInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Creator userId
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// تاریخ و زمان ارسال
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// id of idea that usr commented
        /// </summary>
        public int? RequestIdeaId { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }

        [ForeignKey("RequestIdeaId")]
        public RequestIdeaInfo RequestIdeaInfo { get; set; }
    }
}
