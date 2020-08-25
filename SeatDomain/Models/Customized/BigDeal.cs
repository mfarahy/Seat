using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public partial class BigDeal
    {
        public TimeSpan Time
        {
            get
            {
                return DayDt.TimeOfDay;
            }
        }
        public DateTime Date
        {
            get
            {
                return DayDt.Date;
            }
        }
        public BigDeal()
        { }

        public string QueueState
        {
            get
            {
                if (pd1 == tmax)
                {
                    if(qd1 >= 0.2 * bvol && qd1 <= bvol)return Constants.QueueStates.LightBuyQueue;
                    if (qd1 >=  bvol ) return Constants.QueueStates.HeavyBuyQueue;
                    if (qd1 > 0) return Constants.QueueStates.BuyQueue;
                }
                if (po1 == tmin)
                {
                    if (qo1 >= 0.2 * bvol && qo1 <= bvol) return Constants.QueueStates.LightSellQueue;
                    if (qo1 >= bvol) return Constants.QueueStates.HeavySellQueue;
                    if (qo1 > 0) return Constants.QueueStates.SellQueue;
                }
                return Constants.QueueStates.Balanced;
            }
        }

        public string Symbol { get; set; }
    }
}
