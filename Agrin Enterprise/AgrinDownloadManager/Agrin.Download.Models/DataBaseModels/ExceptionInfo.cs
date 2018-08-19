using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Agrin.Download.DataBaseModels
{
    /// <summary>
    /// exceptions of links
    /// </summary>
    public class ExceptionInfo
    {
        /// <summary>
        /// id
        /// </summary>
        [BsonId]
        public int Id { get; set; }
        /// <summary>
        /// id of link
        /// </summary>
        public int LinkId { get; set; }
        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Link.ToString message
        /// </summary>
        public string FullMessage { get; set; }
        /// <summary>
        /// count of this message detected
        /// </summary>
        public int CountOfError { get; set; }
        /// <summary>
        /// last time error getted
        /// </summary>
        public DateTime LastDateTimeErrorDetected { get; set; }
        /// <summary>
        /// uniq hash code for errors
        /// </summary>
        public string HashCode { get; set; }
        /// <summary>
        /// version of application logged this
        /// </summary>
        public string AppVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AppOS { get; set; }
        /// <summary>
        /// version of operation system logged this
        /// </summary>
        public string OSVersion { get; set; }
        /// <summary>
        /// device name
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// calculateHashCode
        /// </summary>
        /// <returns></returns>
        public string CalculateHash()
        {
            string input = Message + FullMessage;
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
