namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IShareHolderChangeService: IAuditerService<ShareHolderChange> {}
    
    public partial class ShareHolderChangeService : ReadOnlySupportAuditerService<ShareHolderChange,IRepository<ShareHolderChange>, IReadOnlyRepository<ShareHolderChange>,IShareHolderChangeService>,IShareHolderChangeService
        {
    	
    	protected new IShareHolderChangeService This{get{return base.This<IShareHolderChangeService>();}} 
    
    				public ShareHolderChangeService(IRepository<ShareHolderChange> repository, IReadOnlyRepository<ShareHolderChange> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
