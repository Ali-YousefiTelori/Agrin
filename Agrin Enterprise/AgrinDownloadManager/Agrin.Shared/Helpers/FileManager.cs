using Framesoft.Helpers.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.Shared.Helpers
{
    public static class FileManager
    {
        public static string DataBaseFolder { get; set; }

        /// <summary>
        /// ساخت یک پوشه در صورت وجود نداشتن
        /// </summary>
        /// <param name="directory">پوشه ی مورد نظر</param>
        /// <param name="fileName">نام فایل</param>
        /// <returns>ادغام فایل و پوشه</returns>
        static string CreateDirectoryIfNotExist(string directory, string fileName = null)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (string.IsNullOrEmpty(fileName))
                return directory;
            return Path.Combine(directory, fileName);
        }

        static string GetFirstFolderName(int value)
        {
            return RangeOfInteger.GetStartOfRange(100000, value) + "-" + RangeOfInteger.GetEndOfRange(100000, value);
        }

        static string GetSecondFolderName(int value)
        {
            return RangeOfInteger.GetStartOfRange(1000, value) + "-" + RangeOfInteger.GetEndOfRange(1000, value);
        }

        public static string GetPostImageDirectory(int userId, int postId, string fileName)
        {
            return CreateDirectoryIfNotExist(Path.Combine(DataBaseFolder, "Images", "PostImages", GetFirstFolderName(userId), GetSecondFolderName(userId), userId.ToString(), postId.ToString()), fileName);
        }

        public static string GetAgrinApplicationDirectory(string operationSystem)
        {
            return CreateDirectoryIfNotExist(Path.Combine(DataBaseFolder, "AgrinApplication", operationSystem));
        }
    }

}
