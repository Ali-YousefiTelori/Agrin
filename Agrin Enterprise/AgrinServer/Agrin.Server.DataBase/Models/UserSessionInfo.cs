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
    /// session of user that user can login with this
    /// </summary>
    public class UserSessionInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// first key that is like username but guid
        /// </summary>
        public Guid FirstKey { get; set; }
        /// <summary>
        /// second key that is password but guid
        /// </summary>
        public Guid SecondKey { get; set; }
        /// <summary>
        /// created dateTime of session
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// user os name
        /// </summary>
        [StringLength(50)]
        public string OsName { get; set; }
        /// <summary>
        /// user os version number
        /// </summary>
        [StringLength(15)]
        public string OsVersionNumber { get; set; }
        /// <summary>
        /// user os version name
        /// </summary>
        [StringLength(50)]
        public string OsVersionName { get; set; }
        /// <summary>
        /// user device name
        /// </summary>
        [StringLength(100)]
        public string DeviceName { get; set; }
        /// <summary>
        /// user id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// if session is active
        /// </summary>
        public bool IsActive { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
    }
}
