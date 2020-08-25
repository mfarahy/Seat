using Exir.Framework.Common;
using Exir.Framework.Common.Fasterflect;
using Exir.Framework.Common.Linq;
using Exir.Framework.Common.Serialization;
using Exir.Framework.Service.ActionResponses;
using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using Exir.Framework.Service;
using States = SeatDomain.Constants.BackstageJobs.States;
using SeatDomain.Models.Service;
using Exir.Framework.Common.Diagnostics;
using SeatDomain.Repository;
using SeatDomain.Configs;
using Exir.Framework.Common.Entity;
using SeatDomain.Models.SearchModels;

namespace SeatDomain.Services
{
    [IgnoreT4Template]
    public class BackstageJobService : ReadOnlySupportBaseOfService<BackstageJob, IBackstageJobRepository, IBackstageJobReadOnlyRepository, IBackstageJobService>, IBackstageJobService
    {
        protected new IBackstageJobService This => This<IBackstageJobService>();

        public BackstageJobService(IBackstageJobRepository repository, IBackstageJobReadOnlyRepository readOnlyRepository) : base(repository, readOnlyRepository)
        {
        }
        public void Restart(long jobId)
        {
            var server = JobServiceConfig.Instance.Server ?? Environment.MachineName;
            if (String.IsNullOrEmpty(server))
                server = Environment.MachineName;
            Repository.BulkUpdate(false, x => x.BackstageJobPK == jobId, x => new BackstageJob
            {
                Server = server,
                Status = States.New,
                Debug = true,
                TimeToRun = DateTime.Now
            });
        }

        public bool IsInQueue(string service, string action, string tag)
        {
            return IgnoreSecurity().GetDefaultQuery()
                .Where(x => x.Status == States.New && x.TimeToRun <= DateTime.Now && x.Service == service && x.Action == action && x.Tags.Contains(tag))
                .Any();
        }

        public long Run(string service, string action, string tag)
        {
            var job = IgnoreSecurity().GetDefaultQuery()
                .Where(x => x.Status == States.New && x.TimeToRun <= DateTime.Now && x.Service == service && x.Action == action && x.Tags.Contains(tag))
                .FirstOrDefault();
            if (job == null) return -1;
            job.StartTracking();
            return Run(job, null);
        }
        public long Run(BackstageJob job, Func<BackstageJob, string> logMaker)
        {
            Exception exception = null;
            var service = TryCreateService(job, ref exception);
            Dictionary<string, object> args = deserialize_arguments(job, ref exception);
            var time = run_action(job, service, args, ref exception);

            job.Duration = time;

            string log = logMaker != null ? logMaker(job) : String.Empty;

            if (exception != null && !JobServiceConfig.Instance.IsNegligible(exception))
            {
                if (exception is BackstageException)
                {
                    BackstageException bex = (BackstageException)exception;
                    if (bex.MaxRetryCount > 0 && job.RetryCount > bex.MaxRetryCount)
                    {
                        MarkAsError(job, exception, log);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(bex.RedependentCode))
                        {
                            RedependentJob(job, bex.RedependentCode);
                        }
                        else
                        {
                            Reschedule(job, ((BackstageException)exception).Reschedule);
                        }
                    }
                }
                else
                {
                    NameValueCollection nvc = new NameValueCollection();
                    if (args != null)
                        foreach (var arg in args)
                            nvc.Add(arg.Key, arg.Value.SafeToString());
                    LogError("cannot run action {0} on service {1} with arguments {2}", exception, job.Action, job.Service, nvc.SerializeToString());
                    LogDebug(exception.SerializeToString());

                    MarkAsError(job, exception, log);
                }
            }
            else
                MarkAsDone(job, time, log);

