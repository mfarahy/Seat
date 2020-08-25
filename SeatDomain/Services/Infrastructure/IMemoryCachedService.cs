using Exir.Framework.Common;

namespace SeatDomain.Services
{
   
    public interface IMemoryCachedService<T> : IMemoryCachedService<T, IMemoryCachedService<T>>
      where T : class, new()
    {
    }

    public interface IMemoryCachedService<T, TR> : IBaseOfService<T, TR>, IMemoryCachedService
        where T : class, new()
        where TR: IService<T>
    {
        new T[] GetAll();
    }

    public interface IMemoryCachedService : IService
    {
        object[] GetAll();
    }
}
