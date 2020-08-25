using Exir.Framework.Common;
using Exir.Framework.Common.Diagnostics;
using Exir.Framework.Common.Entity;
using Exir.Framework.Common.Fasterflect;
using Exir.Framework.Common.Logging;
using Exir.Framework.Security.ObjectValidation;
using Seat.DataStore;
using SeatDomain;
using SeatDomain.Models;
using SeatDomain.Services;
using SeatService.SeatServiceEngine.DataProvider;
using SeatService.SeatServiceEngine.DataProvider.Tsetmc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SeatService.SeatServiceEngine
{
    public class SeatServiceMediatorEngine : IDisposable
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

        public SeatServiceMediatorEngine(ITsetmcOnlineDataProvider tsetmcOnline, ITsetmcWebServiceDataProvider tsetmcWebService, ICodalDataProvider codalData, ITsetmcStorage storage)
        {
            Assertx.ArgumentNotNull(tsetmcOnline, nameof(tsetmcOnline));
            Assertx.ArgumentNotNull(tsetmcWebService, nameof(tsetmcWebService));
            Assertx.ArgumentNotNull(codalData, nameof(codalData));
            Assertx.ArgumentNotNull(storage, nameof(storage));

            _logger = LogManager.Instance.GetLogger<SeatServiceMediatorEngine>();

            Online = tsetmcOnline;
            WebService = tsetmcWebService;
            CodalData = codalData;
            Storage = storage;
        }

        public async Task WarmupAsync()
        {
            try
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

                foreach (var dbins in dbInstruments.Where(x => !instruments.Any(y => y.InsCode == x.InsCode)))
                    instruments.Add(dbins);

                foreach (var ins in instruments)
                {
                    ins.StartTracking();
                    var index = dbInstruments.FindIndex(x => x.InsCode == ins.InsCode);
                    if (index >= 0)
                    {
                        if (ins.Type == null && dbInstruments[index].Type != null)
                            ins.Type = dbInstruments[index].Type;

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
            catch (Exception exception)
            {
                _logger.Error("WarmupAsync", exception);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
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

        public async Task RefreshObserverMessagesAsync()
        {
            try
            {
                OnOperationStart?.Invoke(this, 2);
                var new_messages = await Online.GetNewMessagesAsync();
                OnOperationStep?.Invoke(this, null);
                if (new_messages.Any())
                {
                    var ins_codes = new_messages.Where(x => x.RelativeInstances != null).SelectMany(x => x.RelativeInstances);
                    var miss = ins_codes.Where(x => !WebService.Instruments.Any(y => y.InsCode == x)).Distinct().ToList();
                    if (miss.Any())
                        await SaveWebServiceInstruments();

                    await Storage.SaveMessagesAsync(new_messages.Reverse());
                }
                OnOperationStep?.Invoke(this, null);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                _logger.Error("RefreshObserverMessages", exception);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task RefreshCodalMessagesAsync()
        {
            try
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
            catch (Exception exception)
            {
                _logger.Error("RefreshCodalMessages", exception);
            }
        }

        public async Task<bool> RefreshTradesAsync()
        {
            try
            {
                if (_is_last_refresh_success)
                    await Storage.AddClientTypeAsync(Online.Data.Values);

                _is_last_refresh_success = await Online.RefreshAsync(TimeSpan.FromMilliseconds(3000));
                return _is_last_refresh_success;
            }
            catch (Exception exception)
            {
                _logger.Error("RefreshTrades", exception);
                return false;
            }
        }

        public async Task RefreshInstrumentsAsync()
        {
            try
            {
                OnOperationStart?.Invoke(this, 4);
                await Online.LoadDataAsync();
                OnOperationStep?.Invoke(this, null);
                _update_instruments();
                OnOperationStep?.Invoke(this, null);
                await SaveWebServiceInstruments();
                OnOperationStep?.Invoke(this, null);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                _logger.Error("RefreshInstruments", exception);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private async Task SaveWebServiceInstruments()
        {
            var modifieds = WebService.Instruments.Where(x => x.ChangeTracker.State == ObjectState.Modified || x.ChangeTracker.State == ObjectState.Added);
            if (modifieds.Any())
                await Storage.UpdateInstancesAsync(modifieds);
        }

        public async Task RefreshClosingPricesAsync()
        {
            try
            {
                var hSrv = StaticServiceFactory.Create<IHistoryService>();

                var newClosingPrices = await WebService.UpdateClosingPricesAsync(null, null);

                if (newClosingPrices != null && newClosingPrices.Count > 0)
                {
                    var dbdevens = await Storage.GetLastDevensAsync();
                    OnOperationStep?.Invoke(this, null);
                    int counter = 0, total = WebService.Instruments.Count;

                    var insCodes = newClosingPrices.Select(x => x.InsCode).Distinct();
                    total = insCodes.Count();


                    var devens = newClosingPrices.Select(x => x.DayDate).Distinct();
                    var ctsrv = StaticServiceFactory.Create<IClientTypeService>();
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

                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                _logger.Error("RefreshClosingPrices", exception);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task UpdateDayTradesAsync(int count)
        {
            try
            {
                var hSrv = StaticServiceFactory.Create<IHistoryService>();

                var list = await hSrv.GetDefaultQuery()
                    .Where(x => !x.HasDetails)
                    .OrderByDescending(x => x.Date)
                    .Select(x => x.Date)
                    .Distinct()
                    .Take(count)
                    .ToListAsync();

                for (int i = 0; i < list.Count; ++i)
                {
                    await UpdateDayTradesAsync(list[i]);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("UpdateDayTradesAsync", exception);
            }
        }

        private async Task UpdateDayTradesAsync(DateTime dt)
        {
            try
            {
                var hSrv = StaticServiceFactory.Create<IHistoryService>();
                var blSrv = StaticServiceFactory.Create<IBestLimitService>();
                var tSrv = StaticServiceFactory.Create<ITradeService>();
                var shSrv = StaticServiceFactory.Create<IShareHolderChangeService>();
                var cpSrv = StaticServiceFactory.Create<IClosingPriceService>();

                var codes = Online.Data.Values.Select(x => x.inscode);

                OnOperationStart?.Invoke(this, codes.Count() * 5);

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
                    try
                    {
                        if (c != null && c.BestLimits.Count > 0 && !await blSrv.GetDefaultQuery()
                        .Where(x => x.InsCode == c.InsCode && x.DateTime == dt)
                        .AnyAsync())
                        {

                            await blSrv.Repository.BulkInsertAsync(c.BestLimits);

                            _logger.InfoFormat("write {0} Best Limits to database.", c.BestLimits.Count);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    OnOperationStep?.Invoke(this, EventArgs.Empty);

                    return c;
                });

                var writeTrades = new TransformBlock<DayTradeDetails, DayTradeDetails>(async c =>
                {

                    try
                    {
                        if (c != null && c.Trades.Count > 0 && !await tSrv.GetDefaultQuery()
                                        .Where(x => x.InsCode == c.InsCode && x.DateTime == dt)
                                        .AnyAsync())
                        {


                            _logger.InfoFormat("write {0} Trades to database.", c.Trades.Count);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    OnOperationStep?.Invoke(this, EventArgs.Empty);

                    return c;
                });

                var writeClosingPrices = new TransformBlock<DayTradeDetails, DayTradeDetails>(async c =>
                {
                    var tomorow = dt.AddDays(1);
                    if (c != null && c.ClosingPriceData.Count > 0 && !await cpSrv.GetDefaultQuery()
                        .Where(x => x.InsCode == c.InsCode && x.DateTime >= dt && x.DateTime < tomorow)
                        .AnyAsync())
                    {
                        try
                        {
                            await cpSrv.Repository.BulkInsertAsync(c.ClosingPriceData);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        _logger.InfoFormat("write {0} Closing Price to database.", c.ClosingPriceData.Count);
                    }
                    OnOperationStep?.Invoke(this, EventArgs.Empty);

                    return c;
                });

                var writeShareHolderStates = new ActionBlock<DayTradeDetails>(async c =>
               {
                   try
                   {
                       if (c != null && c.ShareHolderStates.Count > 0 && !await shSrv.GetDefaultQuery()
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
                   }
                   catch (Exception)
                   {

                       throw;
                   }

                   OnOperationStep?.Invoke(this, EventArgs.Empty);
               });

                extractDayDetailsBlock.LinkTo(writeBestLimits, new DataflowLinkOptions() { PropagateCompletion = true });
                writeBestLimits.LinkTo(writeClosingPrices, new DataflowLinkOptions() { PropagateCompletion = true });
                writeClosingPrices.LinkTo(writeTrades, new DataflowLinkOptions() { PropagateCompletion = true });
                writeTrades.LinkTo(writeShareHolderStates, new DataflowLinkOptions() { PropagateCompletion = true });

                foreach (var code in codes)
                    extractDayDetailsBlock.Post(code);

                extractDayDetailsBlock.Complete();
                await Task.WhenAll(writeShareHolderStates.Completion, writeTrades.Completion, writeClosingPrices.Completion, writeBestLimits.Completion, extractDayDetailsBlock.Completion);

                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                _logger.Error("UpdateDayTradesAsync", exception);
                OnOperationBreak?.Invoke(this, exception);
            }

        }

        public async Task RefreshLiveStates(bool fast)
        {
            try
            {
                var watch = new Stopwatch();
                watch.Start();
                var indexCodes = StaticData.Instruments
                        .Where(x => x.Flow != (byte)FlowTypes.Ati && x.CompanyCode != Constants.CompanyCodes.IDXS)
                        .Select(x => x.InsCode)
                        .ToList();

                if (fast)
                    indexCodes.RemoveFromIList(x => !Online.Data.ContainsKey(x));

                OnOperationStart?.Invoke(this, indexCodes.Count * 2 + 3);

                var liveSrv = StaticServiceFactory.Create<ILiveInstDataService>();
                var today = DateTime.Now.Date;
                var savedData = await liveSrv.GetDefaultQuery()
                    .Where(x => x.DEven >= today)
                    .ToListAsync();
                OnOperationStep?.Invoke(this, EventArgs.Empty);
                int i = 0;
                var readWebSiteBlock = new TransformBlock<long, LiveInstData>(
                       async inxCode =>
                       {
                           InstrumentLastInfo ins = null;
                           try
                           {
                               ins = await Online.FindAsync(null, inxCode);
                           }
                           catch (Exception ex)
                           {
                               _logger.WarnFormat("Cannot find instrument by code {0}; operation skip from this error.", ex, inxCode);
                               return null;
                           }
                           if (ins == null)
                           {
                               _logger.WarnFormat("cannot find instrument with code {0}", inxCode);
                               return null;
                           }
                           else
                               _logger.InfoFormat("instance {0} live data fetched {1}% completed", inxCode, Math.Ceiling(i++ * 1.0 / indexCodes.Count * 100));

                           var r = new LiveInstData();
                           foreach (var field in r.GetFields().Where(x => x.Kind == FieldKinds.Primitive))
                           {
                               var value = ins.GetPropertyValue(field.Name);
                               r.SetValue(field.Name, Typing.ChangeType(value, field.PropertyType));
                           }
                           OnOperationStep?.Invoke(this, EventArgs.Empty);
                           return r;
                       }, new ExecutionDataflowBlockOptions
                       {
                           MaxDegreeOfParallelism = 15
                       });

                List<LiveInstData> newData = new List<LiveInstData>();
                List<LiveInstData> dirtyData = new List<LiveInstData>();
                var writeLiveInstData = new ActionBlock<LiveInstData>(r =>
                {
                    if (r == null) return;

                    var savedInst = savedData.FirstOrDefault(x => x.InsCode == r.InsCode);
                    if (savedInst != null)
                    {
                        savedInst.ResetChanges();
                        foreach (var field in r.GetFields().Where(x => x.Kind == FieldKinds.Primitive))
                        {
                            var value = r.GetPropertyValue(field.Name);
                            savedInst.SetValue(field.Name, Typing.ChangeType(value, field.PropertyType));
                        }
                        if (savedInst.ChangeTracker.State == ObjectState.Modified)
                        {
                            dirtyData.Add(savedInst);
                        }
                    }
                    else
                    {
                        r.DEven = today;
                        newData.Add(r);
                    }

                    OnOperationStep?.Invoke(this, EventArgs.Empty);
                });

                readWebSiteBlock.LinkTo(writeLiveInstData);


                foreach (var code in indexCodes)
                    readWebSiteBlock.Post(code);

                readWebSiteBlock.Complete();
                await readWebSiteBlock.Completion;

                var ov = ObjectRegistry.GetObject<IValidationProvider>();
                newData.RemoveFromIList(x => !ov.Validate(x, Mode.OnInsert.ToString()).IsValid);
                if (newData.Count > 0)
                    await liveSrv.Repository.BulkInsertAsync(newData);
                OnOperationStep?.Invoke(this, EventArgs.Empty);

                dirtyData.RemoveFromIList(x => !ov.Validate(x, Mode.OnUpdate.ToString()).IsValid);
                if (dirtyData.Count > 0)
                    await liveSrv.SaveEntitiesAsync(dirtyData.ToArray());
                OnOperationStep?.Invoke(this, EventArgs.Empty);

                newData.Clear();
                newData.TrimExcess();
                dirtyData.Clear();
                dirtyData.TrimExcess();

                watch.Stop();
                _logger.InfoFormat("RefreshLiveStates take {0}ms", watch.ElapsedMilliseconds);
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                _logger.Error("RefreshLiveStates", exception);
                OnOperationBreak?.Invoke(this, exception);
            }
        }

        public async Task RefreshIndexes()
        {
            try
            {
                var indexCodes = StaticData.Instruments.Where(x => x.CompanyCode == Constants.CompanyCodes.IDXS)
                        .Select(x => x.InsCode)
                        .ToArray();

                OnOperationStart?.Invoke(this, indexCodes.Length + 3);

                List<Index> indices = new List<Index>();
                var readWebSiteBlock = new ActionBlock<long>(
                          async inxCode =>
                          {
                              indices.Add(await Online.GetIndex(inxCode, false));
                              OnOperationStep?.Invoke(this, EventArgs.Empty);
                          }, new ExecutionDataflowBlockOptions
                          {
                              MaxDegreeOfParallelism = 15
                          });

                foreach (var code in indexCodes)
                    readWebSiteBlock.Post(code);

                readWebSiteBlock.Complete();
                await readWebSiteBlock.Completion;

                var indexLastValue = await Online.GetIndexLastValue();
                OnOperationStep?.Invoke(this, EventArgs.Empty);

                if (indices.Count > 0)
                {
                    var idxSrvf = StaticServiceFactory.Create<IIndexLastDayTimeValueService>();
                    var today = DateTime.Now.Date;
                    var idxData = idxSrvf.GetDefaultQuery()
                        .Where(x => x.Dt > today)
                        .ToList();
                    OnOperationStep?.Invoke(this, EventArgs.Empty);

                    var newData = indices.Where(x => x != null).SelectMany(x => x.LastDayTimeValue)
                        .Where(x => x.Dt >= today)
                        .Where(x => !idxData.Any(y => y.InsCode == x.InsCode && x.Dt == y.Dt))
                        .ToArray();

                    foreach (var dtv in newData)
                    {
                        var lv = indexLastValue.FirstOrDefault(x => x.Code == dtv.InsCode);
                        if (lv != null)
                        {
                            dtv.ChangePercent = lv.ChangePercent;
                            dtv.ChangeValue = lv.ChangeValue;
                        }
                    }

                    await idxSrvf.SaveEntitiesAsync(newData);
                    OnOperationStep?.Invoke(this, EventArgs.Empty);
                }
                OnOperationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception exception)
            {
                _logger.Error("RefreshIndexes", exception);
                OnOperationBreak?.Invoke(this, exception);
            }
        }

        public void Dispose()
        {
            Online?.Dispose();
            WebService?.Dispose();
            CodalData?.Dispose();
        }
    }
}
