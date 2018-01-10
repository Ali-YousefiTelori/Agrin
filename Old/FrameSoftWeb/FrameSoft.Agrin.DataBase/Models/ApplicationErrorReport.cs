using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    public class ApplicationErrorReport
    {
        [Key]
        public int ID { get; set; }
        public string ErrorLog { get; set; }
    }
}
