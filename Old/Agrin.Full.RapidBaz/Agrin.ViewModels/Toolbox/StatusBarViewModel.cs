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
        public static Action<double, double, bool, bool,bool> ProgressChanged { get; set; }
        public StatusBarViewModel()
        {
            This = this;
            bool isNormal = false;
            TimeDownloadEngine.TotalItemsChanged = (totalSpeed, totalMaximumProgress, totalProgressValue, isConnecting, isError, complete) =>
            {
                TotalApplicationSpeed = totalSpeed;
                if (isConnecting)
                {
                    isNormal = false;
                    TaskbarProgress.SetState(TaskbarProgress.TaskbarStates.Indeterminate);
                }
                else if (isError)
                {
                    isNormal = false;
                    TaskbarProgress.SetState(TaskbarProgress.TaskbarStates.Error);
                }
                else
                {
                    if (!isNormal)
                        TaskbarProgress.SetState(TaskbarProgress.TaskbarStates.Normal);
                    isNormal = true;
                }
                TaskbarProgress.SetValue(totalProgressValue, totalMaximumProgress);
                if (ProgressChanged != null)
                    ProgressChanged(totalMaximumProgress, totalProgressValue, isConnecting, isError, complete);
            };
        }
    }
}
