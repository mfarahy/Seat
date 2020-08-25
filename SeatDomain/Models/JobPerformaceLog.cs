using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
   public class JobPerformaceLog
    {
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
        public int HitCount { get; set; }
        public int Sum { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public DateTime InsertDt { get; set; }
        public int Mid { get { return Sum / HitCount; } }
    }
}
