using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models
{
    /// <summary>
    /// status of connection or link
    /// </summary>
    public enum ConnectionStatus : byte
    {
        /// <summary>
        /// نامشخص
        /// </summary>
        None = 0,
        /// <summary>
        /// when link is stopped
        /// </summary>
        Stoped = 1,

        #region Downloading
        /// <summary>
        /// when link is creating request
        /// </summary>
        CreatingRequest = 2,
        /// <summary>
        /// when link is connecting
        /// </summary>
        Connecting = 3,
        /// <summary>
        /// when link is downloading
        /// </summary>
        Downloading = 4,
        /// <summary>
        /// when after complete is copying file to disk
        /// </summary>
        CopyingFile = 5,
        /// <summary>
        /// when link in sharing sites is connecting
        /// </summary>
        ConnectingToSharing = 6,
        /// <summary>
        /// when link in sharing sites is login in
        /// </summary>
        LoginToSharing = 7,
        /// <summary>
        /// when link in sharing sites is get address from request
        /// </summary>
        GetAddressFromSharing = 8,
        /// <summary>
        /// when link in sharing sites is finding address from response
        /// </summary>
        FindingAddressFromSharing = 9,

        #endregion

        /// <summary>
        /// when link is complete
        /// </summary>
        Complete = 10,
        /// <summary>
        /// when link has going to error
        /// </summary>
        Error = 11,
    }

    /// <summary>
    /// resumable support for a link
    /// </summary>
    public enum ResumeCapabilityEnum : byte
    {
        /// <summary>
        /// support resumable
        /// </summary>
        Yes = 0,
        /// <summary>
        /// dont support resumable
        /// </summary>
        No = 1,
        /// <summary>
        /// unknown
        /// </summary>
        Unknown = 2
    }

    public enum CheckStatus : byte
    {
        Unknown = 0,
        Busy = 1,
        Success = 2,
        Error = 3
    }

    public enum SizeEnum
    {
        Byte = 0,
        KB = 1,
        MB = 2,
        GB = 3,
        TB = 4
    }
}