            return time;
        }
        private long run_action(BackstageJob job, IService service, Dictionary<string, object> args, ref Exception exception)
        {
            if (exception == null)
            {
                MarkAsInprogress(job);

                Stopwatch watch = new Stopwatch();
                try
                {
                    watch.Start();
                    service.DoAction(job.Action, args);
                    watch.Stop();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                return watch.ElapsedMilliseconds;
            }
            return 0L;
        }

        private Dictionary<string, object> deserialize_arguments(BackstageJob job, ref Exception exception)
        {
            Dictionary<string, object> args = null;
            if (exception == null)
            {
                LogInfo("start deserializing arguments for job {0}", job.BackstageJobPK);
                try
                {
                    args = job.GetArguments();
                }
                catch (Exception ex)
                {
                    LogError("cannot deserialize arguments {0}", ex, job.SerializedArgs);
                    exception = ex;
                }
            }

            return args;
        }

        public virtual IService TryCreateService(BackstageJob job, ref Exception exception)
        {
            IService service = null;
            LogInfo("start create service {0} for job {1}", job.Service, job.BackstageJobPK);

            try
            {
                service = StaticServiceFactory.Create<IService>(job.Service);
            }
            catch (Exception ex)
            {
                LogError("cannot get service {0}", ex, job.Service);
                exception = ex;
            }

            return service;
        }

        public BackstageJob Pop(string server, string[] ignore_queue_names)
        {
            var now = DateTime.Now;
            BackstageJob job;
            var skip = 0;
            IQueryable<BackstageJob> q = null;

            if (String.IsNullOrEmpty(server))
                server = Environment.MachineName;

            if (server == "*")
                q = IgnoreSecurity().GetDefaultQuery();
            else
                q = IgnoreSecurity().GetDefaultQuery().Where(x => server.Equals(x.Server, StringComparison.InvariantCultureIgnoreCase));

            do
            {
                if (ignore_queue_names != null && ignore_queue_names.Length > 0)
                {
                    var ignore_empty_queue = ignore_queue_names.Any(x => x == Constants.BackstageJobs.Queue.Default);
                    if (!ignore_empty_queue)
                        q = q.Where(x => string.IsNullOrEmpty(x.Queue) || !ignore_queue_names.Contains(x.Queue));
                    else
                        q = q.Where(x => !string.IsNullOrEmpty(x.Queue) && !ignore_queue_names.Contains(x.Queue));
                }

                job = q.Where(x => x.Status == States.New && x.TimeToRun <= now)
                       .OrderBy(x => x.Priority)
                       .OrderBy(x => x.Dependency)
                       .ThenBy(x => x.RetryCount)
                       .ThenBy(x => x.BackstageJobPK)
                       .Skip(skip)
                       .FirstOrDefault();

                if (job == null) break;

                if (!string.IsNullOrEmpty(job.Dependency))
                {
                    var dependencyStatus = IgnoreSecurity().GetDefaultQuery()
                        .Where(x => x.UniqueCode == job.Dependency)
                        .OrderByDescending(x => x.BackstageJobPK)
                        .Select(x => x.Status)
                        .FirstOrDefault();

                    if (dependencyStatus == States.Error)
                    {
                        MarkAsError(job, new Exception("Dependent job status is faild"), null);
                        continue;
                    }

                    if (dependencyStatus == States.New ||
                        dependencyStatus == States.Processing)
                    {
                        skip++;
                        continue;
                    }
                }


                if (job.RetryCount > 1)
                    job.Debug = JobServiceConfig.Instance.Debug;

                Repository.BulkUpdate(x => x.BackstageJobPK == job.BackstageJobPK, x => new BackstageJob
                {
                    Status = States.Fetched,
                    RunDt = DateTime.Now
                });

                break;
            }
            while (true);

            return job;
        }

        public int FixFetchedButNotRunJobs()
        {
            var tenMinuesAgo = DateTime.Now.AddMinutes(-10);
            int count = Repository.BulkUpdate(x => x.Status == States.Fetched &&
              x.RunDt < tenMinuesAgo
             , x => new BackstageJob
             {
                 Status = States.New
             });

            if (count > 0)
                LogDebug("count of fetched jobs but not run is {0}. system correct them.", count);

            return count;
        }

        public void Push(string service, string action, Expression<Func<object>> args,
            short priority, string queue = null, string code = null,
            string dependency = null, object[] tags = null, bool debug = false, DateTime? timeToRun = null)
        {
            var job = createJob(new BackstageJobOrder
            {
                Service = service,
                Debug = debug,
                Queue = queue,
                Dependency = dependency,
                Action = action,
                Args = args,
                Code = code,
                Priority = priority,
                Tags = tags,
                TimeToRun = timeToRun
            });
            try
            {
                IgnoreSecurity().Save(job);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Cannot insert duplicate key row in object", StringComparison.Ordinal) < 0)
                {
                    throw;
                }
            }
        }
        private BackstageJob createJob(BackstageJobOrder order)
        {
            string server, sargs;

            server = string.IsNullOrEmpty(JobServiceConfig.Instance.Server) ? Environment.MachineName : JobServiceConfig.Instance.Server;
            sargs = string.Empty;
            if (order.Args != null)
            {
                var body = (NewExpression)order.Args.Body;

                var names = body.Members.Select(x => x.Name).ToList();

                var dic = new Dictionary<string, object>();
                var obj = order.Args.Compile()();
                foreach (var name in names)
                    dic.Add(name, obj.TryGetValue(name));

                var sdic = new Dictionary<string, string>();
                foreach (var entry in dic)
                {
                    if (entry.Value == null)
                    {
                        sdic.Add(entry.Key, "__NULL__");
                    }
                    else
                    {
                        var entrySer = Serializer.GetFastestKnownSerializer(entry.Value.GetType());
                        sdic.Add(entry.Key, Serializer.FastFullSerialize(entry.Value, entrySer));
                    }
                }
                var ser = new JilSerializer();
                sargs = Serializer.FastFullSerialize(sdic, ser);
            }

            string stags = order.Tags == null ? null : String.Join(";", order.Tags);
            if (!String.IsNullOrEmpty(stags) && stags.Length > 50)
                stags = stags.Substring(0, 50);

            var priority = order.Priority;
            if (priority == 0) priority = Constants.BackstageJobs.Priorities.Normal;

            return new BackstageJob
            {
                Action = order.Action,
                Service = order.Service,
                SerializedArgs = sargs,
                Priority = priority,
#if DEBUG
                RemoveIfSuccess = false,
#else
                RemoveIfSuccess = !order.Debug,
#endif
                RetryCount = 1,
                RunAs = Authenticater.Value.CurrentIdentity.Name,
                Status = States.New,
                TimeToRun = order.TimeToRun ?? DateTime.Now,
                UniqueCode = order.Code,
                Dependency = order.Dependency,
                Queue = string.IsNullOrEmpty(order.Queue) ? Constants.BackstageJobs.Queue.Default : order.Queue,
                Server = server,
                Tags = stags,
                Debug = order.Debug
            };
        }
        public virtual void MarkAsInprogress(BackstageJob job)
        {
            try_do(() =>
            {
                const short newStatus = States.Processing;

                Repository.BulkUpdate(false, x => x.BackstageJobPK == job.BackstageJobPK, x => new BackstageJob
                {
                    Status = newStatus
                });

                job.Status = newStatus;

            }, job.BackstageJobPK, nameof(MarkAsInprogress));
        }
        public void Reschedule(BackstageJob job, DateTime timeToRun)
        {
            try_do(() =>
            {
                var newStatus = States.New;
                var priority = job.Priority;

                Repository.BulkUpdate(false, x => x.BackstageJobPK == job.BackstageJobPK, x => new BackstageJob
                {
                    Status = newStatus,
                    RetryCount = (byte)(x.RetryCount + 1),
                    Priority = priority,
                    TimeToRun = timeToRun
                });

                job.TimeToRun = timeToRun;
                job.Status = newStatus;
                job.Priority = priority;
                job.RetryCount++;

            }, job.BackstageJobPK, nameof(Reschedule));
        }
        public void RedependentJob(BackstageJob job, string redependentCode)
        {
            try_do(() =>
            {
                var newStatus = States.New;

                Repository.BulkUpdate(false, x => x.BackstageJobPK == job.BackstageJobPK, x => new BackstageJob
                {
                    Status = newStatus,
                    RetryCount = (byte)(x.RetryCount + 1),
                    Dependency = redependentCode
                });

            }, job.BackstageJobPK, nameof(RedependentJob));
        }
        public void MarkAsError(BackstageJob job, Exception exception, string executionLog)
        {
            try_do(() =>
            {
                var serializedException = exception.SerializeToString();

                var newStatus = States.Error;
                var priority = job.Priority;
                var timeToRun = job.TimeToRun;

                if (JobServiceConfig.Instance.IsRerunable(exception) || job.RetryCount < JobServiceConfig.Instance.RetryCount)
                {
                    newStatus = States.New;

                    if (job.RetryCount > 4)
                    {
                        priority++;
                    }

                    timeToRun = DateTime.Now.AddMinutes(Math.Pow(2, (job.RetryCount + 4)));
                }

                var log = zipLog(executionLog);

                Repository.BulkUpdate(false, x => x.BackstageJobPK == job.BackstageJobPK, x => new BackstageJob
                {
                    Status = newStatus,
                    Error = serializedException,
                    RetryCount = (byte)(x.RetryCount + 1),
                    Priority = priority,
                    ExecutionLog = log,
                    Debug = true,
                    TimeToRun = timeToRun,
                    MaxCpu = job.MaxCpu,
                    MaxMemory = job.MaxMemory,
                    MidMemory = job.MidCpu,
                    MidCpu = job.MidCpu,
                    Duration = job.Duration
                });

                job.Status = newStatus;
                job.Error = serializedException;
                job.RetryCount = (byte)(job.RetryCount + 1);
                job.Priority = priority;
                job.ExecutionLog = log;
                job.Debug = true;
                job.TimeToRun = timeToRun;

            }, job.BackstageJobPK, nameof(MarkAsError));
        }

        public int GetReportPriorityByCount(int count)
        {
            int priortiy;
            if (count > 100000)
                priortiy = 4;
            else
                priortiy = count / 10000;
            return priortiy;
        }

        public ActionResponse RemoveAllDone()
        {
            var count = Repository.BulkDelete(x => x.Status == States.Done);
            return new ActionResponse(String.Format("تعداد {0} آیتم انجام شده حذف گردید.", count))
            {
                Success = true
            };
        }

        public ActionResponse RestartAll(BackstageJobSearchModel searchModel)
        {
            var predicate = search(searchModel);
            var count = Repository.BulkUpdate(predicate, x => new BackstageJob()
            {
                Status = States.New,
                TimeToRun = DateTime.Now.AddSeconds(-1),
            });
            return new ActionResponse($"تعداد {count} وظیفه در صف اجرا قرار گرفتند.") { Success = true };
        }

        public ActionResponse SoftDelete(BackstageJobSearchModel searchModel)
        {
            var predicate = search(searchModel);
            var count = Repository.BulkUpdate(predicate, x => new BackstageJob()
            {
                Status = States.Removed
            });
            return new ActionResponse($"تعداد {count} وظیفه حذف منطقی شدند.") { Success = true };
        }

        public SummaryDataPageResponse<BackstageJob> Report(BackstageJobSearchModel searchModel)
        {
            var predicate = search(searchModel);

            var query = GetDefaultQuery().Where(predicate);

            var count = query.Count();

            var group_data = query.GroupBy(x => x.Action)
                  .Select(x => new JobGroupState
                  {
                      Action = x.Key,
                      Count = x.Count()
                  }).ToArray();

            query = ApplySortingRule(query, searchModel, x => x.BackstageJobPK);
            query = ApplyPagingRule(query, searchModel);

            var brokers = ObjectRegistry.GetObjects<IJobServiceBroker>();
            BackstageJobThreadState state = null;
            if (brokers.Any())
            {
                var broker = brokers.First();
                state = broker.GetState();
                state.JobGroups = group_data;
            }

            return new SummaryDataPageResponse<BackstageJob>(query.ToList(), searchModel.PageNumber, count, false)
            {
                Summary = state
            };
        }
        [ReadOnlySupport]
        private Expression<Func<BackstageJob, bool>> search(BackstageJobSearchModel searchModel)
        {
            Expression<Func<BackstageJob, bool>> query = PredicateBuilder.New<BackstageJob>(true);
            query = PredicateBuilder.And(query, x => x.Status != States.Removed);

            if (!string.IsNullOrEmpty(searchModel.RunAs)) query = PredicateBuilder.And(query, x => x.RunAs.Contains(searchModel.RunAs));
            if (!string.IsNullOrEmpty(searchModel.Service)) query = PredicateBuilder.And(query, q => q.Service.Contains(searchModel.Service));
            if (!string.IsNullOrEmpty(searchModel.Action)) query = PredicateBuilder.And(query, q => q.Action.Contains(searchModel.Action));
            if (searchModel.RunDtFrom != null && searchModel.RunDtFrom > DateTime.MinValue) query = PredicateBuilder.And(query, q => q.RunDt >= searchModel.RunDtFrom.Value);
            if (searchModel.RunDtTo != null && searchModel.RunDtTo > DateTime.MinValue) query = PredicateBuilder.And(query, q => q.RunDt <= searchModel.RunDtTo.Value);
            if (searchModel.TimeToRunFrom != null && searchModel.TimeToRunFrom > DateTime.MinValue) query = PredicateBuilder.And(query, q => q.TimeToRun >= searchModel.TimeToRunFrom.Value);
            if (searchModel.TimeToRunTo != null && searchModel.TimeToRunTo > DateTime.MinValue) query = PredicateBuilder.And(query, q => q.TimeToRun <= searchModel.TimeToRunTo.Value);
            if (searchModel.Status != null) query = PredicateBuilder.And(query, q => q.Status == searchModel.Status);
            if (searchModel.Priority != null) query = PredicateBuilder.And(query, q => q.Priority == searchModel.Priority);
            if (searchModel.RetryCount != null) query = PredicateBuilder.And(query, q => q.RetryCount == searchModel.RetryCount);
            if (searchModel.BackstageJobPK != null) query = PredicateBuilder.And(query, q => q.BackstageJobPK == searchModel.BackstageJobPK.Value);
            if (!string.IsNullOrEmpty(searchModel.Queue)) query = PredicateBuilder.And(query, q => q.Queue == searchModel.Queue);
            if (!string.IsNullOrEmpty(searchModel.Server)) query = PredicateBuilder.And(query, q => q.Server == searchModel.Server);
            if (!string.IsNullOrEmpty(searchModel.Error)) query = PredicateBuilder.And(query, q => !string.IsNullOrEmpty(q.Error) && q.Error.IndexOf(searchModel.Error) >= 0);
            if (!string.IsNullOrEmpty(searchModel.Code)) query = PredicateBuilder.And(query, q => q.UniqueCode.IndexOf(searchModel.Code) >= 0);
            if (!String.IsNullOrEmpty(searchModel.Tags)) query = PredicateBuilder.And(query, q => !string.IsNullOrEmpty(q.Tags) && q.Tags.IndexOf(searchModel.Tags) >= 0);

            query = query.Reduce(nameof(searchModel), searchModel);

            return query;
        }

        public void MarkAsDone(BackstageJob job, long duration, string executionLog)
        {
            try_do(() =>
            {
                var entity = job ?? IgnoreSecurity().GetDefaultQuery().FirstOrDefault(x => x.BackstageJobPK == job.BackstageJobPK);

                if (entity == null) return;

                if (entity.RemoveIfSuccess && !entity.Debug)
                {
                    IgnoreSecurity().Delete(entity);
                    entity.Status = States.Removed;
                }
                else
                {
                    var log = zipLog(executionLog);
                    Repository.BulkUpdate(false, x => x.BackstageJobPK == job.BackstageJobPK, x => new BackstageJob
                    {
                        Status = States.Done,
                        RunDt = DateTime.Now,
                        Error = null,
                        ExecutionLog = log,
                        MaxCpu = job.MaxCpu,
                        MaxMemory = job.MaxMemory,
                        MidMemory = job.MidCpu,
                        MidCpu = job.MidCpu,
                        Duration = job.Duration
                    });
                    job.ExecutionLog = log;
                    job.Error = null;
                    job.RunDt = DateTime.Now;
                    job.Status = States.Done;
                }
            }, job.BackstageJobPK, nameof(MarkAsDone));
        }

        private byte[] zipLog(string log)
        {
            if (String.IsNullOrEmpty(log)) return null;

            try
            {
                using (var mem = new MemoryStream())
                {
                    int orgSize = 0;
                    using (var zip = new GZipStream(mem, CompressionMode.Compress))
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes(log);
                        orgSize = bytes.Length;
                        zip.Write(bytes, 0, bytes.Length);
                        zip.Flush();
                    }
                    var result = mem.ToArray();
                    LogInfo("compress log from size {0} to {1}, compress ratio is {2}", orgSize, result.Length, result.Length * 1.0 * 100 / orgSize);
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogFatal("cannot compress log", ex);
                return System.Text.Encoding.UTF8.GetBytes(log);
            }
        }

        private void try_do(Action action, long backstageJobPK, string nameOfAction)
        {
            var tryCount = 10;
            var marked = false;
            do
            {
                try
                {
                    action();

                    break;
                }
                catch (Exception ex)
                {
                    LogError("cannot {1} job {0} as done", ex, backstageJobPK, nameOfAction);
                }

                tryCount--;

                if (!marked)
                    Thread.Sleep(1000);

            } while (!marked && tryCount > 0);
        }


        public string GetCode<T>(string actionName, params object[] args)
            where T : IService
        {
            var sb = new StringBuilder();
            sb.Append(typeof(T).Name.Abbrivate());
            sb.Append(actionName.Abbrivate());
            foreach (var t in args)
            {
                sb.Append(t);
                sb.Append('-');
            }
            var code = sb.ToString();
            return code.Length > 50 ? code.FarmhashCode64().ToString() : code;
        }

        public void Push(BackstageJobOrder[] jobs)
        {
            Repository.BulkInsert(jobs.Select(x => createJob(x)));
        }

        public void RawRun(string script)
        {
            Repository.RawRun(script);
        }

        public void StopService()
        {
            var broker = ObjectRegistry.GetObject<IJobServiceBroker>();
            broker.StopService(JobServiceConfig.Instance.SecretKey);
        }

        public BackstageJob[] RunByTag(string tag)
        {
            var jobs = SearchByTag(tag);
            List<BackstageJob> result = new List<BackstageJob>();
            foreach (var backstageJob in jobs)
            {
                This.Run(backstageJob, null);
                result.Add(backstageJob);
            }
            return result.ToArray();
        }

        public BackstageJob[] SearchByTag(string tag)
        {
            var jobs = IgnoreSecurity().GetDefaultQuery()
                 .Where(x => (x.Tags == tag || x.Tags.StartsWith(tag + ";") || x.Tags.Contains(";" + tag + ";") || x.Tags.EndsWith(";" + tag)) && x.Status == States.New)
                 .ToArray();
            return jobs;
        }

        public IQueryable<BackstageJob> GetFailedQuery(string[] signatures, DateTime? from, DateTime? to)
        {
            Assertx.ArgumentHasElement(signatures, nameof(signatures));

            var q = PredicateBuilder.New<BackstageJob>(true);

            q = PredicateBuilder.And<BackstageJob>(q, x => x.Status == States.Error &&
                           x.RetryCount >= JobServiceConfig.Instance.RetryCount);

            var server = JobServiceConfig.Instance.Server ?? Environment.MachineName;
            if (String.IsNullOrEmpty(server))
                server = Environment.MachineName;

            if (server != "*")
                q = PredicateBuilder.And<BackstageJob>(q, x => x.Server == server);

            var important_filters = PredicateBuilder.New<BackstageJob>(x => false);
            foreach (var im in signatures)
            {
                var pairs = im.Split('.');
                string service = pairs[0], action = pairs[1];

                important_filters = PredicateBuilder.Or<BackstageJob>(important_filters, x => x.Service == service && x.Action == action);
            }
            q = PredicateBuilder.And<BackstageJob>(q, important_filters);

            if (from != null)
                q = PredicateBuilder.And<BackstageJob>(q, x => x.TimeToRun > from.Value);
            if (to != null)
                q = PredicateBuilder.And<BackstageJob>(q, x => x.TimeToRun <= to.Value);
            return GetDefaultQuery().Where(q);
        }
    }

