using Agrin.Server.DataBase.Models.Relations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// category tags
    /// </summary>
    public class TagInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// title of post category
        /// </summary>
        public string Title { get; set; }

        public virtual ICollection<PostCategoryTagRelationInfo> PostCategoryTagRelationInfoes { get; set; }
        public virtual ICollection<PostTagRelationInfo> PostTagRelationInfoes { get; set; }
    }
}
