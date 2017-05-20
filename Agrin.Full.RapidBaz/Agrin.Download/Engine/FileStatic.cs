using System;
using System.Collections.Generic;
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
                fileName = System.IO.Path.GetFileName(decode);

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
            else if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
                return fileName + ".html";
            return fileName;
        }
    }
}
