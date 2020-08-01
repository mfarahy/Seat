using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat.SeatEngine.DataProvider.Tsetmc
{
    // TseClient.ClosingPriceInfo
    public class ClosingPriceInfo
    {
        public long InsCode
        {
            get;
            set;
        }

        public int DEven
        {
            get;
            set;
        }

        public decimal PClosing
        {
            get;
            set;
        }

        public decimal PDrCotVal
        {
            get;
            set;
        }

        public decimal ZTotTran
        {
            get;
            set;
        }

        public decimal QTotTran5J
        {
            get;
            set;
        }

        public decimal QTotCap
        {
            get;
            set;
        }

        public decimal PriceMin
        {
            get;
            set;
        }

        public decimal PriceMax
        {
            get;
            set;
        }

        public decimal PriceYesterday
        {
            get;
            set;
        }

        public decimal PriceFirst
        {
            get;
            set;
        }

        private DateTime _date;
        public DateTime DayDate
        {
            get
            {
                if (_date == DateTime.MinValue)
                {
                    _date = new DateTime(DEven / 10000, DEven / 100 % 100, DEven % 100);
                }
                return _date;
            }
        }
    }

}
