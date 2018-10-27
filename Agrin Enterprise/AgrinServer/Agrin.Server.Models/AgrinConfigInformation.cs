using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.Models
{
    /// <summary>
    /// server config
    /// </summary>
    public class AgrinConfigInformation
    {
        static AgrinConfigInformation()
        {
            Current = JsonConvert.DeserializeObject<AgrinConfigInformation>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")));
        }
        /// <summary>
        /// current config
        /// </summary>
        public static AgrinConfigInformation Current { get; set; }
        /// <summary>
        /// path of file system storage
        /// </summary>
        public string FileStoragePath { get; set; }
        /// <summary>
        /// sms server domain
        /// </summary>
        public string SMSSenderDomain { get; set; }
        /// <summary>
        /// sms server port
        /// </summary>
        public int SMSSenderPort { get; set; }
        /// <summary>
        /// username of sms server
        /// </summary>
        public string SMSServerUserName { get; set; }
        /// <summary>
        /// psasword of sms server
        /// </summary>
        public string SMSServerPassword { get; set; }

    }
}
