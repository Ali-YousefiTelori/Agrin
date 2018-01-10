using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.BaseViewModels.Lists
{
    public class ListBaseViewModel<T> : ANotifyPropertyChanged
    {
        public virtual IEnumerable<T> GetSelectedItems()
        {
            return null;
        }
    }
}
