using Binance.Net.Objects.Models.Spot.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalpio.Core.Services.Interfaces
{
    public interface IDataService
    {
        public Task FetchData();

        public Task SubscribeToKlines1h();
        public Task SubscribeToKlines24h();
        public Task SubscribeToAllRealTimeTrades();

        public void SubscribeTo24hTicker(string symbol, Action<BinanceStreamRollingWindowTick> action);
        public void SubscribeTo1hTicker(string symbol, Action<BinanceStreamRollingWindowTick> action);
        public void SubscribeToAggregatedTradeStream(string symbol, Action<BinanceStreamAggregatedTrade> action);
    }
}
