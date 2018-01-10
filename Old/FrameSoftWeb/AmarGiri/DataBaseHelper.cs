using Gita.DataBase.Extentions;
using Gita.DataBase.Foundation;
using Gita.DataBase.RDMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmarGiri
{
    public class DataBaseHelper
    {
        public static IDataBaseHelper CurrentDataBase
        {
            get
            {
                return QueryTable<DataBaseHelper>.GetCurrentDatabaseByType(typeof(DataBaseHelper));
            }
            set
            {
                QueryTable<DataBaseHelper>.AddOrEditDataBaseType(typeof(DataBaseHelper), value);
            }
        }
        static string _ApplicationDirectory = "def";

        public static string ApplicationDirectory
        {
            get
            {
                return DataBaseHelper._ApplicationDirectory;
            }
            set
            {
                DataBaseHelper._ApplicationDirectory = value;
            }
        }

        public string LoadDBException = "";

        static bool _isConnected = false;
        public static void LoadDataBase(string path)
        {
            try
            {
                if (_isConnected)
                    return;
                _isConnected = true;
                //ApplicationDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                ApplicationDirectory = path;// @"D:\irdms-src\irandmstest\newPRG\FrameSoftWeb\Library";
                CurrentDataBase = new SQLiteDataBase(System.IO.Path.Combine(ApplicationDirectory, "DataBase", "AmarDB.db"));
                SQLiteDataBase.AddDataBaseKey("AmarDataBase", CurrentDataBase);
            }
            catch(Exception e)
            {
                Gita.Infrastructure.Log.AutoLogger.LogError(e, "GetAmar");
            }
        }
    }
}
