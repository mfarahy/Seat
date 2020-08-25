using Exir.Framework.Common;
using Exir.Framework.Service;
using Exir.Framework.Service.Support;
using SeatDomain.Models;

namespace SeatDomain.Services
{
    public partial interface IKeyValueService : IBaseOfService<KeyValue, IKeyValueService>, IKeyValueCellService<KeyValue>
    { }
    [IgnoreT4Template]
    public partial class KeyValueService : BaseOfService<KeyValue, IKeyValueService>, IKeyValueService
    {
        protected new IKeyValueService This { get { return This<IKeyValueService>(); } }
        public KeyValueService(IRepository<KeyValue> repository) : base(repository)
        {
        }
    }

}
