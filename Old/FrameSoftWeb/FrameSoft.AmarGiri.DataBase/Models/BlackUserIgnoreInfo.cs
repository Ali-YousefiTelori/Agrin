using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class BlackUserIgnoreInfo
    {
        [Key]
        public int ID { get; set; }
        public int GuidID { get; set; }
    }
}
