using SeatDomain.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    class StaticData
    {
        public static string Language;

        public static IEnumerable<ColumnInfo> ColumnsInfo;

        public static ConcurrentBag<Instrument> Instruments;

        public static ConcurrentBag<TseShareInfo> TseShares;

        public static async Task FillStaticDataAsync()
        {
            Settings settings = new Settings();
            Language = settings.Language;
            ColumnsInfo = await FileService.ColumnsInfoAsync();
            Instruments = await FileService.InstrumentsAsync();
            TseShares = await FileService.TseSharesAsync();
        }
    }

}
