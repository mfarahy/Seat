using Seat.SeatEngine.DataProvider;
using Seat.SeatEngine.DataProvider.Tsetmc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat.SeatEngine.Filters
{
    [DisplayName("قدرت خریدار در برابر فروشنده")]
    public class DemandPowerFilter : IFilter
    {
        public DemandPowerFilter()
        {
            Factor = 4;
        }

        [DisplayName("فاکتور")]
        public float Factor { get; set; }

        public bool Eval(TsetmcDataRow row)
        {
            return (row.Buy_I_Volume * 1.0 / row.Buy_CountI) > Factor * (row.Sell_I_Volume * 1.0 / row.Sell_CountI);
        }
    }
}
