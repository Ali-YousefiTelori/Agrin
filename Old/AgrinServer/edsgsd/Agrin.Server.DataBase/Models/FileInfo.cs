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
    }

    /// <summary>
    /// information of file
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// server address of file that file uploaded
        /// </summary>
        public string ServerAddress { get; set; }
        /// <summary>
        /// version of file
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// number of version
        /// when version number is integer
        /// </summary>
        public int? VersionNumber { get; set; }
        /// <summary>
        /// name of file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// post id
        /// </summary>
        public int PostId { get; set; }
        /// <summary>
        /// type of file
        /// </summary>
        public FileType Type { get; set; }
        [ForeignKey("PostId")]
        public virtual PostInfo PostInfo { get; set; }
    }
}
