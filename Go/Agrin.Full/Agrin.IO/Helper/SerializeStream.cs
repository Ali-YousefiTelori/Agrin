using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json;
using Agrin.IO.Streams;

namespace Agrin.IO.Helper
{
    public static class SerializeStream
    {
        //public static string XmlSerializer(object toSerialize)
        //{
        //    XmlWriterSettings settings = new XmlWriterSettings();
        //    settings.Indent = true;
        //    settings.NewLineOnAttributes = true;
        //    settings.ConformanceLevel = ConformanceLevel.Fragment;
        //    StringBuilder sb = new StringBuilder();
        //    using (XmlWriter writer = XmlWriter.Create(sb, settings))
        //    {
        //        XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer);
        //        manager.XamlWriterMode = XamlWriterMode.Expression;
        //        XamlWriter.Save(toSerialize, manager);
        //        return sb.ToString();
        //    }
        //}

        //public static object XmlDeserializer(string xamlText)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(xamlText);
        //    XamlReader.Load(new XmlNodeReader(doc));
        //    return null;
        //}

        //public static void SaveXmlSerializerByFile(string fileName, object value)
        //{
        //    using (MemoryStream mstream = new MemoryStream(StringToBytes(XmlSerializer(value))))
        //    {
        //        using (FileStream compress = new FileStream(fileName, FileMode.Create))
        //        {
        //            using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(compress, System.IO.Compression.CompressionMode.Compress))
        //            {
        //                mstream.Seek(0, SeekOrigin.Begin);
        //                byte[] bytes = new byte[mstream.Length];
        //                int read = mstream.Read(bytes, 0, (int)mstream.Length);
        //                fs.Write(bytes, 0, read);
        //            }
        //        }
        //    }
        //}

        //public static object OpenXmlSerializerByFile(string fileName)
        //{
        //    using (FileStream compress = new FileStream(fileName, FileMode.Open))
        //    {
        //        using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(compress, System.IO.Compression.CompressionMode.Decompress))
        //        {
        //            using (MemoryStream stream = new MemoryStream())
        //            {
        //                fs.CopyTo(stream);
        //                return XmlDeserializer(BytesToString(stream.ToArray()));
        //            }
        //        }
        //    }
        //}

        //public static byte[] XmlByteSerializer<T>(T toSerialize)
        //{
        //    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        //    using (MemoryStream sb = new MemoryStream())
        //    {
        //        serializer.Serialize(sb, toSerialize);
        //        return sb.ToArray();
        //    }
        //}

        //public static T XmlByteDeserializer<T>(byte[] xamlByte)
        //{
        //    System.Xml.XmlReader reader = new System.Xml.XmlTextReader(new MemoryStream(xamlByte));
        //    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        //    //string text = System.Text.UTF8Encoding.UTF8.GetString(xamlByte);
        //    //object val = XmlDeserializer(text.Replace("FileMessage", "Agrin_Engine.Models.AgrinSocket.FileMessage"));
        //    return (T)serializer.Deserialize(reader);
        //}

        //public static T XmlByteDeserializer<T>(MemoryStream xamlByte)
        //{
        //    System.Xml.XmlReader reader = new System.Xml.XmlTextReader(xamlByte);
        //    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        //    //string text = System.Text.UTF8Encoding.UTF8.GetString(xamlByte);
        //    //object val = XmlDeserializer(text.Replace("FileMessage", "Agrin_Engine.Models.AgrinSocket.FileMessage"));
        //    return (T)serializer.Deserialize(reader);
        //}

        //public static void SaveXmlByteSerializerByFile<T>(string fileName, T value)
        //{
        //    using (MemoryStream mstream = new MemoryStream(XmlByteSerializer<T>(value)))
        //    {
        //        using (var compress = IOHelperBase.Current.OpenFileStreamForWrite(fileName, FileMode.Create))
        //        {
        //            using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(compress.GetStream(), System.IO.Compression.CompressionMode.Compress))
        //            {
        //                mstream.Seek(0, SeekOrigin.Begin);
        //                byte[] bytes = new byte[mstream.Length];
        //                int read = mstream.Read(bytes, 0, (int)mstream.Length);
        //                fs.Write(bytes, 0, read);
        //            }
        //        }
        //    }
        //}

        //public static T OpenXmlByteSerializerByFile<T>(string fileName, T value)
        //{
        //    using (var compress = IOHelperBase.Current.OpenFileStreamForRead(fileName, FileMode.Open))
        //    {
        //        using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(compress, System.IO.Compression.CompressionMode.Decompress))
        //        {
        //            using (MemoryStream stream = new MemoryStream())
        //            {
        //                fs.CopyTo(stream);
        //                return XmlByteDeserializer<T>(stream);
        //            }
        //        }
        //    }
        //}

