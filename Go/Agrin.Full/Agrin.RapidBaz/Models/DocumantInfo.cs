using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.RapidBaz.Models
{
    public abstract class DocumantInfo : ANotifyPropertyChanged
    {
        public abstract string Name { get; set; }
    }
}
