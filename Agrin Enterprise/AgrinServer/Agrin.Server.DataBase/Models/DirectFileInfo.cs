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
    /// فایل هایی که کاربران اپلود میکنند یا لینک مستقیم برای فایل میسازند و تمامی چیزی هایی که اپلود میکنند که مربوط به پست ها نمیشود
    /// </summary>
    public class DirectFileInfo
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int ServerId { get; set; }
        public Guid Password { get; set; }

        /// <summary>
        /// در صورتی که تکمیل نشده بودند بعد از یک مدت طولانی از روی دیسک پاک شوند
        /// </summary>
        public bool IsComplete { get; set; }

        public virtual ICollection<DirectFileToUserRelationInfo> DirectFileToUserRelationInfoes { get; set; }
        [ForeignKey("ServerId")]
        public ServerInfo ServerInfo { get; set; }
    }
}
