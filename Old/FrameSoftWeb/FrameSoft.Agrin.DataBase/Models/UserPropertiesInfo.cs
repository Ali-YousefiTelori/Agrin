using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    public class UserPropertiesInfo
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }

        public ulong UserSize { get; set; }
    }
}
