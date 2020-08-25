using Exir.Framework.Common;
using Exir.Framework.Common.Caching;
using Exir.Framework.Service;
using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public partial interface IIndexLastDayTimeValueService
    {
        List<IndexLastDayTimeValue> GetLastDay(long insCode);
        DateTime GetLastDate();
        List<IndexLastDayTimeValue> GetLastDay(long[] insCodes);
        IndexLastDayTimeValue GetOverallIndexLastValue();
    }
    public partial class IndexLastDayTimeValueService
    {
        public IndexLastDayTimeValueService(IRepository<IndexLastDayTimeValue> repository, IReadOnlyRepository<IndexLastDayTimeValue> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache, TimeToLive = 1)]
        [JustReadOnly]
        public List<IndexLastDayTimeValue> GetLastDay(long[] insCodes)
        {
            DateTime lastDay = GetLastDate();

            return AsReadOnly().GetDefaultQuery()
                .Where(x => insCodes.Contains(x.InsCode) && x.Dt > lastDay)
                .ToList();
        }
        [JustReadOnly]
        [Cacheable(CacheName = CacheConstants.InMemoryCache, TimeToLive = 1)]
        public List<IndexLastDayTimeValue> GetLastDay(long insCode)
        {
            DateTime lastDay = GetLastDate();

            return AsReadOnly().GetDefaultQuery()
                .Where(x => x.InsCode == insCode && x.Dt > lastDay)
                .ToList();
        }



        [JustReadOnly]
        [Cacheable(CacheName = CacheConstants.InMemoryCache, TimeToLive = 5)]
        public DateTime GetLastDate()
        {
            var lastDay = DateTime.Now.Date;

            var holidaySrv = ServiceFactory.Create<IHolidayService>();
            if (!holidaySrv.IsWorkingDay(DateTime.Now))
            {
                lastDay = AsReadOnly().GetDefaultQuery()
                    .OrderByDescending(x => x.IndexLastDayTimeValuePK)
                    .Select(x => x.Dt)
                    .FirstOrDefault();
            }

            return lastDay.Date;
        }

        [JustReadOnly]
        [Cacheable(CacheName = CacheConstants.InMemoryCache, TimeToLive = 1)]
        public IndexLastDayTimeValue GetOverallIndexLastValue()
        {
            long[] topCodes = new long[] {
                Constants.KnownInstruments.Overall_Index,
            };

            var values = This.GetLastDay(topCodes);

            return values.OrderByDescending(x => x.Dt).FirstOrDefault();
        }
    }
}
