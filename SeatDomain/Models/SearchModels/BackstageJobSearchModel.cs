using System;
using Exir.Framework.Common;

namespace SeatDomain.Models.SearchModels
{
    public class BackstageJobSearchModel : ISpecification
    {
        public int? BackstageJobPK { get; set; }
        public string RunAs { get; set; }
        public string TraceNo { get; set; }
        public short? Status { get; set; }
        public string Service { get; set; }
        public string Action { get; set; }
        public short? Priority { get; set; }
        public short? RetryCount { get; set; }
        public string Queue { get; set; }
        public string Server { get; set; }
        public DateTime? RunDtFrom { get; set; }
        public DateTime? RunDtTo { get; set; }
        public DateTime? TimeToRunFrom { get; set; }
        public DateTime? TimeToRunTo { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SortingManner[] Sorting { get; set; }
        public string[] Projection { get; set; }
        public string Tags { get;  set; }
    }
}
