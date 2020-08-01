using Seat.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Seat.SeatEngine.DataProvider.Tsetmc
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
