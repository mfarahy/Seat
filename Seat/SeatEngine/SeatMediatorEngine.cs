using Exir.Framework.Common;
using Exir.Framework.Common.Diagnostics;
using Exir.Framework.Common.Entity;
using Exir.Framework.Common.Logging;
using Seat.DataStore;
using Seat.Models;
using Seat.SeatEngine.DataProvider;
using Seat.SeatEngine.DataProvider.Tsetmc;
using Seat.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;

namespace Seat.SeatEngine
{
    public class SeatMediatorEngine : IDisposable
    {
        private ILogger _logger;
        private bool _is_last_refresh_success;

        public event EventHandler<int> OnOperationStart;
        public event EventHandler OnOperationCompleted;
        public event EventHandler<Exception> OnOperationBreak;
        public event EventHandler OnOperationStep;


        public ITsetmcOnlineDataProvider Online { get; }
        public ITsetmcWebServiceDataProvider WebService { get; }
        public ICodalDataProvider CodalData { get; }
        public ITsetmcStorage Storage { get; }

        public SeatMediatorEngine(ITsetmcOnlineDataProvider tsetmcOnline, ITsetmcWebServiceDataProvider tsetmcWebService, ICodalDataProvider codalData, ITsetmcStorage storage)
        {
            Assertx.ArgumentNotNull(tsetmcOnline, nameof(tsetmcOnline));
            Assertx.ArgumentNotNull(tsetmcWebService, nameof(tsetmcWebService));
            Assertx.ArgumentNotNull(codalData, nameof(codalData));
            Assertx.ArgumentNotNull(storage, nameof(storage));

            _logger = LogManager.Instance.GetLogger<SeatMediatorEngine>();

            Online = tsetmcOnline;
            WebService = tsetmcWebService;
            CodalData = codalData;
            Storage = storage;
        }

        public async Task WarmupAsync()
        {
            OnOperationStart?.Invoke(this, 10);

            _logger.Info("Warmup start");

            await WebService.FillDataAsync();
            OnOperationStep?.Invoke(this, EventArgs.Empty);

            List<Task> tasks = new List<Task>();

            tasks.Add(Storage.GetAllInstrumentsAsync());      // 0
            tasks.Add(Online.LoadDataAsync());                // 1
            tasks.Add(WebService.UpdateInstrumentsAsync());   // 2
            tasks.Add(Storage.GetLastMessageAsync());         // 3
            tasks.Add(Storage.GetLastCodalMessageAsync());    // 4
            tasks.Add(Storage.GetLastDevensAsync());          // 5

            foreach (var task in tasks)
                task.ContinueWith(x =>
                {
                    OnOperationStep?.Invoke(this, EventArgs.Empty);
                });

            await Task.WhenAll(tasks);

            var dbInstruments = ((Task<List<Instrument>>)tasks[0]).Result;
            Online.LastObserverMessage = ((Task<ObserverMessage>)tasks[3]).Result;
            CodalData.Last = ((Task<CodalMessage>)tasks[4]).Result;
            var dbdevens = ((Task<Dictionary<long, int>>)tasks[5]).Result;

            _update_instruments();
            OnOperationStep?.Invoke(this, EventArgs.Empty);

            var instruments = WebService.Instruments;
            foreach (var ins in instruments)
            {
                ins.StartTracking();
                var index = dbInstruments.FindIndex(x => x.InsCode == ins.InsCode);
                if (index >= 0)
                {
                    if (ins.Equals(dbInstruments[index]))
                    {
                        ins.ChangeTracker.ResetChanges();
                        ins.ChangeTracker.State = Exir.Framework.Common.ObjectState.Unchanged;
                    }
                    else
                        ins.ChangeTracker.State = Exir.Framework.Common.ObjectState.Modified;

                    dbInstruments.RemoveAt(index);
                }
                else
                    ins.ChangeTracker.State = Exir.Framework.Common.ObjectState.Added;

                if (dbdevens.ContainsKey(ins.InsCode))
                    ins.LastDbDeven = dbdevens[ins.InsCode];
                else
                    ins.LastDbDeven = 0;
            }
            OnOperationStep?.Invoke(this, EventArgs.Empty);

            await Storage.UpdateInstancesAsync(instruments);
            OnOperationStep?.Invoke(this, EventArgs.Empty);

            _is_last_refresh_success = true;

            _logger.Info("Warmup completed");

            OnOperationCompleted?.Invoke(this, EventArgs.Empty);

        }

