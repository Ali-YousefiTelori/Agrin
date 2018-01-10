using Agrin.Helper.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Agrin.OS.Management
{
    public class WindowsController : IWindowsController
    {
        [DllImport("user32.dll")]
        public static extern void LockWorkStation();
        //[DllImport("user32.dll")]
        //public static extern int ExitWindowsEx(int uFlags, int dwReason);
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        public void SetPowerState(ComputerPowerState state)
        {
            switch (state)
            {
                case ComputerPowerState.Lock:
                    {
                        LockWorkStation();
                        break;
                    }
                case ComputerPowerState.Reboot:
                    {
                        DoExitWin(EWX_REBOOT);
                        break;
                    }
                case ComputerPowerState.ShutDown:
                    {
                        DoExitWin(EWX_SHUTDOWN);
                        System.Threading.Thread.Sleep(5000);
                        System.Diagnostics.Process process1 = new System.Diagnostics.Process();
                        string shut_args = " -s -f";

                        process1.StartInfo.FileName = " shutdown.exe";


                        process1.StartInfo.Arguments = shut_args;

                        process1.StartInfo.CreateNoWindow = true;
                        process1.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        process1.Start();
                        break;
                    }
                case ComputerPowerState.LogOff:
                    {
                        ExitWindowsEx((int)state, 0);
                        break;
                    }
                case ComputerPowerState.Hibernate:
                    {
                        SetSuspendState(true, true, true);
                        break;
                    }
                case ComputerPowerState.StandBy:
                    {
                        SetSuspendState(false, true, true);
                        break;
                    }

            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr
        phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name,
        ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool ExitWindowsEx(int flg, int rea);

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int EWX_LOGOFF = 0x00000000;
        internal const int EWX_SHUTDOWN = 0x00000001;
        internal const int EWX_REBOOT = 0x00000002;
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;

        private static void DoExitWin(int flg)
        {
            bool ok;
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            ok = ExitWindowsEx(flg, 0);
        }


        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        //public static bool IsConnectedToInternetLinux()
        //{
        //    System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
        //    try
        //    {
        //        IAsyncResult result = socket.BeginConnect("www.google.com", 80, null, null);
        //        //I set it for 3 sec timeout, but if you are on an internal LAN you can probably 
        //        //drop that down a little because this should be instant if it is going to work
        //        bool success = result.AsyncWaitHandle.WaitOne(3000, true);

        //        if (!success)
        //        {
        //            return false;
        //        }
        //        return true;
        //        // Success
        //        //... 
        //    }
        //    finally
        //    {
        //        //You should always close the socket!
        //        socket.Close();
        //    }
        //}
    }
}
