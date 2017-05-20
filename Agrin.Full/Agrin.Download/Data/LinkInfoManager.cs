using Agrin.Download.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Download.Data
{
    public static class LinkInfoManager
    {
        public static Exception AddLink(string uri)
        {
            try
            {
                LinkInfo info = new LinkInfo(uri);
                //AgrinApplicationData.This.LinkInfoes.Add(info);
                SaveData();
            }
            catch (Exception e)
            {
                return e;
            } 
            return null;
        }

        public static Exception AddRangeLink(List<string> links)
        {
            try
            {
                List<LinkInfo> items = new List<LinkInfo>();
                foreach (var item in links)
                {
                    LinkInfo info = new LinkInfo(item);
                    items.Add(info);
                }

                //AgrinApplicationData.This.LinkInfoes.AddRange(items);
                SaveData();
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }

        static void SaveData()
        {
            SerializeData.SaveDataToFile();
        }
    }
}