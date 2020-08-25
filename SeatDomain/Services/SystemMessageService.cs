namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using M=SeatDomain.Models;
    
    
    public partial interface ISystemMessageService: IReadOnlySupportMemoryCachedService<M.SystemMessage,ISystemMessageService>{}
    
    public partial class SystemMessageService : ReadOnlySupportMemoryCachedService<M.SystemMessage,ISystemMessageService>,ISystemMessageService
        {
    	
    	protected new ISystemMessageService This{get{return base.This<ISystemMessageService>();}} 
    
    	    }
    
}
