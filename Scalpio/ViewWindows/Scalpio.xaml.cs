using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using System.Windows.Forms;
using System.Drawing;
using Scalpio.Controls.Terminal.Common;
using Scalpio.Controls.Data.Interfaces;

namespace Scalpio.ViewWindows
{
    public partial class Scalpio : Window, IMainWindow
    {

        private bool mRestoreIfMove = false;
        //public WindowPreferences Preferences { get; set; }

        public Scalpio() {
            //Preferences = new WindowPreferences();

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged; 
            this.Title += System.Windows.Application.Current.Windows.OfType<Scalpio>().Count();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLimit();
            RenderOptions.ProcessRenderMode = RenderMode.Default;

        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            ChangeLimit();
        }

        private void ChangeLimit()
        {
            var interopHelper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            var activeScreen = Screen.FromHandle(interopHelper.Handle);

            double screenWidth = activeScreen.WorkingArea.Width;
            double screenHeight = activeScreen.WorkingArea.Height;

            this.MaxHeight = screenHeight;
            this.MaxWidth = screenWidth;

//#if DEBUG
//            ((App)System.Windows.Application.Current).GetDebugInfo().upd_Log($"Monitor Changed! \nNew working area! \nMaxHeight: {this.MaxHeight} \nMaxWidth: {this.MaxWidth} ");
//#endif          
        }


        private void CloseClick(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Application.Current.Windows.OfType<Scalpio>().Count() != 1)
            {
                this.Close();
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void ChangeStateClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.BorderThickness = new System.Windows.Thickness(0);
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.BorderThickness = new System.Windows.Thickness(8);
            }
                
        }

        private void MinimizeClick(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowProc));
        }

        private static System.IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }


        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);

            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }

            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }

            Marshal.StructureToPtr(lMmi, lParam, true);
        }


        private void SwitchWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }

        private void rctHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                {
                    SwitchWindowState();
                }

                return;
            }

            else if (WindowState == WindowState.Maximized)
            {
                mRestoreIfMove = true;
                return;
            }

            DragMove();
        }

        private void rctHeader_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mRestoreIfMove = false;
        }


        private void rctHeader_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (mRestoreIfMove)
            {
                mRestoreIfMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;

                WindowState = WindowState.Normal;

                POINT lMousePosition;
                GetCursorPos(out lMousePosition);

                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;

                DragMove();
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }


        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }

        public void Over()
        {
            //Status.Text = "Over";
        }

        public void NotOver()
        {
            //Status.Text = "Not Over";
        }

        public IEnumerable<TradingFrame> GetTradingFrame()
        {
            return FindVisualChildren<TradingFrame>(this);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

    }
}
