using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class GuidIDTable
    {
        [Key]
        public int ID { get; set; }
        public Guid GuidData { get; set; }
        public int UserID { get; set; }
        [Index]
        public DateTime InstallDateTime { get; set; }
    }
}
