using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft.Helper
{
    public static class FileManagerHelper
    {
        public static string domain = UserManagerHelper.domain;
        public static ResponseData<byte[]> GetIconByFileExtention(string fileExtention)
        {
            return DataSerializationHelper.GetRequestData<byte[]>("http://" + domain + "/Design/GetIconByFileExtention/" + fileExtention);
        }
    }
}
