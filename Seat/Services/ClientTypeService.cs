namespace Seat.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using Seat.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IClientTypeService: IAuditerService<ClientType>{}
    
    public partial class ClientTypeService : ReadOnlySupportAuditerService<ClientType,IRepository<ClientType>, IReadOnlyRepository<ClientType>,IClientTypeService>,IClientTypeService
        {
    	
    	protected new IClientTypeService This{get{return base.This<IClientTypeService>();}} 
    
    				public ClientTypeService(IRepository<ClientType> repository, IReadOnlyRepository<ClientType> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
