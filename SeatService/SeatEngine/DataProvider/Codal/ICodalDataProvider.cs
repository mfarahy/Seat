using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider
{
    public interface ICodalDataProvider : IDisposable
    {
        CodalMessage Last { get; set; }

        Task<List<CodalMessage>> GetNewMessages();
    }
}