using Exir.Framework.Common;
using Exir.Framework.Service.ActionResponses;
using Exir.Framework.Service.Auditer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public abstract class ReadOnlySupportBaseOfService<T,TR> : ReadOnlySupportBaseOfService<T, IRepository<T>, IReadOnlyRepository<T>, TR>,
      IReadOnlySupportBaseOfService<T,TR>
        where T : class, new()
        where TR: IService<T>
    {
        public ReadOnlySupportBaseOfService(IRepository<T> repository, IReadOnlyRepository<T> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public ReadOnlySupportBaseOfService(IRepository<T> repository, IReadOnlyRepository<T> readonlyRepository, AuditerInfo info) : base(repository, readonlyRepository, info)
        {
        }
    }
    public abstract class ReadOnlySupportBaseOfService<T> : ReadOnlySupportBaseOfService<T, IRepository<T>, IReadOnlyRepository<T>, IReadOnlySupportBaseOfService<T>>,
         IReadOnlySupportBaseOfService<T>
        where T : class, new()
    {
        public ReadOnlySupportBaseOfService(IRepository<T> repository, IReadOnlyRepository<T> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public ReadOnlySupportBaseOfService(IRepository<T> repository, IReadOnlyRepository<T> readonlyRepository, AuditerInfo info) : base(repository, readonlyRepository, info)
        {
        }
    }

    public abstract class ReadOnlySupportBaseOfService<T, R, RR, TR> : ReadOnlySupportAuditerService<T, R, RR, TR>, IReadOnlySupportBaseOfService<T, R, RR, TR>
        where T : class, new()
        where R : IRepository<T>, RR
        where RR : IReadOnlyRepository<T>
        where TR: IService<T>
    {
        public ReadOnlySupportBaseOfService(R repository, RR readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public ReadOnlySupportBaseOfService(R repository, RR readonlyRepository, AuditerInfo info) : base(repository, readonlyRepository, info)
        {
        }

        public virtual FileContentResponse ExportToJbl(object spec, ISpecificationQuaryable<T> query, FormPageInfo pageInfo)
        {
            throw new NotImplementedException();
        }

        TR IReadOnlySupportBaseOfService<T, R, RR, TR>.AsReadOnly()
        {
            return AsReadOnly<TR>();
        }

        TR IBaseOfService<T, TR>.This()
        {
            return This<TR>();
        }

        public virtual new TR AsReadOnly()
        {
            return (TR)base.AsReadOnly();
        }
    }
}
