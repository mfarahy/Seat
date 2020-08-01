namespace Seat.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using Seat.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface ICodalMessageService: IAuditerService<CodalMessage>{}
    
    public partial class CodalMessageService : ReadOnlySupportAuditerService<CodalMessage,IRepository<CodalMessage>, IReadOnlyRepository<CodalMessage>,ICodalMessageService>,ICodalMessageService
        {
    	
    	protected new ICodalMessageService This{get{return base.This<ICodalMessageService>();}} 
    
    				public CodalMessageService(IRepository<CodalMessage> repository, IReadOnlyRepository<CodalMessage> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
