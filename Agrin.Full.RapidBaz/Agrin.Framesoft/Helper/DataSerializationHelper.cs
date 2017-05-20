using Agrin.Framesoft.String;
using Gita.Infrastructure.UI.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Agrin.Framesoft.Helper
{
    public static class DataSerializationHelper
    {
        public static string Sha1Hash(this string str)
        {
            byte[] data = UTF8Encoding.UTF8.GetBytes(str);
            data = Sha1Hash(data);
            return BitConverter.ToString(data).Replace("-", "");
        }

        static byte[] Sha1Hash(this byte[] data)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(data);
            }
        }

        static string EncryptString(string json)
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

        static byte[] GetBytesPerBuffer(Stream stream, int bufferCount)
        {
            List<byte> read = new List<byte>();
            int totalRead = bufferCount;
            while (stream.Position != stream.Length)
            {
                byte[] readBytes = new byte[totalRead];
                int readCount = stream.Read(readBytes, 0, totalRead);
                if (readCount == bufferCount)
                {
                    read.AddRange(readBytes);
                    break;
                }
                else
                {
                    totalRead -= readCount;
                    read.AddRange(readBytes.ToList().GetRange(0, readCount));
                    if (totalRead == 0)
                        break;
                }
            }
            return read.ToArray();
        }

        public static string EncryptObject(object obj)
        {
            return EncryptString(JsonConvert.SerializeObject(obj));
        }

        static Stream EncryptStream(object data)
        {
            string base64String = EncryptObject(data);
            StreamWriter writer = new StreamWriter(new StreamEncryption(new MemoryStream()));
            writer.Write(base64String);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            return writer.BaseStream;
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

        static string DecryptStream(Stream stream, bool msg)
        {
            string base64String = null;
            using (StreamReader reader = new StreamReader(stream))
            {
                base64String = reader.ReadToEnd();
            }
            if (msg)
            {
                return base64String;
            }

            return DecryptString(base64String);
        }

        public static ResponseData<T> GetRequestData<T>(string uri)
        {
            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(uri);
            _request.AllowAutoRedirect = true;
            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
            {
                var length = response.ContentLength;
                using (var stream = response.GetResponseStream())
                {
                    bool isMessage = response.Headers["Message"] == "1";
                    var res = DecryptStream(stream, isMessage);
                    ResponseData<T> val = new ResponseData<T>();

                    if (isMessage)
                    {
                        val.Message = res;
                        return val;
                    }
                    else
                    {
                        val.Data = JsonConvert.DeserializeObject<T>(res);
                        return val;
                    }
                }

            }
        }

        public static ResponseData<T> SendRequestData<T>(string uri, object data)
        {
            byte[] bytes = null;
            using (var stream = EncryptStream(data))
            {
                bytes = GetBytesPerBuffer(stream, (int)stream.Length);
            }


            HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(uri);
            _request.AllowAutoRedirect = true;
            _request.KeepAlive = true;
            _request.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            _request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            _request.Method = "POST";
            _request.ContentLength = bytes.Length;

            using (var reqStream = _request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
                using (HttpWebResponse response = (HttpWebResponse)_request.GetResponse())
                {
                    var length = response.ContentLength;
                    using (var stream = response.GetResponseStream())
                    {
                        bool isMessage = response.Headers["Message"] == "1";
                        var res = DecryptStream(stream, isMessage);
                        ResponseData<T> val = new ResponseData<T>();

                        if (isMessage)
                        {
                            val.Message = res;
                            return val;
                        }
                        else
                        {
                            val.Data = JsonConvert.DeserializeObject<T>(res);
                            return val;
                        }
                    }
                }
            }
        }

    }
}
