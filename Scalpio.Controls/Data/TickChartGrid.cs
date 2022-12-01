using Scalpio.Render.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalpio.Controls.Data
{
    class TickChartGrid
    {
        private DirectXHost _directXHost;

        public float GridBottomMargin = 24f;
        public float GridLeftMargin = 70f;

        private float _width;
        private float _height;

        float stepVertical;
        float stepHorizontal;

        public TickChartGrid()
        {
            
        }

        public void AddDirectXHost(DirectXHost directXHost, double width, double height)
        {
            _directXHost = directXHost;

            ChangeSize(width, height);

            _directXHost.DXGraphics2DWrapper.CreateSolidColorBrush(new Color4F(0.945f, 0.945f, 0.945f, 1.0f), "GridBrush");
            _directXHost.DXGraphics2DWrapper.CreateSolidColorBrush(new Color4F("#202124"), "LabelBrush");
            _directXHost.DXGraphics2DWrapper.LoadFontFace("Nunito-Regular.ttf");
            _directXHost.DXGraphics2DWrapper.CreateTextFormat("GridText", "Nunito-Regular", 400, 0, 5, 12.0f);
            _directXHost.ContentBox.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ChangeSize(e.NewSize.Width, e.NewSize.Height);
        }

        private void ChangeSize(double width, double height)
        {
            _width = (float)Math.Floor(width) - 200;
            _height = (float)Math.Floor(height);

            stepVertical = (_width - 80f) / 30.0f;
            stepHorizontal = (_height - 26f) / 10.0f;
        }

        public void DrawPriceLabel()
        {
            Rectangle4F priceRect = new Rectangle4F(4, _height, _width - 10, _height - (GridBottomMargin + 7 + stepHorizontal));

            // Drawing Price
            for (int i = 1; i < 10; i++)
            {
                _directXHost.DXGraphics2DWrapper.DrawText(
                    string.Format("{0:F8}", (0.1 + (i * (1 * 0.1)))).Substring(0, 10),  // Text
                    "GridText",                                               // Format
                    "LabelBrush",                                                                          // Rect
                    priceRect                                                     // Brush
                    );

                priceRect.Bottom -= stepHorizontal;
            }
        }

        public void DrawTimeLabel()
        {
            DateTime currentTime = DateTime.UtcNow;

            Rectangle4F dateRect = new Rectangle4F(GridLeftMargin - 3,
                                                        _height - (GridBottomMargin - 3),
                                                        _width,
                                                        _height);

            int time;

            // Drawing Time
            for (int i = 0; i < 30; i++)
            {
                time = currentTime.AddSeconds(1 /* scale */ * -1 * (53 - (i * 2))).Second;

                _directXHost.DXGraphics2DWrapper.DrawText((time < 10) ? "0" + time.ToString() : time.ToString(),
                    "GridText", "LabelBrush", dateRect);

                dateRect.Left += stepVertical;
                dateRect.Right += stepVertical;
            }
        }

        public void DrawGrid()
        {
            

            // Drawing Vertical lines

            _directXHost.DXGraphics2DWrapper.ClearScreen(new Color4F(1.0F, 1.0F, 1.0F, 1.0F));

            // Grid Border
            _directXHost.DXGraphics2DWrapper.DrawRectangle(new Rectangle4F(GridLeftMargin, -1, _width, _height - GridBottomMargin), "GridBrush");

            for (int i = 1; i < 30; i++)
            {
                if (i % 2 == 0)
                {
                    float y = GridLeftMargin + i * stepVertical;
                    _directXHost.DXGraphics2DWrapper.DrawLine(new Point((int)y, 0), new Point((int)y, (int)(_height - GridBottomMargin)), "GridBrush");
                }

            }

            // Drawing Horizontal lines
            for (int i = 0; i < 10; i++)
            {
                float x = (_height - GridBottomMargin) - (i * stepHorizontal);

                if (i != 0)
                    _directXHost.DXGraphics2DWrapper.DrawLine(new Point((int)_width, (int)x), new Point((int)GridLeftMargin, (int)x), "GridBrush");
            }
        }
    }
}
