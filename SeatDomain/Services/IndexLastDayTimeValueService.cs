namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IIndexLastDayTimeValueService: IAuditerService<IndexLastDayTimeValue> {}
    
    public partial class IndexLastDayTimeValueService : ReadOnlySupportAuditerService<IndexLastDayTimeValue,IRepository<IndexLastDayTimeValue>, IReadOnlyRepository<IndexLastDayTimeValue>,IIndexLastDayTimeValueService>,IIndexLastDayTimeValueService
        {
    	
    	protected new IIndexLastDayTimeValueService This{get{return base.This<IIndexLastDayTimeValueService>();}} 
    
    	    }
    
}
