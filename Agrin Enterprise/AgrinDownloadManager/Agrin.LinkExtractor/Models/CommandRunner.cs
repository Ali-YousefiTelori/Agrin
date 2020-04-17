using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.LinkExtractor.Models
{
    public static class CommandRunner
    {
        public static bool CanExecute(string link)
        {
            if (Uri.TryCreate(link, UriKind.Absolute, out Uri uri))
            {

            }

            return false;
        }

        public static Task<LinkExtractResult> Execute(string link)
        {
            return null;
        }
    }
}
