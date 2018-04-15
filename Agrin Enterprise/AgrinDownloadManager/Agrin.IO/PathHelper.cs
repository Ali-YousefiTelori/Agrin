using Agrin.Web.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO
{
    /// <summary>
    /// path helper class for file management and folder management in OS
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// get home dir path
        /// </summary>
        public static Func<string> GetHomeDirectoryAction { get; set; }
        static volatile string _ApplicationBasePath;
        static volatile string _ApplicationTemporaryPath;
        static volatile string _DownloadsPath;

        /// <summary>
        /// application installed path
        /// </summary>
        public static string ApplicationBasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_ApplicationBasePath))
                    throw new NotImplementedException("ApplicationBasePath in Agrin.IO.PathHelper is not initialized!");
                return _ApplicationBasePath;
            }
        }

        /// <summary>
        /// download manager temp save path befor complete links
        /// محل ذخیره ی موقت فایل ها قبل از دانلود
        /// </summary>
        public static string ApplicationTemporaryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_ApplicationTemporaryPath))
                    throw new NotImplementedException("ApplicationTemporaryPath in Agrin.IO.PathHelper is not initialized!");
                return _ApplicationTemporaryPath;
            }
        }
        /// <summary>
        /// downloads path in link info
        /// </summary>
        public static string DownloadsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_DownloadsPath))
                    throw new NotImplementedException("DownloadsPath in Agrin.IO.PathHelper is not initialized!");
                return _DownloadsPath;
            }
        }

        /// <summary>
        /// initialize default path of application
        /// </summary>
        /// <param name="applicationBasePath">application installed folder for save links and groups and ... files to user folder</param>
        /// <param name="applicationTemporaryPath">application temp folder befor download going to complete</param>
        /// <param name="downloadsPath">downloads path of OS</param>
        public static void Initialize(string applicationBasePath, string applicationTemporaryPath, string downloadsPath)
        {
            _ApplicationBasePath = applicationBasePath;
            _ApplicationTemporaryPath = applicationTemporaryPath;
            _DownloadsPath = downloadsPath;

            if (!Directory.Exists(applicationBasePath))
                Directory.CreateDirectory(applicationBasePath);
            if (!Directory.Exists(applicationTemporaryPath))
                Directory.CreateDirectory(applicationTemporaryPath);
            if (!Directory.Exists(downloadsPath))
                Directory.CreateDirectory(downloadsPath);
        }

        /// <summary>
        /// create folder if not exist
        /// </summary>
        /// <param name="path">path directory</param>
        /// <returns>return path address after created</returns>
        public static string CreateDirectoryIfNotExist(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        /// <summary>
        /// create security folder if not exist
        /// </summary>
        /// <param name="path">path directory</param>
        /// <returns>return path address after created</returns>
        public static string CreateSecurityDirectoryIfNotExist(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            //if (!Directory.Exists(path))
            //    Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// get file name of path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
        /// <summary>
        /// Get FileName From ContentDisposition
        /// </summary>
        /// <param name="contentDisposition"></param>
        /// <returns></returns>
        public static string GetFileNameFromContentDisposition(string contentDisposition)
        {
            try
            {
                contentDisposition = contentDisposition.Replace("utf-8", "").Replace("UTF-8", "").Replace("'", "");
                contentDisposition = GetFileNameValidChar(contentDisposition);
                if (contentDisposition == "attachment" || contentDisposition == "attachment;")
                    return "";
                return (new System.Net.Mime.ContentDisposition(contentDisposition)).FileName.Trim(new char[] { '"' });
            }
            catch (Exception c)
            {
                Agrin.Log.AutoLogger.LogError(c, "GetFileNameFromContentDisposition 0 example: " + contentDisposition);

                string txt = contentDisposition;
                try
                {
                    txt = contentDisposition.Substring(contentDisposition.IndexOf("filename=") + 9);
                    if (txt.IndexOf("\"") == 0)
                    {
                        txt = txt.Remove(0, 1);
                        txt = txt.Substring(0, txt.IndexOf("\""));
                    }
                    else if (txt.Contains(";"))
                    {
                        txt = txt.Substring(0, txt.IndexOf(";"));
                    }
                }
                catch (Exception e)
                {
                    Agrin.Log.AutoLogger.LogError(e, "GetFileNameFromContentDisposition 1");
                }


                txt = GetFileNameValidChar(txt);
                return txt.Trim(new char[] { '"' });
            }
        }

        public static string GetLinksFileName(string newFileName)
        {
            if (string.IsNullOrEmpty(newFileName))
                return newFileName;
            string decode = Decodings.FullDecodeString(newFileName.Trim().Trim(new char[] { '/', '\\' }));
            string fileName = null;
            if (Uri.TryCreate(decode, UriKind.Absolute, out Uri uri))
            {
                fileName = Decodings.FullDecodeString(System.IO.Path.GetFileName(uri.AbsolutePath));
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Decodings.FullDecodeString(System.IO.Path.GetFileName(decode));
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
                fileName = Path.GetFileName(GetFileNameValidChar(decode));

            foreach (var item in Path.GetInvalidPathChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            foreach (var item in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
            if (string.IsNullOrEmpty(fileName))
                return "notName.html";
            else if (string.IsNullOrEmpty(GetFileExtention(fileName)))
                return fileName.Trim(new char[] { '"' }) + ".html";
            return fileName.Trim(new char[] { '"' });
        }

        public static string GetFileExtention(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                return GetFileNameValidChar(Path.GetExtension(uri.AbsolutePath));
            }
            return GetFileNameValidChar(Path.GetExtension(url));
        }

        public static string GetFileNameFromUrl(string url)
        {
            string fileName = "";
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                fileName = GetFileNameValidChar(Path.GetFileName(uri.AbsolutePath));
            }
            string ext = "";
            if (!string.IsNullOrEmpty(fileName))
            {
                ext = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(ext))
                    ext = ".html";
                else
                    ext = "";
                return GetFileNameValidChar(fileName + ext);

            }

            fileName = Path.GetFileName(url);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "noName";
            }
            ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext))
                ext = ".html";
            else
                ext = "";
            fileName = fileName + ext;
            if (!fileName.StartsWith("?"))
                fileName = fileName.Split('?').FirstOrDefault();
            fileName = fileName.Split('&').LastOrDefault().Split('=').LastOrDefault();
            return GetFileNameValidChar(fileName);
        }

        public static string GetFileNameValidChar(string fileName)
        {
            foreach (var item in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
#if(MobileApp)
            foreach (var item in androidinvalidChars)
            {
                fileName = fileName.Replace(item.ToString(), "");
            }
#endif
            return fileName;
        }
        /// <summary>
        /// combine two security Path and security file name
        /// </summary>
        /// <param name="paths">security path</param>
        /// <returns>combined path</returns>
        public static string CombineSecurityPath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        /// combine two security Path and security file name
        /// </summary>
        /// <param name="securityPath">security path</param>
        /// <param name="noSecurityPath">no security path</param>
        /// <returns>combined path</returns>
        public static string CombineSecurityPathWithNoSecurity(string securityPath, string noSecurityPath)
        {
            return Path.Combine(securityPath, noSecurityPath);
        }

        /// <summary>
        /// combine two path and file name
        /// </summary>
        /// <param name="paths">paths</param>
        /// <returns>combined path</returns>
        public static string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public static bool EqualPath(string path1, string path2)
        {
            if (path1 == null || path2 == null)
                return path1 == path2;
            if (Path.IsPathRooted(path1) && Path.IsPathRooted(path2))
                return Path.GetFullPath(path1).Equals(Path.GetFullPath(path2));
            return false;
        }

        public static bool FileExist(string filename)
        {
            return System.IO.File.Exists(filename);
        }
    }
}
