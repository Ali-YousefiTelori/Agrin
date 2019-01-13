using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// language supported by
    /// </summary>
    public class LanguageInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// name of language like english, persian
        /// </summary>
        public string Name { get; set; }

        public ICollection<LanguageKeyInfo> LanguageKeyInfoes { get; set; }
    }
}
