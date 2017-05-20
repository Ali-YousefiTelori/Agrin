using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data
{
    [Serializable]
    public class ApplicationServiceData
    {
        public static ApplicationServiceData Current { get; set; }
        List<int> _isPlayLinks = null;

        public List<int> IsPlayLinks
        {
            get
            {
                return _isPlayLinks;
            }
            set { _isPlayLinks = value; }
        }

        static object lockOBJ = new object();
        public static void AddItem(int id)
        {
            lock (lockOBJ)
            {
                if (ApplicationServiceData.Current.IsPlayLinks.Contains(id))
                    return;
                ApplicationServiceData.Current.IsPlayLinks.Add(id);
                SerializeData.SaveAppServiceDataToFile();
            }
        }

        public static void RemoveItem(int id)
        {
            lock (lockOBJ)
            {
                if (!ApplicationServiceData.Current.IsPlayLinks.Contains(id))
                    return;
                ApplicationServiceData.Current.IsPlayLinks.Remove(id);
                SerializeData.SaveAppServiceDataToFile();
            }
        }
    }
}
