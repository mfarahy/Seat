using Exir.Framework.Common;
using Exir.Framework.DataAccess.EntityFramework;
using SeatDomain.Models;
using System.Data.Entity;

namespace SeatDomain.Repository
{
    public class BackstageJobRepository : EntityFrameworkRepository<BackstageJob>, IRepository<BackstageJob>, IBackstageJobRepository
    {
        public BackstageJobRepository() { }

        public BackstageJobRepository(string contextObjectId) : base(contextObjectId)
        {
        }
        public BackstageJobRepository(ContextCreator creator)
            : base(creator)
        {
        }

        public BackstageJobRepository(DbContext context)
           : base(context)
        {
        }

        public BackstageJobRepository(DbSet<BackstageJob> repository, ContextCreator creator)
            : base(repository, creator)
        {
        }

        public void RawRun(string script)
        {
            Context.Database.ExecuteSqlCommand(script);
        }
    }
}
