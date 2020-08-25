namespace SeatDomain.Services
{
    using System;
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Common.Caching;
    using System.Linq;

    public partial interface IHelpInformationService
    {
        HelpInformation[] GetHelps(string formName);
    }

    
    public partial class HelpInformationService
    {
        public HelpInformationService(IRepository<HelpInformation> repository, IReadOnlyRepository<HelpInformation> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }
        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public HelpInformation[] GetHelps(string formName)
        {
            IgnoreSecurity();
            var helps = This<IHelpInformationService>().GetAll();
            return helps.Where(h => h.FormName == formName).ToArray();
        }


    }

}
