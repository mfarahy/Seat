using Seat.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seat.SeatEngine.DataProvider.Tsetmc
{
    public interface ITsetmcOnlineDataProvider : IDisposable
    {
        Task<DayTradeDetails> ExtractDayDetailsAsync(long insCode, DateTime date);
        ConcurrentDictionary<long, TsetmcDataRow> Data { get; }
        long Heven { get; }
        string Index { get; set; }
        string IndexIncreament { get; set; }
        ObserverMessage LastObserverMessage { get; set; }

        void Clear();
        Task<TsetmcDataRow> FindAsync(string symbole);
        Task<IEnumerable<ObserverMessage>> GetNewMessagesAsync();
        Task LoadDataAsync();
        Task<bool> RefreshAsync(TimeSpan timeout);

        Task LoadInstHistoryAsync();
    }
}