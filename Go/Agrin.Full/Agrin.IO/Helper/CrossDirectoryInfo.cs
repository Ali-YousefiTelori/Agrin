using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.IO.Helper
{
    /// <summary>
    /// cross platform directory helper
    /// </summary>
    public class CrossDirectoryInfo
    {
        static CrossDirectoryInfo _Current;
        public static CrossDirectoryInfo Current
        {
            get
            {
                if (_Current == null)
                    _Current = new CrossDirectoryInfo();
                return _Current;
            }
            set
            {
                _Current = value;
            }
        }

        public virtual bool Exist(string path)
        {
            return Directory.Exists(path);
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        public virtual void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        public virtual void Delete(string path, bool recurcive)
        {
            System.IO.Directory.Delete(path, recurcive);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public string[] GetFiles(string sourceFolder)
        {
            return Directory.GetFiles(sourceFolder);
        }

        public string[] GetDirectories(string sourceFolder)
        {
            return Directory.GetDirectories(sourceFolder);
        }
    }
}
