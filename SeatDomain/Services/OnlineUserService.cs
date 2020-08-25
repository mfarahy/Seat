using System.Collections.Generic;
using System.Linq;
using Exir.Framework.Common;
using Exir.Framework.Service;
using Exir.Framework.Service.ActionResponses;
using Newtonsoft.Json;
using SeatDomain.Models;

namespace SeatDomain.Services
{
    public interface IOnlineUserService : ICrudService<OnlineUser>
    {
        SummaryDataPageResponse<OnlineUser> GetOnlineUsers(OnlineUserSearchModel searchModel);
        void ExpireSession(string sessionId);
        void ExpireAllSession();
    }

    public class OnlineUserService : VirtualCrudService<OnlineUser>, IOnlineUserService
    {
        public SummaryDataPageResponse<OnlineUser> GetOnlineUsers(OnlineUserSearchModel searchModel)
        {
            var onlineUserList = GetAllOnlineUsers();


            var count = onlineUserList.Count;
            var query = onlineUserList.AsQueryable();
            query = ApplySortingRule(query, searchModel, x => x.SessionStartedDate);
            query = ApplyPagingRule(query, searchModel);


            return new SummaryDataPageResponse<OnlineUser>(query.ToList(), searchModel.PageNumber, count, false);
        }

        public void ExpireSession(string sessionId)
        {
            var onlineUserCacheKey = $"{Constants.Cache.OnlineUserCacheKeyPrefix}-{sessionId}";
            var cache = ObjectRegistry.GetObject<Exir.Framework.Common.Caching.ICacheProvider>();
            var selectedUser = JsonConvert.DeserializeObject<OnlineUser>(cache.Get(onlineUserCacheKey).ToString());
            if (selectedUser != null)
                selectedUser.IsExpired = true;

            cache.Remove(onlineUserCacheKey);
            cache.Insert(onlineUserCacheKey, JsonConvert.SerializeObject(selectedUser));
        }

        public void ExpireAllSession()
        {
            var cache = ObjectRegistry.GetObject<Exir.Framework.Common.Caching.ICacheProvider>();
            foreach (var onlineUser in GetAllOnlineUsers())
            {
                onlineUser.IsExpired = true;

                cache.Remove($"{Constants.Cache.OnlineUserCacheKeyPrefix}-{onlineUser.SessionId}");
                cache.Insert($"{Constants.Cache.OnlineUserCacheKeyPrefix}-{onlineUser.SessionId}", JsonConvert.SerializeObject(onlineUser));
            }
        }

        private IList<OnlineUser> GetAllOnlineUsers()
        {
            var cache = ObjectRegistry.GetObject<Exir.Framework.Common.Caching.ICacheProvider>();
            List<OnlineUser> onlineUserList = new List<OnlineUser>();
            var csessionId = HttpContextUtility.TryGetSessionID();
            onlineUserList = (from cachedValue in cache.GetKeys(Constants.Cache.OnlineUserCacheKeyPrefix)
                              select JsonConvert.DeserializeObject<OnlineUser>(cache.Get(cachedValue).ToString()) into onlineUser
                              where onlineUser.SessionId != csessionId
                              select onlineUser).ToList();

            return onlineUserList;
        }
    }
}
