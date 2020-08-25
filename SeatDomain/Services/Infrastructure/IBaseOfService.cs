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
    public interface IBaseOfService<T, TR> : IBaseOfService<T>, IAuditerService<T>
    where T : class, new()
    where TR: IService<T>
    {
        new TR IgnoreSecurity();
        new TR AsRule(string securityRule);
        new TR This();
    }

    public interface IBaseOfService<T> : IBaseOfService, IAuditerService<T>
        where T : class, new()
    {
    }

    public interface IBaseOfService
    {
    }
}
