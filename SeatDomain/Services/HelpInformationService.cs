namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using M=SeatDomain.Models;
    
    
    public partial interface IHelpInformationService: IReadOnlySupportMemoryCachedService<M.HelpInformation,IHelpInformationService>{}
    
    public partial class HelpInformationService : ReadOnlySupportMemoryCachedService<M.HelpInformation,IHelpInformationService>,IHelpInformationService
        {
    	
    	protected new IHelpInformationService This{get{return base.This<IHelpInformationService>();}} 
    
    	    }
    
}
