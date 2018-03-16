using Agrin.ComponentModels;
using Agrin.Download.ShortModels.Link;
using Agrin.IO;
using Agrin.Web.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agrin.Download.CoreModels.Link
{
    /// <summary>
    /// path and addresses of link
    /// </summary>
    public class LinkInfoPathCore: NotifyPropertyChanged
    {
        volatile LinkInfoShort _LinkInfo;

        volatile string _MainUriAddress;

        volatile string _AppDirectorySavePath;
        volatile string _UserDirectorySavePath;
        volatile string _SecurityDirectorySavePath;

        volatile string _AppFileName;
        volatile string _UserFileName;
        volatile string _SecurityFileName;
        volatile string _MixerSavePath;
        volatile string _SecurityMixerSavePath;
        volatile string _BackUpMixerSavePath;


        /// <summary>
        /// uri address of link
        /// </summary>
        public string MainUriAddress { get => _MainUriAddress; set => _MainUriAddress = value; }

        /// <summary>
        /// parent of link info
        /// </summary>
        public LinkInfoShort LinkInfo { get => _LinkInfo; set => _LinkInfo = value; }

        /// <summary>
        /// default path from link address
        /// </summary>
        internal string DefaultDirectorySavePath
        {
            get
            {
                return PathHelper.DownloadsPath;//review
            }
        }
        /// <summary>
        /// application save path of link ()
        /// </summary>
        internal string AppDirectorySavePath
        {
            get
            {
                return PathHelper.CreateDirectoryIfNotExist(_AppDirectorySavePath);
            }

            set => _AppDirectorySavePath = value;
        }
        /// <summary>
        /// user save path of link
        /// </summary>
        public string UserDirectorySavePath
        {
            get
            {
                return PathHelper.CreateDirectoryIfNotExist(_UserDirectorySavePath);
            }
            set => _UserDirectorySavePath = value;
        }
        /// <summary>
        /// security save path of link
        /// </summary>
        public string SecurityDirectorySavePath
        {
            get
            {
                return PathHelper.CreateSecurityDirectoryIfNotExist(_SecurityDirectorySavePath);
            }
            set => _SecurityDirectorySavePath = value;
        }
        /// <summary>
        /// directory save path of link
        /// </summary>
        public string DirectorySavePath
        {
            get
            {
                if (!string.IsNullOrEmpty(SecurityDirectorySavePath))
                    return SecurityDirectorySavePath;
                else if (!string.IsNullOrEmpty(UserDirectorySavePath))
                    return UserDirectorySavePath;
                else if (!string.IsNullOrEmpty(AppDirectorySavePath))
                    return AppDirectorySavePath;
                return DefaultDirectorySavePath;
            }
        }

        /// <summary>
        /// default file name from link address
        /// </summary>
        internal string DefaultFileName
        {
            get
            {
                return Decodings.FullDecodeString(PathHelper.GetFileNameFromUrl(MainUriAddress));
            }
        }

        /// <summary>
        /// when application set file name of link
        /// </summary>
        internal string AppFileName { get => _AppFileName; set => _AppFileName = value; }
        /// <summary>
        /// when user set file name of link
        /// </summary>
        public string UserFileName { get => _UserFileName; set => _UserFileName = value; }
        /// <summary>
        /// when security file name is set
        /// </summary>
        public string SecurityFileName { get => _SecurityFileName; set => _SecurityFileName = value; }

        /// <summary>
        /// file name of link
        /// </summary>
        public string FileName
        {
            get
            {
                if (!string.IsNullOrEmpty(SecurityFileName))
                    return SecurityFileName;
                else if (!string.IsNullOrEmpty(UserFileName))
                    return UserFileName;
                else if (!string.IsNullOrEmpty(AppFileName))
                    return AppFileName;
                return DefaultFileName;
            }
        }

        /// <summary>
        /// save directory with save file name
        /// </summary>
        public string FullSaveAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(SecurityDirectorySavePath) || !string.IsNullOrEmpty(SecurityFileName))
                    return PathHelper.CombineSecurityPath(SecurityDirectorySavePath, SecurityFileName);
                return PathHelper.Combine(DirectorySavePath, FileName);
            }
        }

        /// <summary>
        /// security temp save linkinfo path before complete link
        /// </summary>
        public string SecurityTemporarySavePath
        {
            get
            {
                if (!string.IsNullOrEmpty(Agrin.Models.Settings.ApplicationSettingsInfo.Current.PathSettings.TemporarySecurityLinkInfoSavePath))
                    return PathHelper.CombineSecurityPathWithNoSecurity(Agrin.Models.Settings.ApplicationSettingsInfo.Current.PathSettings.TemporaryLinkInfoSavePath, LinkInfo.Id.ToString());
                return null;
            }
        }

        /// <summary>
        /// temp save linkinfo path before complete link
        /// </summary>
        public string TemporarySavePath
        {
            get
            {
                var securityPath = SecurityTemporarySavePath;
                if (!string.IsNullOrEmpty(securityPath))
                    return securityPath;
                return PathHelper.Combine(Agrin.Models.Settings.ApplicationSettingsInfo.Current.PathSettings.TemporaryLinkInfoSavePath, LinkInfo.Id.ToString());
            }
        }

        ///// <summary>
        ///// saved mixer path
        ///// mixer is mixing splited downloaded parts of file connections to one file
        ///// </summary>
        //public string MixerSavePath { get => _MixerSavePath; set => _MixerSavePath = value; }
        ///// <summary>
        ///// security of saved mixer path
        ///// mixer is mixing splited downloaded parts of file connections to one file
        ///// </summary>
        //public string SecurityMixerSavePath { get => _SecurityMixerSavePath; set => _SecurityMixerSavePath = value; }
        ///// <summary>
        ///// backup of saved mixer path
        ///// mixer is mixing splited downloaded parts of file connections to one file
        ///// if path is security can replace with this
        ///// </summary>
        //public string BackUpMixerSavePath { get => _BackUpMixerSavePath; set => _BackUpMixerSavePath = value; }
    }
}
