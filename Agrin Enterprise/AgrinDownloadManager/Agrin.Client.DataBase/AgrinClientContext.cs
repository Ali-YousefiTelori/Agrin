using Agrin.Download.CoreModels.Link;
using Agrin.Models.Serialization.Link;
using CrazyMapper;
using LiteDB;
using SignalGo.Shared.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Client.DataBase
{
    public static class AgrinClientContext
    {
        static string _DataBasePath;

        public static string DataBasePath
        {
            get
            {
                return _DataBasePath;
            }
            set
            {
                _DataBasePath = value;
                DataBaseFilePath = Path.Combine(DataBasePath, "AgrinClientDatabase.db");
            }
        }

        internal static string DataBaseFilePath { get; set; } = "AgrinClientDatabase.db";

        public static ObservableCollection<LinkInfoCore> LinkInfoes { get; set; }

        public static ConcurrentList<LinkInfoCore> MainLoadedLinkInfoes { get; private set; } = new ConcurrentList<LinkInfoCore>();

        public static LinkInfoTable LinkInfoTable { get; set; } = new LinkInfoTable();
        
        public static List<TResult> MapList<T, TResult>(IEnumerable items)
        {
            return Mapper.Map<List<TResult>>(items);
        }
    }
}
