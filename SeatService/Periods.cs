using System;

namespace SeatService
{
    internal class Periods
    {
        public TimeSpan RefreshInstruments { get; set; }
        public TimeSpan TradeStart { get; set; }
        public TimeSpan TradeEnd { get; set; }
        public TimeSpan RefreshObserverMessages { get; set; }
        public TimeSpan RefreshClosingPrices { get; set; }
        public TimeSpan RefreshCodalMessages { get; set; }
        public TimeSpan UpdateDayTrades { get; set; }
        public TimeSpan FastWaitTime { get; set; }
        public TimeSpan SlowWaitTime { get; set; }
        public TimeSpan UpdateDayTradesStart { get; internal set; }
        public TimeSpan UpdateDayTradesEnd { get; internal set; }
        public TimeSpan InTimeRefreshLiveStates { get; set; }
        public TimeSpan OutTimeRefreshLiveStates { get; internal set; }
        public TimeSpan RefreshIndexes { get; set; }
    }
}