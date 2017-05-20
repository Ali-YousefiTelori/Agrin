using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public enum FlowDirection
    {
        LeftToRight,
        RightToLeft
    }

    public static class ViewsUtility
    {
        public static Action<object> NavigateToPage { get; set; }
        public static Action RemoveCurrentPage { get; set; }

        private static FlowDirection _ProjectDirection = FlowDirection.RightToLeft;
        public static FlowDirection ProjectDirection
        {
            get { return _ProjectDirection; }
            set { _ProjectDirection = value; }
        }

        public static string ApplicationLanguage = "_fa";
    }
}
