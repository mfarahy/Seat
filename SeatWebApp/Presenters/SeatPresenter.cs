using System.Collections.Generic;
using System.Collections.Specialized;
using Exir.Framework.Uie.Adapter;
using Exir.Framework.Uie.Bocrud;
using Exir.Framework.Uie.Contracts;
using SeatDomain.Services;

namespace SeatWebApp.Presenters
{

    public class SeatPresenter<T, V> : Presenter<T, V>, ISeatPresener<T>
        where T : class, new()
        where V : IModel
    {
        public IReadOnlySupportMemoryCachedService<T> ReadOnlySupportMemoryCachedService { get; }
        public SeatPresenter(object service) : base(service)
        {
            if (service is IReadOnlySupportMemoryCachedService<T>)
                ReadOnlySupportMemoryCachedService = (IReadOnlySupportMemoryCachedService<T>)service;
        }

        public override IModel Get(string key)
        {
            ReadOnlySupportMemoryCachedService?.AsReadOnly();
            return base.Get(key);
        }

        public override object DoAction(string cmd, object param, IQueryObject queryObject, NameValueCollection parameters, FdlModel model)
        {
            ReadOnlySupportMemoryCachedService?.AsReadOnly();
            return base.DoAction(cmd, param, queryObject, parameters, model);
        }

        public override IQueryResult GetList(IQueryObject filter)
        {
            ReadOnlySupportMemoryCachedService?.AsReadOnly();
            return base.GetList(filter);
        }

        public override IQueryResult GetList(IQueryObject filter, int pageNumber, int pageSize, string[] orderBy, bool computeCount)
        {           
            ReadOnlySupportMemoryCachedService?.AsReadOnly();
            return base.GetList(filter, pageNumber, pageSize, orderBy, computeCount);
        }

        public override IList<IModel> GetByKeis(params string[] kies)
        {
            ReadOnlySupportMemoryCachedService?.AsReadOnly();
            return base.GetByKeis(kies);
        }

        public override int GetCount(IQueryObject filter)
        {
            ReadOnlySupportMemoryCachedService?.AsReadOnly();
            return base.GetCount(filter);
        }



    }

    #region SeatPresenter<T> : SeatPresenter<T, ViewModel>
    public class SeatPresenter<T> : SeatPresenter<T, ViewModel>
where T : class, new()
    {
        public SeatPresenter(object service) : base(service)
        {

        }
    }
    #endregion

    public class SeatTreePresenter<T, V> : TreePresenter<T, V>
         where T : class, new()
        where V : IModel
    {
        public SeatTreePresenter(object service, string pkPropertyName, string parentIDProperty, string childProperty)
            : base(service, pkPropertyName, parentIDProperty, childProperty) { }
    }

    #region SeatTreePresenter<T> : SeatTreePresenter<T, ViewModel>
    public class SeatTreePresenter<T> : SeatTreePresenter<T, ViewModel>
   where T : class, new()
    {
        public SeatTreePresenter(object service, string pkPropertyName, string parentIDProperty, string childProperty)
       : base(service, pkPropertyName, parentIDProperty, childProperty)
        {
            if (service is IReadOnlySupportMemoryCachedService<T>)
                Service = (IReadOnlySupportMemoryCachedService<T>)service;
        }
        public IReadOnlySupportMemoryCachedService<T> Service { get; }

        public override IModel Get(string key)
        {
            Service?.AsReadOnly();
            return base.Get(key);
        }

        public override object DoAction(string cmd, object param, IQueryObject queryObject, NameValueCollection parameters, FdlModel model)
        {
            Service?.AsReadOnly();
            return base.DoAction(cmd, param, queryObject, parameters, model);
        }

        public override IQueryResult GetList(IQueryObject filter)
        {
            Service?.AsReadOnly();
            return base.GetList(filter);
        }

        public override IQueryResult GetList(IQueryObject filter, int pageNumber, int pageSize, string[] orderBy, bool computeCount)
        {
            Service?.AsReadOnly();
            return base.GetList(filter, pageNumber, pageSize, orderBy, computeCount);
        }

        public override IList<IModel> GetByKeis(params string[] kies)
        {
            Service?.AsReadOnly();
            return base.GetByKeis(kies);
        }

        public override int GetCount(IQueryObject filter)
        {
            Service?.AsReadOnly();
            return base.GetCount(filter);
        }
    }
    #endregion

    public interface ISeatPresener<T> : ISeatPresener
        where T : class, new()
    {
        IReadOnlySupportMemoryCachedService<T> ReadOnlySupportMemoryCachedService { get; }
    }

    public interface ISeatPresener : IExtendedManager, IUserCommand, IPresenterAdapter, ISummarizerPresenter, IMultipleUpdaterPresenter
    {
    }
}