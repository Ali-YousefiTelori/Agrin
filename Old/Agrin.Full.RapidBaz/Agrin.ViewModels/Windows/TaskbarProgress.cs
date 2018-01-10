using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Agrin.ViewModels.Windows
{
    

    [ComImportAttribute()]
    [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITaskbarList3
    {
        // ITaskbarList
        [PreserveSig]
        void HrInit();
        [PreserveSig]
        void AddTab(IntPtr hwnd);
        [PreserveSig]
        void DeleteTab(IntPtr hwnd);
        [PreserveSig]
        void ActivateTab(IntPtr hwnd);
        [PreserveSig]
        void SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2
        [PreserveSig]
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3
        [PreserveSig]
        void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
        [PreserveSig]
        void SetProgressState(IntPtr hwnd, Agrin.ViewModels.Windows.TaskbarProgress.TaskbarStates state);
    }

    [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    [ComImportAttribute()]
    public class TaskbarInstance
    {
    }

    public static class TaskbarProgress
    {
        static TaskbarProgress()
        {
            if (TaskbarSupported)
            {
                taskbarInstance = (ITaskbarList3)new TaskbarInstance();
            }
        }

        static Window _MainWindow;

        public static Window MainWindow
        {
            get { return TaskbarProgress._MainWindow; }
            set
            {
                TaskbarProgress._MainWindow = value;
                intropper = new WindowInteropHelper(value);
            }
        }

        public enum TaskbarStates
        {
            NoProgress = 0,
            Indeterminate = 0x1,
            Normal = 0x2,
            Error = 0x4,
            Paused = 0x8
        }

        private static ITaskbarList3 taskbarInstance = null;
        public static bool TaskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);
        static WindowInteropHelper intropper = null;
        public static void SetState(TaskbarStates taskbarState)
        {
            if (TaskbarSupported && MainWindow != null) MainWindow.Dispatcher.BeginInvoke(new Action(() => taskbarInstance.SetProgressState(intropper.Handle, taskbarState)));
        }

        public static void SetValue(double progressValue, double progressMax)
        {
            if (TaskbarSupported && MainWindow != null) MainWindow.Dispatcher.BeginInvoke(new Action(() => taskbarInstance.SetProgressValue(intropper.Handle, (ulong)progressValue, (ulong)progressMax)));
        }
    }
}
