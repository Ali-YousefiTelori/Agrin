using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class ErrorLogTable
    {
        [Key]
        public int ID { get; set; }
        public string Message { get; set; }
        public DateTime InsertDateTime { get; set; }
    }
}
