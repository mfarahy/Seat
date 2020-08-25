using Exir.Framework.Common;
using SeatDomain.Models;
using SeatDomain.Models.Service;
using System.Linq;

namespace SeatDomain.Repository
{
    public interface IUserActivityRepository : IUserActivityReadOnlyRepository
    {
    }
    public interface IUserActivityReadOnlyRepository : IRepository<UserActivity>
    {
        UserActivitySummary TakeSummary(IQueryable<UserActivity> expression);
    }
}
