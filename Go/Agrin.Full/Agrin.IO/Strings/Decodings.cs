using System;
using System.Text;

namespace Agrin.IO.Strings
{
    public static class Decodings
    {
        public static string FullDecodeString(string txt)
        {
            if (String.IsNullOrEmpty(txt))
                return "";
#if(MobileApp || __ANDROID__)
            if (String.IsNullOrEmpty(txt))
                return txt;
            return HtmlDecoding(UrlDecode(txt.Trim()));
#else
            if (String.IsNullOrEmpty(txt))
                return txt;
            return HtmlDecoding(UrlDecode(System.Net.WebUtility.HtmlDecode(txt.Trim())));
#endif
        }

        public static string HtmlDecoding(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("&lt;", "<");
                value = value.Replace("&gt;", ">");
                value = value.Replace("&nbsp;", " ");
                value = value.Replace("&quot;", "\"");
                value = value.Replace("&#39;", "\'");
                value = value.Replace("&amp;", "&");
                //value = value.Replace("/", "\\");
                return value;
            }
            return string.Empty;
        }

        public static string UrlDecode(string value)
        {
            if (value == null)
            {
                return null;
            }
            int length = value.Length;
            UrlDecoder decoder = new UrlDecoder(length,Encoding.UTF8);
            for (int i = 0; i < length; i++)
            {
                char ch = value[i];
                if (ch == '+')
                {
                    ch = ' ';
                }
                else if ((ch == '%') && (i < (length - 2)))
                {
                    if ((value[i + 1] == 'u') && (i < (length - 5)))
                    {
                        int num3 = HttpEncoderUtility.HexToInt(value[i + 2]);
                        int num4 = HttpEncoderUtility.HexToInt(value[i + 3]);
                        int num5 = HttpEncoderUtility.HexToInt(value[i + 4]);
                        int num6 = HttpEncoderUtility.HexToInt(value[i + 5]);
                        if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
                        {
                            goto Label_010B;
                        }
                        ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                        i += 5;
                        decoder.AddChar(ch);
                        continue;
                    }
                    int num7 = HttpEncoderUtility.HexToInt(value[i + 1]);
                    int num8 = HttpEncoderUtility.HexToInt(value[i + 2]);
                    if ((num7 >= 0) && (num8 >= 0))
                    {
                        byte b = (byte)((num7 << 4) | num8);
                        i += 2;
                        decoder.AddByte(b);
                        continue;
                    }
                }
            Label_010B:
                if ((ch & 0xff80) == 0)
                {
                    decoder.AddByte((byte)ch);
                }
                else
                {
                    decoder.AddChar(ch);
                }
            }
            return decoder.GetString();
        }
    }
}
