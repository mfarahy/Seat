using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public interface IReadOnlySupportMemoryCachedService<T> : IReadOnlySupportMemoryCachedService<T, IReadOnlySupportMemoryCachedService<T>>
      where T : class, new()
    {
    }
    public interface IReadOnlySupportMemoryCachedService<T, TR> : IReadOnlySupportMemoryCachedService<T, IRepository<T>, TR>
        where T : class, new()
        where TR: IService<T>
    {
    }
    public interface IReadOnlySupportMemoryCachedService<T, R, TR> : IReadOnlySupportMemoryCachedService<T, IRepository<T>, IReadOnlyRepository<T>, TR>
        where T : class, new()
        where TR: IService<T>
    {
    }
    public interface IReadOnlySupportMemoryCachedService<T, R, RR, TR> : IReadOnlySupportBaseOfService<T, R, RR, TR>, IMemoryCachedService
        where T : class, new()
        where R : IRepository<T>, RR
        where RR : IReadOnlyRepository<T>
        where TR: IService<T>
    {
        new T[] GetAll();
    }
}
