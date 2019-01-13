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
    /// sub category of posts
    /// like موسقی شاد و غمگین و غیره
    /// نرم افزار ویندوز، لینوکس و غیره
    /// اینها مهم هستند
    /// </summary>
    public class PostSubCategoryInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// name of subcategory
        /// </summary>
        public int LanguageKeyId { get; set; }
        /// <summary>
        /// parent category of this sub category
        /// </summary>
        public int PostCategoryId { get; set; }
        /// <summary>
        /// all of the sub categories of post category
        /// this is many to many relation
        /// </summary>
        public virtual ICollection<PostCategorySubCategoryRelationInfo> PostCategorySubCategoryRelationInfoes { get; set; }

        [ForeignKey("LanguageKeyId")]
        public LanguageKeyInfo LanguageKeyInfo { get; set; }
        [ForeignKey("PostCategoryId")]
        public PostCategoryInfo PostCategoryInfo { get; set; }
    }
}
