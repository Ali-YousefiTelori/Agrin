using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// types of role accesses
    /// </summary>
    public enum RoleAccessType : byte
    {
        None = 0,
        /// <summary>
        /// access of a normal user registred in system
        /// </summary>
        NormalUser = 1,
        /// <summary>
        /// system admin access
        /// </summary>
        SystemAdmin = 2
    }

    /// <summary>
    /// user roles and access types
    /// </summary>
    public class UserRoleInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// access type of role
        /// </summary>
        public RoleAccessType AccessType { get; set; }
        /// <summary>
        /// user id that have role
        /// </summary>
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserInfo UserInfo { get; set; }
    }
}
