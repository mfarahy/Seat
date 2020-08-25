using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public class Index
    {
        public string Name { get; set; }
        public long Code { get; set; }
        public List<DayValue> History { get; set; }
        public string LVal18AFC { get; set; }
        public string Title { get; set; }
        public float LastValue { get; set; }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
        public IndexLastDayTimeValue[] LastDayTimeValue { get; set; }
        public List<long> RelatedCompanies { get; set; }
        public TimeSpan PublishTime { get; set; }
        public float ChangeValue { get; set; }
        public float ChangePercent { get; set; }
    }


}
