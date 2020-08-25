using CommandLine;
using Exir.Framework.Common;
using Exir.Framework.Common.Logging;
using Exir.Framework.DataAccess;
using Seat.DataStore;
using SeatDomain;
using SeatDomain.Configs;
using SeatDomain.Models;
using SeatDomain.Services;
using SeatDomain.Services.Periodically;
using SeatService.SeatServiceEngine;
using SeatService.SeatServiceEngine.DataProvider;
using SeatService.SeatServiceEngine.DataProvider.Tsetmc;
using Spring.Aop.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeatService
{
    public partial class SeatService : ServiceBase
    {
        private ILogger _logger;
        private SeatServiceMediatorEngine _engine;
        private Thread _refreshTradesThread;
        private Thread _refreshWorksThread;
        private bool _stop;
        private bool _pause;
        private Options _options;
        private Thread _thread;
        private Thread _vthread;
        private DateTime _lastJobExec;
        public SeatService()
        {
            InitializeComponent();
        }
        public void Start(string[] args)
        {
            OnStart(args);
        }
        private ServiceHost _host;
        protected override void OnStart(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fa-IR");
            _logger = LogManager.Instance.GetLogger<SeatService>();
            _logger.Info("OnStart()");

            if (_host != null)
            {
                _host.Close();
            }

            _host = new ServiceHost(typeof(JobServiceBroker));
            _host.Open();

            foreach (var address in _host.BaseAddresses)
                _logger.InfoFormat("JobServiceBroker was host on {0}", address.ToString());

            if (args != null && args.Length > 0)
                Parser.Default.ParseArguments<Options>(args)
                       .WithParsed(o =>
                       {
                           _options = o;
                       });
            if (_options == null)
                _options = new Options()
                {
                    EnableAll = true
                };

            _engine = new SeatServiceMediatorEngine(new TsetmcOnlineDataProvider(),
               new TsetmcWebServiceDataProvider(),
               new CodalDataProvider(),
               new TsetmcStorage());

            _refreshTradesThread = new Thread(new ThreadStart(refreshTrades));
            _refreshWorksThread = new Thread(new ThreadStart(refreshWorks));
            _stop = false;

            AsyncHelper.RunSync(_engine.WarmupAsync);


            _refreshWorksThread.Start();

            if (_options.EnableAll || _options.RefreshTrades)
                _refreshTradesThread.Start();

            _thread = new Thread(new ThreadStart(doJob));
            _thread.Start();

            _vthread = new Thread(new ThreadStart(doPeriodicallyJob));
            _vthread.Start();

            _lastJobExec = DateTime.MinValue;
        }

        public void doPeriodicallyJob()
        {
            var periodicallyServices = ObjectRegistry.GetObjectsOfType<IPeriodicallyService>();
            var jobLogSrv = StaticServiceFactory.Create<IJobLogService>();
            List<string> blackList = new List<string>();
            do
            {
                _logger.Info("check time to run periodically jobs...");
                try
                {
                    foreach (var periodicallySrvEntry in periodicallyServices)
                    {
                        var periodicallySrv = (IPeriodicallyService)periodicallySrvEntry.Value;
                        var lastExec = jobLogSrv.GetLastRunDate(periodicallySrv, periodicallySrvEntry.Key);

                        if (periodicallySrv.GetPeriod() < TimeSpan.FromDays(1) ||
                            (DateTime.Now.TimeOfDay >= periodicallySrv.GetPeriodicallyExecutionTimeOfDay() &&
                            lastExec.Date != DateTime.Now.Date))

                            if (lastExec + periodicallySrv.GetPeriod() < DateTime.Now)
                            {
                                var result = RunPeriodicallyJob(periodicallySrvEntry.Key);
                                if (!result)
                                    CheckBlackList(jobLogSrv, blackList, periodicallySrvEntry);
                            }
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("an exception occurred on executing vespertine", ex);
                }

                TransactionContext.Current.Clear();

                for (int i = 0; i < JobServiceConfig.Instance.PeriodicallyWaitTime && !_stop; ++i)
                    Thread.Sleep(1000);

                if (blackList.Count > 0)
                {
                    foreach (var key in blackList)
                        periodicallyServices.Remove(key);
                    blackList.Clear();
                }

            } while (!_stop);
            canStop();
        }
        private void CheckBlackList(IJobLogService jobLogSrv, List<string> blackList, KeyValuePair<string, object> periodicallySrvEntry)
        {
            var last10 = jobLogSrv.GetDefaultQuery()
                .Where(x => x.JobName == periodicallySrvEntry.Key)
                .OrderByDescending(x => x.JobLogPK)
                .Select(x => x.Status)
                .Take(10)
                .ToList();
            if (last10.Where(x => x == Constants.JobLogStates.Error).Count() >= 10)
            {
                blackList.Add(periodicallySrvEntry.Key);
                _logger.WarnFormat("Job {0} has more than 10 error in last it's log so remove it from execution list", periodicallySrvEntry.Key);

            }
        }
        public static bool RunPeriodicallyJob(string jobKey)
        {
            return RunPeriodicallyJob(jobKey, DateTime.MinValue);
        }

        public static bool RunPeriodicallyJob(string jobKey, DateTime date)
        {
            LogMemoryState();

            var authenticater = ObjectRegistry.GetObject<IAuthenticaterProvider>(true);
            authenticater.Authenticate(Constants.KnownUsers.admin);

            var srv = StaticServiceFactory.Create<IJobLogService>();
            var periodicallySrv = StaticServiceFactory.Create<IPeriodicallyService>(jobKey);
            var result = srv.RunFrom(periodicallySrv, jobKey, date);

            GC.Collect();

            return result;
        }
        private static void LogMemoryState()
        {
            var logger = LogManager.Instance.GetLogger(typeof(JobServiceBroker));


            var maximum_usage_memory = Process.GetCurrentProcess().PeakWorkingSet64;
            var gc_memory_usage = GC.GetTotalMemory(false);

            logger.InfoFormat("memory usage is {0} and gc total memory is {1}", maximum_usage_memory, gc_memory_usage);
        }

        public IEnumerable<KeyValuePair<string, int>> Queue
        {
            get
            {
                return _queue;
            }
        }

        private ConcurrentDictionary<string, int> _queue;
        private static EntityFrameworkLogger _ef_logger;
        private void doJob()
        {
            short wait_bar_index = 0;
            char[] wait_bar = new char[] { '|', '/', '-', '\\' };
            bool debug = JobServiceConfig.Instance.Debug;
            if (debug)
            {
                if (_ef_logger == null)
                    _ef_logger = new EntityFrameworkLogger(_logger);
                DbInterception.Add(new EntityFrameworkLogger(_logger));
            }

            int delay_time = JobServiceConfig.Instance.DelayTime;
            var jobSrv = StaticServiceFactory.Create<IBackstageJobService>();
            _logger.Info("start get all base info data from database");
            var baseInfoServices = ServiceFactory.CreateAll<IMemoryCachedService>();
            baseInfoServices.ForEachAction(x =>
            {
                x.IgnoreSecurity();
                try
                {
                    x.GetAll();
                }
                catch (Exception ex)
                {
                    var serviceType = AopUtils.GetAllInterfaces(x).First(y => y.Name.StartsWith("IService") && y.IsGenericType);
                    var messg = String.Format("Invoking GetAll method on service {0} cause throw exception", serviceType.GetGenericArguments()[0].Name);
                    _logger.Error(messg);
                }
            });
            bool job_fetched = false;
            _queue = new ConcurrentDictionary<string, int>();
            string server = String.IsNullOrEmpty(JobServiceConfig.Instance.Server) ? Environment.MachineName : JobServiceConfig.Instance.Server;
            DateTime lastFixTime = DateTime.MinValue;
            do
            {
                if (Environment.UserInteractive)
                {
                    if (Console.CursorLeft > 0)
                        Console.WriteLine();

                    Console.Write(wait_bar[wait_bar_index++]);

                    if (wait_bar_index > wait_bar.Length - 1)
                        wait_bar_index = 0;

                    Console.CursorLeft = 0;
                }

                job_fetched = false;
                try
                {
                    BackstageJob job = null;
                    try
                    {
                        string[] ignore_queue_names = _queue.Where(x => x.Value >= JobServiceConfig.Instance.GetQueueCapacity(x.Key))
                            .Select(x => x.Key)
                            .Distinct()
                            .ToArray();

                        job = jobSrv.Pop(server, ignore_queue_names);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormat("cannot pop backstage job because {0}", ex, ex.Message);
                        Thread.Sleep(delay_time);
                        continue;
                    }

                    if (job != null)
                    {
                        if (Environment.UserInteractive)
                            Console.WriteLine();

                        _queue.AddOrUpdate(job.Queue ?? Constants.BackstageJobs.Queue.Default, 1, (key, value) => value + 1);

                        job_fetched = true;
                        _logger.InfoFormat("a job {0} in service {1} candidated to run as {2} in queue {3}", job.Action, job.Service, job.RunAs, job.Queue);

                        ThreadPool.QueueUserWorkItem(new WaitCallback(run), job);
                    }

                    if ((DateTime.Now - lastFixTime).TotalMinutes > 10)
                    {
                        lastFixTime = DateTime.Now;
                        jobSrv.FixFetchedButNotRunJobs();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    _logger.ErrorFormat("an exception occurred on executing job", ex);
                }

                TransactionContext.Current.Clear();

                if (!job_fetched)
                    Thread.Sleep(delay_time);

            } while (!_stop);
            canStop();
        }

        private void canStop()
        {
            CanStop = true;
            CanShutdown = true;
        }

        private void run(object jobObject)
        {
            CallContext.FreeNamedDataSlot(TransactionContext.CallContextTransactionContextKey);

            BackstageJob job = (BackstageJob)jobObject;

            MemoryLogAdapter memAdapter = null;
            bool is_ef_log_enabled = job.Debug && JobServiceConfig.Instance.Debug && _ef_logger != null;
            if (is_ef_log_enabled)
            {
                memAdapter = _logger.Adapters.Where(x => x is MemoryLogAdapter).Cast<MemoryLogAdapter>().FirstOrDefault();
                memAdapter.Start();
                _ef_logger.Start();
            }

            var jobSrv = StaticServiceFactory.Create<IBackstageJobService>();

            Exception exception = null;
            for (int i = 0; i < 10; ++i)
            {
                exception = authenticate(job);
                if (exception == null)
                {
                    break;
                }
                else
                {
                    _logger.DebugFormat("try authenticate again for {0} of {1}", i + 1, 10);
                    Thread.Sleep(1000);
                }
            }

            var time = jobSrv.Run(job, ejob =>
            {
                string log = null;
                if (memAdapter != null)
                {
                    log = memAdapter.Stop();
                }
                return log;
            });

            if (is_ef_log_enabled)
                _ef_logger.Stop();

            _queue.AddOrUpdate(job.Queue ?? Constants.BackstageJobs.Queue.Default, 0, (key, value) => value - 1);

            if (time > 1000)
                GC.Collect(2, GCCollectionMode.Forced, true);
        }

        private Exception authenticate(BackstageJob job)
        {
            var authenticater = ObjectRegistry.GetObject<IAuthenticaterProvider>(true);
            Exception exception = null;

            _logger.InfoFormat("start authenticate as {0} for job(PK) {1}", job.RunAs, job.BackstageJobPK);

            if (!job.RunAs.Equals(authenticater.CurrentIdentity.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    authenticater.Authenticate(job.RunAs);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat("cannot authenticate as user {0} for job(PK)", ex, job.RunAs, job.BackstageJobPK);
                    _logger.Debug(ex.SerializeToString());
                    exception = ex;
                }
            }

            return exception;
        }

        public void TryStop()
        {
            _stop = true;
        }
        private void refreshTrades()
        {
            _logger.Info("refresh trade thread started");

            var periods = new Periods
            {
                TradeStart = TimeSpan.FromHours(8.5),
                TradeEnd = TimeSpan.FromHours(17),
                FastWaitTime = TimeSpan.FromMilliseconds(500),
                SlowWaitTime = TimeSpan.FromSeconds(15)
            };
            while (!_stop)
            {
                while (_pause) Thread.Sleep(1000);

                bool isHoliday = DateTime.Now.DayOfWeek == DayOfWeek.Friday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday;
                bool isInTime = !isHoliday && DateTime.Now.TimeOfDay >= periods.TradeStart && DateTime.Now.TimeOfDay <= periods.TradeEnd;

                if (isInTime)
                {
                    _engine.RefreshTradesAsync().Wait();
                }
                if (isHoliday)
                {
                    Thread.Sleep((int)TimeSpan.FromMinutes(10).TotalMilliseconds);
                }
                else
                {
                    if (isInTime)
                        Thread.Sleep((int)periods.FastWaitTime.TotalMilliseconds);
                    else
                        Thread.Sleep((int)periods.SlowWaitTime.TotalMilliseconds);
                }
            }
        }
        private void refreshWorks()
        {
            _logger.Info("main thread started");

            var periods = new Periods
            {
                RefreshInstruments = TimeSpan.FromMinutes(5),
                TradeStart = TimeSpan.FromHours(8.5),
                TradeEnd = TimeSpan.FromHours(17),
                RefreshObserverMessages = TimeSpan.FromSeconds(5),
                RefreshClosingPrices = TimeSpan.FromHours(23),
                RefreshCodalMessages = TimeSpan.FromMinutes(1),
                UpdateDayTradesStart = TimeSpan.FromHours(0),
                UpdateDayTradesEnd = TimeSpan.FromHours(6),
                UpdateDayTrades = TimeSpan.FromMinutes(5),
                FastWaitTime = TimeSpan.FromMilliseconds(500),
                SlowWaitTime = TimeSpan.FromSeconds(15),
                InTimeRefreshLiveStates = TimeSpan.FromMinutes(5),
                OutTimeRefreshLiveStates = TimeSpan.FromHours(1),
                RefreshIndexes = TimeSpan.FromMinutes(5)
            };

            var lastUpdate = new LastUpdate
            {
                RefreshInstruments = DateTime.MinValue,
                RefreshClosingPrices = DateTime.MinValue,
                RefreshObserverMessages = DateTime.MinValue,
                RefreshCodalMessages = DateTime.MinValue,
                UpdateDayTrades = DateTime.MinValue,
                RefreshLiveStates = DateTime.MinValue,
                RefreshIndexes = DateTime.MinValue
            };
            while (!_stop)
            {
                while (_pause) Thread.Sleep(1000);

                bool isHoliday = DateTime.Now.DayOfWeek == DayOfWeek.Friday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday;
                bool isInTime = !isHoliday && DateTime.Now.TimeOfDay >= periods.TradeStart && DateTime.Now.TimeOfDay <= periods.TradeEnd;

                if (_options.EnableAll || _options.RefreshInstruments)
                    if (isInTime && DateTime.Now - lastUpdate.RefreshInstruments > periods.RefreshInstruments)
                    {
                        _logger.Trace("start refresh instruments");
                        AsyncHelper.RunSync(_engine.RefreshInstrumentsAsync);
                        lastUpdate.RefreshInstruments = DateTime.Now;
                    }

                if (_options.EnableAll || _options.RefreshObserverMessages)
                    if (isInTime && DateTime.Now - lastUpdate.RefreshObserverMessages > periods.RefreshObserverMessages)
                    {
                        _logger.Trace("start refresh observer messages");
                        AsyncHelper.RunSync(_engine.RefreshObserverMessagesAsync);
                        lastUpdate.RefreshObserverMessages = DateTime.Now;
                    }

                if (_options.EnableAll || _options.RefreshClosingPrices)
                    if (!isInTime && DateTime.Now - lastUpdate.RefreshClosingPrices > periods.RefreshClosingPrices)
                    {
                        _logger.Trace("start refresh closing prices");
                        AsyncHelper.RunSync(_engine.RefreshClosingPricesAsync);
                        lastUpdate.RefreshClosingPrices = DateTime.Now;
                    }

                if (_options.EnableAll || _options.RefreshCodalMessages)
                    if (DateTime.Now - lastUpdate.RefreshCodalMessages > periods.RefreshCodalMessages)
                    {
                        _logger.Trace("start refresh codal messages");
                        AsyncHelper.RunSync(_engine.RefreshCodalMessagesAsync);
                        lastUpdate.RefreshCodalMessages = DateTime.Now;
                    }

                if (_options.EnableAll || _options.UpdateDayTrades)
                    if ((isHoliday ||
                    (DateTime.Now.TimeOfDay > periods.UpdateDayTradesStart && DateTime.Now.TimeOfDay < periods.UpdateDayTradesEnd)) &&
                    DateTime.Now - lastUpdate.UpdateDayTrades > periods.UpdateDayTrades)
                    {
                        _logger.Trace("start refresh day trade details");
                        AsyncHelper.RunSync(() => _engine.UpdateDayTradesAsync(Settings.Default.UpdateDayTradesCount));
                        lastUpdate.UpdateDayTrades = DateTime.Now;
                    }

                if (_options.EnableAll || _options.RefreshLiveStates)
                    if (!isHoliday &&
                    ((isInTime && DateTime.Now - lastUpdate.RefreshLiveStates > periods.InTimeRefreshLiveStates) ||
                    (!isInTime && DateTime.Now - lastUpdate.RefreshLiveStates > periods.OutTimeRefreshLiveStates)))
                    {
                        _logger.Trace("start refresh day instrument live state");
                        AsyncHelper.RunSync(() => _engine.RefreshLiveStates(isInTime));
                        lastUpdate.RefreshLiveStates = DateTime.Now;
                    }

                if (_options.EnableAll || _options.RefreshIndexes)
                    if (!isHoliday && isInTime && DateTime.Now - lastUpdate.RefreshIndexes > periods.RefreshIndexes)
                    {
                        _logger.Trace("start refresh day indecies time data");
                        AsyncHelper.RunSync(_engine.RefreshIndexes);
                        lastUpdate.RefreshIndexes = DateTime.Now;
                    }

                Thread.Sleep((int)periods.SlowWaitTime.TotalMilliseconds);
            }
        }

        protected override void OnPause()
        {
            _pause = true;
        }

        protected override void OnContinue()
        {
            _pause = false;
        }

        protected override void OnStop()
        {
            _stop = true;

            if (_host != null)
            {
                _host.Close();
                _host = null;
            }
        }
    }

    public class Options
    {
        [Option('i', "instrument", Required = false, HelpText = "Enable refresh instruments.")]
        public bool RefreshInstruments { get; set; }
        [Option('o', "observer-messages", Required = false, HelpText = "Enable refresh messages.")]
        public bool RefreshObserverMessages { get; set; }
        [Option('c', "closing-prices", Required = false, HelpText = "Enable refresh closing prices.")]
        public bool RefreshClosingPrices { get; set; }
        [Option('m', "codal-messages", Required = false, HelpText = "Enable refresh codal messages.")]
        public bool RefreshCodalMessages { get; set; }
        [Option('d', "day-trades", Required = false, HelpText = "Enable refresh day trades.")]
        public bool UpdateDayTrades { get; set; }
        [Option('l', "live-state", Required = false, HelpText = "Enable refresh live state.")]
        public bool RefreshLiveStates { get; set; }
        [Option('x', "indecies", Required = false, HelpText = "Enable refresh indecies.")]
        public bool RefreshIndexes { get; set; }
        [Option('t', "trades", Required = false, HelpText = "Enable refresh trades.")]
        public bool RefreshTrades { get; set; }
        [Option('e', "enable-all", Required = false, HelpText = "Enable all refreshes.")]
        public bool EnableAll { get; set; }
    }

}
