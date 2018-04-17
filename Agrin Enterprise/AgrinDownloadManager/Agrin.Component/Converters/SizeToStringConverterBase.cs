using Agrin.Models;
using MvvmGo.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Converters
{
    public class SizeToStringConverterBase
    {
        public string GetSizeString(double Size, int Digits)
        {
            if (Size < 0)
                return ApplicationResourceBase.Current.GetAppResource("Unknown");
            int i = 0;
            for (i = 0; i < 6; i++)
            {
                if (Size >= 1024)
                    Size /= 1024;
                else
                    break;
            }

            var perfix = "";
            if (IsEnglish)
                perfix = ApplicationResourceBase.Current.GetAppResource(((SizeEnum)i).ToString(), "_en");
            else
                perfix = ApplicationResourceBase.Current.GetAppResource(((SizeEnum)i).ToString());


            return String.Format("{0:0." + new String('0', Digits) + "}", Size) + " " + perfix;
        }

        public string GetSizeString(object value)
        {
            double val = 0;
            if (value != null)
            {
                double.TryParse(value.ToString(), out val);
            }

            string size = GetSizeString(val, Digits);
            size = IsPerSecound ? size + "/s" : size;
            return size;
        }


        bool _IsPerSecound;
        public bool IsPerSecound
        {
            get { return _IsPerSecound; }
            set { _IsPerSecound = value; }
        }

        int _Digits = 2;
        public int Digits
        {
            get { return _Digits; }
            set { _Digits = value; }
        }

        public bool IsEnglish { get; set; } = true;
    }
}
