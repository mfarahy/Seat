using System;

namespace Seat
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
    }
}