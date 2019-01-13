
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
    /// type of file
    /// </summary>
    public enum FileType : byte
    {
        /// <summary>
        /// none
        /// </summary>
        None = 0,
        /// <summary>
        /// small image of post logo
        /// </summary>
        SmallLogo = 1,
        /// <summary>
        /// background image of post
        /// </summary>
        BackGroundImage = 2,
        /// <summary>
        /// data of file
        /// </summary>
        Data = 3,
    }

    //[Flags]
    //public enum OperationSystemType : int
    //{
    //    None = 0,
    //    Windows = 1,
    //    WindowsUniversal = 2,
    //    Android = 4,
    //    IOS = 8,
    //    Mac = 16,
    //    Linux = 32,
    //    Any = Windows | WindowsUniversal | Android | IOS | Mac | Linux
    //}

    /// <summary>
    /// information of file
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// created date time
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// domain address of file that has file for user to download
        /// </summary>
        public int ServerId { get; set; }
        /// <summary>
        /// text version of file
        /// </summary>
        [StringLength(20)]
        public string Version { get; set; }
        /// <summary>
        /// file password for download from server
        /// don't let users to download your files easy
        /// </summary>
        public Guid Password { get; set; }
        /// <summary>
        /// number of version
        /// when version number is integer
        /// </summary>
        public int? VersionNumber { get; set; }

        /// <summary>
        /// if the files not completed to upload must delete from disk after a times
        /// </summary>
        public bool IsComplete { get; set; }
        /// <summary>
        /// type of file
        /// </summary>
        public FileType Type { get; set; }
        /// <summary>
        /// creator user id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// post id
        /// </summary>
        public int? PostId { get; set; }
        /// <summary>
        /// visit card icon file
        /// </summary>
        public int? VisitCardId { get; set; }
        /// <summary>
        /// request idea application log file uploaded from client
        /// </summary>
        public int? RequestIdeaId { get; set; }

        [ForeignKey("PostId")]
        public virtual PostInfo PostInfo { get; set; }
        [ForeignKey("ServerId")]
        public ServerInfo ServerInfo { get; set; }
        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
    }
}
