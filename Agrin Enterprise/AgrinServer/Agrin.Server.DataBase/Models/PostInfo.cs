using Framesoft.Helpers.DataTypes;
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
    /// types of a post
    /// </summary>
    public enum PostType : byte
    {
        /// <summary>
        /// unknown
        /// </summary>
        None = 0,
        /// <summary>
        /// post of video files
        /// </summary>
        Video = 1,
        /// <summary>
        /// post of sounds
        /// </summary>
        Sound = 2,
    }

    /// <summary>
    /// data of a post
    /// </summary>
    public class PostInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// id od post category
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// userId
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// type of post
        /// </summary>
        public PostType PostType { get; set; }
        /// <summary>
        /// when post is a video post you see this
        /// </summary>
        public int? PostVideoId { get; set; }
        /// <summary>
        /// when post is a music post you see this
        /// </summary>
        public int? PostMusicId { get; set; }
        /// <summary>
        /// views count
        /// </summary>
        public long ViewCount { get; set; }
        /// <summary>
        /// created DateTime
        /// </summary>
        [DateTimeKind(DateTimeKind.Local)]
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// last update post
        /// </summary>
        [DateTimeKind(DateTimeKind.Local)]
        public DateTime LastUpdateDateTime { get; set; }
        /// <summary>
        /// last update file versions
        /// </summary>
        [DateTimeKind(DateTimeKind.Local)]
        public DateTime LastUpdateFileVersionDateTime { get; set; }

        [ForeignKey("PostVideoId")]
        public virtual PostVideoInfo PostVideoInfo { get; set; }

        [ForeignKey("PostMusicId")]
        public virtual PostSoundInfo PostMusicInfo { get; set; }

        [ForeignKey("CategoryId")]
        public virtual PostCategoryInfo PostCategoryInfo { get; set; }

        [ForeignKey("UserId")]
        public virtual UserInfo UserInfo { get; set; }

        public virtual ICollection<FileInfo> FileInfoes { get; set; }

        public virtual ICollection<PostCategoryTagInfo> PostCategoryTagInfoes { get; set; }

    }
}
