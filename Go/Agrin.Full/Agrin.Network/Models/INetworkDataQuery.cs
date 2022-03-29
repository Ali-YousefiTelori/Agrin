using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Network.Models
{
    /// <summary>
    /// اینترفیس که یک دیتا میگرد و مقدار بر میگرداند که میتواند در لیست هوشمند اضافه شود یا خیر؟
    /// </summary>
    public interface INetworkDataQuery
    {
        bool CanAddToLinkList(string data);
    }
}
