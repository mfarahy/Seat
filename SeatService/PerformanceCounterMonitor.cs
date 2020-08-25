using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Exir.Framework.Common.Logging;
using SeatDomain.Models;

namespace SeatService
{
    public class PerformanceCounterMonitor : IDisposable
    {
        private static ConcurrentDictionary<object, JobPerformaceLog>[] _log = new ConcurrentDictionary<object, JobPerformaceLog>[2];

        private PerformanceCounter _memoryUsage;
        private PerformanceCounter _processorTimeCounter;
        private Thread _diagnosisthread;
        private ILogger _logger;
        public const int CPU_LOG_INDEX = 0;
        public const int MEMORY_LOG_INDEX = 1;
        public const string CPU_CATEGORY_NAME = "Processor";
        public const string MEMORY_CATEGORY_NAME = "Memory";
        public void Start()
        {
            _logger = LogManager.Instance.GetLogger(typeof(PerformanceCounterMonitor));

            _logger.Info("OnStart()");

            _processorTimeCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryUsage = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            _logger.InfoFormat("CPU usage counter: ");
            _logger.InfoFormat("Category: {0}", _processorTimeCounter.CategoryName);
            _logger.InfoFormat("Instance: {0}", _processorTimeCounter.InstanceName);
            _logger.InfoFormat("Counter name: {0}", _processorTimeCounter.CounterName);
            _logger.InfoFormat("Help text: {0}", _processorTimeCounter.CounterHelp);
            _logger.InfoFormat("------------------------------");
            _logger.InfoFormat("Memory usage counter: ");
            _logger.InfoFormat("Category: {0}", _memoryUsage.CategoryName);
            _logger.InfoFormat("Counter name: {0}", _memoryUsage.CounterName);
            _logger.InfoFormat("Help text: {0}", _memoryUsage.CounterHelp);
            _logger.InfoFormat("------------------------------");

            _log[CPU_LOG_INDEX] = new ConcurrentDictionary<object, JobPerformaceLog>();
            _log[MEMORY_LOG_INDEX] = new ConcurrentDictionary<object, JobPerformaceLog>();

            _diagnosisthread = new Thread(new ThreadStart(LogPerformanceCounters));
            _diagnosisthread.Start();

        }
        private void LogPerformanceCounters()
        {
            byte cpu_usage = 0, mem_usage = 0;
            while (true)
            {
                for (int i = 0; i < _log.Length; ++i)
                {
                    byte usage = 0;
                    switch (i)
                    {
                        case CPU_LOG_INDEX:
                            cpu_usage = usage = (byte)_processorTimeCounter.NextValue();
                            break;
                        case MEMORY_LOG_INDEX:
                            mem_usage = usage = (byte)_memoryUsage.NextValue();
                            break;
                    }
                    List<object> expired_kies = new List<object>();
                    foreach (var log in _log[i])
                    {
                        if ((DateTime.Now - log.Value.InsertDt).TotalHours > 2)
                        {
                            expired_kies.Add(log.Key);
                            continue;
                        }

                        if (String.IsNullOrEmpty(log.Value.CategoryName))
                        {
                            switch (i)
                            {
                                case CPU_LOG_INDEX:
                                    log.Value.CategoryName = "Processor";
                                    log.Value.CounterName = "% Processor Time";
                                    break;
                                case MEMORY_LOG_INDEX:
                                    log.Value.CategoryName = "Memory";
                                    log.Value.CounterName = "% Committed Bytes In Use";
                                    break;
                            }
                            log.Value.Start = usage;
                        }
                        log.Value.HitCount++;
                        log.Value.Max = Math.Max(usage, log.Value.Max);
                        log.Value.Min = Math.Min(log.Value.Min, usage);
                        log.Value.Sum += usage;
                    }
                    if (expired_kies.Count > 0)
                    {
                        JobPerformaceLog _;
                        foreach (var key in expired_kies)
                            _log[i].TryRemove(key, out _);
                    }
                }

                Thread.Sleep(2000);
            }
        }

        public void BeginLog(object jobPK)
        {
            for (int i = 0; i < _log.Length; ++i)
                _log[i].TryAdd(jobPK, new JobPerformaceLog() { InsertDt = DateTime.Now });
        }
        public JobPerformaceLog[] EndLog(object jobPK)
        {
            List<JobPerformaceLog> logs = new List<JobPerformaceLog>();
            for (int i = 0; i < _log.Length; ++i)
            {
                JobPerformaceLog log;
                if (_log[i].TryRemove(jobPK, out log))
                    logs.Add(log);
            }
            return logs.ToArray();
        }

        public void Dispose()
        {
            _diagnosisthread.Abort();
            _diagnosisthread = null;
            _processorTimeCounter.Dispose();
            _memoryUsage.Dispose();
        }
    }

}
