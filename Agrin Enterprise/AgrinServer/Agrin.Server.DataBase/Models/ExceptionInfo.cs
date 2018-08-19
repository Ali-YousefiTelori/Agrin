using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agrin.Server.DataBase.Models
{
    /// <summary>
    /// exception info is helpfull for users to know how to fix problems
    /// </summary>
    public class ExceptionInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// if exception was webexception this will be fill for error code like 404 503
        /// </summary>
        public int? HttpErrorCode { get; set; }
        /// <summary>
        /// static error code for Agrin Application that must be a library to show an error for example error code 1 is object null reference
        /// </summary>
        public int? ErrorCode { get; set; }
        /// <summary>
        /// type of exception like system.ioexception
        /// </summary>
        public string ExceptionType { get; set; }
        /// <summary>
        /// error message is ex.Message
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// help url is a url for users to find a help for this exception
        /// </summary>
        public string HelpUrl { get; set; }
    }
}
