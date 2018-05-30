using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models.Relations
{
    public enum DirectFileFolderAccessType : byte
    {
        None = 0,
        Creator = 1,
        Viewer = 2,
    }

    public class DirectFileToUserRelationInfo
    {
        public int UserId { get; set; }
        public long DirectFileId { get; set; }

        /// <summary>
        /// permission of user to access to file
        /// </summary>
        public DirectFileFolderAccessType AccessType { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
        [ForeignKey("DirectFileId")]
        public DirectFileInfo DirectFileInfo { get; set; }
    }
}
