using Exir.Framework.Common;
using Exir.Framework.Service;
using SeatDomain.Services.Periodically;
using System;

namespace SeatService
{
    public class SessionCleanerPeriodicallyService : AbstractService, IPeriodicallyService
    {
        public void ExecutePeriodicallyJob(DateTime lastExecution, DateTime date)
        {
            var session = ObjectRegistry.GetObject<ISessionProvider>();
            session.Cleanup();
        }

        public DateTime GetBeginingExecutionTime()
        {
            return DateTime.Now;
        }

        public TimeSpan GetPeriod()
        {
            return TimeSpan.FromMinutes(30);
        }

        public TimeSpan GetPeriodicallyExecutionTimeOfDay()
        {
            return TimeSpan.FromMinutes(0);
        }

        public bool IsContainCurrentDate()
        {
            return true;
        }
    }
}
