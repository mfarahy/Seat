using Exir.Framework.Common;
using Exir.Framework.Service;
using SeatDomain.Models;
using SeatDomain.Models.Service;
using SeatDomain.Repository;
using System.Linq;

namespace SeatDomain.Services
{
    public partial interface IUserActivityService : IReadOnlySupportBaseOfService<UserActivity, IUserActivityRepository, IUserActivityReadOnlyRepository, IUserActivityService>
    {
        UserActivitySummaryDataPageResponse Search(SearchSpecification<UserActivity> searchObject);
    }

    [IgnoreT4Template]

    public partial class UserActivityService : ReadOnlySupportBaseOfService<UserActivity, IUserActivityRepository, IUserActivityReadOnlyRepository, IUserActivityService>, IUserActivityService
    {
        protected new IUserActivityService This { get { return base.This<IUserActivityService>(); } }
        public UserActivityService(IUserActivityRepository repository, IUserActivityReadOnlyRepository readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        //int pageNumber,int pageSize,string[] orderBy,string whereClouse,object[] whereClouseArguments
        public UserActivitySummaryDataPageResponse Search(SearchSpecification<UserActivity> searchObject)
        {
            var query = GetDefaultQuery();

            if (searchObject.WhereClouse != null)
                query = query.Where(searchObject.WhereClouse);

            var summaryObject = ReadOnlyRepository.TakeSummary(query);

            query = ApplySortingRule(query, searchObject, x => x.Id);
            query = ApplyPagingRule(query, searchObject);

            return new UserActivitySummaryDataPageResponse(query, searchObject.PageNumber, summaryObject.TotalActivityCount, false)
            {
                Summary = summaryObject,
            };
        }

    }

}
