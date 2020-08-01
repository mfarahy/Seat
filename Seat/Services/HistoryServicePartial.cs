using Newtonsoft.Json.Bson;
using Seat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat.Services
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
