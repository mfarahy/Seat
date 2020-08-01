using Seat.SeatEngine.DataProvider;
using Seat.SeatEngine.DataProvider.Tsetmc;

namespace Seat.SeatEngine.Filters
{
    public interface IFilter
    {
        bool Eval(TsetmcDataRow row);
    }
}