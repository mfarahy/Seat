using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exir.Framework.Common;

namespace SeatDomain.Models.SearchModels
{
    public class CacheReportSearchModel : ISpecification
    {
        public string Key { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SortingManner[] Sorting { get; set; }
        public string[] Projection { get; set; }
    }
}
