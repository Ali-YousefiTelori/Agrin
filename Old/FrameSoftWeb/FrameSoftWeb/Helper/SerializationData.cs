using Agrin.Framesoft.String;
using Gita.Infrastructure.UI.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FrameSoftWeb.Helper
{
    public static class SerializationData
    {
        public static string DecryptStream(Stream stream)
        {
            string base64String = null;
            using (StreamReader reader = new StreamReader(new StreamDecryption(stream)))
            {
                base64String = reader.ReadToEnd();
            }
            return DecryptString(base64String);
        }

        public static string DecryptDataString(string dataJson)
        {
            var bytes = Convert.FromBase64String(dataJson);
            using (var stream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return DecryptStream(stream);
            }
        }

        public static string EncryptObject(object obj)
        {
            return EncryptString(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }

        public static T DecryptObject<T>(string text)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(DecryptString(text));
        }

        public static string EncryptString(string json)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i]++;
            }
            var base64String = Convert.ToBase64String(bytes);
            var encode = StringEncoding.Base64Encode(base64String);
            return encode;
        }

        public static string DecryptString(string data)
        {
            var decode = StringEncoding.Base64Decode(data);
            var bytes = Convert.FromBase64String(decode);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i]--;
            }
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return json;
        }

        public static string GetTextBetweenTwoValue(string content, string str1, string str2, bool singleLine = true)
        {
            RegexOptions pattern;
            if (singleLine)
            {
                pattern = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline;
            }
            else
            {
                pattern = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace;
            }
            Regex regex = new Regex(str1 + "(.*)" + str2, pattern);
            return regex.Match(content).Groups[1].ToString();
        }
    }
}
