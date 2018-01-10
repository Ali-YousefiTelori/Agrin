using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class ApplicationVersionTable
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(23)]
        [MaxLength(23)]
        public string Version { get; set; }
    }
}
