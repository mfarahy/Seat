namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IHistoryService: IAuditerService<History> {}
    
    public partial class HistoryService : ReadOnlySupportAuditerService<History,IRepository<History>, IReadOnlyRepository<History>,IHistoryService>,IHistoryService
        {
    	
    	protected new IHistoryService This{get{return base.This<IHistoryService>();}} 
    
    				public HistoryService(IRepository<History> repository, IReadOnlyRepository<History> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
