using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Exir.Framework.Common;
using Exir.Framework.Common.Fasterflect;
using Exir.Framework.Service;
using Exir.Framework.Service.ActionResponses;
using Newtonsoft.Json;
using Exir.Framework.Common.Caching;
using SeatDomain.Models.Customized;
using SeatDomain.Models.SearchModels;

namespace SeatWebApp.Services
{
    public interface ICacheReportService : ICrudService<CacheReport>
    {
        SummaryDataPageResponse<CacheReport> GetMemoryCacheItems(CacheReportSearchModel searchModel);
        ActionResponse DeleteAllCache();
    }
    public class CacheReportService : VirtualCrudService<CacheReport>, ICacheReportService
    {
        public override CacheReport GetEntity(object key, Timestamp version)
        {
            var value = HttpContext.Current.Cache[key.ToString()];
            return ToCacheReportItem(key.ToString(), value, true);
        }

        public SummaryDataPageResponse<CacheReport> GetMemoryCacheItems(CacheReportSearchModel searchModel)
        {
            var cacheList = new Dictionary<string, CacheReport>();
            foreach (var cache in HttpContext.Current.Cache)
            {
                if (cache is DictionaryEntry)
                {
                    var item = (DictionaryEntry)cache;
                    string key = item.Key?.ToString()?.ToLower();
                    if (!String.IsNullOrEmpty(key) && !cacheList.ContainsKey(key) && (string.IsNullOrEmpty(searchModel.Key) || key.Contains(searchModel.Key)))
                    {
                        cacheList.Add(key, ToCacheReportItem(item.Key, item.Value));
                    }
                }
            }

            List<CacheReport> data = null;
            if (!String.IsNullOrEmpty(searchModel.Key))
                data = cacheList.Where(x => x.Key.IndexOf(searchModel.Key) >= 0).Select(x => x.Value).ToList();
            else
                data = cacheList.Values.ToList();

            var count = data.Count;
            var query = data.AsQueryable();
            query = ApplySortingRule(query, searchModel, x => x.Key);
            query = ApplyPagingRule(query, searchModel);


            return new SummaryDataPageResponse<CacheReport>(query.ToList(), searchModel.PageNumber, count, false);
        }

        private CacheReport ToCacheReportItem(object key, object value, bool serializeValue = false)
        {
            var item = new CacheReport
            {
                Key = key.ToString()
            };

            if (serializeValue)
            {
                item.Value = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }

            item.HitCount = value.TryGetFieldValue("HitCount")?.ToString();
            item.ExpireDt = value.TryGetFieldValue("ExpireDt")?.ToString();
            return item;
        }
        public override bool Delete(CacheReport entity)
        {
            if (HttpContext.Current.Cache != null && HttpContext.Current.Cache[entity.Key] != null)
                HttpContext.Current.Cache.Remove(entity.Key);
            return true;
        }

        public override void DeleteByKeis(object[] keis)
        {
            foreach (var key in keis)
            {
                Delete(new CacheReport { Key = key.ToString() });
            }
        }

        public ActionResponse DeleteAllCache()
        {
            var serviceCache = ObjectRegistry.GetObject<ICacheProvider>();
            serviceCache.SendMessage(CacheConstants.KnownChannels.Cache(), CacheConstants.Cache_Messages.RefreshCache(), String.Empty);

            return new ActionResponse(true);
        }
    }
}
