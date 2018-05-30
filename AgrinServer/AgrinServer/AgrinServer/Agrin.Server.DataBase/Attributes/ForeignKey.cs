using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations.Schema
{
#if (PORTABLE)
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
#endif
}
