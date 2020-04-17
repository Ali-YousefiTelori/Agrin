using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UltraStreamGo;

namespace Agrin.Log
{
    public class AutoLogger
    {
        public static string AppVersion { get; set; }
        public static string AppOS { get; set; }
        public static string OSVersionNumber { get; set; }
        public static string OSVersionName { get; set; }
        public static string DeviceName { get; set; }

        public static string ApplicationDirectory;
        public static string LogText(string text)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("<Text Log Start>");
            str.AppendLine($"App Version: {AppVersion}");
            str.AppendLine($"App OS: {AppOS}");
            str.AppendLine($"OS Version Name: {OSVersionName}");
            str.AppendLine($"OS Version Number: {OSVersionNumber}");
            str.AppendLine($"Device Name: {DeviceName}");
            str.AppendLine(text);
            str.AppendLine(DateTime.Now.ToString());
            str.AppendLine("<Text Log End>");
            string fileName = Path.Combine(ApplicationDirectory, "Error Logs.log");
            try
            {
                lock (logObject)
                {
                    using (var stream = CrossFileInfo.Current.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        stream.Seek(0, SeekOrigin.End);
                        byte[] bytes = Encoding.UTF8.GetBytes(System.Environment.NewLine + str.ToString());
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch
            {

            }
            return str.ToString();
        }

        static object logObject = new object();
        public static string LogError(Exception e, string title)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                lock (logObject)
                {
                    str.AppendLine(title);
                    str.AppendLine($"App Version: {AppVersion}");
                    str.AppendLine($"App OS: {AppOS}");
                    str.AppendLine($"OS Version Name: {OSVersionName}");
                    str.AppendLine($"OS Version Number: {OSVersionNumber}");
                    str.AppendLine($"Device Name: {DeviceName}");
                    str.AppendLine("--------------------To String--------------------");
                    str.AppendLine(e.ToString());
                    str.AppendLine("--------------------To String--------------------");
                    str.AppendLine("Time : " + DateTime.Now.ToString());
                    str.AppendLine("--------------------------------------------------------------------------------------------------");
                    str.AppendLine("--------------------------------------------------------------------------------------------------");
                    string fileName = Path.Combine(ApplicationDirectory, "Error Logs.log");

                    using (var stream = CrossFileInfo.Current.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        stream.Seek(0, SeekOrigin.End);
                        byte[] bytes = Encoding.UTF8.GetBytes(System.Environment.NewLine + str.ToString());
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch
            {

            }
            return str.ToString();
        }
    }
}
