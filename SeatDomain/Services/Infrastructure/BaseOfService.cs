using Exir.Framework.Common;
using System.Linq;
using Exir.Framework.Service.ActionResponses;
using System.Collections.Specialized;
using System;
using Exir.Framework.Service.Auditer;

namespace SeatDomain.Services
{
    public abstract class BaseOfService<T> : BaseOfService<T, IBaseOfService<T>>, IBaseOfService<T>
      where T : class, new()
    {
        public BaseOfService(IRepository<T> repository) : base(repository)
        {
        }

    }

    public abstract class BaseOfService<T, TR> : BaseOfService<T, IRepository<T>, IBaseOfService<T, TR>>, IBaseOfService<T, TR>
        where T : class, new()
        where TR: IService<T>
    {
        public BaseOfService(IRepository<T> repository) : base(repository)
        {
        }

        TR IBaseOfService<T, TR>.AsRule(string securityRule)
        {
            return AsRule<TR>(securityRule);
        }

        TR IBaseOfService<T, TR>.IgnoreSecurity()
        {
            return IgnoreSecurity<TR>();
        }

        TR IBaseOfService<T, TR>.This()
        {
            return This<TR>();
        }
    }

    public abstract class BaseOfService<T, R, TR> : AuditerService<T, R>, IBaseOfService<T, TR>
          where T : class, new()
          where R : IRepository<T>
          where TR: IService<T>
    {
        public BaseOfService(R repository) : base(repository)
        {
        }
    
        TR IBaseOfService<T, TR>.AsRule(string securityRule)
        {
            return AsRule<TR>(securityRule);
        }

        TR IBaseOfService<T, TR>.IgnoreSecurity()
        {
            return IgnoreSecurity<TR>();
        }

        TR IBaseOfService<T, TR>.This()
        {
            return This<TR>();
        }

        public virtual new TR This()
        {
            return base.This<TR>();
        }

        public virtual new TR IgnoreSecurity()
        {
            return base.IgnoreSecurity<TR>();
        }

        public virtual new TR AsRule(string securityRule)
        {
            return base.AsRule<TR>(securityRule);
        }
    }

}
