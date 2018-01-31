using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Helpers
{
    public abstract class ApplicationResourceBase
    {
        public static ApplicationResourceBase Current { get; set; }
        public string LanguageFlag { get; set; }

        public abstract string GetAppResource(string key);
        public abstract string GetAppResource(string key, string lang);
    }
}
