using Exir.Framework.Common;
using Exir.Framework.Common.Logging;
using Seat.Models;
using Seat.SeatEngine.DataProvider;
using Seat.SeatEngine.DataProvider.Tsetmc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seat.DataStore
{
    public class TsetmcStorage : ITsetmcStorage
    {
        private ILogger _logger;

        public TsetmcStorage()
        {
            _logger = LogManager.Instance.GetLogger<TsetmcStorage>();
        }

        private SeatEntitiesDbContext _createContext()
        {
            return new SeatEntitiesDbContext("SeatDB", "SeatDB");
        }

        public async Task AddClientTypeAsync(IEnumerable<TsetmcDataRow> instances)
        {
            using (var ctx = _createContext())
            {
                var changed_instances = instances
                    .Where(x => x.IsValid() && x.IsTypeClientChanges());

                var clientTypes = changed_instances
                    .Select(x => new ClientType
                    {
                        Prev_Buy_CountI = x.prev_Buy_CountI,
                        Prev_Buy_CountN = x.prev_Buy_CountN,
                        Prev_Buy_I_Volume = x.prev_Buy_I_Volume,
                        Prev_Pl = x.prev_pl,
                        Prev_Buy_N_Volume = x.prev_Buy_N_Volume,
                        Prev_Sell_CountI = x.prev_Sell_CountI,
                        Prev_Sell_CountN = x.prev_Sell_CountN,
                        Prev_Sell_I_Volume = x.prev_Sell_I_Volume,
                        Prev_Sell_N_Volume = x.prev_Sell_N_Volume,
                        Prev_TVol = x.prev_tvol,
                        Buy_CountI = x.Buy_CountI,
                        Buy_CountN = x.Buy_CountN,
                        Buy_I_Volume = x.Buy_I_Volume,
                        Buy_N_Volume = x.Buy_N_Volume,
                        DayDt = DateTime.Now,
                        InsCode = x.inscode,
                        Pl = x.pl,
                        Sell_CountI = x.Sell_CountI,
                        Sell_CountN = x.Sell_CountN,
                        Sell_I_Volume = x.Sell_I_Volume,
                        Sell_N_Volume = x.Sell_N_Volume,
                        TVol = x.tvol,
                        heven = x.LastTradeTime,
                        VisitCount = x.visitcount
                    });

                if (!clientTypes.Any()) return;

                ctx.Set<ClientType>().AddRange(clientTypes);

                var changeCount = await ctx.SaveChangesAsync();

                _logger.InfoFormat("successfuly {0} client types was added", clientTypes.Count());

                foreach (var instance in changed_instances)
                    instance.Backup();
            }


        }

        public async Task<List<Instrument>> GetAllInstrumentsAsync()
        {
            using (var ctx = _createContext())
            {
                return await ctx.Set<Instrument>().AsNoTracking().ToListAsync();
            }
        }

        public async Task<ObserverMessage> GetLastMessageAsync()
        {
            using (var ctx = _createContext())
            {
                var msg = await ctx.Set<Message>().OrderByDescending(x => x.MessagePK).FirstOrDefaultAsync();

                if (msg != null)
                    return new ObserverMessage
                    {
                        Description = msg.Description,
                        MessageDt = msg.MessageDt,
                        Subject = msg.Subject
                    };
                return null;
            }
        }

        public async Task<CodalMessage> GetLastCodalMessageAsync()
        {
            using (var ctx = _createContext())
            {
                return await ctx.Set<CodalMessage>().OrderByDescending(x => x.TracingNo).FirstOrDefaultAsync();
            }
        }

        public async Task<int> SaveMessagesAsync(IEnumerable<CodalMessage> codalMessages)
        {
            using (var ctx = _createContext())
            {
                var tracing_no = codalMessages.Select(x => x.TracingNo).ToArray();
                var exists = ctx.Set<CodalMessage>().Where(x => tracing_no.Contains(x.TracingNo)).Select(x => x.TracingNo).ToArray();

                var msgs = codalMessages
                    .Where(x => !exists.Contains(x.TracingNo))
                    .Select(x => new CodalMessage
                    {
                        CompanyName = x.CompanyName.MaxLength(100),
                        InsCode = x.InsCode,
                        PublishDateTime = x.PublishDateTime,
                        SentDateTime = x.SentDateTime,
                        Symbol = x.Symbol.MaxLength(100),
                        Title = x.Title.MaxLength(1500, true),
                        TracingNo = x.TracingNo
                    });

                ctx.Set<CodalMessage>().AddRange(msgs);

                return await ctx.SaveChangesAsync();
            }
        }

        public async Task<int> SaveMessagesAsync(IEnumerable<ObserverMessage> observerMessages)
        {
            using (var ctx = _createContext())
            {
                Dictionary<long, Instrument> attached_instances = new Dictionary<long, Instrument>();
                foreach (var obmsg in observerMessages)
                {
                    var msg = new Message
                    {
                        Description = obmsg.Description,
                        MessageDt = obmsg.MessageDt,
                        Subject = obmsg.Subject,
                    };
                    if (obmsg.RelativeInstances != null)
                    {
                        if (obmsg.RelativeInstances.Count == 1)
                            msg.InsCode = obmsg.RelativeInstances[0];
                        else
                            foreach (var icode in obmsg.RelativeInstances.Distinct())
                            {
                                if (!attached_instances.ContainsKey(icode))
                                {
                                    var ins = new Instrument
                                    {
                                        InsCode = icode
                                    };
                                    ctx.Entry(ins).State = System.Data.Entity.EntityState.Unchanged;
                                    attached_instances.Add(icode, ins);
                                }
                                var instance = attached_instances[icode];
                                msg.Instruments.Add(instance);
                            }
                    }
                    ctx.Set<Message>().Add(msg);
                }

                if (observerMessages.Any())
                    _logger.InfoFormat("successfuly {0} observer message was saved in database", observerMessages.Count());

                return await ctx.SaveChangesAsync();
            }
        }

        public async Task UpdateInstancesAsync(IEnumerable<Instrument> instruments)
        {
            using (var ctx = _createContext())
            {
                int added = 0, modified = 0;
                foreach (var instrument in instruments.Where(x => x.IsValid()))
                {
                    if (instrument.ChangeTracker.State == ObjectState.Added)
                    {
                        ctx.Set<Instrument>().Add(instrument);
                        ++added;
                        continue;
                    }

                    if (instrument.ChangeTracker.State == ObjectState.Modified)
                    {
                        modified++;
                        var entry = ctx.Entry(instrument);
                        entry.State = EntityState.Modified;

                        if (instrument.ChangeTracker.ChangedProperties.Count > 0)
                        {
                            foreach (var p in instrument.GetFields().Where(x => x.Kind == FieldKinds.Primitive))
                                entry.Property(p.Name).IsModified = false;

                            foreach (var ch in instrument.ChangeTracker.ChangedProperties)
                                entry.Property(ch).IsModified = true;
                        }
                    }
                }
                var changeCount = await ctx.SaveChangesAsync();

                _logger.InfoFormat("successfuly {0} ins was updated and {1} ins was added", modified, added);

                foreach (var instrument in instruments.Where(x => x.IsValid()))
                    instrument.ResetChanges();
            }
        }

        public async Task AddHistoryAsync(List<History> lists)
        {
            using (var ctx = _createContext())
            {
                ctx.Set<History>().AddRange(lists);

                var changeCount = await ctx.SaveChangesAsync();

                _logger.InfoFormat("successfuly {0} history was updated", changeCount);

            }
        }

        public async Task<Dictionary<long, int>> GetLastDevensAsync()
        {
            using (var ctx = _createContext())
            {
                var lastOnes = await ctx.Set<History>()
                    .GroupBy(x => x.InsCode)
                    .Select(x => new
                    {
                        x.Key,
                        MaxDeven = x.Max(y => y.DEven)
                    })
                    .ToListAsync();

                var result = new Dictionary<long, int>();
                result.AddRange(lastOnes.Select(x => new KeyValuePair<long, int>(x.Key, x.MaxDeven)));
                return result;
            }
        }
    }
}
