using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// server infoes of agrin
    /// </summary>
    public class ServerInfo
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Domain { get; set; }
        [StringLength(16)]
        public string IpAddress { get; set; }
        public ICollection<DirectFileInfo> DirectFileInfoes { get; set; }
    }
}
