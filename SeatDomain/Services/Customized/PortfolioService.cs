namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Models;
    using Exir.Framework.Service.Auditer;

    public partial interface IPortfolioService : IAuditerService<Portfolio> { }
    [IgnoreT4Template]
    public partial class PortfolioService : ReadOnlySupportAuditerService<Portfolio, IRepository<Portfolio>, IReadOnlyRepository<Portfolio>, IPortfolioService>, IPortfolioService
    {

        protected new IPortfolioService This { get { return base.This<IPortfolioService>(); } }

        public PortfolioService(IRepository<Portfolio> repository, IReadOnlyRepository<Portfolio> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public override object Save(Portfolio entity)
        {
            if (!entity.HasKey())
            {
                entity.Owner = Authenticater.Value.CurrentIdentity.Name.ToLower();
            }
            return base.Save(entity);
        }

    }

}
