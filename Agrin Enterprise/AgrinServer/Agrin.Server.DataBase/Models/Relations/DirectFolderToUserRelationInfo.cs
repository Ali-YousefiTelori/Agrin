using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models.Relations
{
    public class DirectFolderToUserRelationInfo
    {
        public int UserId { get; set; }
        public int DirectFolderId { get; set; }

        /// <summary>
        /// permission of user to access to file
        /// </summary>
        public DirectFileFolderAccessType AccessType { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }

        [ForeignKey("DirectFolderId")]
        public DirectFolderInfo DirectFolderInfo { get; set; }
    }
}
