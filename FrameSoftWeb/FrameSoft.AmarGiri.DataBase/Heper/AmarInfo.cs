using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameSoft.AmarGiri.DataBase.Heper
{
    public class AmarInfo
    {
        public int OnlineCount { get; set; }
        public int OnlineApplicationCount { get; set; }

        public int TodayApplicationCount { get; set; }
        public long TodayCount { get; set; }
        public int TodayIPCount { get; set; }
        public int TodayInstallCount { get; set; }

        public int YesterdayApplicationCount { get; set; }
        public long YesterdayCount { get; set; }
        public int YesterdayIPCount { get; set; }
        public int YesterdayInstallCount { get; set; }

        public int LastWeekApplicationCount { get; set; }
        public long LastWeekCount { get; set; }
        public int LastWeekIPCount { get; set; }
        public int LastWeekInstallCount { get; set; }

        public int LastMonthApplicationCount { get; set; }
        public long LastMonthCount { get; set; }
        public int LastMonthIPCount { get; set; }
        public int LastMonthInstallCount { get; set; }

        public int TotalApplicationAndInstallCount { get; set; }
        public long TotalCount { get; set; }
        public int TotalIPCount { get; set; }
    }
}
