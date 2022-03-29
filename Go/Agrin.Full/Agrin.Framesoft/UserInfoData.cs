using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft
{
    public class UserInfoData
    {
        public Guid ApplicationGuid { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long Size { get; set; }
    }
}
