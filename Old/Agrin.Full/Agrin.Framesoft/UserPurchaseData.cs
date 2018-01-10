using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Framesoft
{
    public class UserPurchaseData
    {
        public int ID { get; set; }
        public int UserID { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public long PurchaseTime { get; set; }
        public string PurchaseToken { get; set; }
        public string ProductId { get; set; }
    }
}
