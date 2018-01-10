using Agrin.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Models.Settings
{
    /// <summary>
    /// setting of path and file address
    /// </summary>
    public class ApplicationPathSettingsInfo
    {
        volatile string _TemporaryLinkInfoSavePath;
        volatile string _TemporarySecurityLinkInfoSavePath;

        volatile string _DownloadsSavePath;

        /// <summary>
        /// temp path before download going to comlete state
        /// </summary>
        public string TemporaryLinkInfoSavePath
        {
            get
            {
                if (!string.IsNullOrEmpty(TemporarySecurityLinkInfoSavePath))
                    return TemporarySecurityLinkInfoSavePath;
                if (string.IsNullOrEmpty(_TemporaryLinkInfoSavePath))
                    _TemporaryLinkInfoSavePath = PathHelper.ApplicationTemporaryPath;
                return PathHelper.CreateDirectoryIfNotExist(_TemporaryLinkInfoSavePath);
            }
            set => _TemporaryLinkInfoSavePath = value;
        }

        /// <summary>
        /// security temp path before download going to comlete state
        /// </summary>
        public string TemporarySecurityLinkInfoSavePath
        {
            get
            {
                return PathHelper.CreateSecurityDirectoryIfNotExist(_TemporarySecurityLinkInfoSavePath);
            }
            set => _TemporarySecurityLinkInfoSavePath = value;
        }

        /// <summary>
        /// os downloads path after link complete and save parts of connection to this address
        /// </summary>
        public string DownloadsSavePath
        {
            get
            {
                if (string.IsNullOrEmpty(_DownloadsSavePath))
                    _DownloadsSavePath = PathHelper.ApplicationTemporaryPath;
                return PathHelper.CreateDirectoryIfNotExist(_DownloadsSavePath);
            }
            set => _DownloadsSavePath = value;
        }
    }
}
