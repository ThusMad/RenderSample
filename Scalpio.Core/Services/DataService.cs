using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Interfaces;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Sockets;
using Scalpio.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalpio.Core.Services
{
    public class DataService : IDataService
    {
        private readonly Dictionary<string, BinanceSymbol> _symbolDictionary;

        private readonly IBinanceClient _binanceClient;
        private readonly IBinanceSocketClient _binanceSocketClient;

        private readonly Dictionary<string, List<Action<BinanceStreamRollingWindowTick>>> _hourTickerSubscriptionList;
        private readonly Dictionary<string, List<Action<BinanceStreamRollingWindowTick>>> _24hourTickerSubscriptionList;
        private readonly Dictionary<string, List<Action<BinanceStreamAggregatedTrade>>> _aggregatedTradesSubscriptionList;

        public DataService(IBinanceClient binanceClient, IBinanceSocketClient binanceSocketClient)
        {
            _binanceClient = binanceClient;
            _binanceSocketClient = binanceSocketClient;

            _symbolDictionary = new Dictionary<string, BinanceSymbol>();

            _hourTickerSubscriptionList = new Dictionary<string, List<Action<BinanceStreamRollingWindowTick>>>();
            _24hourTickerSubscriptionList = new Dictionary<string, List<Action<BinanceStreamRollingWindowTick>>>();
            _aggregatedTradesSubscriptionList = new Dictionary<string, List<Action<BinanceStreamAggregatedTrade>>>();
        }

        public async Task FetchData()
        {
            var result = await _binanceClient.SpotApi.ExchangeData.GetExchangeInfoAsync();
            if(result == null)
            {
                return;
            }
            if(!result.Success)
            {
                return;
            }
            foreach (var data in result.Data.Symbols)
            {
                _symbolDictionary.Add(data.Name, data);
            }
        }

        private void UpdateHourTicker(DataEvent<IEnumerable<BinanceStreamRollingWindowTick>> obj)
        {
            foreach (var data in obj.Data)
            {
                if (_hourTickerSubscriptionList.ContainsKey(data.Symbol))
                {
                    var list = _hourTickerSubscriptionList[data.Symbol];
                    if (list != null && list.Count > 0)
                    {
                        Task.Run(() =>
                        {
                            foreach (var action in list)
                            {
                                action.Invoke(data);
                            }
                        });
                    }
                }
            }
        }

        private void Update24HourTicker(DataEvent<IEnumerable<BinanceStreamRollingWindowTick>> obj)
        {
            foreach (var data in obj.Data)
            {
                if (_24hourTickerSubscriptionList.ContainsKey(data.Symbol))
                {
                    var list = _24hourTickerSubscriptionList[data.Symbol];
                    if (list != null && list.Count > 0)
                    {
                        Task.Run(() =>
                        {
                            foreach (var action in list)
                            {
                                action.Invoke(data);
                            }
                        });
                    }
                }
            }
        }

        private void UpdateAggregatedTradeStream(DataEvent<BinanceStreamAggregatedTrade> obj)
        {
            if (_aggregatedTradesSubscriptionList.ContainsKey(obj.Data.Symbol))
            {
                var list = _aggregatedTradesSubscriptionList[obj.Data.Symbol];
                if (list != null && list.Count > 0)
                {
                    Task.Run(() =>
                    {
                        foreach (var action in list)
                        {
                            action.Invoke(obj.Data);
                        }
                    });
                }
            }
        }

        public async Task SubscribeToKlines1h()
        {
            var result = await _binanceSocketClient.SpotStreams.SubscribeToAllRollingWindowTickerUpdatesAsync(TimeSpan.FromHours(1), UpdateHourTicker);
        }

        public async Task SubscribeToKlines24h()
        {
            var result = await _binanceSocketClient.SpotStreams.SubscribeToAllRollingWindowTickerUpdatesAsync(TimeSpan.FromHours(24), Update24HourTicker);
        }

        public async Task SubscribeToAllRealTimeTrades()
        {
            var result = await _binanceSocketClient.SpotStreams.SubscribeToAggregatedTradeUpdatesAsync("BTCUSDT", UpdateAggregatedTradeStream);
            //foreach (var symbol in _symbolDictionary.Values.Where(symbol => symbol.Status == Binance.Net.Enums.SymbolStatus.Trading))
            //{
            //    var result = await _binanceSocketClient.SpotStreams.SubscribeToAggregatedTradeUpdatesAsync(symbol.Name, UpdateAggregatedTradeStream);
            //}
            
        }

        public void SubscribeTo1hTicker(string symbol, Action<BinanceStreamRollingWindowTick> action)
        {
            if (_hourTickerSubscriptionList.ContainsKey(symbol))
            {
                var list = _hourTickerSubscriptionList[symbol];
                if (list != null && list.Count > 0)
                {
                    list.Add(action);
                }
                else
                {
                    _hourTickerSubscriptionList[symbol] = new List<Action<BinanceStreamRollingWindowTick>>();
                    _hourTickerSubscriptionList[symbol].Add(action);
                }
            }
            else
            {
                _hourTickerSubscriptionList.Add(symbol, new List<Action<BinanceStreamRollingWindowTick>>() { action });
            }
        }

        public void SubscribeTo24hTicker(string symbol, Action<BinanceStreamRollingWindowTick> action)
        {
            if (_24hourTickerSubscriptionList.ContainsKey(symbol))
            {
                var list = _24hourTickerSubscriptionList[symbol];
                if (list != null && list.Count > 0)
                {
                    list.Add(action);
                }
                else
                {
                    _24hourTickerSubscriptionList[symbol] = new List<Action<BinanceStreamRollingWindowTick>>();
                    _24hourTickerSubscriptionList[symbol].Add(action);
                }
            }
            else
            {
                _24hourTickerSubscriptionList.Add(symbol, new List<Action<BinanceStreamRollingWindowTick>>() { action });
            }
        }

        public void SubscribeToAggregatedTradeStream(string symbol, Action<BinanceStreamAggregatedTrade> action)
        {
            if (_aggregatedTradesSubscriptionList.ContainsKey(symbol))
            {
                var list = _aggregatedTradesSubscriptionList[symbol];
                if (list != null && list.Count > 0)
                {
                    list.Add(action);
                }
                else
                {
                    _aggregatedTradesSubscriptionList[symbol] = new List<Action<BinanceStreamAggregatedTrade>>();
                    _aggregatedTradesSubscriptionList[symbol].Add(action);
                }
            }
            else
            {
                _aggregatedTradesSubscriptionList.Add(symbol, new List<Action<BinanceStreamAggregatedTrade>>() { action });
            }
        }
    }
}
