using Gita.DataBase.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmarGiri.Foundation
{
    [DataBaseAttribute("AmarTable", "AmarDataBase")]
    public interface IAmarDTO
    {
        [DataBaseAttribute("ID", true, false)]
        long ID { get; set; }
        
        [DataBaseAttribute("IP")]
        string IP { get; set; }//154.124.204.125

        [DataBaseAttribute("DateTime")]
        long DateTime { get; set; }//2010/01/02 12:20
    }
}
