using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public class UserSessionInfo
    {
        [Key]
        public long Id { get; set; }
        public Guid FirstKey { get; set; }
        public Guid SecondKey { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string OsName { get; set; }
        public string OsVersionNumber { get; set; }
        public string OsVersionName { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
    }
}