        private void _update_instruments()
        {
            foreach (var ins in WebService.Instruments)
            {
                if (Online.Data.ContainsKey(ins.InsCode))
                {
                    var onlineInstrument = Online.Data[ins.InsCode];

                    if (ins.ChangeTracker.State != Exir.Framework.Common.ObjectState.Added)
                        ins.ResetChanges();

                    ins.BVol = onlineInstrument.bvol;
                    ins.Cs = onlineInstrument.cs;
                    ins.Eps = onlineInstrument.eps;
                    ins.Pe = onlineInstrument.pe;
                    ins.Z = onlineInstrument.z;
                }
            }
            var misses = Online.Data.Values.Where(x => !WebService.Instruments.Any(y => y.InsCode == x.inscode));
            foreach (var miss in misses)
            {
                Online.Data.TryRemove(miss.inscode, out _);
            }
        }

        public async Task RefreshObserverMessages()
        {
            OnOperationStart?.Invoke(this, 2);
            var new_messages = await Online.GetNewMessagesAsync();
            OnOperationStep?.Invoke(this, null);
            if (new_messages.Any())
            {
                var ins_codes = new_messages.Where(x => x.RelativeInstances != null).SelectMany(x => x.RelativeInstances);
                var miss = ins_codes.Where(x => !WebService.Instruments.Any(y => y.InsCode == x));
                if (miss.Any())
                {
                    await WebService.UpdateInstrumentsAsync();
                    new_messages = new_messages.Where(x => x.RelativeInstances == null || x.RelativeInstances.All(m => WebService.Instruments.Any(y => y.InsCode == m)));

                    _logger.WarnFormat("found {0} ins codes from observer message that is not valid", String.Join(",", miss.Select(x => x.ToString())));
                }

                await Storage.SaveMessagesAsync(new_messages.Reverse());
            }
            OnOperationStep?.Invoke(this, null);
            OnOperationCompleted?.Invoke(this, EventArgs.Empty);
        }

        public async Task RefreshCodalMessages()
        {
            OnOperationStart?.Invoke(this, 3);

            var new_messages = await CodalData.GetNewMessages();
            OnOperationStep?.Invoke(this, null);
            new_messages.Reverse();

            foreach (var message in new_messages)
            {
                var instance = WebService.Instruments.FirstOrDefault(x => x.Symbol == message.Symbol);
                if (instance != null)
                    message.InsCode = instance.InsCode;
            }
            OnOperationStep?.Invoke(this, null);

            if (new_messages.Any())
            {
                await Storage.SaveMessagesAsync(new_messages);
            }
            OnOperationStep?.Invoke(this, null);
            OnOperationCompleted?.Invoke(this, EventArgs.Empty);
        }

        public async Task<bool> RefreshTrades()
        {
            if (_is_last_refresh_success)
                await Storage.AddClientTypeAsync(Online.Data.Values);

            _is_last_refresh_success = await Online.RefreshAsync(TimeSpan.FromMilliseconds(3000));
            return _is_last_refresh_success;
        }

