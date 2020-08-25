namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Common.Caching;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using System.Collections.Generic;
    using System.Linq;

    public partial interface IDashboardUserNoteService : IReadOnlySupportMemoryCachedService<DashboardUserNote, IDashboardUserNoteService>
    {
        void BulkInsert(IEnumerable<DashboardUserNote> entries);
        int[] GetClosedNoteIds(string username);
    }

    [IgnoreT4Template]

    public partial class DashboardUserNoteService : ReadOnlySupportMemoryCachedService<DashboardUserNote, IDashboardUserNoteService>, IDashboardUserNoteService
    {
        public DashboardUserNoteService(IRepository<DashboardUserNote> repository,IReadOnlyRepository<DashboardUserNote> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        [CacheableInvalidator]

        public void BulkInsert(IEnumerable<DashboardUserNote> entries)
        {
            Repository.BulkInsert(entries);
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public int[] GetClosedNoteIds(string username)
        {
            return GetDefaultQuery()
                .Where(x => x.UserName == username)
                .Select(x => x.NoteId)
                .ToArray();
        }
    }

}
