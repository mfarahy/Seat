using Exir.Framework.Common;
using Exir.Framework.Service.ActionResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Models.Service
{
    public class UserActivitySummaryDataPageResponse : SummaryDataPageResponse<UserActivity, UserActivitySummary>
    {
        public UserActivitySummaryDataPageResponse(ActionResponse anotherResponse) : base(anotherResponse)
        {
        }

        public UserActivitySummaryDataPageResponse(IEnumerable<UserActivity> data, int page_number, int total_record_count, bool filterable) : base(data, page_number, total_record_count, filterable)
        {
        }
    }
    public class MostVisitedPage 
    {
        public string PageName { get; set; }
        public int Count { get; set; }
    }

    public class DayActivity 
    {
        public DateTime Date { get; set; }
        public string TimeCaption
        {
            get
            {
                var pd = Date.ToPersian();
                return $"{pd.Year}-{pd.Month}-{pd.Day}";
            }
        }
        public int Count { get; set; }
    }
    public class HourActivity 
    {
        public int Hour { get; set; }
        public string HourCaption
        {
            get
            {
                return Hour.ToString("D2") + ":00";
            }
        }
        public int Count { get; set; }
    }
    public class UserActivitySummary 
    {
        public MostVisitedPage[] MostVisitedPages { get; set; }
        public DayActivity[] DayActivities { get; set; }
        public HourActivity[] HourActivities { get; set; }
        public DateTime LastActivityDt { get; set; }
        public DateTime FirstActivityDt { get; set; }
        public int TotalActivityCount { get; set; }

    }
}
