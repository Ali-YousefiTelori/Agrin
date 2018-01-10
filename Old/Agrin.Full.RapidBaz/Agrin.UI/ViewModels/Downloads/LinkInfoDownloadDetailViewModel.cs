using Agrin.Download.Web;
using Agrin.Helper.ComponentModel;
using Agrin.UI.Views.Downloads;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.UI.ViewModels.Downloads
{
    public class LinkInfoDownloadDetailViewModel : ANotifyPropertyChanged<LinkInfoDownloadDetail>
    {
        #region Constructors
        public LinkInfoDownloadDetailViewModel()
        {

        }

        #endregion

        #region Commands

        #endregion

        #region Events

        #endregion

        #region Fields

        #endregion

        #region Properties

        LinkInfo _CurrentLinkInfo;
        public LinkInfo CurrentLinkInfo
        {
            get { return _CurrentLinkInfo; }
            set
            {
                _CurrentLinkInfo = value;
                OnPropertyChanged("CurrentLinkInfo");
            }
        }

        #endregion

        #region Methods

        #endregion

        #region CommandsMethods

        #endregion
    }
}
