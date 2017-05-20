using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agrin.Helper.ComponentModel
{
    public enum ComputerPowerState
    {
        //LogOff = 0,
        Reboot = 2,
        ShutDown = 1,
        LogOff = 4,
        Lock = 5,
        Hibernate = 6,
        StandBy = 7,
        None = 8
    }

    public interface IWindowsController
    {
        void SetPowerState(ComputerPowerState state);
    }


    public static class WindowsControllerBase
    {
        public static IWindowsController Current { get; set; }
        public static void SetPowerState(ComputerPowerState state)
        {
            Current.SetPowerState(state);
        }
    }
}
