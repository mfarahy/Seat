using Seat.DataStore;
using Seat.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seat.SeatEngine.DataProvider
{
    public interface ICodalDataProvider : IDisposable
    {
        CodalMessage Last { get; set; }

        Task<List<CodalMessage>> GetNewMessages();
    }
}