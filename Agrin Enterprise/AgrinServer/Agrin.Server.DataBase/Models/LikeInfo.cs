using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public class LikeInfo
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int? RequestIdeaId { get; set; }

        [ForeignKey("RequestIdeaId")]
        public RequestIdeaInfo RequestIdeaInfo { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
    }
}
