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
    public class PostCategoryTagInfo
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
        /// <summary>
        /// header tag of tags
        /// این مشخص میکنه که این تگ میتونه جزو تگ های هدر باشه یا نه
        /// هر پست فقط یک هدر تگ میتونه داشته باشه و در واقع هر پست باید یک هدر تگ داشته باشه
        /// </summary>
        public bool IsHeaderTag { get; set; }
        public virtual ICollection<PostCategoryInfo> PostCategoryInfoes { get; set; }
        public virtual ICollection<PostInfo> PostInfoes { get; set; }
    }
}
