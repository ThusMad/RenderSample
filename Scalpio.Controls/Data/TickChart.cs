using Binance.Net.Objects.Models.Spot.Socket;
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
    class TickChart
    {
        private DirectXHost _directXHost;

        public float GridBottomMargin = 24f;
        public float GridLeftMargin = 70f;

        private float _width;
        private float _height;

        private long tmin;
        private long tmax;

        public TickChart()
        {

        }

        public void AddDirectXHost(DirectXHost directXHost, double width, double height)
        {
            _directXHost = directXHost;

            ChangeSize(width, height);

            _directXHost.DXGraphics2DWrapper.CreateSolidColorBrush(new Color4F("#FA2862"), "LabelBrushR");
            _directXHost.DXGraphics2DWrapper.CreateSolidColorBrush(new Color4F("#4DE188"), "LabelBrushG");
            _directXHost.ContentBox.SizeChanged += OnSizeChanged;

            _directXHost.DXGraphics2DWrapper.BeginDraw();
            _directXHost.DXGraphics2DWrapper.ClearScreen(new Color4F(1f, 1f, 1f, 0f));
            _directXHost.DXGraphics2DWrapper.EndDraw();
            _directXHost.DXGraphics2DWrapper.Present();

            tmin = DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeMilliseconds();
            tmax = DateTimeOffset.UtcNow.AddSeconds(30).ToUnixTimeMilliseconds();
        }

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ChangeSize(e.NewSize.Width, e.NewSize.Height);
        }

        public void Draw(List<BinanceStreamAggregatedTrade> trades)
        {
            foreach (var trade in trades)
            {
                var point = Normalize(trade.Price, ((DateTimeOffset)trade.TradeTime).ToUnixTimeMilliseconds());

                if (trade.BuyerIsMaker)
                {
                    _directXHost.DXGraphics2DWrapper.FillRectangle(point, "LabelBrushR");
                }
                else
                {
                    _directXHost.DXGraphics2DWrapper.FillRectangle(point, "LabelBrushG");
                }
            }
        }

        public void ChangeSize(double width, double height)
        {
            _width = (float)Math.Floor(width) - 200;
            _height = (float)Math.Floor(height);
        }

        public Rectangle4F Normalize(decimal price, long timestamp)
        {
            //var cc = DateTimeOffset.UtcNow.AddSeconds(30).ToUnixTimeMilliseconds();
            //var ccm = DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeMilliseconds();
            //var x = (int)(_height - ((double)price * ((_height / 16000))));
            //var y = (int)((timestamp - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) * (double)_width / 60000);
            
            var x = (int)(_height * (double)((price - 16300.0M) / (18000.0M - 16000.0M)));
            var y = (int)(_width * (double)((timestamp - (double)tmin) / (tmax - (double)tmin)));

            return new Rectangle4F(
                y - 2,                                
                x - 2,
                y + 2,
                x + 2
                );
        }
    }
}
