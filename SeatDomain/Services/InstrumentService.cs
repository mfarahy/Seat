namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;
    
    public partial interface IInstrumentService: IReadOnlySupportMemoryCachedService<Instrument,IRepository<Instrument>, IReadOnlyRepository<Instrument>,IInstrumentService> {}
    
    public partial class InstrumentService : ReadOnlySupportMemoryCachedService<Instrument,IRepository<Instrument>, IReadOnlyRepository<Instrument>,IInstrumentService>,IInstrumentService
        {
    	
    	protected new IInstrumentService This{get{return base.This<IInstrumentService>();}} 
    
    	    }
    
}
