using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    public class UserInfo
    {
        [Key]
        public int ID { get; set; }

        public Guid ApplicationGuid { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(40)]
        [MaxLength(40)]
        public string UserName { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(40)]
        [MaxLength(40)]
        public string Email { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        [MaxLength(50)]
        public string Password { get; set; }

        public bool ConfirmFromEmail { get; set; }
        public Guid ConfirmHash { get; set; }
        public DateTime RegisterDateTime { get; set; }
        public long UserSize { get; set; }
    }
}
