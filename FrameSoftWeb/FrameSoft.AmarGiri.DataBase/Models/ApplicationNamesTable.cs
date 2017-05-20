using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class ApplicationNamesTable
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(25)]
        [MaxLength(25)]
        public string Name { get; set; }
    }
}
