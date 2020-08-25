namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using M=SeatDomain.Models;
    
    
    public partial interface IHardLogService: IReadOnlySupportBaseOfService<M.HardLog,IHardLogService>{}
    
    public partial class HardLogService : ReadOnlySupportBaseOfService<M.HardLog,IHardLogService>,IHardLogService
        {
    	
    	protected new IHardLogService This{get{return base.This<IHardLogService>();}} 
    
    				public HardLogService(IRepository<M.HardLog> repository, IReadOnlyRepository<M.HardLog> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
