using Exir.Framework.Common.Logging;
using Exir.Framework.Common;
using Exir.Framework.Service.ActionResponses;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SeatDomain.Services.Periodically;
using System.Text;
using System.Threading;
using SeatDomain.Models;
using System.IO.Compression;

namespace SeatDomain.Services
{
    public partial interface IJobLogService : IReadOnlySupportBaseOfService<JobLog, IJobLogService>
    {
        bool RunFrom(IPeriodicallyService periodicallyService, string jobKey, DateTime from);
        bool ExecuteJob(IPeriodicallyService periodicallyService, string jobKey, DateTime from, DateTime to);
        DataPageResponse GetLightQuery(SearchSpecification<JobLog> specification);
        DateTime GetLastRunDate(IPeriodicallyService periodicallyService, string jobKey);
    }

    public partial class JobLogService : ReadOnlySupportBaseOfService<JobLog, IJobLogService>, IJobLogService
    {
        private ILogger _logger;

        public JobLogService(IRepository<JobLog> repository, IReadOnlyRepository<JobLog> readOnlyRepository) : base(repository, readOnlyRepository)
        {
            _logger = LogManager.Instance.GetLogger<JobLogService>();
        }

        public DataPageResponse GetLightQuery(SearchSpecification<JobLog> specification)
        {
            var q = GetDefaultQuery();
            if (specification.WhereClouse != null)
                q = q.Where(specification.WhereClouse);
            var sq = q.Select(x => new
            {
                x.JobLogPK,
                x.JobName,
                x.RunDt,
                x.Status,
                x.Duration,
                x.Audit_CreateDate
            });
            sq = ApplySortingRule(sq, specification, x => x.JobLogPK);
            sq = ApplyPagingRule(sq, specification);

            return new DataPageResponse(sq.ToList(), specification.PageNumber, int.MaxValue, false);
        }

        public bool RunFrom(IPeriodicallyService periodicallyService, string jobKey, DateTime from)
        {
            _logger.InfoFormat("RunForm jobKey:{0}, from:{1})", jobKey, from);

            DateTime lastSuccessExecDt;
            if (from != DateTime.MinValue)
                lastSuccessExecDt = from;
            else
                lastSuccessExecDt = GetLastRunDate(periodicallyService, jobKey);

            if (periodicallyService.GetPeriod() >= TimeSpan.FromDays(1))
                lastSuccessExecDt = lastSuccessExecDt.Date + periodicallyService.GetPeriodicallyExecutionTimeOfDay();

            DateTime untilExecDt = (periodicallyService.IsContainCurrentDate()) ? DateTime.Now : DateTime.Now.AddDays(-1);
            var intervals = (untilExecDt - lastSuccessExecDt).TotalMinutes / periodicallyService.GetPeriod().TotalMinutes;
            DateTime to = lastSuccessExecDt;
            from = lastSuccessExecDt;
            for (int i = 0; i < Math.Floor(intervals); ++i)
            {
                to = to + periodicallyService.GetPeriod();
                var result = ExecuteJob(periodicallyService, jobKey, from, to);
                from = to;
                if (!result) return false;
            }

            return true;
        }

        public DateTime GetLastRunDate(IPeriodicallyService periodicallyService, string jobKey)
        {
            DateTime lastSuccessExecDt;

            lastSuccessExecDt = IgnoreSecurity()
            .GetDefaultQuery()
            .Where(x => x.JobName == jobKey && x.Status == Constants.JobLogStates.Success)
            .OrderByDescending(x => x.RunDt)
            .Select(x => x.RunDt)
            .FirstOrDefault();

            if (lastSuccessExecDt == DateTime.MinValue)
                lastSuccessExecDt = periodicallyService.GetBeginingExecutionTime();

            return lastSuccessExecDt;
        }

        public bool ExecuteJob(IPeriodicallyService periodicallyService, string jobKey, DateTime from, DateTime to)
        {
            _logger.InfoFormat("run job {0} from {0} to {1}", jobKey, from, to);

            EnableCommandLog();

            if (from == to || (DateTime.Now - from).TotalHours < 1 || to < from)
            {
                _logger.InfoFormat("job {0} cannot run for date from {1} to {2} because invalid date!", jobKey, from, to);
                return false;
            }
            _logger.InfoFormat("last success executed date for job {0} is {1}", jobKey, from);

            _logger.InfoFormat("start run job {0}", jobKey);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var jobLog = SaveJob(jobKey, to);

            try
            {
                periodicallyService.ExecutePeriodicallyJob(from, to);
                jobLog.Status = Constants.JobLogStates.Success;
                _logger.InfoFormat("job {0} executed for date {1} successfuly.", jobKey, from);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("exception occurred on executing job {0} on day {1}", ex, jobKey, from);
                jobLog.Status = Constants.JobLogStates.Error;
                ZipJobException(jobKey, ex, jobLog, from);
            }

            watch.Stop();
            jobLog.Duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            SaveJob(jobKey, from, jobLog);

            return jobLog.Status == Constants.JobLogStates.Success;
        }

        private static object _isCommandLogEnabledLock = new object();
        private static bool _isCommandLogEnabled = false;
        private void EnableCommandLog()
        {
            if (!_isCommandLogEnabled)
            {
                lock (_isCommandLogEnabledLock)
                {
                    if (!_isCommandLogEnabled)
                    {
                        _isCommandLogEnabled = true;
                    }
                }
            }
        }

        private void ZipJobException(string jobKey, Exception ex, JobLog jobLog, DateTime from, DateTime? to = null)
        {
            try
            {
                using (var zipmem = new MemoryStream())
                {
                    using (ZipArchive zip = new ZipArchive(zipmem, ZipArchiveMode.Create))
                    {
                        string date = DateTime.Now.ToString("yyyyMMdd");
                        string name = String.Format("{0}-{1}.txt", jobKey, date);
                        var entry = zip.CreateEntry(name);

                        var mem_buffer = Encoding.UTF8.GetBytes(ex.SerializeToString());
                        var stream = entry.Open();
                        stream.Write(mem_buffer, 0, mem_buffer.Length);
                    }
                    jobLog.Log = zipmem.ToArray();
                }
            }
            catch (Exception ex2)
            {
                _logger.ErrorFormat("exception occurred on zipping log job {0} on [{1}]-[{2}]", ex2, jobKey, from, to);
            }
        }

        private JobLog SaveJob(string jobKey, DateTime runDt, JobLog jobLog = null)
        {
            if (jobLog == null)
            {
                jobLog = new JobLog()
                {
                    JobName = jobKey,
                    RunDt = runDt,
                    Duration = TimeSpan.Zero,
                    Status = Constants.JobLogStates.Executing,
                    Log = new byte[] { byte.MaxValue }
                };
            }
            else
            {
                if (!jobLog.ChangeTracker.ChangeTrackingEnabled)
                    jobLog.StartTracking();
            }

            IgnoreSecurity();
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    base.Save(jobLog);
                    break;
                }
                catch (Exception ex)
                {
                    LogError("Cannot save job log!", ex);
                    Thread.Sleep(5000);
                }
            }

            return jobLog;
        }
    }

}


