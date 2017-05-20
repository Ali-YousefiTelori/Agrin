using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    public class UserPurchase
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }

        public long PurchaseTime { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        [MaxLength(300)]
        public string PurchaseToken { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        [MaxLength(100)]
        public string ProductId { get; set; }

        public DateTime InsertDateTime { get; set; }
    }
}
