using SignalGo;
using SignalGo.Shared.DataTypes;
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
    /// category of post
    /// </summary>
    [CustomDataExchanger("PostCategoryTagInfoes",
          ExchangeType = CustomDataExchangerType.Ignore, LimitationMode = LimitExchangeType.Both)]
    public class PostCategoryInfo
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
        ///// <summary>
        ///// parent of this category
        ///// </summary>
        //public int? ParentCategoryId { get; set; }

        //[ForeignKey("ParentCategoryId")]
        //public virtual PostCategoryInfo ParentCategoryInfo { get; set; }

        public virtual ICollection<PostCategoryTagInfo> PostCategoryTagInfoes { get; set; }
        public virtual ICollection<PostInfo> Posts { get; set; }
    }
}
