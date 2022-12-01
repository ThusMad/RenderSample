using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Scalpio.Core
{
    public static class NativeMethods
    {
        public const uint GW_HWNDNEXT = 2;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
    }
}
