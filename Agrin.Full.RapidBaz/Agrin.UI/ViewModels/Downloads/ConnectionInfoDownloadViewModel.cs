using Agrin.Download.Web.Link;
using Agrin.Helper.ComponentModel;
using Agrin.ViewModels.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agrin.UI.ViewModels.Downloads
{
    public class ConnectionInfoDownloadViewModel
    {
        public ConnectionInfoDownloadViewModel()
        {
            PlayCommand = new RelayCommand<LinkWebRequest>(PlayConnectionInfo, CanPlayConnectionInfo);
            StopCommand = new RelayCommand<LinkWebRequest>(StopConnectionInfo, CanStopConnectionInfo);
        }

        public RelayCommand<LinkWebRequest> PlayCommand { get; private set; }
        public RelayCommand<LinkWebRequest> StopCommand { get; private set; }

        private bool CanPlayConnectionInfo(LinkWebRequest connectionInfo)
        {
            if (connectionInfo != null && connectionInfo.CommandBindingChanged == null)
            {
                connectionInfo.CommandBindingChanged = () =>
                   {
                       ApplicationHelper.EnterDispatcherThreadActionBegin(() =>
                           {
                               CommandManager.InvalidateRequerySuggested();
                           });
                   };
            }
            if (connectionInfo == null)
                return false;
            return connectionInfo.CanPlay;
        }
        private void PlayConnectionInfo(LinkWebRequest connectionInfo)
        {
            connectionInfo.Play(true);
        }

        private bool CanStopConnectionInfo(LinkWebRequest connectionInfo)
        {
            if (connectionInfo == null)
                return false;
            return connectionInfo.CanStop;
        }

        private void StopConnectionInfo(LinkWebRequest connectionInfo)
        {
            connectionInfo.Stop(false);
        }
    }
}
