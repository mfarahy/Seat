using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public class DayValue
    {
        public DayValue()
        {
        }

        public DayValue(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }

        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}
