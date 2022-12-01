using Binance.Net.Objects.Models.Spot.Socket;
using CommunityToolkit.Mvvm.Input;
using Scalpio.Controls.Data;
using Scalpio.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Scalpio.Controls.ViewModels
{
    internal class TradingFrameViewModel : INotifyPropertyChanged
    {
        private Guid frameGuid;
        private ManualResetEvent event_2 = new ManualResetEvent(false);

        private double _renderTargetHeight;
        private double _renderTargetWidth;

        private DirectXHost? _dxHostGrid;
        private DirectXHost? _dxHostPointer;
        private DirectXHost? _dxHostChart;
        private Window? _hostWindow;
        private FrameworkElement? _hostBox;

        private TickChartGrid _tickChartGrid;
        private TickChartPointer _tickChartPointer;
        private TickChart _tickChart;

        private IRelayCommand? _onRenderSizeChanged;
        private IRelayCommand? _onRenderLoaded;
        private IRelayCommand? _onRenderMouseEnter;
        private IRelayCommand? _onRenderMouseLeave;

        private Thread pointerThread;
        private Thread gridThread;
        private Thread chartThread;

        private readonly IDataService _dataService;

        List<BinanceStreamAggregatedTrade> list;

        public TradingFrameViewModel(IDataService dataService)
        {
            frameGuid = Guid.NewGuid();

            _dataService = dataService;
            list = new List<BinanceStreamAggregatedTrade>();

            _tickChart = new TickChart();
            _tickChartGrid = new TickChartGrid();
            _tickChartPointer = new TickChartPointer();

            Task.Run(async () =>
            {
                await _dataService.SubscribeToAllRealTimeTrades();
                SwitchToSymbol();
            });
        }

        public void ShowWindow()
        {
            _dxHostGrid?.ShowWindow();
            _dxHostPointer?.ShowWindow();
            _dxHostChart?.ShowWindow();
        }

        public void HideWindow()
        {
            _dxHostGrid?.HideWindow();
            _dxHostPointer?.HideWindow();
            _dxHostChart?.HideWindow();
        }

        public IRelayCommand OnRenderSizeChanged
        {
            get { return _onRenderSizeChanged ?? (_onRenderSizeChanged = new RelayCommand<SizeChangedEventArgs>(RenderSizeChangedHandler)); }
        }

        public IRelayCommand OnRenderLoaded
        {
            get { return _onRenderLoaded ?? (_onRenderLoaded = new RelayCommand<RoutedEventArgs>(RenderLoadedHandler)); }
        }

        public IRelayCommand OnRenderMouseEnter
        {
            get { return _onRenderMouseEnter ?? (_onRenderMouseEnter = new RelayCommand<MouseEventArgs>(RenderMouseEnter)); }
        }

        public IRelayCommand OnRenderMouseLeave
        {
            get { return _onRenderMouseLeave ?? (_onRenderMouseLeave = new RelayCommand<MouseEventArgs>(RenderMouseLeave)); }
        }

        private void SwitchToSymbol()
        {
            _dataService.SubscribeToAggregatedTradeStream("BTCUSDT", (d) =>
            {
                lock(list)
                {
                    list?.Add(d);
                }
            });
        }

        private void RenderMouseEnter(MouseEventArgs e)
        {
            event_2.Set();
        }

        private void RenderMouseLeave(MouseEventArgs e)
        {
            event_2.Reset();

            if (_tickChartPointer != null && _dxHostPointer != null && _dxHostPointer.DXGraphics2DWrapper != null)
            {
                _dxHostPointer.DXGraphics2DWrapper.BeginDraw();
                _dxHostPointer.DXGraphics2DWrapper.ClearScreen(new Render.Data.Color4F(1f, 1f, 1f, 0f));
                _dxHostPointer.DXGraphics2DWrapper.EndDraw();
                _dxHostPointer.DXGraphics2DWrapper.Present();               
            }
        }

        private void RenderLoadedHandler(RoutedEventArgs e)
        {
            if (e != null && e.OriginalSource != null)
            {
                if(e.OriginalSource is FrameworkElement fe)
                {
                    _renderTargetHeight = fe.ActualHeight;
                    _renderTargetWidth = fe.ActualWidth;

                    var app = System.Windows.Application.Current;
                    _hostWindow = app.MainWindow;
                    _hostBox = fe;

                    _dxHostPointer = new DirectXHost(fe, _hostWindow);
                    _dxHostChart = new DirectXHost(fe, _hostWindow);
                    _dxHostGrid = new DirectXHost(fe, _hostWindow);
                    

                    _tickChartGrid.AddDirectXHost(_dxHostGrid, _renderTargetWidth, _renderTargetHeight);
                    _tickChart.AddDirectXHost(_dxHostChart, _renderTargetWidth - 200, _renderTargetHeight);
                    _tickChartPointer.AddDirectXHost(_dxHostPointer, _renderTargetWidth, _renderTargetHeight);

                    pointerThread = new Thread(() => PointerRenderLoop())
                    {
                        Name = frameGuid.ToString() + "_pointerLoop"
                    };
                    pointerThread.Start();

                    gridThread = new Thread(() => GridRenderLoop())
                    {
                        Name = frameGuid.ToString() + "_gridLoop"
                    };
                    gridThread.Start();

                    chartThread = new Thread(() => ChartRenderLoop())
                    {
                        Name = frameGuid.ToString() + "_chartLoop"
                    };
                    chartThread.Start();
                }
            }
        }

        private void ChartRenderLoop()
        {
            while (true)
            {
                if (_tickChart != null && _dxHostChart != null && _dxHostChart.DXGraphics2DWrapper != null)
                {
                    lock (list)
                    {
                        lock (_dxHostChart.DXGraphics2DWrapper)
                        {
                            _dxHostChart.DXGraphics2DWrapper.BeginDraw();
                            _tickChart.Draw(list);
                            Debug.WriteLine(list.Count);
                            _dxHostChart.DXGraphics2DWrapper.EndDraw();
                            _dxHostChart.DXGraphics2DWrapper.Flush();
                            _dxHostChart.DXGraphics2DWrapper.Present(); 
                            //_dxHostChart.DXGraphics2DWrapper.Present();
                        }

                        list.Clear();
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        private void PointerRenderLoop()
        {
            while (true)
            {
                if (_tickChartPointer != null && _dxHostPointer != null && _dxHostPointer.DXGraphics2DWrapper != null)
                {
                    event_2.WaitOne();

                    lock (_dxHostPointer.DXGraphics2DWrapper)
                    {
                        _dxHostPointer.DXGraphics2DWrapper.BeginDraw();
                        _tickChartPointer.Render();
                        _dxHostPointer.DXGraphics2DWrapper.EndDraw();
                        _dxHostPointer.DXGraphics2DWrapper.Flush();
                        _dxHostPointer.DXGraphics2DWrapper.Present();
                    }

                    Thread.Sleep(6);
                }
            }
        }

        private void GridRenderLoop()
        {
            while (true)
            {
                if (_tickChartGrid != null && _dxHostGrid != null && _dxHostGrid.DXGraphics2DWrapper != null)
                {
                    lock (_dxHostGrid.DXGraphics2DWrapper)
                    {
                        _dxHostGrid.DXGraphics2DWrapper.BeginDraw();
                        _tickChartGrid.DrawGrid();
                        _tickChartGrid.DrawTimeLabel();
                        _tickChartGrid.DrawPriceLabel();
                        _dxHostGrid.DXGraphics2DWrapper.EndDraw();
                        _dxHostGrid.DXGraphics2DWrapper.Present();
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        private void RenderSizeChangedHandler(SizeChangedEventArgs e)
        {
            if(_dxHostGrid != null && _dxHostPointer != null && _dxHostChart != null)
            {
                _dxHostGrid.ChangeSize(e);
                _dxHostPointer.ChangeSize(e);
                _dxHostChart.ChangeSize(e.NewSize.Width - 200, e.NewSize.Height);
            }
        }

        #region PropertyChanged
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
