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

    [Flags]
    public enum OperationSystemType : int
    {
        None = 0,
        Windows = 1,
        WindowsUniversal = 2,
        Android = 4,
        IOS = 8,
        Mac = 16,
        Linux = 32,
        Any = Windows | WindowsUniversal | Android | IOS | Mac | Linux
    }

    /// <summary>
    /// information of file
    /// </summary>
    public class PostFileInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// domain address of file that had file for user to download
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
        /// type of file
        /// </summary>
        public FileType Type { get; set; }
        /// <summary>
        /// supported os types
        /// </summary>
        public OperationSystemType OperationSystemSupports { get; set; }
        /// <summary>
        /// post id
        /// </summary>
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public virtual PostInfo PostInfo { get; set; }
    }
}
