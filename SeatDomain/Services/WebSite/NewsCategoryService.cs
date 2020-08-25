namespace SeatDomain.Services
{
    using Exir.Framework.Common;
    using Exir.Framework.Service;
    using SeatDomain.Exceptions;
    using SeatDomain.Models;

    public partial interface INewsCategoryService : IReadOnlySupportMemoryCachedService<NewsCategory, INewsCategoryService> { }

    [IgnoreT4Template]

    public partial class NewsCategoryService : ReadOnlySupportMemoryCachedService<NewsCategory, INewsCategoryService>, INewsCategoryService
    {
        protected new INewsCategoryService This { get { return This<INewsCategoryService>(); } }
        public NewsCategoryService(IRepository<NewsCategory> repository,IReadOnlyRepository<NewsCategory> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }


        public override bool Delete(NewsCategory entity)
        {
            CheckIsSystemCategory(entity.CategoryPk);
            return base.Delete(entity);
        }

        public override void DeleteByKeis(object[] keis)
        {
            foreach (var pk in keis)
            {
                CheckIsSystemCategory(pk);
            }
            base.DeleteByKeis(keis);
        }

        private void CheckIsSystemCategory(object pk)
        {
            var item = GetEntity(pk, null);
            if (item.Code == Constants.News.TopNewsCategoryCode || item.Code == Constants.News.SlideNewsCategoryCode)
                throw new SeatException("امکان حذف دسته خبرهای سیستمی وجود ندارد");
        }
    }

}
