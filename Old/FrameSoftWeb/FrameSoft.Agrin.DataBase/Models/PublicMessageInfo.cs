using Agrin.Framesoft.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    public class PublicMessageInfo
    {
        [Key]
        public int ID { get; set; }

        public DateTime MessageDateTime { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public LimitMessageEnum LimitMessageMode { get; set; }
    }
}
