using SeatDomain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public interface ITsetmcWebServiceDataProvider : IDisposable
    {
        ConcurrentBag<Instrument> Instruments { get; }
        IEnumerable<TseShareInfo> TseShares { get; }
        Task FillDataAsync();
        Task<List<ClosingPriceInfo>> UpdateClosingPricesAsync( Action<int> onStart, Action<int> onPageDone);
        Task<bool> UpdateInstrumentsAsync();
    }
}