using System;

namespace Seat
{
    internal class LastUpdate
    {
        public DateTime RefreshInstruments { get; set; }
        public DateTime RefreshClosingPrices { get; set; }
        public DateTime RefreshObserverMessages { get; set; }
        public DateTime RefreshCodalMessages { get; set; }
        public DateTime UpdateDayTrades { get; set; }
    }
}