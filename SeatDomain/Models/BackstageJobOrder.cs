using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models
{
    public class BackstageJobOrder
    {
        public string Service { get; set; }
        public string Action { get; set; }
        public Expression<Func<object>> Args { get; set; }
        public short Priority { get; set; }
        public string Queue { get; set; }
        public string Code { get; set; }
        public string Dependency { get; set; }
        public object[] Tags { get; set; }
        public bool Debug { get; set; }
        public DateTime? TimeToRun { get; set; }
    }
}
