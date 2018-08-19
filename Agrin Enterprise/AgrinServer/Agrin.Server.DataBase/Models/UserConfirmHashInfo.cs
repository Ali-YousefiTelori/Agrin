using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public class UserConfirmHashInfo
    {
        /// <summary>
        /// آی دی 
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// آی دی کاربر
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// شماره ی رندوم
        /// </summary>
        public int RandomNumber { get; set; }
        /// <summary>
        /// متن رندوم جهت ارسال به ایمیل
        /// </summary>
        public Guid RandomGuid { get; set; }
        /// <summary>
        /// قبلا یکبار اضافه شده یا خیر
        /// </summary>
        public bool IsUsed { get; set; }
        /// <summary>
        /// تاریخ ساخت
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// کلید خارجی کلاس کاربر
        /// </summary>
        [ForeignKey("UserId")]
        [CustomDataExchanger(LimitExchangeType.Both)]
        public virtual UserInfo UserInfo { get; set; }
    }
}
