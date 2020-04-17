using Agrin.Server.Models.Validations;
using SignalGo.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// وضعیت درخواست
    /// </summary>
    public enum RequestIdeaStatus : byte
    {
        /// <summary>
        /// ساخته شده
        /// </summary>
        Created = 1,
        /// <summary>
        /// در حال بررسی
        /// </summary>
        Checking = 2,
        /// <summary>
        /// تکراری
        /// </summary>
        Duplicate = 3,
        /// <summary>
        /// حل شده
        /// </summary>
        Fixed = 4,
        /// <summary>
        /// نیاز به توضیحات بیشتر
        /// </summary>
        MoreInformation = 5
    }

    /// <summary>
    /// نوع درخواست ایده
    /// </summary>
    public enum RequestIdeaType : byte
    {
        /// <summary>
        /// ایده
        /// </summary>
        Idea = 1,
        /// <summary>
        /// باگ
        /// </summary>
        Bug = 2,
        /// <summary>
        /// توضیحات و جزئیات خطا
        /// </summary>
        ExceptionInformation = 3
    }
    /// <summary>
    /// users request and idea to add feature in app
    /// </summary>
    [CustomDataExchanger(ExchangeType = CustomDataExchangerType.TakeOnly, LimitationMode = LimitExchangeType.IncomingCall,
        Properties = new string[] { "HttpErrorCode", "ExceptionType", "ErrorMessage", "StackTrace", "Title", "Mesage" })]
    public class RequestIdeaInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// created date time
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// last update dateTime for added new comment from user
        /// </summary>
        public DateTime UpdatedDateTime { get; set; }
        /// <summary>
        /// Creator userId
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// type of idea
        /// </summary>
        public RequestIdeaType Type { get; set; }
        /// <summary>
        /// status of idea
        /// </summary>
        public RequestIdeaStatus Status { get; set; }
        /// <summary>
        /// title of request
        /// </summary>
        [DisplayName("تیتر")]
        [TextLengthValidation(4, 100)]
        public string Title { get; set; }
        /// <summary>
        /// message of request
        /// </summary>
        [DisplayName("متن")]
        [TextLengthValidation(5, 5000)]
        public string Message { get; set; }

        #region exception description
        /// <summary>
        /// if exception was webexception this will be fill for error code like 404 503
        /// </summary>
        public int? HttpErrorCode { get; set; }
        /// <summary>
        /// type of exception like system.ioexception
        /// </summary>
        public string ExceptionType { get; set; }
        /// <summary>
        /// error message is ex.Message
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// trace of message
        /// </summary>
        public string StackTrace { get; set; }

        #endregion

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }

        public virtual ICollection<CommentInfo> CommentInfoes { get; set; }
        public virtual ICollection<LikeInfo> LikeInfoes { get; set; }
        public virtual ICollection<FileInfo> FileInfoes { get; set; }

        [NotMapped]
        public int CommentsCount { get; set; }
        [NotMapped]
        public int LikesCount { get; set; }
        [NotMapped]
        public bool IsLiked { get; set; }
    }
}
