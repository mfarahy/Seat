namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using M=SeatDomain.Models;
    
    
    public partial interface IAspNetRoleService: IReadOnlySupportBaseOfService<M.AspNetRole,IAspNetRoleService>{}
    
    public partial class AspNetRoleService : ReadOnlySupportBaseOfService<M.AspNetRole,IAspNetRoleService>,IAspNetRoleService
        {
    	
    	protected new IAspNetRoleService This{get{return base.This<IAspNetRoleService>();}} 
    
    				public AspNetRoleService(IRepository<M.AspNetRole> repository, IReadOnlyRepository<M.AspNetRole> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
