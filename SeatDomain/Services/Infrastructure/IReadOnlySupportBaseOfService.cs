using Exir.Framework.Common;
using System.Linq;
using Exir.Framework.Service;
using Exir.Framework.Service.ActionResponses;
using System.Collections.Specialized;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using Exir.Framework.Service.Auditer;

namespace SeatDomain.Services
{
    public interface IReadOnlySupportBaseOfService<T, TR> : IReadOnlySupportBaseOfService<T, IRepository<T>, IReadOnlyRepository<T>, TR>
           where T : class, new()
          where TR: IService<T>
    {
    }

    public interface IReadOnlySupportBaseOfService<T> : IReadOnlySupportBaseOfService<T, IRepository<T>, IReadOnlyRepository<T>, IReadOnlySupportBaseOfService<T>>
            where T : class, new()
    {
    }

    public interface IReadOnlySupportBaseOfService<T, R, RR, TR> : IReadOnlySupportBaseOfService, IAuditerService<T>, IReadOnlySupportCrudService<T, RR>, IBaseOfService<T, TR>
        where T : class, new()
        where R : IRepository<T>, RR
        where RR : IReadOnlyRepository<T>
        where TR: IService<T>
    {
        new TR AsReadOnly();
    }

    public interface IReadOnlySupportBaseOfService
    {
    }
}
