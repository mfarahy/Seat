namespace Seat.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using Seat.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IMessageService: IAuditerService<Message>{}
    
    public partial class MessageService : ReadOnlySupportAuditerService<Message,IRepository<Message>, IReadOnlyRepository<Message>,IMessageService>,IMessageService
        {
    	
    	protected new IMessageService This{get{return base.This<IMessageService>();}} 
    
    				public MessageService(IRepository<Message> repository, IReadOnlyRepository<Message> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
