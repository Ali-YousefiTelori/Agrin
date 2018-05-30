using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models.Relations
{
    public class PostCategoryTagRelationInfo
    {
        /// <summary>
        /// header tag of tags
        /// این مشخص میکنه که این تگ میتونه جزو تگ های هدر باشه یا نه
        /// هر پست فقط یک هدر تگ میتونه داشته باشه و در واقع هر پست باید یک هدر تگ داشته باشه
        /// </summary>
        public bool IsHeaderTag { get; set; }

        public int PostCategoryId { get; set; }
        public int TagId { get; set; }

        [ForeignKey("PostCategoryId")]
        public PostCategoryInfo PostCategoryInfo { get; set; }

        [ForeignKey("TagId")]
        public TagInfo TagInfo { get; set; }
    }
}
