namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using M=SeatDomain.Models;
    
    
    public partial interface IHolidayService: IReadOnlySupportMemoryCachedService<M.Holiday,IHolidayService>{}
    
    public partial class HolidayService : ReadOnlySupportMemoryCachedService<M.Holiday,IHolidayService>,IHolidayService
        {
    	
    	protected new IHolidayService This{get{return base.This<IHolidayService>();}} 
    
    	    }
    
}
