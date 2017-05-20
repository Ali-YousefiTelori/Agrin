using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class IPsTable
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(16)]
        [MaxLength(16)]
        public string IPValue { get; set; }

        [Index]
        public DateTime LastVisitTime { get; set; }
    }
}
