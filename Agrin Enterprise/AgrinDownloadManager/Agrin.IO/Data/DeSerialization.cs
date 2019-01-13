using Agrin.IO.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Data
{
    /// <summary>
    /// deserialization system
    /// </summary>
    public static class DeserializationData
    {
        public static T OpenFromFile<T>(string path)
            where T : class
        {
            if (string.IsNullOrEmpty(path))
                return null;
            using (var stream = IOHelperBase.Current.OpenFileStreamForRead(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
            {
                List<byte> result = new List<byte>();
                while (stream.Position < stream.Length)
                {
                    byte[] buffer = new byte[1024 * 100];
                    var readCount = stream.Read(buffer, 0, buffer.Length);
                    result.AddRange(buffer.ToList().GetRange(0, readCount));
                }
                var json = Encoding.UTF8.GetString(result.ToArray());
                return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
            }

        }
    }
}
