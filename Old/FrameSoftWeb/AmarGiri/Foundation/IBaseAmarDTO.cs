using Gita.DataBase.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmarGiri.Foundation
{
    [DataBaseAttribute("AmarBaseTable", "AmarDataBase")]
    public interface IBaseAmarDTO
    {
        [DataBaseAttribute("ID", true, false)]
        long ID { get; set; }

        [DataBaseAttribute("Application")]
        string Application { get; set; }//Agrin Android

        [DataBaseAttribute("ApplicationVersion")]
        string ApplicationVersion { get; set; }//1.4.141

        [DataBaseAttribute("ApplicationGuid")]
        string ApplicationGuid { get; set; }

        [DataBaseAttribute("OSName")]
        string OSName { get; set; }//Android

        [DataBaseAttribute("OSVersion")]
        string OSVersion { get; set; }//4.2
    }
}
