using SeatDomain.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public interface ITsetmcOnlineDataProvider : IDisposable
    {
        Task<List<Index>> GetIndexLastValue();
        Task<Index> GetIndex(long indexCode, bool includeHistory);
        Task<DayTradeDetails> ExtractDayDetailsAsync(long insCode, DateTime date);
        ConcurrentDictionary<long, TsetmcDataRow> Data { get; }
        long Heven { get; }
        ObserverMessage LastObserverMessage { get; set; }

        void Clear();
        Task<InstrumentLastInfo> FindAsync(string symbole, long? insCode);
        Task<IEnumerable<ObserverMessage>> GetNewMessagesAsync();
        Task LoadDataAsync();
        Task<bool> RefreshAsync(TimeSpan timeout);

        Task LoadInstHistoryAsync();
    }
}