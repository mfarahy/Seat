using Exir.Framework.Common;
using System.Linq;
using System;
using System.Linq.Expressions;
using Exir.Framework.Common.Caching;
using System.Collections.Generic;
using System.Collections;
using Exir.Framework.Security.SchemaSecurity;
using Exir.Framework.Service;

namespace SeatDomain.Services
{
    public abstract class ReadOnlySupportMemoryCachedService<T, TR> : ReadOnlySupportMemoryCachedService<T, IRepository<T>, IReadOnlyRepository<T>, TR>, IReadOnlySupportMemoryCachedService<T, TR>
        where T : class, IEntityBase, new()
         where TR : IService<T>
    {
        #region ctors
        public ReadOnlySupportMemoryCachedService(IRepository<T> repository, IReadOnlyRepository<T> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

       
        #endregion

    }
    public abstract class ReadOnlySupportMemoryCachedService<T> : ReadOnlySupportMemoryCachedService<T, IRepository<T>, IReadOnlyRepository<T>, ReadOnlySupportMemoryCachedService<T>>, IReadOnlySupportMemoryCachedService<T>
        where T : class, IEntityBase, new()
    {
        #region ctors
        public ReadOnlySupportMemoryCachedService(IRepository<T> repository, IReadOnlyRepository<T> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

    
        #endregion

        #region imp...
        IReadOnlySupportMemoryCachedService<T> IReadOnlySupportBaseOfService<T, IRepository<T>, IReadOnlyRepository<T>, IReadOnlySupportMemoryCachedService<T>>.AsReadOnly()
        {
            return AsReadOnly<IReadOnlySupportMemoryCachedService<T>>();
        }

        IReadOnlySupportMemoryCachedService<T> IBaseOfService<T, IReadOnlySupportMemoryCachedService<T>>.AsRule(string securityRule)
        {
            return AsRule<IReadOnlySupportMemoryCachedService<T>>(securityRule);
        }

        IReadOnlySupportMemoryCachedService<T> IBaseOfService<T, IReadOnlySupportMemoryCachedService<T>>.IgnoreSecurity()
        {
            return IgnoreSecurity<IReadOnlySupportMemoryCachedService<T>>();
        }

        IReadOnlySupportMemoryCachedService<T> IBaseOfService<T, IReadOnlySupportMemoryCachedService<T>>.This()
        {
            return This<IReadOnlySupportMemoryCachedService<T>>();
        }
        #endregion
    }

    public abstract class ReadOnlySupportMemoryCachedService<T, R, RR, TR> :
        ReadOnlySupportBaseOfService<T, R, RR, TR>, IReadOnlySupportMemoryCachedService<T, R, RR, TR>
        where T : class, IEntityBase, new()
        where R : IRepository<T>, RR
        where RR : IReadOnlyRepository<T>
        where TR : IService<T>

    {
        #region CTORS

        public ReadOnlySupportMemoryCachedService(R repository, RR readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }
     
        #endregion

      
        [ThreadSafe]

        public bool FullCacheMode
        {
            get
            {
                return GetOfCurrentContext<bool>("FullCacheMode");
            }
            set
            {
                SetToCurrentContext("FullCacheMode", value);
            }
        }
        protected virtual new IReadOnlySupportMemoryCachedService<T, R, RR, TR> This { get { return This<IReadOnlySupportMemoryCachedService<T, R, RR, TR>>(); } }

        [Cacheable(KeySpel = "GetCacheKeyWithoutPrefix(#key)", CacheName = CacheConstants.InMemoryCache)]

        [ReadOnlySupport]

        public override T GetEntity(object key, Timestamp version)
        {
            try
            {
                var newInstance = GetNewEntity();
                var pk = newInstance.GetFields().Where(x => x.Kind == FieldKinds.PrimaryKey).First();
                newInstance.SetValue(pk.Name, Typing.ChangeType(key, pk.PropertyType));

                if (String.IsNullOrEmpty(PeekRule()))
                    AsRule(Mode.OnSingleRead.ToString());

                var entity = This.GetAll().AsQueryable()
                    .Where((Expression<Func<T, bool>>)newInstance.GetPrimaryKeyPrediacate())
                    .First();

                entity.ChangeTracker.ResetChanges();
                return entity;
            }
            catch (Exception ex)
            {
                LogWarn("Cannot get entity from cache entity type is {0} and pk is {1}", ex, typeof(R).Name, key);
                return base.GetEntity(key, version);
            }
        }

        [ReadOnlySupport]

        public override IQueryable<T> GetDefaultQuery()
        {
            if (FullCacheMode)
                return This.GetAll().AsQueryable();
            else
                return base.GetDefaultQuery();
        }

        [ReadOnlySupport]

        public override IQueryable<T> GetDefaultQuery(IEnumerable<IField> navigationalFields)
        {
            if (FullCacheMode)
                return This.GetAll().AsQueryable();
            else
                return base.GetDefaultQuery(navigationalFields);
        }

        [ReadOnlySupport]

        public override IQueryable<T> GetDefaultQuery<TProperty>(Expression<Func<T, TProperty>> selection)
        {
            if (FullCacheMode)
                return This.GetAll().AsQueryable();
            else
                return base.GetDefaultQuery(selection);
        }

        [ReadOnlySupport]

        public override IQueryable GetQuery(IQueryable<T> baseQuery, object queryObject)
        {
            if (FullCacheMode)
                return This.GetAll().AsQueryable();
            else
                return base.GetQuery(baseQuery, queryObject);
        }
        public override string GetCacheKeyWithoutPrefix(params object[] args)
        {
#if DEBUG
            string typename = typeof(T).Name;
            typename = typename.ToLower();

#endif
            var rule = PeekRule();
            var array = new ArrayList { rule };
            if (args != null)
                array.AddRange(args);

            array.Add(PeekCheckSecurity());

            if (!String.IsNullOrEmpty(rule))
                array.Add(rule);

            if (Authenticater.Value.CurrentIdentity.IsAuthenticated)
            {
                var security = ObjectRegistry.GetObject<ISchemaSecurityProvider>();
                var roles = String.Join("_", security
                    .GetUserRoles(Authenticater.Value.CurrentIdentity.Name)
                    .Distinct()
                    .Select(x => x.Key)
                    .OrderBy(x => x))
                    .ToLower()
                    .Abbrivate();
                array.Add(roles);
            }

            return base.GetCacheKeyWithoutPrefix(array.ToArray());
        }

        [Cacheable(KeySpel = "GetCacheKeyWithoutPrefix()", CacheName = CacheConstants.InMemoryCache, AllowNullOrEmpty = false)]

        [ReadOnlySupport]

        public virtual T[] GetAll()
        {
            base.AsReadOnly();
            return base.GetDefaultQuery(null).ToArray();
        }
        [ReadOnlySupport]

        object[] IMemoryCachedService.GetAll()
        {
            base.AsReadOnly();
            return base.GetDefaultQuery(null).ToArray().Cast<object>().ToArray();
        }
    }

}
