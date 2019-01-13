using Agrin.Server.DataBase.Models.Relations;
using SignalGo.Shared.DataTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// category of post
    /// like, کاربردی و موسیقی و فیلم و کتاب و نرم افزار
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
        /// title of post category like music,film,application
        /// </summary>
        public int LanguageKeyId { get; set; }
        /// <summary>
        /// all of the sub categories of post category
        /// this is many to many relation
        /// </summary>
        public virtual ICollection<PostCategorySubCategoryRelationInfo> PostCategorySubCategoryRelationInfoes { get; set; }
        public virtual ICollection<PostInfo> Posts { get; set; }

        [ForeignKey("LanguageKeyId")]
        public LanguageKeyInfo LanguageKeyInfo { get; set; }
    }
}
