using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models.Relations
{
    /// <summary>
    /// many to many relation of post category and post sub category
    /// some names are duplicated for this we made many to many relation to stop doing duplicate name of post sub category for different category
    /// </summary>
    public class PostCategorySubCategoryRelationInfo
    {
        /// <summary>
        /// post category key
        /// </summary>
        public int PostCategoryId { get; set; }
        /// <summary>
        /// post sub category key
        /// </summary>
        public int PostSubCategoryId { get; set; }

        [ForeignKey("PostCategoryId")]
        public PostCategoryInfo PostCategoryInfo { get; set; }

        [ForeignKey("PostSubCategoryId")]
        public PostSubCategoryInfo PostSubCategoryInfo { get; set; }
    }
}
