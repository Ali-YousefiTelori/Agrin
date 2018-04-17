using MvvmGo.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Converters
{
    public class EnumToStringConverterBase
    {
        public string GetString(object data)
        {
            if (data == null)
                return ApplicationResourceBase.Current.GetAppResource("Unknown");
            return ApplicationResourceBase.Current.GetAppResource(data.ToString());
        }
    }
}
