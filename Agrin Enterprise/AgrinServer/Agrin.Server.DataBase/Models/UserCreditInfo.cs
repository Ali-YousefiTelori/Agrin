using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// نوع پرداخت یا دریافت هزنیه
    /// </summary>
    public enum CreditType : byte
    {
        Unknown = 0,
        /// <summary>
        /// gift credit from admin
        /// این هدیه هیچ پشتوانه ای ندارد
        /// </summary>
        Gift = 1,
        /// <summary>
        /// buy credit from bank payment
        /// </summary>
        BuyCredit =2,
        /// <summary>
        /// transfer or gift from a user to another
        /// </summary>
        Transfer = 3,

    }

    /// <summary>
    /// جدول لاگ اتفاقاتی که برای کیف پول فرد میوفتد
    /// </summary>
    public class UserCreditInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// آی دی کاربر
        /// </summary>
        public int ToUserId { get; set; }
        /// <summary>
        /// نوع پرداخت یا دریافت
        /// </summary>
        public CreditType Type { get; set; }
        /// <summary>
        /// مقدار هزینه
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// از چه کاربری فرستاده شده؟ مثلا ادمین
        /// </summary>
        public int? FromUserId { get; set; }
        /// <summary>
        /// uniq
        /// </summary>
        public Guid Key { get; set; }

        [ForeignKey("ToUserId")]
        public UserInfo ToUserInfo { get; set; }

        [ForeignKey("FromUserId")]
        public UserInfo FromUserInfo { get; set; }
    }
}
