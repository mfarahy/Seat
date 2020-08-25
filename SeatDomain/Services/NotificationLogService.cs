namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface INotificationLogService: IAuditerService<NotificationLog> {}
    
    public partial class NotificationLogService : ReadOnlySupportAuditerService<NotificationLog,IRepository<NotificationLog>, IReadOnlyRepository<NotificationLog>,INotificationLogService>,INotificationLogService
        {
    	
    	protected new INotificationLogService This{get{return base.This<INotificationLogService>();}} 
    
    				public NotificationLogService(IRepository<NotificationLog> repository, IReadOnlyRepository<NotificationLog> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
