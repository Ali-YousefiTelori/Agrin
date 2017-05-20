using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrin.IO.Strings
{
    public static class Text
    {
        public static string GetTextBetweenTwoValue(string content, string str1, string str2, bool singleLine = true)
        {
            RegexOptions pattern;
            if (singleLine)
            {
                pattern = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline;
            }
            else
            {
                pattern = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace;
            }
            Regex regex = new Regex(str1 + "(.*)" + str2, pattern);
            var v = regex.Match(content);
            return v.Groups[1].ToString();
        }
        /// <summary>
        /// A B C AA BB CC ABC, value must > 0
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntToLetters(int value)
        {
            string result = string.Empty;
            while (--value >= 0)
            {
                result = (char)('A' + value % 26) + result;
                value /= 26;
            }
            return result;
        }
    }
}
