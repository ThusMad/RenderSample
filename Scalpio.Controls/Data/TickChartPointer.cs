using CryptoExchange.Net.CommonObjects;
using Scalpio.Render.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Scalpio.Controls.Data
{
    class TickChartPointer
    {
        private const int WM_MOUSEMOVE = 0x0200; //event id for winapi

        private DirectXHost _directXHost;

        public float GridBottomMargin = 24f;
        public float GridLeftMargin = 70f;

        private float _width;
        private float _height;

        private Point position;
        private Point relativePoint;

        private bool isCursorNone = false;

        public TickChartPointer()
        {

        }

        public void AddDirectXHost(DirectXHost directXHost, double width, double height)
        {
            _directXHost = directXHost;

            ChangeSize(width, height);

            _directXHost.DXGraphics2DWrapper.CreateSolidColorBrush(new Color4F(0.945f, 0.945f, 0.945f, 1.0f), "GridBrush");
            _directXHost.DXGraphics2DWrapper.CreateSolidColorBrush(new Color4F("#3C4043"), "LabelBrush");
            _directXHost.DXGraphics2DWrapper.LoadFontFace("Nunito-Regular.ttf");
            _directXHost.DXGraphics2DWrapper.CreateTextFormat("GridText", "Nunito-Regular", 400, 0, 5, 12.0f);
            _directXHost.ContentBox.SizeChanged += OnSizeChanged;

            (_directXHost.ContentBox as DependencyObject).Dispatcher.Invoke(() => HwndSource.FromHwnd(new WindowInteropHelper(_directXHost.ParentWnd).Handle)?.AddHook(WndProc));
            relativePoint = _directXHost.ContentBox.TransformToAncestor(_directXHost.ParentWnd)
                          .Transform(new Point(0, 0));

        }

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ChangeSize(e.NewSize.Width, e.NewSize.Height);
        }

        public void ChangeSize(double width, double height)
        {
            _width = (float)Math.Floor(width);
            _height = (float)Math.Floor(height);

            relativePoint = _directXHost.ContentBox.TransformToAncestor(_directXHost.ParentWnd)
                          .Transform(new Point(0, 0));
        }

        public void Render()
        {
            _directXHost.DXGraphics2DWrapper.ClearScreen(new Color4F(1.0f, 1.0f, 1.0f, 0.0f));

            float x = (float)position.X;
            float y = (float)position.Y;

            if (y < _height - 30)
            {
                DrawHorizontalLine(y);

                //double price = DrawPriceTip(target, y);

                //DrawVolumeTip(target, y, price);
            }
            if (x > 70f && x < _width - 0)
            {
                DrawVerticalLine(x);
                //DrawTimeTip(target, x);
            }

            if ((position.Y > _height - 30 || position.X < 70f || position.X > _width - 200))
            {
                _directXHost.ContentBox.Dispatcher.Invoke(() =>
                {
                    _directXHost.ContentBox.Cursor = Cursors.None;
                });

                isCursorNone = true;
            }
            else
            {
                _directXHost.ContentBox.Dispatcher.Invoke(() =>
                {
                    _directXHost.ContentBox.Cursor = Cursors.Cross;
                });

                isCursorNone = false;
            }
          
        }

        public void DrawHorizontalLine(float y)
        {
            _directXHost.DXGraphics2DWrapper.DrawLine(new System.Drawing.Point(70, (int)y), new System.Drawing.Point((int)_width - 2, (int)y), "LabelBrush");
            _directXHost.DXGraphics2DWrapper.DrawLine(new System.Drawing.Point((int)_width, (int)y), new System.Drawing.Point((int)_width, (int)y), "LabelBrush" /*2f*/);
        }

        public void DrawVerticalLine(float x)
        {
            var xx = (int)Math.Floor(x);

            _directXHost.DXGraphics2DWrapper.DrawLine(new System.Drawing.Point(xx, (int)_height - 25), new System.Drawing.Point(xx, 0), "LabelBrush");
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEMOVE:
                    position.X = unchecked((short)(long)lParam) - relativePoint.X;// - base.Margin.Left - relativePoint.X;
                    position.Y = unchecked((short)((long)lParam >> 16)) - relativePoint.Y;// - base.Margin.Top - relativePoint.Y;
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }
    }
}
