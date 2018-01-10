using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FrameSoft.Agrin.DataBase.Models
{
    /// <summary>
    /// پیغامی که از طرف کاربر دریافت میشود
    /// </summary>
    public class UserMessageReceiveInfo
    {
        [Key]
        public int ID { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string Message { get; set; }
        public int GuidID { get; set; }
        public bool IsReplay { get; set; }
    }
}
