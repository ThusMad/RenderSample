using System;
using System.Runtime.InteropServices;

namespace Scalpio.Render.Helpers {
    public static class NativeMethods {
        public const int
         WS_CHILD = 0x40000000,
         WS_VISIBLE = 0x10000000,
         LBS_NOTIFY = 0x00000001,
         HOST_ID = 0x00000002,
         LISTBOX_ID = 0x00000001,
         WS_VSCROLL = 0x00200000,
         WS_EX_LAYERED = 0x00080000,
         WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
         WS_EX_TRANSPARENT = 0x00000020,
         WS_BORDER = 0x00800000,
         WS_OVERLAPPED =
             0x00000000 |
             0x00C00000 |
             0x00080000 |
             0x00040000 |
             0x00020000 |
             0x00010000;


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public Int32 X;
            public Int32 Y;
        }

        [DllImport("user32.dll")]
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT point);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(int dwExStyle,
                                              string lpszClassName,
                                              string lpszWindowName,
                                              int style,
                                              int x, int y,
                                              int width, int height,
                                              IntPtr hwndParent,
                                              IntPtr hMenu,
                                              IntPtr hInst,
                                              [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        public static extern bool DestroyWindow(IntPtr hwnd);
    }
}
