//using Framesoft.Helpers.DataTypes;
using Agrin.Server.DataBase.Models.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// وضعیت کاربر
    /// </summary>
    public enum UserStatus : byte
    {
        None = 0,
        /// <summary>
        /// فقط ثبت نام کرده
        /// </summary>
        JustRegistered = 1,
        /// <summary>
        /// تایید شده
        /// </summary>
        Confirm = 2,
        /// <summary>
        /// بلاک شده
        /// </summary>
        Blocked = 3
    }

    /// <summary>
    /// user table thet user can login
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// id of this table
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// user name is phone number or email address
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// name of user
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// family of user
        /// </summary>
        public string Family { get; set; }
        /// <summary>
        /// display name of user
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// status of user registred
        /// </summary>
        public UserStatus Status { get; set; }
        
        /// <summary>
        /// سایز اپلودی که فیکس و ثابت هست و فایل های درون ان هم پاک نخواهد شد
        /// بعد از پاک کردن فایل ها سایز برمیگردد
        /// </summary>
        public long StaticUploadSize { get; set; }
        /// <summary>
        /// سایز اپلود که ثابت نیست و فایل های درون ان بعد از دانلود یا نهایتا سه روز پاک میشوند
        /// بعد از پاک شدن فایل ها حجم برگشت داده نمیشود
        /// </summary>
        public long RoamUploadSize { get; set; }
        /// <summary>
        /// هزینه موجودی کاربر که با آن خرید و شارژ حجم و ... انجام میدهد
        /// هزینه به ریال می باشد
        /// </summary>
        public long Credit { get; set; }
        /// <summary>
        /// ای دی کاربر در تلگرام
        /// </summary>
        public int? TelegramUserId { get; set; }

        /// <summary>
        /// created date time
        /// </summary>
        public DateTime CreatedDateTime { get; set; }

        public virtual ICollection<PostInfo> PostInfoes { get; set; }
        /// <summary>
        /// هزینه هایی که به این کاربر انتقال داده شده
        /// </summary>
        public virtual ICollection<UserCreditInfo> FromUserCreditInfoes { get; set; }
        /// <summary>
        /// تمامی هزینه های کاربر
        /// </summary>
        public virtual ICollection<UserCreditInfo> ToUserCreditInfoes { get; set; }
        /// <summary>
        /// direct file infoes
        /// </summary>
        public virtual ICollection<DirectFileToUserRelationInfo> DirectFileToUserRelationInfoes { get; set; }
        /// <summary>
        /// directfolder infoes
        /// </summary>
        public virtual ICollection<DirectFolderToUserRelationInfo> DirectFolderToUserRelationInfoes { get; set; }
        /// <summary>
        /// sessions of user info, user can login with those sessions
        /// </summary>
        public virtual ICollection<UserSessionInfo> UserSessionInfoes { get; set; }
        /// <summary>
        /// user confirm hashes for register and confirm phone number
        /// </summary>
        public virtual ICollection<UserConfirmHashInfo> UserConfirmHashInfoes { get; set; }
        /// <summary>
        /// user likes
        /// </summary>
        public virtual ICollection<LikeInfo> LikeInfoes { get; set; }
        /// <summary>
        /// user Acceses
        /// </summary>
        public virtual ICollection<UserRoleInfo> UserRoles { get; set; }
        /// <summary>
        /// files of users like error logs
        /// </summary>
        public virtual ICollection<FileInfo> Files { get; set; }
        
    }
}
