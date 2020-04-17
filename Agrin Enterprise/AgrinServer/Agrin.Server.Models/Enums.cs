using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.Models
{
    public enum MessageType : int
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
        SessionAccessDenied = 9,
        UsernameOrPasswordIncorrect = 10,
        NotSupportYet = 11,
        Duplicate = 12,
        StorageFull = 13,
        NotFound = 14,
        IsNotConfirm = 15,
        AccessDenied = 16,
        CodeNotExist = 17,
        DataIsNotValid = 18,
        ValidationsError = 19
    }
}
