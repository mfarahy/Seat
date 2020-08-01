using Seat.Models;
using Seat.SeatEngine.DataProvider.Tsetmc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seat.DataStore
{
    public interface ITsetmcStorage
    {
        Task AddClientTypeAsync(IEnumerable<TsetmcDataRow> instances);
        Task<List<Instrument>> GetAllInstrumentsAsync();
        Task<CodalMessage> GetLastCodalMessageAsync();
        Task<ObserverMessage> GetLastMessageAsync();
        Task<int> SaveMessagesAsync(IEnumerable<CodalMessage> codalMessages);
        Task<int> SaveMessagesAsync(IEnumerable<ObserverMessage> observerMessages);
        Task UpdateInstancesAsync(IEnumerable<Instrument> instruments);
        Task AddHistoryAsync(List<History> lists);
        Task<Dictionary<long, int>> GetLastDevensAsync();
    }
}