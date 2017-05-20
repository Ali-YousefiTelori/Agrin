using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrin.Data.Code
{
    public class CodeLuncher
    {
        public static void LunchCSCode(string site, string typeName, string methosName)
        {

#if(MobileApp)

#else
            var provider = CSharpCodeProvider.CreateProvider("c#");
            var options = new CompilerParameters();
            System.Net.WebClient wbc = new System.Net.WebClient();
            wbc.Proxy = null;
            string text = wbc.DownloadString(site);

            foreach (var item in GetRefrences(text))
            {
                options.ReferencedAssemblies.Add(item);
            }
            string code = GetCode(text);
            var results = provider.CompileAssemblyFromSource(options, new[] { code });
            if (results.Errors.Count > 0)
            {
                foreach (var error in results.Errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                var t = results.CompiledAssembly.GetType(typeName);
                t.GetMethod(methosName).Invoke(null, null);
            }
#endif

        }
        static string[] GetRefrences(string text)
        {
            Regex regExp = new Regex("<Refrences>(.*?)</Refrences>",RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            string str = regExp.Match(text).Groups[1].Value;
            List<string> retText = new List<string>();
            foreach (var item in str.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                retText.Add(item);
            }
            return retText.ToArray();
        }

        public static string GetCode(string text)
        {
            Regex regExp = new Regex("<CSharpCode>(.*?)</CSharpCode>",RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            string str = regExp.Match(text).Groups[1].Value;
            return str.Trim();
        }
    }
}