    public interface IBackstageJobService : IReadOnlySupportBaseOfService<BackstageJob, IBackstageJobRepository, IBackstageJobReadOnlyRepository, IBackstageJobService>
    {
        IQueryable<BackstageJob> GetFailedQuery(string[] jobSignatures, DateTime? from, DateTime? to);
        int FixFetchedButNotRunJobs();
        BackstageJob[] SearchByTag(string tag);
        BackstageJob[] RunByTag(string tag);
        IService TryCreateService(BackstageJob job, ref Exception exception);
        void RedependentJob(BackstageJob job, string redependentCode);
        void StopService();
        void Reschedule(BackstageJob job, DateTime timeToRun);
        void RawRun(string script);
        ActionResponse SoftDelete(BackstageJobSearchModel searchModel);
        ActionResponse RemoveAllDone();
        void Push(BackstageJobOrder[] jobs);
        /// <summary>
        /// دریافت اولویت اجرای گزارش با توجه به تعداد خروجی
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        int GetReportPriorityByCount(int count);
        ActionResponse RestartAll(BackstageJobSearchModel searchModel);
        void Restart(long jobId);
        string GetCode<T>(string actionName, params object[] args)
            where T : IService;
        void MarkAsDone(BackstageJob job, long duration, string executionLog);
        BackstageJob Pop(string server, string[] ignore_queue_names);
        void Push(string service, string action, Expression<Func<object>> args,
             short priority, string queue = null, string code = null,
             string dependency = null, object[] tags = null, bool debug = false, DateTime? timeToRun = null);
        void MarkAsError(BackstageJob job, Exception exception, string executionLog);
        void MarkAsInprogress(BackstageJob job);
        SummaryDataPageResponse<BackstageJob> Report(BackstageJobSearchModel searchModel);
        bool IsInQueue(string service, string action, string tag);
        long Run(BackstageJob job, Func<BackstageJob, string> logMaker);
        long Run(string service, string action, string tag);


    }

}