        public async Task RefreshInstruments()
        {
            OnOperationStart?.Invoke(this, 4);

            Online.Clear();
            OnOperationStep?.Invoke(this, null);
            await Online.LoadDataAsync();
            OnOperationStep?.Invoke(this, null);
            _update_instruments();
            OnOperationStep?.Invoke(this, null);
            var modifieds = WebService.Instruments.Where(x => x.ChangeTracker.State == Exir.Framework.Common.ObjectState.Modified);
            if (modifieds.Any())
                await Storage.UpdateInstancesAsync(modifieds);
            OnOperationStep?.Invoke(this, null);
            OnOperationCompleted?.Invoke(this, EventArgs.Empty);
        }

        public async Task RefreshClosingPrices()
        {
            var hSrv = ServiceFactory.Create<IHistoryService>();

            var codes = WebService.Instruments.Select(x => x.InsCode).ToList();

            var newClosingPrices = await WebService.UpdateClosingPricesAsync(codes, totalPages =>
            {
                OnOperationStart?.Invoke(this, totalPages + codes.Count + 4);
            }, pageNumber =>
            {
                OnOperationStep?.Invoke(this, null);
            });

            if (newClosingPrices != null && newClosingPrices.Count > 0)
            {
                var dbdevens = await Storage.GetLastDevensAsync();
                OnOperationStep?.Invoke(this, null);
                int counter = 0, total = WebService.Instruments.Count;

                var insCodes = newClosingPrices.Select(x => x.InsCode).Distinct();
                total = insCodes.Count();


                var devens = newClosingPrices.Select(x => x.DayDate).Distinct();
                var ctsrv = ServiceFactory.Create<IClientTypeService>();
                var maxVisitCounts = await ctsrv.GetDefaultQuery()
                    .Where(x => devens.Contains(x.DayDt))
                    .GroupBy(x => new { x.InsCode, x.DayDt })
                    .Select(x => new { x.Key.DayDt, x.Key.InsCode, VisitCount = x.Max(y => y.VisitCount) })
                    .ToListAsync();
                OnOperationStep?.Invoke(this, null);

                foreach (var insCode in insCodes)
                {
                    var ins = StaticData.Instruments.FirstOrDefault(x => x.InsCode == insCode);
                    if (ins == null) continue;
                    ++counter;
                    if (!dbdevens.ContainsKey(ins.InsCode) || dbdevens[ins.InsCode] < ins.LastDeven)
                    {
                        var closingPricesFromFile = newClosingPrices.Where(x => x.InsCode == ins.InsCode);

                        List<History> histories = null;
                        if (!dbdevens.ContainsKey(ins.InsCode))
                        {
                            _logger.InfoFormat("update database {2}% for instrument {0} closing price to {1}", ins.InsCode, ins.LastDeven, Math.Round(counter * 1.0 / total * 100));

                            histories = closingPricesFromFile.Select(x => new History(x)).ToList();

                        }
                        else
                        {
                            _logger.InfoFormat("update database {3}%  for instrument {0} closing price from {1} to {2}", ins.InsCode, dbdevens[ins.InsCode], ins.LastDeven, Math.Round(counter * 1.0 / total * 100));

                            histories = closingPricesFromFile.Where(x => x.DEven > dbdevens[ins.InsCode])
                               .Select(x => new History(x))
                               .ToList();

                        }

                        var vcs = maxVisitCounts.Where(x => x.InsCode == ins.InsCode).ToList();
                        foreach (var vc in vcs)
                        {
                            histories.Where(x => x.Date == vc.DayDt)
                                .ForEachAction(x => x.VisitCount = vc.VisitCount);
                        }

                        vcs.Clear();
                        vcs = null;

                        await hSrv.BulkSaveAsync(histories);
                        histories.Clear();
                        histories.TrimExcess();
                        histories = null;
                    }
                    OnOperationStep?.Invoke(this, null);
                }

                dbdevens = await Storage.GetLastDevensAsync();
                OnOperationStep?.Invoke(this, null);

                foreach (var ins in WebService.Instruments)
                    if (dbdevens.ContainsKey(ins.InsCode))
                        ins.LastDbDeven = dbdevens[ins.InsCode];
                OnOperationStep?.Invoke(this, null);

                newClosingPrices.Clear();
                newClosingPrices.TrimExcess();
                newClosingPrices = null;
            }

            codes.Clear();
            codes.TrimExcess();
            codes = null;
            OnOperationCompleted?.Invoke(this, EventArgs.Empty);
        }

