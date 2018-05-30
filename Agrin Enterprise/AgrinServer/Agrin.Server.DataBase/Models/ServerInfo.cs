using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    public class ServerInfo
    {
        [Key]
        public int Id { get; set; }
        public string Domain { get; set; }
        public string IpAddress { get; set; }
        public ICollection<DirectFileInfo> DirectFileInfoes { get; set; }
    }
}
