using Agrin.BaseViewModels.Models;
using Agrin.BaseViewModels.Toolbox;
using Agrin.Download.Engine;
using Agrin.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.ViewModels.Toolbox
{
    public class StatusBarViewModel : StatusBarBaseViewModel
    {
        public static StatusBarViewModel This { get; set; }

        public static Action<LinkStatusInfo> ProgressChanged { get; set; }

        public StatusBarViewModel()
        {
            This = this;
            bool isNormal = false;
            LinkStatusInfo status = new LinkStatusInfo();
            TimeDownloadEngine.TotalItemsChanged = (totalSpeed, totalMaximumProgress, totalProgressValue, isConnecting, isError, complete) =>
            {
                TotalApplicationSpeed = totalSpeed;
                if (isConnecting)
                {
                    isNormal = false;
                    TaskbarProgress.SetState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Paused);
                }
                else if (isError)
                {
                    isNormal = false;
                    TaskbarProgress.SetState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Error);
                }
                else
                {
                    if (!isNormal)
                        TaskbarProgress.SetState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal);
                    isNormal = true;
                }
                TaskbarProgress.SetValue(totalProgressValue, totalMaximumProgress);
                if (ProgressChanged != null)
                {
                    status.IsComplete = complete;
                    status.IsConnecting = isConnecting;
                    status.IsError = isError;
                    status.TotalMaximumProgress = totalMaximumProgress;
                    status.TotalProgressValue = totalProgressValue;
                    status.TotalSpeed = totalSpeed;
                    ProgressChanged(status);
                }
            };
        }
    }
}
