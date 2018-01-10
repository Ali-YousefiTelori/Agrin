using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Helper.Converters
{
    public enum MonoSizeEnum
    {
        Byte = 0,
        KB = 1,
        MB = 2,
        GB = 3,
        TB = 4,
        PB = 5,
        EXB = 6,
        Unknown
    }
    public static class MonoConverters
    {
        public static string GetSizeStringEnum(double Size)
        {
            if (Size < 0)
                return MonoSizeEnum.Unknown.ToString();
            int i = 0;
            for (i = 0; i < 7; i++)
            {
                if (Size >= 1024)
                    Size /= 1024;
                else
                    break;
            }
            return String.Format("{0:0.000}", Size) + " " + ((MonoSizeEnum)i).ToString();
        }

        public static string[] GetSizeStringSplit(double Size)
        {
            string[] str = new string[] { "", "" };
            if (Size < 0)
            {
                str[1] = MonoSizeEnum.Unknown.ToString();
                return str;
            }

            int i = 0;
            for (i = 0; i < 7; i++)
            {
                if (Size >= 1024)
                    Size /= 1024;
                else
                    break;
            }
            str[0] = String.Format("{0:0.000}", Size);
            str[1] = ((MonoSizeEnum)i).ToString();
            return str;
        }
    }
}