        static MemoryStream Serialize(object value)
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            formatter.Serialize(stream, value);
            return stream;
        }

        static object Deserialize(Stream stream)
        {
            var formatter = new BinaryFormatter();
            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            stream.Seek(0, SeekOrigin.Begin);
            return formatter.Deserialize(stream);
        }

        public static void SaveSerializeStream(string fileName, object value)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });
            System.IO.File.WriteAllText(fileName, json, Encoding.UTF8);

            //using (MemoryStream mstream = Serialize(value))
            //{
            //    using (var compress = IOHelperBase.Current.OpenFileStreamForWrite(fileName, FileMode.Create))
            //    {
            //        using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(compress.GetStream(), System.IO.Compression.CompressionMode.Compress))
            //        {
            //            mstream.Seek(0, SeekOrigin.Begin);
            //            byte[] bytes = new byte[mstream.Length];
            //            int read = mstream.Read(bytes, 0, (int)mstream.Length);
            //            fs.Write(bytes, 0, read);
            //        }
            //    }
            //}
        }
        //public static Action<Exception> ExceptionHandle { get; set; }
        public static T OpenSerializeStream<T>(string fileName) where T : class
        {
            try
            {
                if (!System.IO.File.Exists(fileName))
                    return null;
                var text = System.IO.File.ReadAllText(fileName, Encoding.UTF8);
                try
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
                    if (data != null)
                        return data;
                }
                catch
                {

                }
                using (var compress = IOHelperBase.Current.OpenFileStreamForRead(fileName, FileMode.Open))
                {
                    if (compress.Length > 0)
                        using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(new NormalStream(compress), System.IO.Compression.CompressionMode.Decompress))
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                fs.CopyTo(stream);
                                return (T)Deserialize(stream);
                            }
                        }
                    else
                        return null;
                }
            }
            catch (Exception e)
            {
                //ExceptionHandle?.Invoke(e);
                Agrin.Log.AutoLogger.LogError(e, "OpenSerializeStream", true);
                return null;
            }
        }

        //public static object DeSerializeBytesObject(byte[] value)
        //{
        //    if (value == null || value.Length == 0)
        //        return null;
        //    using (MemoryStream compress = new MemoryStream(value))
        //    {
        //        using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(compress, System.IO.Compression.CompressionMode.Decompress))
        //        {
        //            using (MemoryStream stream = new MemoryStream())
        //            {
        //                fs.CopyTo(stream);
        //                return Deserialize(stream);
        //            }
        //        }
        //    }
        //}

        //public static byte[] SerializeObjectBytes(object value)
        //{
        //    if (value == null)
        //        return new byte[] { };
        //    byte[] allBytes;
        //    using (MemoryStream mstream = Serialize(value))
        //    {
        //        using (MemoryStream compress = new MemoryStream())
        //        {
        //            using (System.IO.Compression.GZipStream fs = new System.IO.Compression.GZipStream(compress, System.IO.Compression.CompressionMode.Compress))
        //            {
        //                mstream.Seek(0, SeekOrigin.Begin);
        //                byte[] bytes = new byte[mstream.Length];
        //                int read = mstream.Read(bytes, 0, (int)mstream.Length);
        //                fs.Write(bytes, 0, read);
        //            }
        //            allBytes = compress.ToArray();
        //        }
        //    }
        //    return allBytes;
        //}

        //public static object DeSerializeNotCompressBytesObject(byte[] value)
        //{
        //    if (value == null || value.Length == 0)
        //        return null;
        //    using (MemoryStream stream = new MemoryStream(value))
        //    {
        //        return Deserialize(stream);
        //    }
        //}

        //public static byte[] SerializeNotCompressObjectBytes(object value)
        //{
        //    if (value == null)
        //        return new byte[] { };
        //    using (MemoryStream stream = Serialize(value))
        //    {
        //        return stream.ToArray();
        //    }
        //}

        //public static byte[] GetStreamBytes(Stream stream)
        //{
        //    stream.Seek(0, SeekOrigin.Begin);
        //    BinaryReader br = new BinaryReader(stream);
        //    return br.ReadBytes((int)stream.Length);
        //}

        //public static byte[] StringToBytes(string text)
        //{
        //    if (text == null)
        //        return new byte[] { };
        //    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        //    return encoding.GetBytes(text);
        //}

        //public static string BytesToString(byte[] data)
        //{
        //    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        //    return encoding.GetString(data);
        //}
    }
}
