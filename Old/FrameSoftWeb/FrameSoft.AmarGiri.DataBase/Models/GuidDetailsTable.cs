using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class GuidDetailsTable
    {
        [Key]
        public int ID { get; set; }
        public int GuidID { get; set; }
        public int IPID { get; set; }
        public int ApplicationNameID { get; set; }
        public int ApplicationVersionID { get; set; }
        public int OSNameID { get; set; }
        public int OSVersionID { get; set; }
        [Index]
        public DateTime LastVisitTime { get; set; }
    }
}
