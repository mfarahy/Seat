using SeatDomain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public partial interface IHistoryService
    {
        Task BulkSaveAsync(IEnumerable<History> entities);
    }
    public partial class HistoryService
    {
        public async Task BulkSaveAsync(IEnumerable<History> entities)
        {
            await Repository.BulkInsertAsync(entities);
        }
    }
}
