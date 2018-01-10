using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Logger
    {
        public static void WriteLine(object data)
        {
            Console.WriteLine(data);
        }

        public static void WriteLine(string title, object data)
        {
            WriteLine(title);
            WriteLine(data);
        }
    }
}
