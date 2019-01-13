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
    /// language Keys
    /// </summary>
    public class LanguageKeyInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// language map key
        /// like Comedy
        /// </summary>
        [StringLength(20)]
        public string Key { get; set; }
        /// <summary>
        /// content of language map
        /// </summary>
        [StringLength(50)]
        public string Content { get; set; }
        /// <summary>
        /// parent of language like persian or english
        /// </summary>
        public int LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public LanguageInfo LanguageInfo { get; set; }

        public ICollection<PostCategoryInfo> PostCategoryInfoes { get; set; }
    }
}
