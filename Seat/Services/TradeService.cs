namespace Seat.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using Seat.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface ITradeService: IAuditerService<Trade>{}
    
    public partial class TradeService : ReadOnlySupportAuditerService<Trade,IRepository<Trade>, IReadOnlyRepository<Trade>,ITradeService>,ITradeService
        {
    	
    	protected new ITradeService This{get{return base.This<ITradeService>();}} 
    
    				public TradeService(IRepository<Trade> repository, IReadOnlyRepository<Trade> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
