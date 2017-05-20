using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Models
{
    public class AmarTable
    {
        [Key]
        public int ID { get; set; }

        public byte DayDate { get; set; }
        public byte MonthDate { get; set; }
        public int YearDate { get; set; }

        public long Count { get; set; }
    }
}
