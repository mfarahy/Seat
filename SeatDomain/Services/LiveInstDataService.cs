namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface ILiveInstDataService: IAuditerService<LiveInstData> {}
    
    public partial class LiveInstDataService : ReadOnlySupportAuditerService<LiveInstData,IRepository<LiveInstData>, IReadOnlyRepository<LiveInstData>,ILiveInstDataService>,ILiveInstDataService
        {
    	
    	protected new ILiveInstDataService This{get{return base.This<ILiveInstDataService>();}} 
    
    				public LiveInstDataService(IRepository<LiveInstData> repository, IReadOnlyRepository<LiveInstData> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
