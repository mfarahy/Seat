namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using M=SeatDomain.Models;
    
    
    public partial interface IJobLogService: IReadOnlySupportBaseOfService<M.JobLog,IJobLogService>{}
    
    public partial class JobLogService : ReadOnlySupportBaseOfService<M.JobLog,IJobLogService>,IJobLogService
        {
    	
    	protected new IJobLogService This{get{return base.This<IJobLogService>();}} 
    
    	    }
    
}
