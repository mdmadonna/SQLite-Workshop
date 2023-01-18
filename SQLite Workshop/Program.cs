using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (IsRunning()) { Application.Exit(); Environment.Exit(0); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowCmd flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private enum ShowCmd
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9
        };

        private struct Windowplacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
            public Rectangle rcDevice;
        }

        /// <summary>
        /// Determine if SQLite Workshop is already running and, if yes,
        /// move the focus to the running application and close this
        /// instance
        /// </summary>
        /// <returns></returns>
        private static bool IsRunning()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            var process = processes.FirstOrDefault(p => p.Id != currentProcess.Id);
            if (process == null) return false;

            ActivateApplication(process.MainWindowHandle);
            return true;
        }

        /// <summary>
        /// Activate an application window, If it is minimized
        /// restore it to view.
        /// </summary>
        /// <param name="hWnd">handle of the window to activate</param>
        private static void ActivateApplication(IntPtr hWnd)
        {
            Windowplacement placement = new Windowplacement();
            GetWindowPlacement(hWnd, ref placement);
            if (placement.showCmd == (int)ShowCmd.SW_SHOWMINIMIZED)
                ShowWindow(hWnd, ShowCmd.SW_RESTORE);
            SetForegroundWindow(hWnd);
        }
    }
}
