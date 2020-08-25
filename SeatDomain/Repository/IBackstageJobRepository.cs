using Exir.Framework.Common;
using SeatDomain.Models;

namespace SeatDomain.Repository
{
    public interface IBackstageJobRepository : IBackstageJobReadOnlyRepository
    {
        void RawRun(string script);
    }
    public interface IBackstageJobReadOnlyRepository : IRepository<BackstageJob>
    {
    }
}
