using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Data.Math
{
    public static class SizeMath
    {


        public static double GetSizeCenter(double size, long buffer)
        {
            long t = (long)size / 2;
            return t - (t % buffer);
        }

        //public static List<ILinkThreadInfo> SortByPosition(FastCollection<ILinkThreadInfo> collactions)
        //{
        //    List<ILinkThreadInfo> col = new List<ILinkThreadInfo>(collactions);
        //    int n = collactions.Count - 1;
        //    for (int i = 0; i < n; i++)
        //    {
        //        for (int j = 0; j < n - i; j++)
        //        {
        //            if (col[j].StartPosition > col[j + 1].StartPosition)
        //            {
        //                ILinkThreadInfo temp = col[j];
        //                col[j] = col[j + 1];
        //                col[j + 1] = temp;
        //            }
        //        }
        //    }
        //    return col;
        //}

        //public static void SetLimitSize(LinkInfo linkInfo)
        //{
        //    List<ThreadConnectionDownload> limitingList = new List<ThreadConnectionDownload>();
        //    TimeSpan timeS = new TimeSpan(0, 0, 0, 1);
        //    foreach (var item in linkInfo.ThreadLinks)
        //    {
        //        item.TimeSecound = timeS;
        //        if (item.Status == StatusEnum.Downloading)
        //            limitingList.Add(item);
        //    }

        //    if (limitingList.Count == 0)
        //    {
        //        limitingList = null;
        //        return;
        //    }

        //    int lim = linkInfo.LimitSizePerTime / limitingList.Count;
        //    for (int i = 0; i < limitingList.Count - 1; i++)
        //    {
        //        limitingList[i].LimitSizePerTime = lim;
        //    }
        //    limitingList[limitingList.Count - 1].LimitSizePerTime = linkInfo.LimitSizePerTime / limitingList.Count + (linkInfo.LimitSizePerTime % limitingList.Count);
        //    limitingList.Clear();
        //    limitingList = null;
        //}

        public static TimeSpan getTimeSpan(double buffer, double downSize, TimeSpan time)
        {
            if (downSize > buffer)
            {
                double get = downSize - buffer;
                get = buffer / get;
                long l = (long)(10000000 + (10000000 / get));
                TimeSpan retTime = new TimeSpan(l);
                return retTime;
            }
            return time;
        }
    }
}
