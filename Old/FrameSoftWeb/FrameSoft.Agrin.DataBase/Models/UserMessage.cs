using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    public class UserMessage
    {
        [Key]
        public int ID { get; set; }
        public bool Answered { get; set; }//پاسخ داده شده از طرف مدیر
        public bool IsReplay { get; set; }//این پیام پاسخ از طرف مدیر هست نه پیغام از طرف کاربر
        public int LastUserMessageID { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(23)]
        [MaxLength(23)]
        public string Name { get; set; }

        public int GuidId { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(23)]
        [MaxLength(23)]
        public string Email { get; set; }

        public string Message { get; set; }
        public DateTime InsertDateTime { get; set; }
    }
}
