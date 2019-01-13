using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UltraStreamGo;

namespace Agrin.IO.Helpers
{
    public class DirectoryMoveHelper
    {
        public DirectoryMoveHelper(string source, string target)
        {
            FileCount = CrossDirectoryInfo.Current.GetFiles(source, "*", SearchOption.AllDirectories).Length;
            _sourceDir = source;
            _targetDir = target;
        }

        public bool RollbackWhenError { get; set; } = true;

        public int FileCount { get; set; }

        string _sourceDir, _targetDir;
        /// <summary>
        /// sourceName,TargetName,Position
        /// </summary>
        public Action<string, string, int> MovedAction { get; set; }
        public Action CompleteAction { get; set; }
        public Action<Exception, Exception> ErrorMovingAction { get; set; }

        public void Move()
        {
            int position = 0;
            Thread task = new Thread(() =>
            {
                try
                {
                    MoveDir(_sourceDir, _targetDir, ref position);
                    CompleteAction?.Invoke();
                }
                catch (Exception ex)
                {
                    Exception rollbackError = null;
                    try
                    {
                        if (RollbackWhenError)
                            Rollback();
                    }
                    catch (Exception ex2)
                    {
                        rollbackError = ex2;
                    }
                    ErrorMovingAction?.Invoke(ex, rollbackError);
                }
            });
            task.IsBackground = false;
            task.Start();
        }

        void Rollback()
        {
            foreach (var item in deletedDirectory)
            {
                if (item.Item1 != null)
                    CrossDirectoryInfo.Current.CreateDirectory(item.Item1);
            }
            foreach (var item in movedItems)
            {
                if (item.Item3)
                    CrossFileInfo.Current.Copy(item.Item2, item.Item1);
                else
                    CrossFileInfo.Current.Move(item.Item2, item.Item1);
            }
            foreach (var item in deletedDirectory)
            {
                if (!item.Item3 && CrossDirectoryInfo.Current.Exists(item.Item2))
                    CrossDirectoryInfo.Current.Delete(item.Item2, true);
            }
        }

        List<Tuple<string, string, bool>> movedItems = new List<Tuple<string, string, bool>>();
        List<Tuple<string, string, bool>> deletedDirectory = new List<Tuple<string, string, bool>>();

        void MoveDir(string sourceFolder, string destFolder, ref int position)
        {
            bool existDir = CrossDirectoryInfo.Current.Exists(destFolder);
            Tuple<string, string, bool> tuple = new Tuple<string, string, bool>(null, destFolder, existDir);
            if (!existDir)
                CrossDirectoryInfo.Current.CreateDirectory(destFolder);
            deletedDirectory.Add(tuple);

            // Get Files & Copy
            string[] files = CrossDirectoryInfo.Current.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);

                // ADD Unique File Name Check to Below!!!!
                string dest = Path.Combine(destFolder, name);
                bool exist = CrossFileInfo.Current.Exists(dest);
                if (exist)
                    CrossFileInfo.Current.Delete(dest);
                CrossFileInfo.Current.Move(file, dest);
                movedItems.Add(new Tuple<string, string, bool>(file, dest, exist));
                position++;
                MovedAction?.Invoke(file, dest, position);
            }

            // Get dirs recursively and copy files
            string[] folders = CrossDirectoryInfo.Current.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                MoveDir(folder, dest, ref position);
            }
            CrossDirectoryInfo.Current.Delete(sourceFolder, true);
            deletedDirectory.Remove(tuple);
            tuple = new Tuple<string, string, bool>(sourceFolder, destFolder, existDir);
            deletedDirectory.Add(tuple);
        }
    }
}
