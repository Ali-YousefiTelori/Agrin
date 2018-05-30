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
    /// post video info
    /// </summary>
    public class PostVideoInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// title
        /// </summary>
        public string Title { get; set; }
    }
}
