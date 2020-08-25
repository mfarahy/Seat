namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IInstrumentCategoryService: IReadOnlySupportMemoryCachedService<InstrumentCategory,IRepository<InstrumentCategory>, IReadOnlyRepository<InstrumentCategory>,IInstrumentCategoryService> {}
    
    public partial class InstrumentCategoryService : ReadOnlySupportMemoryCachedService<InstrumentCategory,IRepository<InstrumentCategory>, IReadOnlyRepository<InstrumentCategory>,IInstrumentCategoryService>,IInstrumentCategoryService
        {
    	
    	protected new IInstrumentCategoryService This{get{return base.This<IInstrumentCategoryService>();}} 
    
    				public InstrumentCategoryService(IRepository<InstrumentCategory> repository, IReadOnlyRepository<InstrumentCategory> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
