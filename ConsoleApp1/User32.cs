using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MarketingPlatform.Client.NativeMethods
{
    class User32
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                  int x, int y, int width, int height, uint flags);

        [DllImport("USER32.DLL")]

        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);



        //Sets a window to be a child window of another window

        [DllImport("USER32.DLL")]

        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);



        //Sets window attributes

        [DllImport("USER32.DLL")]

        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);



        //Gets window attributes

        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);



        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);



        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);


        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);



        /// <summary>Enumeration of the different ways of showing a window using

        /// ShowWindow</summary>

        public enum WindowShowStyle : uint

        {

            /// <summary>Hides the window and activates another window.</summary>

            /// <remarks>See SW_HIDE</remarks>

            Hide = 0,

            /// <summary>Activates and displays a window. If the window is minimized

            /// or maximized, the system restores it to its original size and

            /// position. An application should specify this flag when displaying

            /// the window for the first time.</summary>

            /// <remarks>See SW_SHOWNORMAL</remarks>

            ShowNormal = 1,

            /// <summary>Activates the window and displays it as a minimized window.</summary>

            /// <remarks>See SW_SHOWMINIMIZED</remarks>

            ShowMinimized = 2,

            /// <summary>Activates the window and displays it as a maximized window.</summary>

            /// <remarks>See SW_SHOWMAXIMIZED</remarks>

            ShowMaximized = 3,

            /// <summary>Maximizes the specified window.</summary>

            /// <remarks>See SW_MAXIMIZE</remarks>

            Maximize = 3,

            /// <summary>Displays a window in its most recent size and position.

            /// This value is similar to "ShowNormal", except the window is not

            /// actived.</summary>

            /// <remarks>See SW_SHOWNOACTIVATE</remarks>

            ShowNormalNoActivate = 4,

            /// <summary>Activates the window and displays it in its current size

            /// and position.</summary>

            /// <remarks>See SW_SHOW</remarks>

            Show = 5,

            /// <summary>Minimizes the specified window and activates the next

            /// top-level window in the Z order.</summary>

            /// <remarks>See SW_MINIMIZE</remarks>

            Minimize = 6,

            /// <summary>Displays the window as a minimized window. This value is

            /// similar to "ShowMinimized", except the window is not activated.</summary>

            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>

            ShowMinNoActivate = 7,

            /// <summary>Displays the window in its current size and position. This

            /// value is similar to "Show", except the window is not activated.</summary>

            /// <remarks>See SW_SHOWNA</remarks>

            ShowNoActivate = 8,

            /// <summary>Activates and displays the window. If the window is

            /// minimized or maximized, the system restores it to its original size

            /// and position. An application should specify this flag when restoring

            /// a minimized window.</summary>

            /// <remarks>See SW_RESTORE</remarks>

            Restore = 9,

            /// <summary>Sets the show state based on the SW_ value specified in the

            /// STARTUPINFO structure passed to the CreateProcess function by the

            /// program that started the application.</summary>

            /// <remarks>See SW_SHOWDEFAULT</remarks>

            ShowDefault = 10,

            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread

            /// that owns the window is hung. This flag should only be used when

            /// minimizing windows from a different thread.</summary>

            /// <remarks>See SW_FORCEMINIMIZE</remarks>

            ForceMinimized = 11

        }

        /*
        * Window field offsets for GetWindowLong()
        */
        public const int GWL_WNDPROC = -4;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_HWNDPARENT = -8;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int GWL_USERDATA = -21;
        public const int GWL_ID = -12;


        //assorted constants needed

        public const int WS_OVERLAPPED = 0x00000000;
        public const int WS_POPUP = unchecked((int)0x80000000);
        public const int WS_CHILD = 0x40000000; //child window
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;

        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000; // window with a title bar  /* WS_BORDER | WS_DLGFRAME  */
        public const int WS_BORDER = 0x00800000;  // window with border
        public const int WS_DLGFRAME = 0x00400000; // window with double border but no title
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_GROUP = 0x00020000;
        public const int WS_TABSTOP = 0x00010000;

        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;

        public const int WS_EX_DLGMODALFRAME = 0x0001;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int WM_SETICON = 0x0080;

        public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_ACCEPTFILES = 0x00000010;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;

        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTREADING = 0x00002000;
        public const int WS_EX_TRREADING = 0x00000000;
        public const int WS_EX_EFTSCROBAR = 0x00004000;
        public const int WS_EX_RIGHTSCROBAR = 0x00000000;

        public const int WS_EX_CONTROPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;

        public const int WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const int WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const int WS_EX_LAYERED = 0x00080000;

        [StructLayout(LayoutKind.Sequential)]

        public struct RECT

        {

            public int Left;

            public int Top;

            public int Right;

            public int Bottom;

        }



        [DllImport("user32.dll")]

        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
    }
}
