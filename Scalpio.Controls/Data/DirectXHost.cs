using Scalpio.Render.DirectX;
using Scalpio.Render.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Interop;

namespace Scalpio.Controls.Data
{
    class DirectXHost : HwndHost
    {
        public bool IsVisible = true;

        private Point relativePoint;

        public int _Width;
        public int _Height;

        public FrameworkElement ContentBox { get; private set; }
        public Window ParentWnd;

        public IntPtr Win32Handle { get; private set; }
        private IntPtr ParentHwnd { get; set; }

        public DXGraphics2DWrapper DXGraphics2DWrapper { get; private set; }

        public DirectXHost(FrameworkElement contentBox, Window parentWindow)
        {
            ContentBox = contentBox;
            ParentWnd = parentWindow;

            ParentHwnd = ((HwndSource)System.Windows.PresentationSource.FromVisual(parentWindow)).Handle;

            _Width = (int)ContentBox.ActualWidth;
            _Height = (int)ContentBox.ActualHeight;

            SnapsToDevicePixels = true;

            relativePoint = ContentBox.TransformToAncestor(ParentWnd)
                             .Transform(new Point(0, 0));

            Win32Handle = NativeMethods.CreateWindowEx(NativeMethods.WS_EX_TRANSPARENT, "static", "",
                         NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE | NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED,
                         (int)relativePoint.X, (int)relativePoint.Y,
                         _Width, _Height,
                         ParentHwnd,
                         (IntPtr)NativeMethods.HOST_ID,
                         IntPtr.Zero,
                         0);

            unsafe
            {
                DXGraphics2DWrapper = new DXGraphics2DWrapper();
                DXGraphics2DWrapper.Initialize(Win32Handle);
                Debug.WriteLine(Win32Handle);
            }
        }

        public void ShowWindow()
        {
            if (!IsVisible)
            {
                NativeMethods.ShowWindow(Win32Handle, 5);
                IsVisible = true;
            }
        }

        public void HideWindow()
        {
            if(IsVisible)
            {
                NativeMethods.ShowWindow(Win32Handle, 0);
                IsVisible = false;
            }
        }

        public void ChangeSize(SizeChangedEventArgs e)
        {
            relativePoint = ContentBox.TransformToAncestor(ParentWnd)
                                        .Transform(new Point(0, 0));

            _Width = (int)e.NewSize.Width;
            _Height = (int)e.NewSize.Height;

            NativeMethods.MoveWindow(Win32Handle, (int)relativePoint.X, (int)relativePoint.Y, _Width, _Height, false);
           

            Task.Run(() =>
            {
                if (DXGraphics2DWrapper != null)
                {
                    lock (DXGraphics2DWrapper)
                    {
                        DXGraphics2DWrapper?.Resize(_Width, _Height);
                    }
                }

            });
        }

        public void ChangeSize(double width, double height)
        {
            relativePoint = ContentBox.TransformToAncestor(ParentWnd)
                                        .Transform(new Point(0, 0));

            _Width = (int)width;
            _Height = (int)height;

            NativeMethods.MoveWindow(Win32Handle, (int)relativePoint.X, (int)relativePoint.Y, _Width, _Height, false);


            Task.Run(() =>
            {
                if (DXGraphics2DWrapper != null)
                {
                    lock (DXGraphics2DWrapper)
                    {
                        DXGraphics2DWrapper?.Resize((int)_Width, (int)_Height);
                    }
                }

            });
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            if (hwndParent.Handle != this.ParentHwnd)
            {
                NativeMethods.SetParent(this.Win32Handle, hwndParent.Handle);
            }

            return new HandleRef(this, this.Win32Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(Win32Handle);
        }
    }
}
