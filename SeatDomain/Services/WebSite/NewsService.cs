using Exir.Framework.Common.Caching;
using Exir.Framework.Service.ActionResponses;

namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using SeatDomain.Models;
    using System.Collections.Generic;
    using System.Linq;

    [IgnoreT4Template]

    public partial interface INewsService : IReadOnlySupportMemoryCachedService<News, INewsService>
    {
        List<News> GetTopNews(string categoryCode);
        News GetTopNewsById(int id);
        News GetSlideById(int id);
        byte[] GetImage(int id);
        List<News> GetSlides(string categoryCode);
        DataPageResponse<NewsRow> Report(SearchSpecification<NewsRow> searchModel);
        News GetWithDetails(int id);
    }

    public partial class NewsService : ReadOnlySupportMemoryCachedService<News, INewsService>, INewsService
    {
        public NewsService(IRepository<News> repository,IReadOnlyRepository<News> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        public override object Save(News entity)
        {
            if (!entity.HasKey())
            {
                if (GetDefaultQuery().Where(e => e.CategoryPk == entity.CategoryPk).Any(e => e.SortNumber == 1))
                {
                    Repository.BulkUpdate(x => x.CategoryPk == entity.CategoryPk, news => new News { SortNumber = news.SortNumber + 1 });
                }
                entity.SortNumber = 1;
            }
            return base.Save(entity);
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public List<News> GetTopNews(string categoryCode)
        {
            return GetByCategoryCode(categoryCode)
                .OrderBy(x => x.SortNumber).ToList();
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public News GetTopNewsById(int id)
        {
            return GetByCategoryCode(Constants.News.TopNewsCategoryCode).FirstOrDefault(e => e.NewsPk == id);
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public News GetSlideById(int id)
        {
            return GetByCategoryCode(Constants.News.SlideNewsCategoryCode).FirstOrDefault(e => e.NewsPk == id);
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public byte[] GetImage(int id)
        {
            var item = GetDefaultQuery().FirstOrDefault(e => e.NewsPk == id);
            return item?.Image;
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public List<News> GetSlides(string categoryCode)
        {
            return GetByCategoryCode(categoryCode).OrderBy(e => e.SortNumber).ToList();
        }

        public override IQueryable<News> GetDefaultQuery()
        {
            IgnoreSecurity();
            return base.GetDefaultQuery().Where(e => !e.IsArchived && !e.IsDeleted);
        }

        public DataPageResponse<NewsRow> Report(SearchSpecification<NewsRow> searchModel)
        {
            var q = GetQuery();
            var count = q.Count();
            if (searchModel.WhereClouse != null)
                q = q.Where(searchModel.WhereClouse);

            q = ApplySortingRule(q, searchModel, x => x.CategoryPk);
            q = ApplyPagingRule(q, searchModel);

            return new DataPageResponse<NewsRow>(q.ToList(), searchModel.PageNumber, count);
        }

        private IQueryable<NewsRow> GetQuery()
        {
            var categorySrv = ServiceFactory.Create<INewsCategoryService>();
            categorySrv.MakeFriend(this);

            return from news in GetDefaultQuery()
                   join newsCategory in categorySrv.IgnoreSecurity().GetDefaultQuery() on news.CategoryPk equals newsCategory.CategoryPk
                   select new NewsRow
                   {
                       SortNumber = news.SortNumber,
                       NewsPk = news.NewsPk,
                       Subject = news.Subject,
                       Abstract = news.Abstract,
                       Content = news.Content,
                       CategoryPk = news.CategoryPk,
                       IsDeleted = news.IsDeleted,
                       IsArchived = news.IsArchived,
                       NewsArchiveDate = news.NewsArchiveDate,
                       NewsExpireDate = news.NewsExpireDate,
                       Audit_CreateDate = news.Audit_CreateDate,
                       Audit_CreatorUserName = news.Audit_CreatorUserName,
                       Audit_CreatorIP = news.Audit_CreatorIP,
                       Audit_LastModifyDate = news.Audit_LastModifyDate,
                       Audit_LastModifierUserName = news.Audit_LastModifierUserName,
                       Audit_LastModifierIP = news.Audit_LastModifierIP,
                       Metadata = news.Metadata,
                       Image = news.Image,
                       CategoryTitle = newsCategory.CategoryTitle
                   };
        }

        private IQueryable<News> GetByCategoryCode(string code)
        {
            var categorySrv = ServiceFactory.Create<INewsCategoryService>();
            categorySrv.MakeFriend(this);

            var query = from news in GetDefaultQuery()
                        join newsCategory in categorySrv.IgnoreSecurity().GetDefaultQuery() on news.CategoryPk equals newsCategory.CategoryPk
                        where newsCategory.Code == code
                        orderby news.SortNumber
                        select news;
            return query;
        }

        public News GetWithDetails(int id)
        {
            return AsRule(Constants.RuleSets.WithDetails.ToString()).GetDefaultQuery(news => new { news.NewsCategory }).FirstOrDefault(news => news.NewsPk == id);
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        private bool IsTopNews(int newsPk)
        {
            return GetByCategoryCode(Constants.News.TopNewsCategoryCode).Any(e => e.NewsPk == newsPk);
        }


    }

}
