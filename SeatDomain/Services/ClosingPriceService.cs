namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IClosingPriceService: IAuditerService<ClosingPrice> {}
    
    public partial class ClosingPriceService : ReadOnlySupportAuditerService<ClosingPrice,IRepository<ClosingPrice>, IReadOnlyRepository<ClosingPrice>,IClosingPriceService>,IClosingPriceService
        {
    	
    	protected new IClosingPriceService This{get{return base.This<IClosingPriceService>();}} 
    
    				public ClosingPriceService(IRepository<ClosingPrice> repository, IReadOnlyRepository<ClosingPrice> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
