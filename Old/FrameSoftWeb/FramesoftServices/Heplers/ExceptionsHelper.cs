using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Framesoft.Services.Heplers
{
    public static class ExceptionsHelper
    {
        public static Func<Exception, string> GetSqlExceptionLog { get; set; }

        public static string GetFullString(this Exception ex)
        {
            if (GetSqlExceptionLog != null)
            {
                var sqlLog = GetSqlExceptionLog(ex);
                if (!string.IsNullOrEmpty(sqlLog))
                    return sqlLog;
            }
            return GetAllInner(ex);
        }

        static string GetTab()
        {
            return "	";
        }

        static string GetAllInner(Exception e)
        {
            string msg = "Start Exception:" + System.Environment.NewLine + "{" + System.Environment.NewLine;
            string tabs = "";
            int i = 0;
            while (e != null)
            {
                tabs = GetTab();
                var doubleTab = tabs + GetTab();
                msg += tabs + (i == 0 ? "Exception" : "Inner") + System.Environment.NewLine + tabs + "{" + System.Environment.NewLine + doubleTab + GetTextMessageFromException(e, doubleTab) + System.Environment.NewLine + tabs + "}" + System.Environment.NewLine;
                e = e.InnerException;
                i++;
            }
            return msg + System.Environment.NewLine + "}" + System.Environment.NewLine + "End Exception";
        }

        static string GetTextMessageFromException(Exception e, string tabs)
        {
            if (e == null)
                return "No Exception";
            else
            {
                if (e.Message == null)
                {
                    if (e.StackTrace == null)
                    {
                        return "Exception is Null or Empty (Logger)";
                    }
                    else
                    {
                        return "FaghatStack :" + GetText(e.StackTrace, tabs);
                    }
                }
                else
                {
                    if (e.StackTrace == null)
                    {
                        return "Stack Is Null But Message:" + GetText(e.Message, tabs);
                    }
                    else
                    {
                        return "Message:" + GetText(e.Message, tabs) + System.Environment.NewLine + tabs + "Stack:  " + GetText(e.StackTrace, tabs);
                    }
                }
            }
        }

        static string GetText(string value, string tabs)
        {
            var array = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                    text.AppendLine(GetTab() + array[i]);
                else
                    text.AppendLine(tabs + GetTab() + array[i]);
            }
            return text.ToString();
        }
    }
}