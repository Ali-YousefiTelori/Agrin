using Agrin.IO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO
{
    public static class FileStatic
    {
        public static string GetLinksFileName(string newFileName)
        {
            if (string.IsNullOrEmpty(newFileName))
                return newFileName;
            string decode = Agrin.IO.Strings.Decodings.FullDecodeString(newFileName.Trim().Trim(new char[] { '/', '\\' }));
            Uri uri = null;
            string fileName = null;
            if (Uri.TryCreate(decode, UriKind.Absolute, out uri))
            {
                fileName = Agrin.IO.Strings.Decodings.FullDecodeString(System.IO.Path.GetFileName(uri.AbsolutePath));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Agrin.IO.Strings.Decodings.FullDecodeString(System.IO.Path.GetFileName(decode));
                    if (fileName.Contains("="))
                    {
                        int l = fileName.LastIndexOf('=');
                        if (l < fileName.Length - 1)
                        {
                            string name = fileName.Substring(l + 1, fileName.Length - l - 1);
                            if (!string.IsNullOrEmpty(name))
                                fileName = name;
                        }

                    }
                }
            }
            else
                fileName = System.IO.Path.GetFileName(Agrin.IO.Helper.MPath.GetFileNameValidChar(decode));

            foreach (var item in System.IO.Path.GetInvalidPathChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            if (string.IsNullOrEmpty(fileName))
                return "notName.html";
            else if (string.IsNullOrEmpty(MPath.GetFileExtention(fileName)))
                return fileName + ".html";
            return fileName;
        }

        static Dictionary<string, byte[]> icons = new Dictionary<string, byte[]>();

        static object loclOBJ = new object();
        public static byte[] GetFileIcon(string extension)
        {
            lock (loclOBJ)
            {
                extension = extension.Trim(new char[] { '.' });
                if (icons.ContainsKey(extension))
                    return icons[extension];
                string iconDirectories = Path.Combine(Agrin.IO.Helper.MPath.CurrentUserAppDirectory, "ExtIcons");
                if (!Directory.Exists(iconDirectories))
                    CrossDirectoryInfo.Current.CreateDirectory(iconDirectories);
                string fileName = Path.Combine(iconDirectories, extension + ".png");
                if (System.IO.File.Exists(fileName))
                {
                    FileInfo file = new FileInfo(fileName);
                    if (file.Length < 1024 * 1024 * 1)
                    {
                        return IOHelperBase.Current.ReadAllBytes(fileName);
                    }
                }
                var icon = Framesoft.Helper.FileManagerHelper.GetIconByFileExtention(extension);
                if (icon.Data != null && icon.Data.Length > 0)
                {
                    IOHelperBase.Current.WriteAllBytes(fileName, icon.Data);
                    icons.Add(extension, icon.Data);
                }
                return icon.Data;
            }
        }
    }
}
