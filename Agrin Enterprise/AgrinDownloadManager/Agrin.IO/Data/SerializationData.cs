using Agrin.IO.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.IO.Data
{
    public static class SerializationData
    {
        public static void SaveToFile(string path, object data)
        {
            if (string.IsNullOrEmpty(path))
                return;
            var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
            using (var stream = IOHelperBase.OpenFileStreamForWrite(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
            {
               var bytes=  Encoding.UTF8.GetBytes(json);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
