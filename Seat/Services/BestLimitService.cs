namespace Seat.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using Seat.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IBestLimitService: IAuditerService<BestLimit>{}
    
    public partial class BestLimitService : ReadOnlySupportAuditerService<BestLimit,IRepository<BestLimit>, IReadOnlyRepository<BestLimit>,IBestLimitService>,IBestLimitService
        {
    	
    	protected new IBestLimitService This{get{return base.This<IBestLimitService>();}} 
    
    				public BestLimitService(IRepository<BestLimit> repository, IReadOnlyRepository<BestLimit> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
