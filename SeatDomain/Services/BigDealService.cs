namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IBigDealService: IAuditerService<BigDeal> {}
    
    public partial class BigDealService : ReadOnlySupportAuditerService<BigDeal,IRepository<BigDeal>, IReadOnlyRepository<BigDeal>,IBigDealService>,IBigDealService
        {
    	
    	protected new IBigDealService This{get{return base.This<IBigDealService>();}} 
    
    	    }
    
}
