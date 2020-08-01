namespace Seat.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using Seat.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IInstrumentService: IAuditerService<Instrument>{}
    
    public partial class InstrumentService : ReadOnlySupportAuditerService<Instrument,IRepository<Instrument>, IReadOnlyRepository<Instrument>,IInstrumentService>,IInstrumentService
        {
    	
    	protected new IInstrumentService This{get{return base.This<IInstrumentService>();}} 
    
    				public InstrumentService(IRepository<Instrument> repository, IReadOnlyRepository<Instrument> readOnlyRepository): base(repository, readOnlyRepository)
                {
                }
    
    			    }
    
}
