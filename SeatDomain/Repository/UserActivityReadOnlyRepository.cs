using EntityFramework.Extensions;
using Exir.Framework.Common;
using Exir.Framework.DataAccess.EntityFramework;
using SeatDomain.Models;
using SeatDomain.Models.Service;
using System;
using System.Data.Entity;
using System.Linq;

namespace SeatDomain.Repository
{
    public class UserActivityReadOnlyRepository : EntityFrameworkRepository<UserActivity>, IRepository<UserActivity>, IUserActivityRepository
    {
        public UserActivityReadOnlyRepository()
        {
        }

        public UserActivityReadOnlyRepository(string contextObjectId) : base(contextObjectId)
        {
        }

        public UserActivityReadOnlyRepository(ContextCreator creator) : base(creator)
        {
        }

        public UserActivityReadOnlyRepository(DbContext context) : base(context)
        {
        }

        public UserActivityReadOnlyRepository(DbSet<UserActivity> repository, ContextCreator creator) : base(repository, creator)
        {
        }

        public UserActivitySummary TakeSummary(IQueryable<UserActivity> query)
        {

            var overal = from user in query
                         group user by 1 into g
                         select new
                         {
                             MinDt = g.Min(x => x.EntryDt),
                             MaxDt = g.Max(x => x.EntryDt),
                             Count = g.Count()
                         };

            var most = (from user in query.Where(x => !String.IsNullOrEmpty(x.PageName))
                        group user by user.PageName into g
                        select new MostVisitedPage
                        {
                            PageName = g.Key,
                            Count = g.Count()
                        }).OrderByDescending(x => x.Count).Take(10);

            var daily = (from user in query
                         group user by DbFunctions.TruncateTime(user.EntryDt) into g
                         orderby g.Key descending
                         select new DayActivity
                         {
                             Date = g.Key.Value,
                             Count = g.Count()
                         }).Take(14);

            var hourly = from user in query
                         group user by user.EntryDt.Hour into g
                         orderby g.Key descending
                         select new HourActivity
                         {
                             Hour = g.Key,
                             Count = g.Count()
                         };

            overal.Future();
            most.Future();
            daily.Future();
            hourly.Future();
            var firstOveral = overal.FirstOrDefault();

            var summary = new UserActivitySummary()
            {
                HourActivities = hourly.ToArray(),
                DayActivities = daily.ToArray(),
                FirstActivityDt = firstOveral?.MinDt ?? DateTime.MinValue,
                LastActivityDt = firstOveral?.MaxDt ?? DateTime.MinValue,
                MostVisitedPages = most.ToArray(),
                TotalActivityCount = firstOveral?.Count ?? 0
            };

            return summary;
        }
    }
}
