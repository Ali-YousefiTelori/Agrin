using AmarGiri.Foundation;
using Gita.DataBase.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmarGiri.Model
{
    public class AmarDTO : QueryTable<AmarDTO>, IAmarDTO
    {
        public long ID { get; set; }
        public string IP { get; set; }

        long _DateTime;
        public long DateTime
        {
            get { return _DateTime; }
            set { _DateTime = value; }
        }

        public DateTime GetDateTime
        {
            get
            {
                return new DateTime(DateTime);
            }
        }
    }
}
