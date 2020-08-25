using Exir.Framework.Common;
using Exir.Framework.Common.Caching;
using Exir.Framework.Service;
using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public partial interface IInstrumentService
    {
        Instrument[] GetTopIndecies();
    }

    public partial class InstrumentService
    {
        public InstrumentService(IRepository<Instrument> repository, IReadOnlyRepository<Instrument> readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }



        [JustReadOnly]
        [Cacheable(CacheName = CacheConstants.InMemoryCache, TimeToLive = 1)]
        public Instrument[] GetTopIndecies()
        {
            var indexValueSrv = ServiceFactory.Create<IIndexLastDayTimeValueService>();
            indexValueSrv.MakeFriend(this);

            long[] topCodes = new long[] {
                Constants.KnownInstruments.Industy_Index,
                Constants.KnownInstruments.OTC_Overall_Index,
                Constants.KnownInstruments.Overall_Index,
                Constants.KnownInstruments.TotalEquel_Weithed,
            };

            var indecies = AsReadOnly().GetDefaultQuery()
                .Where(x => topCodes.Contains(x.InsCode))
                .ToList();

            var values = indexValueSrv.GetLastDay(topCodes);

            foreach (var index in indecies)
            {
                index.IndexValues.AddRange(values.Where(x => x.InsCode == index.InsCode));
                if (index.IndexValues.Count > 0)
                {
                    var min = index.IndexValues.Min(x => x.Value)*0.999;
                    foreach (var iv in index.IndexValues)
                        iv.Value -= min;
                    index.LastValue = index.IndexValues[index.IndexValues.Count - 1].Value;
                    index.ChangePercent = index.IndexValues[index.IndexValues.Count - 1].ChangePercent ?? 0;
                    index.ChangeValue = index.IndexValues[index.IndexValues.Count - 1].ChangeValue ?? 0;
                }
            }
            return indecies.ToArray();
        }
    }
}
