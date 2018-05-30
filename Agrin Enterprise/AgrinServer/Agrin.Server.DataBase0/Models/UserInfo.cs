//using Framesoft.Helpers.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public enum UserStatus : byte
    {
        None = 0,
        JustRegistred = 1,
        Confirm = 2,
        Blocked = 3
    }
    /// <summary>
    /// user table thet user can login
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// id of this table
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// user name is phone number or email address
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// guid password of user for login
        /// </summary>
        public Guid Password { get; set; }
        /// <summary>
        /// name of user
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// family of user
        /// </summary>
        public string Family { get; set; }
        /// <summary>
        /// display name of user
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// status of user registred
        /// </summary>
        public UserStatus Status { get; set; }
        /// <summary>
        /// created date time
        /// </summary>
        //[DateTimeKind(DateTimeKind.Local)]
        public DateTime CreatedDateTime { get; set; }

        public virtual ICollection<PostInfo> PostInfoes { get; set; }
    }
}
