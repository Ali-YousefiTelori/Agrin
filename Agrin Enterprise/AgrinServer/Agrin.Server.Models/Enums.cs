﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.Models
{
    public enum ErrorMessage : int
    {
        None = 0,
        Success = 1,
        PleaseFillAllData = 2,
        PleaseInstallNewVersion = 3,
        WrongData = 4,
        ServerException = 5,
        FileNotFound = 6,
        DataOverFlow = 7,
        ClientIpIsNotValid = 8,
        SessionAccessDenied = 9
    }
}