        public async Task UpdateDayTrades(int count)
        {
            var hSrv = ServiceFactory.Create<IHistoryService>();

            var list = await hSrv.GetDefaultQuery()
                .Where(x => !x.HasDetails)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Date)
                .Distinct()
                .Take(count)
                .ToListAsync();

            for (int i = 0; i < list.Count; ++i)
            {
                await UpdateDayTrades(list[i]);
            }
        }

        private async Task UpdateDayTrades(DateTime dt)
        {
            var hSrv = ServiceFactory.Create<IHistoryService>();
            var blSrv = ServiceFactory.Create<IBestLimitService>();
            var tSrv = ServiceFactory.Create<ITradeService>();
            var shSrv = ServiceFactory.Create<IShareHolderChangeService>();

            var codes = Online.Data.Values.Select(x => x.inscode);

            OnOperationStart?.Invoke(this, codes.Count() * 4);

            var extractDayDetailsBlock = new TransformBlock<long, DayTradeDetails>(
                async insCode =>
                {
                    var r = await Online.ExtractDayDetailsAsync(insCode, dt);
                    OnOperationStep?.Invoke(this, EventArgs.Empty);
                    return r;
                }, new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 15
                });


            var writeBestLimits = new TransformBlock<DayTradeDetails, DayTradeDetails>(async c =>
            {

                if (c.BestLimits.Count > 0 && !await blSrv.GetDefaultQuery()
                    .Where(x => x.InsCode == c.InsCode && x.DateTime == dt)
                    .AnyAsync())
                {
                    await blSrv.Repository.BulkInsertAsync(c.BestLimits);
                    _logger.InfoFormat("write {0} Best Limits to database.", c.BestLimits.Count);
                }

                OnOperationStep?.Invoke(this, EventArgs.Empty);

                return c;
            });

            var writeTrades = new TransformBlock<DayTradeDetails, DayTradeDetails>(async c =>
            {

                if (c.Trades.Count > 0 && !await tSrv.GetDefaultQuery()
                    .Where(x => x.InsCode == c.InsCode && x.DateTime == dt)
                    .AnyAsync())
                {
                    await tSrv.Repository.BulkInsertAsync(c.Trades);
                    _logger.InfoFormat("write {0} Trades to database.", c.Trades.Count);
                }
                OnOperationStep?.Invoke(this, EventArgs.Empty);

                return c;
            });

            var writeShareHolderStates = new ActionBlock<DayTradeDetails>(async c =>
           {
               if (c.ShareHolderStates.Count > 0 && !await shSrv.GetDefaultQuery()
                   .Where(x => x.InsCode == c.InsCode && x.DateTime == dt)
                   .AnyAsync())
               {
                   await shSrv.Repository.BulkInsertAsync(c.ShareHolderStates);
                   _logger.InfoFormat("write {0} Share Holder States to database.", c.ShareHolderStates.Count);
               }

               hSrv.Repository.BulkUpdate(x => x.InsCode == c.InsCode && x.Date == c.DayDate, x => new History
               {
                   HasDetails = true
               });

               OnOperationStep?.Invoke(this, EventArgs.Empty);
           });

            extractDayDetailsBlock.LinkTo(writeBestLimits);
            writeBestLimits.LinkTo(writeTrades);
            writeTrades.LinkTo(writeShareHolderStates);

            foreach (var code in codes)
                extractDayDetailsBlock.Post(code);

            extractDayDetailsBlock.Complete();
            await writeTrades.Completion;
        }

        public void Dispose()
        {
            Online?.Dispose();
            WebService?.Dispose();
            CodalData?.Dispose();
        }
    }
}
