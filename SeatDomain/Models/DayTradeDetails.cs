using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public class DayTradeDetails
    {
        public long InsCode { get; set; }
        public DateTime DayDate { get; set; }
        public List<BestLimit> BestLimits { get; set; }

        public List<Trade> Trades { get; set; }
        public List<ShareHolderChange> ShareHolderStates { get; set; }
        public List<ClosingPrice> ClosingPriceData { get; set; }
    }
}
